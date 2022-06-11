using MetaMetrics.Api;
using MetaMetricsViewer.Console.Models;
using MetaMetricsViewer.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using InfluxDB.Client.Core.Flux.Domain;
using Newtonsoft.Json;

namespace MetaMetricsViewer.Console.Lib
{
    public class InfluxDBTest
    {
        private readonly IMetaMetricsService? _metaMetricsService;
        public static int TimeValuesCount = 0;
        private readonly MetaMetricsRangeInfo[] _ranges = MetaMetricsRangeInfo.InitRanges();
        private readonly InfluxDBService? _service;

        private readonly string[] _defaultMeasurements = new string[]
        {
            "metakis__webcontext", "metakis__workcontextrecords", "metakis__patientdata", "metatext__parsedocument_counter"
        };

        private readonly string[] _sublicense = new string[]
        {
            "AMC", "Ameos.P21", "Ameos.Schoenebeck", "AsklepiosHHAKA"
        };

        public InfluxDBTest(IServiceProvider serviceProvider)
        {
            _metaMetricsService = serviceProvider.GetService<IMetaMetricsService>();
            _service = serviceProvider.GetService<InfluxDBService>();
        }

        public async Task BuildWpfTable()
        {
            //await TestMap();

            //await TestFilter();

            //await InitModel();

            string[] defaultMeasurements = new string[] { "metakis__webcontext", "metakis__workcontextrecords", "metakis__patientdata", "metatext__parsedocument_counter", };
            var timeLines = new Dictionary<string, MetaMetricsInstallationTimeLine>();
            //await InitModel();
            var client = await MetaMetricsConnector.Main();
            var subLicenses = await GetSublicenseList();
            var res = new List<MetaMetricsInstallationTimeLine>();

            foreach (var subLicense in subLicenses)
            {
                System.Console.WriteLine($"Querying {string.Join(", ", subLicense)}");

                MetaMetricsQuery? lastQuery;
                var query = lastQuery = new MetaMetricsQuery()
                    .OffsetHours(0)
                    .FromLastDays(6)
                    .To()
                    .Each(24)
                    //.Sublicense(subLicense)
                    .Server("AWV87")
                    //.Measurement(defaultMeasurements)
                    .OnValue();
                var sub = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);

                //var json = JsonConvert.SerializeObject(sub, Formatting.Indented, new JsonSerializerSettings
                //{
                //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //});

                if (sub.Count==0)
                {
                    var expected = MetaMetricsInstallationService.Instance[subLicense];
                    sub.Add(new MetaMetricsInstallationTimeLine(lastQuery, expected));
                }
              
                res.AddRange(sub);
            }
        }

