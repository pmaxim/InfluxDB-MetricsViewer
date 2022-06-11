using System.Collections.Generic;

namespace MetaMetrics.Api
{
    public class MetaMetricsTime4LinesRequestDto
    {
        public string Sublicense { set; get; }
        public string License { set; get; }
        public string Version { set; get; }
        public string App { set; get; }
        public string Environment { set; get; }
        public string Project { set; get; }
        public string Server { set; get; }
        public List<string> Measurement { set; get; } = new List<string>();
        public MetaMetricsRangeInfoDto Range { set; get; }
        public int EachHours { set; get; }
    }

    public class MetaMetricsTime4LinesPaginationRequestDto
    {
        public List<string> Sublicenses { set; get; }
        public string License { set; get; }
        public string Version { set; get; }
        public string App { set; get; }
        public string Environment { set; get; }
        public string Project { set; get; }
        public string Server { set; get; }
        public List<string> Measurement { set; get; } = new List<string>();
        public MetaMetricsRangeInfoDto Range { set; get; }
        public int EachHours { set; get; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IsNotValue { get; set; }
        public bool IsCreateEmpty { get; set; }
        public SortColumns SortColumns { get; set; }
    }

    public class SortColumns
    {
        public string ColumnName { get; set; }
        public bool IsName { get; set; }
        public bool IsTime { get; set; }
        public bool IsServer { get; set; }
        public bool IsVersion { get; set; }
        public bool IsProduct { get; set; }
    }
}
