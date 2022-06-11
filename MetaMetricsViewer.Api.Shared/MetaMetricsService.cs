using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using Microsoft.Extensions.Options;
using NodaTime;

namespace MetaMetrics.Api
{
    public class MetaMetricsClientService
    {
        // public static string token = "ieVJ18EyvsGmfv2-Ft0H3saECWI4Np6tl1hQe7V1k3-b88kxNFhn-ZSEjtk6QHS8hdqDFuLJMlSZF8yF0tZJAA==";
        // public static string url = "https://metrics.metait.de";
        private readonly InfluxDBClientOptions _options;

        public MetaMetricsClientService(IOptions<InfluxDBOptions> options)
        {
            this._options = new InfluxDBClientOptions.Builder()
                .Url(options.Value.Url)
                .AuthenticateToken(options.Value.Token)
                .TimeOut(TimeSpan.FromSeconds(120))
                //.ReadWriteTimeOut(TimeSpan.FromSeconds(120))
                .Build();
        }

        public async Task<InfluxDBClient> Create()
        {
            return InfluxDBClientFactory.Create(_options);
        }
    }

    public class MetaMetricsQueryService
    {
        public static string bucket = "MetaMetrics";
        public static string org = "Meta IT GmbH";
        private MetaMetricsClientService clientService;
        private MetaMetricsInstallationService installationMap;

        public MetaMetricsQueryService(MetaMetricsClientService clientService, MetaMetricsInstallationService installationMap)
        {
            this.clientService = clientService;
            this.installationMap = installationMap;
        }

        public async Task<List<MetaMetricsInstallationTimeLine>> GetMetrics(MetaMetricsQuery query, bool addExpected = false, Action<Exception, string> onError = null)
        {
            using (var influxClient = await clientService.Create())
            {
                var api = influxClient.GetQueryApi();
                var select = query.Query;
                try
                {
                    var tables = await api.QueryAsync(select, org);
                    return ToTimeLines(tables, query, addExpected);
                }
                catch (Exception ex)
                {
                    onError?.Invoke(ex, select);
                }

                return new List<MetaMetricsInstallationTimeLine>();
            }
        }

        private List<MetaMetricsInstallationTimeLine> ToTimeLines(IEnumerable<FluxTable> tables, MetaMetricsQuery query, bool addExpected)
        {
            var timeLines = new Dictionary<string, MetaMetricsInstallationTimeLine>();
            if (tables != null)
            {
                foreach (var table in tables)
                {
                    var p = 0;
                    var _pgroup = -1;
                    var _psgroup = -1;
                    var _ptime = -1;
                    var _pvalue = -1;
                    var _pitem = -1;
                    foreach (var column in table.Columns)
                    {
                        switch (column.Label.ToLower())
                        {
                            case "item":
                                _pitem = p;
                                break;
                            case "_value":
                                _pvalue = p;
                                break;
                            case "_time":
                                _ptime = p;
                                break;
                            default:
                                if (column.Label == query.GroupBy)
                                {
                                    _pgroup = p;
                                }
                                else if (column.Label == query.SubGroupBy)
                                {
                                    _psgroup = p;
                                }

                                break;
                        }

                        p++;
                    }

                    if (_ptime >= 0 && _pvalue >= 0)
                    {
                        foreach (var record in table.Records)
                        {
                            var sub = (_pgroup == -1 ? "" : record.GetValueByIndex(_pgroup) as string) ?? "";
                            if (installationMap.Installations.ContainsKey(sub.ToInstallation()) && !installationMap.Installations.ContainsKey(sub))
                            {
                                var orginst = installationMap[sub.ToInstallation()];
                                installationMap.Installations.Remove(sub.ToInstallation());
                                orginst.Key = sub;
                                installationMap.Installations[orginst.Key] = orginst;
                            }

                            var inst = installationMap[sub];
                            if (!string.IsNullOrEmpty(inst.MapTo))
                            {
                                inst = installationMap[inst.MapTo];
                            }

                            if (inst.Ignore)
                            {
                                continue;
                            }

                            sub = inst.Fullname;
                            var measurement = _psgroup == -1 ? "" : record.GetValueByIndex(_psgroup) as string;
                            var item = _pitem == -1 ? "" : record.GetValueByIndex(_pitem) as string;
                            var time = record.GetValueByIndex(_ptime) as Instant?;
                            var value = record.GetValueByIndex(_pvalue) as long?;
                            if (!string.IsNullOrEmpty(sub) && time != null && value != null)
                            {
                                MetaMetricsInstallationTimeLine metaMetricsTimeLine;
                                if (!timeLines.TryGetValue(sub, out metaMetricsTimeLine))
                                {
                                    metaMetricsTimeLine = new MetaMetricsInstallationTimeLine(query, inst) { };
                                    timeLines[metaMetricsTimeLine.Name] = metaMetricsTimeLine;
                                }

                                metaMetricsTimeLine.Add(measurement, item, (Instant)time, (long)value, query.EveryHourCombine, query.OffsetCombine, query.RequestEndTime);
                            }
                        }
                    }
                }
            }

            installationMap.Save();
            query.RealStartTime = timeLines.Values.Any() ? timeLines.Values.Min(s => s.FirstTime) : DateTime.Today;
            query.RealEndTime = timeLines.Values.Any() ? timeLines.Values.Max(s => s.LastTillTime) : DateTime.Today;
            if (addExpected)
            {
                foreach (var expected in installationMap.Installations.Values)
                {
                    if (string.IsNullOrEmpty(expected.MapTo) && !expected.Ignore)
                    {
                        if (!timeLines.ContainsKey(expected.Fullname))
                        {
                            timeLines[expected.Fullname] = new MetaMetricsInstallationTimeLine(query, expected);
                        }
                    }
                }
            }

            return timeLines.Values.OrderBy(n => n.Name).ToList();
        }
    }
}