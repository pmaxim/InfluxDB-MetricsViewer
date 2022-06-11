using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using NodaTime;

namespace MetaMetrics.Api
{
        
    public class MetaMetricsConnector
    {
        const string token = "1p3l6eCQryVOLmAO5AuNsC6ZOXqFkOGMJHOZgagvCi8EXCnle2iXTrbxyZXNBz_JMvDypl9kkuiOzRp9aOob9g==";
        const string bucket = "MetaMetrics";
        const string org = "Meta IT GmbH";

        public static async Task<InfluxDBClient> Main(params string[] args)
        {
            //var client = InfluxDBClientFactory.Create("http://192.168.1.248:8086", token.ToCharArray());
            //var clients = InfluxDBClientFactory.Create("https://metrics.metait.de", token.ToCharArray());

            var clients = InfluxDBClientFactory.Create(InfluxDBClientOptions.Builder
                .CreateNew()
                .AuthenticateToken(token)
                .Url("https://metrics.metait.de")
                .TimeOut(TimeSpan.MaxValue)
                .Build());
            //metrics.metakis.de
            return clients;
        }

        public static async Task<string[]> QuerySubLicenses(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"sublicense\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryItems(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"item\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryMeasurements(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"_measurement\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryEnvironments(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"env\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryProjects(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"project\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryVersions(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"version\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryLicenses(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"license\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryApps(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"app\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }
        
        public static async Task<string[]> QueryServers(InfluxDBClient client, Action<Exception,string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"server\")";
            try
            {
                var tables = await api.QueryAsync(select, org);
                return ToStrings(tables).OrderBy(n=>n).ToArray();
            }
            
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }

        public static async Task<string[]> QueryTest(InfluxDBClient client, Action<Exception, string> onError)
        {
            var api = client.GetQueryApi();
            var select = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"server\")";
            try
            {
                var tables = await api.QueryAsync(select, org);

                foreach (FluxTable fluxTable in tables)
                {
                    
                }

                return ToStrings(tables).OrderBy(n => n).ToArray();
            }

            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }
            return Array.Empty<string>();
        }

        public static List<string> ToStrings(IEnumerable<FluxTable> tables)
        {
            var items = new List<string>();
            if (tables != null)
            {
                foreach (var table in tables)
                {
                    var p = 0;
                    var _pvalue = -1;
                    foreach (var column in table.Columns)
                    {
                        switch (column.Label.ToLower())
                        {
                            case "_value":
                                _pvalue = p;
                                break;
                        }

                        p++;
                    }

                    if (_pvalue >= 0)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = record.GetValueByIndex(_pvalue) as string;
                            items.Add(value);
                        }
                    }
                }
            }

            return items;
        }

        public static List<MetaMetricsInstallationTimeLine> ToTimeLines(IEnumerable<FluxTable> tables, 
            MetaMetricsQuery query, bool addExected, bool? isNotValue = null)
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
                    var _pserver = -1;
                    var _plicense = -1;
                    var _pproject = -1;
                    var _pversion = -1;
                    var _sublicense = -1;

