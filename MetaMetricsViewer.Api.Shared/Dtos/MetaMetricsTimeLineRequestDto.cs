namespace MetaMetrics.Api
{
    public class MetaMetricsTimeLineRequestDto
    {
        public string Sublicense { set; get; }
        public string Measurement { set; get; }
        public MetaMetricsRangeInfoDto Range { set; get; }
        public int EachHours { set; get; }
    }
}
