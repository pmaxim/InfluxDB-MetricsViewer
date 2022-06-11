using System;

namespace MetaMetrics.Api
{
    public class MetaMetricsMeasureRequestDto
    {
        public string Sublicense { set; get; }
        public MetaMetricsRangeInfoDto Range { set; get; }
    }
}
