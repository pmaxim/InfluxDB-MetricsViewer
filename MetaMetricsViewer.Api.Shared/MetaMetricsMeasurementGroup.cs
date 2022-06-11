using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace MetaMetrics.Api
{
    public class MetaMetricsMeasurementGroup: IComparer<MetaMetricsTimeValue>
    {
        public MetaMetricsInstallationTimeLine TimeLine { private set; get; }
        public MetaMetricsMeasurementGroup(MetaMetricsInstallationTimeLine timeLine, string measurement)
        {
            TimeLine = timeLine;
            MeasurementName = measurement;
        }

        public double Avg =>TimeValues.Any(n=>n.Value>0)? TimeValues.Where(n=>n.Value>0).Average(n => n.Value):0;
        public long Sum => TimeValues.Sum(n => n.Value);
        public long Max => TimeValues.Max(n => n.Value);

        private string _measurementName;

        public string MeasurementName
        {
            private set
            {
                _measurementName = value;
                Measurement = _measurementName.Replace('-','_').ToEnum(MetaMetricsMeasurementType.Unknown);
            }
            get
            {
                if (Measurement != MetaMetricsMeasurementType.Unknown)
                    return Measurement.ToString();
                return _measurementName;
            }
        }
        
        public string MeasurementDisplay
        {
            get
            {
                if (Measurement != MetaMetricsMeasurementType.Unknown)
                {
                    return Measurement.GetEnumAttribute<MetaMetricsTitleAttribute, MetaMetricsMeasurementType>()?.FirstOrDefault()?.Title ?? MeasurementName;
                }
                return MeasurementName;
            }
        }
        public string MeasurementDescription
        {
            get
            {
                if (Measurement != MetaMetricsMeasurementType.Unknown)
                {
                    return Measurement.GetEnumAttribute<MetaMetricsTitleAttribute, MetaMetricsMeasurementType>()?.FirstOrDefault()?.Description ?? "";
                }
                return "";
            }
        }
        
        public MetaMetricsMeasurementType Measurement { private set; get; }

        public Dictionary<string, MetaMetricsItemTimeValues> ItemTimeValues { set; get; } = new Dictionary<string, MetaMetricsItemTimeValues>();
        public List<MetaMetricsTimeValue> TimeValues { set; get; } = new List<MetaMetricsTimeValue>();
        public DateTime FirstTime => TimeValues.Min(n => n.From);
        public DateTime LastFromTime => TimeValues.Max(n => n.From);
        public DateTime LastTillTime => TimeValues.Max(n => n.Till);

        private MetaMetricsQuery query;
        public MetaMetricsQuery Query
        {
            get { return query ?? TimeLine.Query;}
            set { query = value; }
        }

        public override string ToString()
        {
            return $"{MeasurementName} ({TimeValues.Count}: {Sum}) [{FirstTime} - {LastTillTime}]";
        }

        public void Add(string itemname, Instant time, long value, int combine, int offset, DateTime maxDate)
        {
            var date = time.ToDateTimeUtc();
            date = date.ToLocalTime().AddHours(offset);//.Group(1);
            var till = date.AddHours(1).AddMinutes(-1);
            if (till > maxDate)
                till = maxDate;
            var newValue = new MetaMetricsTimeValue(this) { From = date, Till = till, Value = value, Exists = true};
            
            var index = TimeValues.BinarySearch(newValue, this);
            if (index < 0)
            {
                index = ~index;
                TimeValues.Insert(index, newValue);
            }
            else
            {
                var item = TimeValues[index];
                item.Value += value;
            }

            if (!string.IsNullOrEmpty(itemname))
            {
                MetaMetricsItemTimeValues item;
                if (!ItemTimeValues.TryGetValue(itemname, out item))
                {
                    item = new MetaMetricsItemTimeValues(this) { ItemName = itemname };
                    ItemTimeValues[item.ItemName] = item;
                }
                item.Add(newValue.From, newValue.Till, value);
            }
        }
        
        public int Compare(MetaMetricsTimeValue x, MetaMetricsTimeValue y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.From.CompareTo(y.From);
        }
    }
}