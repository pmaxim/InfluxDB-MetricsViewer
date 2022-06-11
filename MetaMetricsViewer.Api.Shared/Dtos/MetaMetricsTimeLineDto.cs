using System.Collections.Generic;

namespace MetaMetrics.Api
{
    public class MetaMetricsTimeLineDto
    {
        public string Measurement { get; set; }
        public IEnumerable<MetaMetricsTimeValueDto> TimeValues { set; get; }
    }
}
