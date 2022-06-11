using InfluxDB.Client.Core.Flux.Domain;
using MetaMetrics.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api = MetaMetrics.Api;

namespace MetaMetricsViewer.Service
{
    public class MetaMetricsService : IMetaMetricsService
    {
        private readonly string bucket = Api.MetaMetricsQueryService.bucket;
        private readonly InfluxDBService _service;

        public MetaMetricsService(InfluxDBService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<string>> GetSublicenseList()
        {
            var query = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"sublicense\")";

            var results = await _service.QueryAsync(query);
            return GetSubLicenses(results.SelectMany(x => x.Records.Select(y => y.GetValue().ToString())));
        }

        public async Task<IEnumerable<string>> GetSublicenseList(MetaMetricsRangeInfoDto req)
        {

            var requestStartTime = req.StartDate().ToString("yyyy-MM-ddTHH:mm:ss") + "Z";

            var query = $@"from(bucket: ""{bucket}"")
                          |> range(start: {requestStartTime}, stop: now())
                          |> filter(fn: (r) => r[""_field""] == ""value"")
                          |> keep(columns: [""sublicense""])
                          |> distinct()";

            var results = await _service.QueryAsync(query);

            return results.Select(fluxTable => fluxTable.Records.Select(z => z.GetValueByKey("sublicense"))
                .First()).Select(v => v.ToString());
        }

        public async Task<IEnumerable<string>> GetFilterValueList(MetaMetricsRangeInfoDto req, string value)
        {

            var requestStartTime = req.StartDate().ToString("yyyy-MM-ddTHH:mm:ss") + "Z";

            var sb = $@"from(bucket: ""{bucket}"")
                          |> range(start: {requestStartTime}, stop: now())
                          |> filter(fn: (r) => r[""_field""] == ""value"")";
            var query = new StringBuilder();
            query.AppendLine(sb);
            query.AppendLine($"|> keep(columns: [\"{value}\"])");
            // query.AppendLine($"|> distinct(column: \"{value}\")");
            query.AppendLine("|> distinct()");

            var results = await _service.QueryAsync(query.ToString());

            return results.Select(fluxTable => fluxTable.Records.Select(z => z.GetValueByKey($"{value}"))
                .First()).Select(v => v.ToString());
        }

        public async Task<Api.MetaMetricsFiltersDTO> GetFiltersValue(MetaMetricsRangeInfoDto req)
        {
            var model = new Api.MetaMetricsFiltersDTO
            {
                SubLicenses = await GetFilterValueList(req, "sublicense"),
                Apps = await GetFilterValueList(req, "app"),
                Measurements = await GetFilterValueList(req, "_measurement"),
                Environments = await GetFilterValueList(req, "env"),
                Projects = await GetFilterValueList(req, "project"),
                Versions = await GetFilterValueList(req, "version"),
                Licenses = await GetFilterValueList(req, "license"),
                Servers = await GetFilterValueList(req, "server")
            };

            return model;
        }

        public async Task<Api.MetaMetricsFiltersDTO> GetFilters()
        {
            var client = await MetaMetricsConnector.Main();
            var model = new Api.MetaMetricsFiltersDTO
            {
                SubLicenses = await MetaMetricsConnector.QuerySubLicenses(client, (ex, q) => { }),
                Apps = await MetaMetricsConnector.QueryApps(client, (ex, q) => { }),
                Measurements = await MetaMetricsConnector.QueryMeasurements(client, (ex, q) => { }),
                Environments = await MetaMetricsConnector.QueryEnvironments(client, (ex, q) => { }),
                Projects = await MetaMetricsConnector.QueryProjects(client, (ex, q) => { }),
                Versions = await MetaMetricsConnector.QueryVersions(client, (ex, q) => { }),
                Licenses = await MetaMetricsConnector.QueryLicenses(client, (ex, q) => { }),
                Servers = await MetaMetricsConnector.QueryServers(client, (ex, q) => { }),
                Items = await MetaMetricsConnector.QueryItems(client, (ex, q) => { })
            };

            return model;
        }

        public async Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementList()
        {
            var query = $"import \"influxdata/influxdb/schema\"\rschema.tagValues(bucket: \"{bucket}\", tag: \"_measurement\")";

            var results = await _service.QueryAsync(query);

            if (results == null) return Array.Empty<Api.MetaMetricsMeasurementDto>();

            var measurements = results.SelectMany(x => x.Records.Select(y => y.GetValue().ToString()))
                .Select(n => GetMeasurementName(n))
                .GroupBy(n => n)
                .Select(n => new Api.MetaMetricsMeasurementDto(n.Key))
                .OrderBy(n => n.MeasurementType == Api.MetaMetricsMeasurementType.Unknown)
                .ThenBy(n => n.MeasurementType)
                .ThenBy(n => n.MeasurementName)
                .Where(n => n.MeasurementType != Api.MetaMetricsMeasurementType.Ignore)
                .ToArray();

            return measurements;
        }

        public async Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementListBySublicense(Api.MetaMetricsMeasureRequestDto req)
        {
            var requestStartTime = req.Range.StartDate().ToString("yyyy-MM-ddTHH:mm:ss") + "Z";

            var query = $@"from(bucket: ""{bucket}"")
                          |> range(start: {requestStartTime}, stop: now())
                          |> filter(fn: (r) => r[""_field""] == ""value"" and r[""sublicense""] == ""{req.Sublicense.Replace("\\", "\\\\")}"")
                          |> keep(columns: [""sublicense"", ""_measurement""])
                          |> distinct()";

            var results = await _service.QueryAsync(query);

            if (results == null) return Array.Empty<Api.MetaMetricsMeasurementDto>();

            var measurements = results.SelectMany(x => x.Records.Select(y => y.GetValueByKey("_measurement").ToString()))
                .Select(n => GetMeasurementName(n))
                .GroupBy(n => n)
                .Select(n => new Api.MetaMetricsMeasurementDto(n.Key))
                .OrderBy(n => n.MeasurementType == Api.MetaMetricsMeasurementType.Unknown)
                .ThenBy(n => n.MeasurementType)
                .ThenBy(n => n.MeasurementName)
                .Where(n => n.MeasurementType != Api.MetaMetricsMeasurementType.Ignore)
                .ToArray();

            return measurements;
        }

        public async Task<MetaMetricsInstallationDto> GetInstallationBySublicense(string sublicense)
        {

            string query = $@"from(bucket: ""{bucket}"")
                           |> range(start: 2021-01-01, stop: now())
                           |> filter(fn: (r) => r[""_field""] == ""value"" and r[""sublicense""] == ""{sublicense.Replace("\\", "\\\\")}"")
                           |> keep(columns: [""sublicense"", ""license"", ""_time""])
                           |> sort(columns: [""sublicense"", ""_time""], desc: true)
                           |> limit(n: 1)";

            try
            {
                var results = await _service.QueryAsync(query);
                if (results == null) return null;

                return results.SelectMany(x => x.Records.Select(y => new Api.MetaMetricsInstallationDto
                {
                    Sublicense = sublicense,
                    License = y.GetValueByKey("license").ToString(),
                    LastTimestamp = y.GetTimeInDateTime().Value
                }
                                )).FirstOrDefault();
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to get data for sublicense: {sublicense}!");
                return null;
            }
        }

        public async Task<MetaMetricsTimeLineDto> GetTimeLine(MetaMetricsTimeLineRequestDto req)
        {

            var query = new MetaMetricsQuery()
                    .OffsetHours(0)
                    .From(req.Range.StartDate())
                    .To()
                    .Each(req.EachHours)
                    .Measurement(req.Measurement)
                    .Sublicense(req.Sublicense)
                    .OnValue();

            var results = await _service.QueryAsync(query.Query);

            if (results == null) return null;

            var timeValues = results.SelectMany(x => x.Records.Select(y => new Api.MetaMetricsTimeValueDto
                {
                    Timestamp = y.GetTimeInDateTime().Value,
                    Value = (long)y.GetValue()
                })).ToArray();

            return new Api.MetaMetricsTimeLineDto { TimeValues = timeValues };
        }

        public async Task<List<MetaMetricsTime4LinesDTO>> GetTime4Lines(MetaMetricsTime4LinesRequestDto req)
        {
            MetaMetricsQuery query = new MetaMetricsQuery()
                .OffsetHours(0)
                .From(req.Range.StartDate())
                .To()
                .Each(req.EachHours)
                .License(req.License)
                .Version(req.Version)
                .App(req.App)
                .Environment(req.Environment)
                .Project(req.Project)
                .Server(req.Server)
                //.Measurement(req.Measurement.ToArray()) //todo if use Measurement
                .Sublicense(req.Sublicense)
                .OnValue();
            
            var results = await _service.QueryAsync(query.Query);

            return results == null ? new List<MetaMetricsTime4LinesDTO>() : BuildAllMeasurement(results, req.Measurement);
        }

        public async Task<MetaMetricsTime4LinesPaginationDTO> GetTime4LinesPagination(MetaMetricsTime4LinesPaginationRequestDto req)
        {
            const int take = 50;
            var m = new MetaMetricsTime4LinesPaginationDTO();
            var client = await MetaMetricsConnector.Main();
            var sub = new List<MetaMetricsInstallationTimeLine>();
            var skip = 0;
            while (true)
            {
                var sublicenses = req.Sublicenses.Skip(skip).Take(take).ToArray();
                if(sublicenses.Length == 0) break;
                var query = new MetaMetricsQuery()
                    .OffsetHours(0)
                    .From(req.Range.StartDate())
                    .To()
                    .Each(req.EachHours)
                    .License(req.License)
                    .Version(req.Version)
                    .App(req.App)
                    .Environment(req.Environment)
                    .Project(req.Project)
                    .Server(req.Server)
                    .Measurement(req.Measurement.ToArray()) //todo if use Measurement
                    .Sublicense(sublicenses)
                    .OnValue();
                var subCurrent = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);
                sub.AddRange(subCurrent);
                skip += take;
            }


            m.Count = sub.Count();
            if (req.PageNumber > 1) sub = sub.Skip((req.PageNumber - 1) * req.PageSize).ToList();
            if (!sub.Any()) return m;
            sub = sub.Take(req.PageSize).ToList();
            m.List = BuildLineModelQueryMeasurement(sub);

            return m;
        }

