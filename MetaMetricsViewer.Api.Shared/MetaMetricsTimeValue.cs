using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MetaMetrics.Api
{
    public class MetaMetricsItemTimeValues : IComparer<MetaMetricsItemTimeValue>
    {
        public MetaMetricsMeasurementGroup Group;
        public MetaMetricsItemTimeValues(MetaMetricsMeasurementGroup group)
        {
            this.Group = group;
        }

        public string ItemName { set; get; }

        public string Title => ItemName.ToEnum(MetaMetricsMeasurementType.Unknown).GetEnumAttribute<MetaMetricsTitleAttribute, MetaMetricsMeasurementType>()?.FirstOrDefault()?.Title ?? ItemName;

        public string Description => ItemName.ToEnum(MetaMetricsMeasurementType.Unknown).GetEnumAttribute<MetaMetricsTitleAttribute, MetaMetricsMeasurementType>()?.FirstOrDefault()?.Description ?? "";

        public List<MetaMetricsItemTimeValue> ItemTimeValues { set; get; } = new List<MetaMetricsItemTimeValue>();
        public MetaMetricsQuery Query => Group.Query;

        public void Add(DateTime from, DateTime till, long value)
        {         
            var newValue = new MetaMetricsItemTimeValue() { From = from, Till = till, Value = value, Exists = true};
            var index = ItemTimeValues.BinarySearch(newValue, this);
            if (index < 0)
            {
                index = ~index;
                ItemTimeValues.Insert(index, newValue);
            }
            else
            {
                var item = ItemTimeValues[index];
                item.Value += value;
            }
        }       
        
        public DateTime FirstTime => ItemTimeValues.Min(n => n.From);
        public DateTime LastFromTime => ItemTimeValues.Max(n => n.From);
        public DateTime LastTillTime => ItemTimeValues.Max(n => n.Till);

        public int Compare(MetaMetricsItemTimeValue x, MetaMetricsItemTimeValue y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.From.CompareTo(y.From);
        }
    }

    public class MetaMetricsItemTimeValue : IMetaMetricsTimeValue
    {    
        public string FromTitle => From.ToString("dd.MM. HH:mm");
        public string Title => $"{From.ToString("dd.MM. HH:mm").Replace(" 00:00","")}: {Value.ToString("N0")}";
        public DateTime From { set; get; }
        public DateTime Till { set; get; }
        public long Value { set; get; }
        public bool Exists { set; get; }

        public override string ToString()
        {
            return $"{From:yyyy-MM-ddTHH:mm:ss}Z ({Value})";
        }
    }

    public class MetaMetricsTimeValue : IMetaMetricsTimeValue
    {
        public MetaMetricsTimeValue(MetaMetricsMeasurementGroup group)
        {
            this.Group = group;
        }

        public MetaMetricsMeasurementGroup Group { private set; get; }
        public string MeasurementName => Group.MeasurementName;
        public MetaMetricsMeasurementType Measurement => Group.Measurement;
        public string FromTitle => From.ToString("dd.MM. HH:mm");
        public string Title => $"{From.ToString("dd.MM. HH:mm").Replace(" 00:00","")}: {Value.ToString("N0")}";
        public DateTime From { set; get; }
        public DateTime Till { set; get; }
        public long Value { set; get; }
        public bool Exists { set; get; }

        public override string ToString()
        {
            return $"{From:yyyy-MM-ddTHH:mm:ss}Z ({Value}): {MeasurementName}";
        }
        
    }
}