                    foreach (var column in table.Columns)
                    {
                        switch (column.Label.ToLower())
                        {
                            case "item":
                                _pitem = p;
                                break;
                            case "server":
                                _pserver = p;
                                break;
                            case "license":
                                _plicense = p;
                                break;
                            case "project":
                                _pproject = p;
                                break;
                            case "version":
                                _pversion = p;
                                break;
                            case "_value":
                                _pvalue = p;
                                break;
                            case "_time":
                                _ptime = p;
                                break;
                            default:
                                if(column.Label.ToLower() == "sublicense") _sublicense = p;
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

                    if ( _ptime >= 0 && _pvalue >= 0 || (bool)isNotValue)
                    {
                        foreach (var record in table.Records)
                        {
                            var sub = (_pgroup == -1 ? "" : record.GetValueByIndex(_pgroup) as string)??"";
                            
                            var server = _pserver == -1 ? "" : record.GetValueByIndex(_pserver) as string;
                            var license = _plicense == -1 ? "" : record.GetValueByIndex(_plicense) as string;
                            var version = _pversion == -1 ? "" : record.GetValueByIndex(_pversion) as string;
                            var project = _pproject == -1 ? "" : record.GetValueByIndex(_pproject) as string;
                            var sublicense = _sublicense == -1 ? "" : record.GetValueByIndex(_sublicense) as string;

                            if (MetaMetricsInstallationService.Instance.Installations.ContainsKey(sub.ToInstallation()) && !MetaMetricsInstallationService.Instance.Installations.ContainsKey(sub))
                            {
                                var orginst = MetaMetricsInstallationService.Instance[sub.ToInstallation()];
                                MetaMetricsInstallationService.Instance.Installations.Remove(sub.ToInstallation());
                                orginst.Key = sub;
                                MetaMetricsInstallationService.Instance.Installations[orginst.Key] = orginst;
                            }
                            var inst = MetaMetricsInstallationService.Instance[sub];
                            if (!string.IsNullOrEmpty(inst.MapTo))
                            {
                                inst = MetaMetricsInstallationService.Instance[inst.MapTo];
                            }
                            if (inst.Ignore)
                            {
                                continue;
                            }
                            if (!string.IsNullOrEmpty(server))
                                inst.Server = server;
                            if (!string.IsNullOrEmpty(license))
                                inst.License = license;
                            if (!string.IsNullOrEmpty(sublicense))
                                inst.Sublicense = sublicense;
                            if (!string.IsNullOrEmpty(version))
                            {
                                var ver = new Version(version);
                                if (!string.IsNullOrEmpty(inst.Version))
                                {
                                    var oldver = new Version(inst.Version.Split('/').First());
                                    oldver = new Version(oldver.Major, oldver.Minor, oldver.Build);
                                    if (oldver > ver)
                                        ver = oldver;
                                }
                                var date = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2);
                                date = date.AddSeconds(-date.Second);
                                date = date.AddMinutes(-date.Minute);
                                inst.Version = $"{ver.Major}.{ver.Minor}.{ver.Build} / {date.ToString("dd.MM. HH")}:00";
                            }

                            sub = inst.Key;
                            
                            var measurement = _psgroup == -1 ? "" : record.GetValueByIndex(_psgroup) as string;
                            var item = _pitem == -1 ? "" : record.GetValueByIndex(_pitem) as string;

                            var product = measurement.Split('_').First().ToLower();
                            switch (product)
                            {
                                case "metatext":
                                    inst.AllProducts["MetaTEXT"] = true;
                                    break;
                                case "metakis":
                                case "metakisdbbatch":
                                case "metakisdbservice":
                                case "metakisbatch":
                                case "metakisbatchservice":
                                    inst.AllProducts["MetaKIS"] = true;
                                    break;
                                default:
                                    break;
                            }

                            if ((bool)isNotValue)
                            {
                                MetaMetricsInstallationTimeLine metaMetricsTimeLine;
                                if (!timeLines.TryGetValue(sub, out metaMetricsTimeLine))
                                {
                                    metaMetricsTimeLine = new MetaMetricsInstallationTimeLine(query, inst) { };
                                    timeLines[sub] = metaMetricsTimeLine;
                                }
                                continue;
                            }
                            
                            var time = record.GetValueByIndex(_ptime) as Instant?;
                            var value = record.GetValueByIndex(_pvalue) as long?;
                            if (!string.IsNullOrEmpty(sub) && time != null && value != null)
                            {
                                MetaMetricsInstallationTimeLine metaMetricsTimeLine;
                                if (!timeLines.TryGetValue(sub, out metaMetricsTimeLine))
                                {
                                    metaMetricsTimeLine = new MetaMetricsInstallationTimeLine(query, inst) { };
                                    timeLines[sub] = metaMetricsTimeLine;
                                }
                                metaMetricsTimeLine.Add(measurement, item, (Instant)time, (long)value, query.EveryHourCombine, query.OffsetCombine, query.RequestEndTime);
                            }
                        }
                    }
                }
            }
            foreach (var item in timeLines)
            {
                if (item.Value.Installation.LastData < item.Value.LastTillTime)
                {
                    item.Value.Installation.LastData = item.Value.LastTillTime;
                }
            }
            
            MetaMetricsInstallationService.Instance.Save();
            query.RealStartTime = timeLines.Values.Any()? timeLines.Values.Min(s => s.FirstTime):DateTime.Today;
            query.RealEndTime = timeLines.Values.Any()?timeLines.Values.Max(s => s.LastTillTime):DateTime.Today;
            /*
            if (addExected)
            {
                foreach (var expected in MetaMetricsInstallationService.Instance.Installations.Values)
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
            */

            return timeLines.Values.OrderBy(n => n.Name).ToList();
        }

        public static async Task<List<MetaMetricsInstallationTimeLine>> QueryMeasurement(InfluxDBClient client, 
            MetaMetricsQuery query, bool addExected, Action<Exception,string> onError, bool? isNotValue = null)
        {
            var api = client.GetQueryApi();
            var select = query.Query;
            try
            {
                var tables = await api.QueryAsync(select, org);
                var res = ToTimeLines( tables,query, addExected, isNotValue);
                var p = 0;
                foreach (var re in res)
                {
                    re.Nr = ++p;
                }
                return res;
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex, select);
            }

            return new List<MetaMetricsInstallationTimeLine>();
        }
    
    }
}
