using System.Collections.Generic;

namespace MetaMetrics.Api
{
    public class MetaMetricsTime4LinesPaginationDTO
    {
        public int Count { get; set; }
        public List<MetaMetricsTime4LinesDTO> List { get; set; } = new List<MetaMetricsTime4LinesDTO>();
    }
}
