using MetaMetrics.Api;

namespace MetaMetricsViewer.Console.Models
{
    public class LinesMetaMetricsTime4LinesDTO
    {
        public string? Name { get; set; }
        public string? Sublicense { get; set; }
        public string? Server { get; set; }
        public string? Version { get; set; }
        public string? App { get; set; }
        public List<MetaMetricsTime4LinesDTO> List { get; set; } = new List<MetaMetricsTime4LinesDTO>();
    }
}
