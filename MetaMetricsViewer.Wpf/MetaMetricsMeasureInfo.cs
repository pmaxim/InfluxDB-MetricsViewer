using System.Linq;
using MetaMetrics.Api;

namespace MetaMetricsViewer.Wpf
{
    public class MetaMetricsMeasureInfo
    {
        public string MeasurementName { set; get; }
        public string MeasurementDisplay { set; get; }
        public MetaMetricsMeasurementType MeasurementType { set; get; }

        public MetaMetricsMeasureInfo(string measurementName)
        {
            MeasurementName = measurementName;
            MeasurementDisplay = measurementName;
            MeasurementType = measurementName.ToEnum(MetaMetricsMeasurementType.Unknown);
            if (MeasurementType != MetaMetricsMeasurementType.Unknown)
            {
                if (MeasurementType.GetEnumAttribute<MetaMetricsUsageAttribute, MetaMetricsMeasurementType>().Any())
                    IsUsage = true;
                if (MeasurementType.GetEnumAttribute<MetaMetricsImportAttribute, MetaMetricsMeasurementType>().Any())
                    IsImport = true;
                if (MeasurementType.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                    IsCounter = true;
                if (MeasurementType.GetEnumAttribute<MetaMetricsItemAttribute, MetaMetricsMeasurementType>().Any())
                    IsItem = true;
                if (MeasurementType.GetEnumAttribute<MetaMetricsMetaKISAttribute, MetaMetricsMeasurementType>().Any())
                    IsMetaKIS = true;
                if (MeasurementType.GetEnumAttribute<MetaMetricsMetaTEXTAttribute, MetaMetricsMeasurementType>().Any())
                    IsMetaTEXT = true;
                MeasurementDisplay = MeasurementType.GetEnumAttribute<MetaMetricsTitleAttribute, MetaMetricsMeasurementType>()?.FirstOrDefault()?.Title ?? MeasurementDisplay;
            }
        }

        public bool IsMetaTEXT { get; set; }

        public bool IsMetaKIS { get; set; }

        public bool IsCounter { get; set; }

        public bool IsItem { get; set; }

        public bool IsImport { get; set; }

        public bool IsUsage { get; set; }
    }
}