        public async Task<List<MetaMetricsInstallationTimeLine>> GetTime4LinesPaginationSub(MetaMetricsTime4LinesPaginationRequestDto req)
        {
            
            var client = await MetaMetricsConnector.Main();
            var query = new MetaMetricsQuery(null, req.IsNotValue)
                .OffsetHours(0)
                .From(req.Range.StartDate())
                .To()
                .Every(req.Range.Every)
                .License(req.License)
                .Version(req.Version)
                .App(req.App)
                .Environment(req.Environment)
                .Project(req.Project)
                .Server(req.Server)
                //.Measurement(req.Measurement.ToArray()) //todo if use Measurement
                .Sublicense(req.Sublicenses.ToArray())
                .CreateEmpty(req.IsCreateEmpty)
                .OnValue();
           return await MetaMetricsConnector.QueryMeasurement(client, query, false, null, req.IsNotValue);
        }


        public async Task<List<MetaMetricsInstallationTimeLine>> GetTime4LinesPaginationSub1(MetaMetricsTime4LinesPaginationRequestDto req)
        {
            const int take = 10;
            var m = new MetaMetricsTime4LinesPaginationDTO();
            var client = await MetaMetricsConnector.Main();
            var sub = new List<MetaMetricsInstallationTimeLine>();
            var skip = 0;
            while (true)
            {
                var sublicenses = req.Sublicenses.Skip(skip).Take(take).ToArray();
                if (sublicenses.Length == 0) break;

                var query = new MetaMetricsQuery(null, false)
                    .OffsetHours(0)
                    .From(req.Range.StartDate())
                    .To()
                    .Each(req.EachHours)
                    .License(req.License)
                    .Version(req.Version)
                    .App(req.App)
                    .Environment(req.Environment)
                    .Project(req.Project)
                    .Server(req.Server)
                    //.Measurement(req.Measurement.ToArray()) //todo if use Measurement
                    .Sublicense(sublicenses)
                    .OnValue();
                var subCurrent = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);
                sub.AddRange(subCurrent);
                skip += take;
            }

