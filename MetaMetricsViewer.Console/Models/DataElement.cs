using MetaMetrics.Api;

namespace MetaMetricsViewer.Console.Models
{
    public class DataElement
    {
        public int Position { get; set; }
        public MetaMetricsInstallationDto? Installation { get; set; }
        public string? Sublicense { get; set; }
    }
}