        private async Task TestMap()
        {
            var client = await MetaMetricsConnector.Main();
            var subLicenses = await GetSublicenseList();
            var res = new List<MetaMetricsInstallationTimeLine>();
            var count = 0;
            foreach (var subLicense in subLicenses)
            {
                MetaMetricsQuery? lastquery;
                var query = lastquery = new MetaMetricsQuery()
                    .From(DateTime.Now.AddYears(-20))
                    .To(DateTime.Now)
                    //.OffsetHours(0)
                    //.FromLastDays(6)
                    //.To()
                    //.Each(24)
                    .Sublicense(subLicense)
                    .OnValue();
                var sub = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);
                if (sub.Count==0)
                {
                    var expected = MetaMetricsInstallationService.Instance[subLicense];
                    sub.Add(new MetaMetricsInstallationTimeLine(lastquery, expected));
                }
                else
                {
                    foreach (var p in sub)
                    {
                        if(p.HospitalLatitude == 0) continue;
                        var d = 1;
                    }
                }

                count++;
                res.AddRange(sub);
            }
        }

        public async Task TestFilterFromQueryMeasurement()
        {
            var range = new MetaMetricsRangeInfoDto
            {
                Default = true,
                Display = "1w",
                Weeks = 1
            };
            var startDate = range.StartDate();
            var client = await MetaMetricsConnector.Main();
            var subLicenses = await GetSublicenseList();
            var measurements = await MetaMetricsConnector.QueryMeasurements(client, (ex, q) => { });
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                .FromLastDays(1)
                //.From(startDate)
                .To()
                .Each(24)
                .Sublicense(subLicenses.Take(2).ToArray())
                //.Measurement(_defaultMeasurements) //_defaultMeasurements
                //.App("MetaKIS")
                //.OnField("value")
                //.License("13b01711-6442-d4ba-9a77-6f392538f99a")
                //.Version("2.0.7986.19308")
                //.Environment("release")
                //.Project("MetaKIS")
                //.Server("HAM-APMET02")
                .OnValue();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var sub = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);

            var ts = stopWatch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            System.Console.WriteLine("Time Query: " + elapsedTime);

            
            System.Console.WriteLine("Total request: " + sub.Count());

            stopWatch.Start();

            var lineModel = BuildLineModelQueryMeasurement(sub);

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            System.Console.WriteLine("Time model build: " + elapsedTime);
            ConsolePrint(lineModel);
        }

        private static IEnumerable<LinesMetaMetricsTime4LinesDTO> BuildLineModelQueryMeasurement(List<MetaMetricsInstallationTimeLine> sub)
        {
            var m = new List<LinesMetaMetricsTime4LinesDTO>();
            foreach (var p in sub)
            {
                var i = new LinesMetaMetricsTime4LinesDTO
                {
                    Name = p.Name,
                    Sublicense = p.Installation.Display,
                    Server = p.Server,
                    Version = p.Version
                };

                foreach (var timeValue in p.TimeValues)
                {
                    var tl = timeValue.TimeValues.Select(value => new MetaMetricsTimeValueDto { Value = value.Value, Timestamp = value.Till }).ToList(); ;
                    var time4Line = new MetaMetricsTime4LinesDTO();
                    if (tl.Any()) time4Line.MetaMetricsTimeLine.TimeValues = tl;
                    time4Line.Server = p.Server;
                    time4Line.Version = p.Version;
                    i.List.Add(time4Line);
                }
                m.Add(i);
            }
            return m;
        }

        public async Task TestWithOutValues()
        {
            var range = new MetaMetricsRangeInfoDto
            {
                Default = true,
                Display = "1w",
                Weeks = 1
            };
            var startDate = range.StartDate();
            var client = await MetaMetricsConnector.Main();

            var df = await MetaMetricsConnector.QueryTest(client, (ex, q) => { });

            var subLicenses = await GetSublicenseList();
            var measurements = await MetaMetricsConnector.QueryMeasurements(client, (ex, q) => { });
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                //.FromLastDays(7)
                .From(startDate)
                .To()
                .Each(24)
                .Sublicense(subLicenses.Take(50).ToArray())
                //.Measurement(_defaultMeasurements)
                //.App("MetaKIS")
                //.OnField("value")
                //.License("13b01711-6442-d4ba-9a77-6f392538f99a")
                //.Version("2.0.7986.19308")
                //.Environment("release")
                .Project("MetaKIS")
                .Server("HAM-APMET02")
                .OnValue();

            var request = await _service!.QueryAsync(query.Query);
        }

        public async Task TestFilterFromQueryAsync()
        {
            var range = new MetaMetricsRangeInfoDto
            {
                Default = true,
                Display = "1w",
                Weeks = 1
            };
            var startDate = range.StartDate();
            var client = await MetaMetricsConnector.Main();
            var subLicenses = await GetSublicenseList();
            var measurements = await MetaMetricsConnector.QueryMeasurements(client, (ex, q) => { });
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                //.FromLastDays(7)
                .From(startDate)
                .To()
                .Each(24)
                .Sublicense(subLicenses.Take(50).ToArray())
                //.Measurement(_defaultMeasurements)
                //.App("MetaKIS")
                .OnField("value")
                //.License("13b01711-6442-d4ba-9a77-6f392538f99a")
                //.Version("2.0.7986.19308")
                //.Environment("release")
                //.Project("MetaKIS")
                //.Server("HAM-APMET02")
                .OnValue();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var request = await _service!.QueryAsync(query.Query);

            var ts = stopWatch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            System.Console.WriteLine("Time Query: " + elapsedTime);

            var fluxTables = request.ToList();
            System.Console.WriteLine("Total request: " + fluxTables.Count());
            
            stopWatch.Start();
            var model = BuildAllMeasurement(fluxTables, _defaultMeasurements); //measurements or _defaultMeasurements
            
            var lineModel = BuildLineModel(model, subLicenses);

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            System.Console.WriteLine("Time model build: " + elapsedTime);
            ConsolePrint(lineModel);
        }

        private void ConsolePrint(IEnumerable<LinesMetaMetricsTime4LinesDTO> lineModel)
        {
            var index = 1;
            foreach (var lines in lineModel)
            {
                var line = $"{index++} | {lines.Sublicense} | ";
                foreach (var measurement in _defaultMeasurements)
                {
                    if (lines.List.Any(z => z.Measurement == measurement)) line += $"{measurement} |";
                    else line += "none |";
                }

                line += $"{lines.Server} |{lines.Version} |{lines.App}";

                System.Console.WriteLine(line);
            }
        }

        private static List<LinesMetaMetricsTime4LinesDTO> BuildLineModel(IEnumerable<MetaMetricsTime4LinesDTO> model, IEnumerable<string> subLicenses)
        {
            
            var m = new List<LinesMetaMetricsTime4LinesDTO>();
            foreach (var subLicense in subLicenses)
            {
                if(!model.Any(z => z.Sublicense == subLicense)) continue;
                var first = model.First(z => z.Sublicense == subLicense);
                m.Add(new LinesMetaMetricsTime4LinesDTO
                {
                    Sublicense = subLicense,
                    Server = first.Server,
                    Version = first.Version,
                    App = first.App,
                    List = model.Where(z => z.Sublicense == subLicense).ToList()
                });
            }
            return m;
        }



        private static IEnumerable<MetaMetricsTime4LinesDTO> BuildAllMeasurement(IEnumerable<FluxTable> results, ICollection<string> measurements)
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

            return model.OrderBy(z=>z.Sublicense).ToList();
        }

        private async Task TestFilter()
        {
            var client = await MetaMetricsConnector.Main();
            var subLicenses = await GetSublicenseList();
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                .FromLastDays(6)
                .To()
                .Each(24)
                .Sublicense("AsklepiosHHAKN")
                .App("MetaKIS")
                .OnField("value")
                .License("ae4af267-7821-4f5b-80fc-e9f46e54464f")
                .Version("2.0.7986.19308")
                .Environment("release")
                .Project("MetaKIS")
                .Server("HAM-APMET02")
                .Sublicense("AsklepiosHHAKN")
                .OnValue();
            var sub = await MetaMetricsConnector.QueryMeasurement(client, query, false, null);

        }


        public async Task Test4Lines()
        {
            var dynamicColumns = await BuildDynamicColumns4Lines();

            foreach (var dynamicColumn in dynamicColumns)
            {
                var r = new MetaMetricsTimeLineRequestDto
                {
                    Measurement = dynamicColumn.Measurement[0],
                    Range = dynamicColumn.Range,
                    Sublicense = dynamicColumn.Sublicense,
                    EachHours = dynamicColumn.EachHours
                };
                var t1 = await GetTimeLine(r);
                var count1 = t1.TimeValues.Count();
                var t2 = await GetTimeLine4Lines(dynamicColumn);
                //var count2 = t2.TimeValues.Count();
            }
        }

        private async Task<List<MetaMetricsTime4LinesRequestDto>> BuildDynamicColumns4Lines()
        {
            var dynamicColumns = new List<MetaMetricsTime4LinesRequestDto>();
            var position = 0;
            var licenses = await GetSublicenseList();

            var rangeDefault = _ranges.FirstOrDefault(z => z.Default) ?? _ranges.First(z => z.Display == "1w");

            var arrayLicenses = licenses as string[] ?? licenses.ToArray();

            foreach (var license in arrayLicenses.Take(10))
            {
                var installation = await GetInstallationBySublicense(license);
                var timeLine = new MetaMetricsTime4LinesRequestDto
                {
                    Sublicense = installation?.Sublicense,
                    Range = new MetaMetricsRangeInfoDto
                    {
                        Display = rangeDefault.Display,
                        Weeks = rangeDefault.Weeks,
                        Default = rangeDefault.Default
                    },
                    Measurement = _defaultMeasurements.ToList(),
                    EachHours = 1
                };
                dynamicColumns.Add(timeLine);
                System.Console.WriteLine($"Get Installation: {license}. Current: {position++} From: {arrayLicenses.Count()}");
            }

            return dynamicColumns;
        }

        public async Task TestSpeed()
        {
            var count = 0;
            var dynamicColumns = await BuildDynamicColumns();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var dynamicColumn in dynamicColumns)
            {
                count++;
                var threadName = $"Thread_{count}";
                var myThread = new Thread(() => TestGetTimeLine(dynamicColumn, stopWatch, threadName))
                {
                    Name = threadName
                };
                myThread.Start();
                System.Console.WriteLine($"Run: {myThread.Name}");
            }
        }

        private async Task<List<MetaMetricsTimeLineRequestDto>> BuildDynamicColumns()
        {
            var dynamicColumns = new List<MetaMetricsTimeLineRequestDto>();
            var position = 0;
            var licenses = await GetSublicenseList();
            
            var rangeDefault = _ranges.FirstOrDefault(z => z.Default) ?? _ranges.First(z => z.Display == "1w");

            var arrayLicenses = licenses as string[] ?? licenses.ToArray();

            foreach (var license in arrayLicenses)
            {
                var installation = await GetInstallationBySublicense(license);

                foreach (var measurement in _defaultMeasurements)
                {
                    var timeLine = new MetaMetricsTimeLineRequestDto
                    {
                        Sublicense = installation?.Sublicense,
                        Range = new MetaMetricsRangeInfoDto
                        {
                            Display = rangeDefault.Display,
                            Weeks = rangeDefault.Weeks,
                            Default = rangeDefault.Default
                        },
                        Measurement = measurement,
                        EachHours = 1
                    };
                    dynamicColumns.Add(timeLine);
                }
                System.Console.WriteLine($"Get Installation: {license}. Current: {position++} From: {arrayLicenses.Count()}");
            }

            return dynamicColumns;
        }

        

        private async Task InitModel()
        {
            var client = await MetaMetricsConnector.Main();
            var apps = await MetaMetricsConnector.QueryApps(client, (ex, q) => { });
            var measurements = await MetaMetricsConnector.QueryMeasurements(client, (ex, q) => { });
            var environments = await MetaMetricsConnector.QueryEnvironments(client, (ex, q) => { });
            var projects = await MetaMetricsConnector.QueryProjects(client, (ex, q) => { });
            var versions = await MetaMetricsConnector.QueryVersions(client, (ex, q) => { });
            var licenses = await MetaMetricsConnector.QueryLicenses(client, (ex, q) => { });
            var allSubLicenses = await MetaMetricsConnector.QuerySubLicenses(client, (ex, q) => { });
            var server = await MetaMetricsConnector.QueryServers(client, (ex, q) => { });
            var items = await MetaMetricsConnector.QueryItems(client, (ex, q) => { });
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

                List<string> comb;
                if (!subLicenses.TryGetValue(inst.Key, out comb))
                {
                    subLicenses[inst.Key] = comb = new List<string>();
                    comb.Add(inst.Key);
                }
                if (inst.Key != s)
                    comb.Add(s);
            }

            foreach (var expected in MetaMetricsInstallationService.Instance.Installations.Values)
            {
                if (!string.IsNullOrEmpty(expected.MapTo) || expected.Ignore) continue;
                if (!subLicenses.ContainsKey(expected.Key))
                {
                    subLicenses[expected.Key] =  new List<string>(new[] { expected.Key });
                }
            }
        }

        public void TestGetTimeLine(MetaMetricsTimeLineRequestDto dynamicColumn, Stopwatch stopWatch, string nameThread)
        {
            Task.Run(async () =>
            {
                var timeValue = await GetTimeLine(dynamicColumn);
                if (timeValue.TimeValues.Count() != 0)
                {
                    TimeValuesCount++;
                    System.Console.WriteLine($"timeValue.TimeValues: {timeValue.TimeValues.Count()} CountTotal: {TimeValuesCount}");
                }
                System.Console.WriteLine($"Сompleted: {nameThread}");
                var ts = stopWatch.Elapsed;
                var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                System.Console.WriteLine("RunTime " + elapsedTime);
            }).Wait();
        }

        private async Task<IEnumerable<string>> GetSublicenseList()
        {
            return await _metaMetricsService?.GetSublicenseList()!;
        }

        private async Task<MetaMetricsInstallationDto?> GetInstallationBySublicense(string license)
        {
            return await _metaMetricsService?.GetInstallationBySublicense(license)!;
        }

        private async Task<IEnumerable<MetaMetricsMeasurementDto>> GetMeasurementList()
        {
            return await _metaMetricsService?.GetMeasurementList()!;
        }

        private async Task<MetaMetricsTimeLineDto> GetTimeLine(MetaMetricsTimeLineRequestDto dto)
        {
            return await _metaMetricsService?.GetTimeLine(dto)!;
        }

        private async Task<List<MetaMetricsTime4LinesDTO>> GetTimeLine4Lines(MetaMetricsTime4LinesRequestDto dto)
        {
            return await _metaMetricsService?.GetTime4Lines(dto)!;
        }
    }

}