            return sub;
        }


        private static List<MetaMetricsTime4LinesDTO> BuildLineModelQueryMeasurement(List<MetaMetricsInstallationTimeLine> sub)
        {
            var m = new List<MetaMetricsTime4LinesDTO>();
            foreach (var p in sub)
            {
                var i = new MetaMetricsTime4LinesDTO
                {
                    //Name = p.Name,
                    Sublicense = p.Installation.Display,
                    Server = p.Server,
                    Version = p.Version,
                    App = p.Products,
                    MetaMetricsTimeLine =
                    {
                        TimeValues = p.TimeValues
                            .Select(value => new MetaMetricsTimeValueDto { Value = value.Sum, Timestamp = value.LastTillTime }).ToList()
                    }
                };
                m.Add(i);
            }
            return m;
        }


        public async Task<MetaMetricsTime4LinesPaginationDTO> GetTime4LinesPagination1(MetaMetricsTime4LinesPaginationRequestDto req)
        {
            var m = new MetaMetricsTime4LinesPaginationDTO();
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                .From(req.Range.StartDate())
                .To()
                .Each(req.EachHours)
                .License(req.License)
                .Version(req.Version)
                .App(req.App)
                .Environment(req.Environment)
                .Project(req.Project)
                .Server(req.Server)
                //.Measurement(req.Measurement.ToArray()) //todo if use Measurement
                .Sublicense(req.Sublicenses.ToArray())
                .OnValue();

            var results = await _service.QueryAsync(query.Query);
            var fluxTables = results.ToList();
            m.Count = fluxTables.Count();
            if (req.PageNumber > 1) fluxTables = fluxTables.Skip((req.PageNumber - 1) * req.PageSize).ToList();
            if (!fluxTables.Any()) return m;
            fluxTables = fluxTables.Take(req.PageSize).ToList();
            m.List = BuildAllMeasurement(fluxTables, req.Measurement);

            return m;
        }

