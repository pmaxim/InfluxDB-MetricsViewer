using MetaMetricsViewer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Api = MetaMetrics.Api;

namespace MetaMetricsViewer.Web.Angular.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class MetaMetricsController : ControllerBase
    {
        private readonly IMetaMetricsService _service;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MetaMetricsController> _logger;
        private IMemoryCache cache;

        public MetaMetricsController(IMetaMetricsService service,
            IMemoryCache memoryCache,
            IConfiguration configuration, ILogger<MetaMetricsController> logger)
        {
            _service = service;
            cache = memoryCache;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetSublicenseList()
        {
            if (!cache.TryGetValue("sublicenses", out IEnumerable<string> sublicenses))
            {
                sublicenses = await _service.GetSublicenseList();
                if (sublicenses != null)
                {
                    cache.Set("sublicenses", sublicenses,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
                }
            }
            return sublicenses;
        }

        [HttpPost]
        public async Task<IEnumerable<string>> GetSublicenseListReq([FromBody] Api.MetaMetricsRangeInfoDto req)
        {
            if (!cache.TryGetValue("sublicenses", out IEnumerable<string> sublicenses))
            {
                sublicenses = await _service.GetSublicenseList(req);
                if (sublicenses != null)
                {
                    cache.Set("sublicenses", sublicenses,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
                }
            }
            return sublicenses;
        }

        [HttpGet]
        public async Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementList()
        {
            if (cache.TryGetValue("measurements", out IEnumerable<Api.MetaMetricsMeasurementDto> measurements))
                return measurements;
            measurements = await _service.GetMeasurementList();
            if (measurements != null)
            {
                cache.Set("measurements", measurements,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
            }
            return measurements;
        }

        [HttpPost]
        public async Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementListBySublicense([FromBody] Api.MetaMetricsMeasureRequestDto req)
        {
            var result = await _service.GetMeasurementListBySublicense(req);
            return result;
        }

        [HttpPost]
        public async Task<Api.MetaMetricsInstallationDto> GetInstallationBySublicense([FromBody] string sublicense)
        {
            if (cache.TryGetValue(sublicense, out Api.MetaMetricsInstallationDto installation)) return installation;
            installation = await _service.GetInstallationBySublicense(sublicense);
            if (installation != null)
            {
                cache.Set(sublicense, installation,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));
            }
            return installation;
        }

        [HttpPost]
        public async Task<Api.MetaMetricsTimeLineDto> GetTimeLine([FromBody] Api.MetaMetricsTimeLineRequestDto req)
        {
            var result = await _service.GetTimeLine(req);
            return result;
        }

        [HttpGet]
        public async Task<Api.MetaMetricsFiltersDTO> GetFilters()
        {
            if (cache.TryGetValue("filters", out Api.MetaMetricsFiltersDTO filters)) return filters;
            filters = await _service.GetFilters();
            if (filters != null)
            {
                cache.Set("filters", filters,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
            }
            return filters;
        }

        [HttpPost]
        public async Task<Api.MetaMetricsFiltersDTO> GetFiltersValue([FromBody] Api.MetaMetricsRangeInfoDto req)
        {
            if (cache.TryGetValue(KeyBuildFilter(req), out Api.MetaMetricsFiltersDTO filters)) return filters;
            filters = await _service.GetFiltersValue(req);
            if (filters != null)
            {
                cache.Set(KeyBuildFilter(req), filters,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
            }
            return filters;
        }

        [HttpPost]
        public async Task<List<Api.MetaMetricsTime4LinesDTO>> GetTime4Lines([FromBody] Api.MetaMetricsTime4LinesRequestDto req)
        {
            var result = await _service.GetTime4Lines(req);
            return result;
        }

        //without caching
        [HttpPost]
        public async Task<Api.MetaMetricsTime4LinesPaginationDTO> GetTime4LinesPagination1([FromBody] Api.MetaMetricsTime4LinesPaginationRequestDto req)
        {
            var sub = await _service.GetTime4LinesPaginationSub(req);

            var m = new Api.MetaMetricsTime4LinesPaginationDTO
            {
                Count = sub.Count()
            };

            sub = SortColumnsSub(sub, req.SortColumns);
            
            if (req.PageNumber > 1) sub = sub.Skip((req.PageNumber - 1) * req.PageSize).ToList();
            if (!sub.Any()) return m;
            sub = sub.Take(req.PageSize).ToList();
            m.List = BuildLineModelQueryMeasurement(sub);

            return m;
        }


        [HttpPost]
        public async Task<Api.MetaMetricsTime4LinesPaginationDTO> GetTime4LinesPagination([FromBody] Api.MetaMetricsTime4LinesPaginationRequestDto req)
        {
            if (!cache.TryGetValue(KeyBuild(req), out List<Api.MetaMetricsInstallationTimeLine> sub))
            {
                sub = await _service.GetTime4LinesPaginationSub(req);
                if (sub != null)
                {
                    cache.Set(KeyBuild(req), sub, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));
                }
            }

            var m = new Api.MetaMetricsTime4LinesPaginationDTO
            {
                Count = sub.Count()
            };

            sub = SortColumnsSub(sub, req.SortColumns);

            if (req.PageNumber > 1) sub = sub.Skip((req.PageNumber - 1) * req.PageSize).ToList();
            if (!sub.Any()) return m;
            sub = sub.Take(req.PageSize).ToList();
            m.List = BuildLineModelQueryMeasurement(sub);

            return m;
        }

        private static List<Api.MetaMetricsInstallationTimeLine> SortColumnsSub(List<Api.MetaMetricsInstallationTimeLine> sub, Api.SortColumns sort)
        {
            if(string.IsNullOrEmpty(sort.ColumnName)) return sub;
            switch (sort.ColumnName)
            {
                case "name":
                    return sort.IsName ? sub.OrderByDescending(z => z.Name).ToList() : sub;
                case "time":
                    return sort.IsTime ? sub.OrderByDescending(z => z.LastTillTime).ToList() : sub.OrderBy(z => z.LastTillTime).ToList();
                case "server":
                    return sort.IsServer ? sub.OrderByDescending(z => z.Server).ToList() : sub.OrderBy(z => z.Server).ToList();
                case "version":
                    return sort.IsVersion ? sub.OrderByDescending(z => z.Version).ToList() : sub.OrderBy(z => z.Version).ToList();
                case "product":
                    return sort.IsProduct ? sub.OrderByDescending(z => z.Products).ToList() : sub.OrderBy(z => z.Products).ToList();
            }
            return sub;
        }

        private static int KeyBuild(Api.MetaMetricsTime4LinesPaginationRequestDto req)
        {
            var pageNumber = req.PageNumber;
            var pageSize = req.PageSize;
            req.PageNumber = 0;
            req.PageSize = 0;
            var sortColumns = JsonConvert.SerializeObject(req.SortColumns);
            req.SortColumns = new Api.SortColumns();
            var json = JsonConvert.SerializeObject(req);
            req.PageSize = pageSize;
            req.PageNumber = pageNumber;
            req.SortColumns = JsonConvert.DeserializeObject<Api.SortColumns>(sortColumns);
            return json.GetHashCode();
        }

        private static int KeyBuildFilter(Api.MetaMetricsRangeInfoDto req)
        {
            var json = JsonConvert.SerializeObject(req);
            return json.GetHashCode();
        }

        private static List<Api.MetaMetricsTime4LinesDTO> BuildLineModelQueryMeasurement(List<Api.MetaMetricsInstallationTimeLine> sub)
        {
            var m = new List<Api.MetaMetricsTime4LinesDTO>();
            foreach (var p in sub)
            {
                var i = new Api.MetaMetricsTime4LinesDTO
                {
                    Sublicense = p.Sublicense,
                    Name = p.Name,
                    Server = p.Server,
                    Version = p.Version,
                    Time = p.LastTillTime.ToShortDateString(),
                    App = p.Products
                };

                foreach (var t in p.TimeValues)
                {
                    var it = new Api.MetaMetricsTimeLineDto
                    {
                        Measurement = t.MeasurementName,
                        TimeValues = t.TimeValues
                            .Select(value => new Api.MetaMetricsTimeValueDto
                                { Value = value.Value, Timestamp = value.Till }).ToList()
                    };
                    i.TimeLines.Add(it);
                }
                m.Add(i);
            }
            return m;
        }
    }
}
