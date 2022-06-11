using System.Collections.Generic;
using System.Threading.Tasks;
using Api = MetaMetrics.Api;

namespace MetaMetricsViewer.Service
{
    public interface IMetaMetricsService
    {
        Task<IEnumerable<string>> GetSublicenseList();
        Task<IEnumerable<string>> GetSublicenseList(Api.MetaMetricsRangeInfoDto req);
        Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementList();
        Task<IEnumerable<Api.MetaMetricsMeasurementDto>> GetMeasurementListBySublicense(Api.MetaMetricsMeasureRequestDto req);
        Task<Api.MetaMetricsInstallationDto> GetInstallationBySublicense(string sublicense);
        Task<Api.MetaMetricsTimeLineDto> GetTimeLine(Api.MetaMetricsTimeLineRequestDto req);
        Task<List<Api.MetaMetricsTime4LinesDTO>> GetTime4Lines(Api.MetaMetricsTime4LinesRequestDto req);
        Task<Api.MetaMetricsFiltersDTO> GetFilters();
        Task<Api.MetaMetricsFiltersDTO> GetFiltersValue(Api.MetaMetricsRangeInfoDto req);
        Task<Api.MetaMetricsTime4LinesPaginationDTO> GetTime4LinesPagination(Api.MetaMetricsTime4LinesPaginationRequestDto req);

        Task<List<Api.MetaMetricsInstallationTimeLine>> GetTime4LinesPaginationSub(Api.MetaMetricsTime4LinesPaginationRequestDto req);
    }
}