        private static List<MetaMetricsTime4LinesDTO> BuildAllMeasurement(IEnumerable<FluxTable> results, ICollection<string> measurements)
        {
            var model = new List<MetaMetricsTime4LinesDTO>();
            foreach (var result in results)
            {
                var json = JsonConvert.SerializeObject(result.Records.First().Values);
                var time4Line = JsonConvert.DeserializeObject<MetaMetricsTime4LinesDTO>(json);
                if (time4Line != null && measurements.Contains(time4Line.Measurement))
                {
                    time4Line!.MetaMetricsTimeLine.TimeValues = result.Records.Select(record => new MetaMetricsTimeValueDto
                    {
                        Timestamp = record.GetTimeInDateTime().Value,
                        Value = (long)record.GetValue()
                    });
                }
                else
                {
                    if (model.Count > 0) continue;
                }

                model.Add(time4Line);
            }

            return model;
        }

        private static MetaMetricsTime4LinesDTO InitMetaMetricsTime4Lines(FluxTable result)
        {
            var json = JsonConvert.SerializeObject(result.Records.First().Values);
            var time4Line = JsonConvert.DeserializeObject<MetaMetricsTime4LinesDTO>(json);
            time4Line!.MetaMetricsTimeLine.TimeValues = result.Records.Select(record => new MetaMetricsTimeValueDto
            {
                Timestamp = record.GetTimeInDateTime().Value,
                Value = (long)record.GetValue()
            });

            return time4Line;
        }

        private static string GetMeasurementName(string name)
        {
            var measurement = name.Replace('-', '_').ToEnum(Api.MetaMetricsMeasurementType.Unknown);
            if (measurement != MetaMetricsMeasurementType.Unknown)
                return measurement.ToString();
            return name;
        }

        private static IEnumerable<string> GetSubLicenses(IEnumerable<string> allSubLicenses)
        {
            var subLicenses = new Dictionary<string, List<string>>();
            foreach (var s in allSubLicenses)
            {
                if (MetaMetricsInstallationService.Instance.Installations.ContainsKey(s.ToInstallation()) && !MetaMetricsInstallationService.Instance.Installations.ContainsKey(s))
                {
                    var orginst = MetaMetricsInstallationService.Instance[s.ToInstallation()];
                    MetaMetricsInstallationService.Instance.Installations.Remove(s.ToInstallation());
                    orginst.Key = s;
                    MetaMetricsInstallationService.Instance.Installations[orginst.Key] = orginst;
                }
                var inst = MetaMetricsInstallationService.Instance[s];
                if (!string.IsNullOrEmpty(inst.MapTo))
                {
                    inst = MetaMetricsInstallationService.Instance[inst.MapTo];
                }
                if (inst.Ignore)
                {
                    continue;
                }

                if (!subLicenses.TryGetValue(inst.Key, out var comb))
                {
                    subLicenses[inst.Key] = comb = new List<string>();
                    comb.Add(inst.Key);
                }
                if (inst.Key != s)
                    comb.Add(s);
            }
            
            return subLicenses.Select(license => license.Key).OrderBy(z=>z).ToList();
        }
    }
}
