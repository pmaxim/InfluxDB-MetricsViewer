using System;
using System.Collections.Generic;
using System.Text;

namespace MetaMetrics.Api
{
    
    public class MetaMetricsQuery
    {
        
        public const string default_bucket = "MetaMetrics";
        
        public const string field__total = "total";
        public const string field__value = "value";
        public const string field__sublicense = "value";

        public bool IsNotValue = false;

        public MetaMetricsQuery(string bucket = null, bool? isNotValue = null)
        {
            Bucket = bucket ?? default_bucket;
            if (isNotValue != null) IsNotValue = (bool)isNotValue;
        }

        public List<MetaMetricsFilter> Filters { get; } = new List<MetaMetricsFilter>();

        public int LastDays { set; get; } = 7;
        public int Offset { set; get; } = 0;
        public int OffsetCombine { set; get; } = 0;
        
        public int EveryHour { set; get; } = 1;
        public string Every { get; set; }
        public int EveryHourCombine { set; get; } = 6;
        
        public string GroupBy { set; get; } = "sublicense";
        public string SubGroupBy { set; get; } = "_measurement";
        public string Aggregate { set; get; } = "sum";

        public DateTime? StartTime { set; get; }
        public DateTime? StopTime { set; get; }
        public string Bucket { set; get; }
        public string CreateEmpty { get; set; }


        public DateTime RequestStartTime { set; get; }
        public DateTime RequestEndTime { set; get; }
        public DateTime RealStartTime { set; get; }
        public DateTime RealEndTime { set; get; }
        public string Query
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine($"from(bucket: \"{Bucket}\")");
                RequestStartTime = DateTime.Today.AddDays(-LastDays).AddHours(Offset);
                RequestEndTime = DateTime.Now;
                var stopdate = "now()";
                if (StartTime != null)
                {
                    RequestStartTime = ((DateTime)StartTime).AddHours(Offset);
                    if (StopTime != null)
                    {
                        RequestEndTime = (DateTime)StopTime;
                        stopdate = RequestEndTime.ToString("yyyy-MM-ddTHH:mm:ss")+ "Z";
                    }
                }
                else if (StopTime != null)
                {
                    RequestEndTime = (DateTime)StopTime;
                    stopdate = RequestEndTime.ToString("yyyy-MM-ddTHH:mm:ss")+ "Z";
                }
                
                sb.AppendLine($"  |> range(start: {RequestStartTime.ToString("yyyy-MM-ddTHH:mm:ss") + "Z"}, stop: {stopdate})");
                
                foreach (var filter in Filters)
                {
                    sb.AppendLine($"  |> filter(fn: {filter.Expression})");
                }

                //sb.AppendLine($"  |> group(columns: [\"{GroupBy}\", \"{SubGroupBy}\", \"item\"])");

                //sb.AppendLine($"  |> keep(columns: [\"app\", \"server\", " +
                //              $"\"_time\", \"_value\", \"result\", \"table\", \"_start\", \"_stop\", " +
                //              $"\"_field\", \"_measurement\", \"env\", \"license\", \"mtype\", \"project\", \"sublicense\", \"unit\", \"version\"])");

                if (IsNotValue)
                {
                    sb.AppendLine("  |> distinct()");
                }
                else
                {
                    sb.AppendLine(string.IsNullOrEmpty(Every)
                        ? $"  |> aggregateWindow(every: {EveryHour}h, fn: {Aggregate}, createEmpty: {CreateEmpty})"
                        : $"  |> aggregateWindow(every: {Every}, fn: {Aggregate}, createEmpty: {CreateEmpty})");

                    sb.AppendLine($"  |> yield(name: \"{Aggregate}\")");
                }
              
                
                return sb.ToString();
            }
        }
    }
}