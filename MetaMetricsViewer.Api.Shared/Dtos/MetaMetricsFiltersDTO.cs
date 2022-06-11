using System.Collections.Generic;

namespace MetaMetrics.Api
{
    public class MetaMetricsFiltersDTO
    {
        public IEnumerable<string> Apps { get; set; }
        public IEnumerable<string> Environments { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public IEnumerable<string> Versions { get; set; }
        public IEnumerable<string> Licenses { get; set; }
        public IEnumerable<string> Servers { get; set; }
        public IEnumerable<string> Items { get; set; }
        public IEnumerable<string> SubLicenses { get; set; }
        public IEnumerable<string> Measurements { get; set; }
    }
}
