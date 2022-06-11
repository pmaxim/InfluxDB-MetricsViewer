namespace MetaMetrics.Api
{
    public class InfluxDBOptions
    {
        public const string Section = "InfluxDBOptions";

        public string Url { get; set; }
        public string Token { get; set; }
        public string Bucket { get; set; }
        public string Org { get; set; }
    }
}