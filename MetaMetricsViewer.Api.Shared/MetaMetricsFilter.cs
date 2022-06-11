using System.Linq;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;

namespace MetaMetrics.Api
{
    public class MetaMetricsFilter
    {
        public string RecordName { set; get; } = "r";
        public string FieldName { set; get; } = "item";
        public string CompareType { set; get; } = "==";
        public string Delimiter { set; get; } = "\"";
        public string CompareValue { set; get; } = "";
        public string[] CompareValues { set; get; } 
        public string Expression => CompareValues==null?$"({RecordName}) => {RecordName}[\"{FieldName}\"] {CompareType} {Delimiter}{CompareValue.Replace("\\","~").Replace("~","\\\\")}{Delimiter}":($"({RecordName}) => "+string.Join(" or ",CompareValues.Select(v=>$"{RecordName}[\"{FieldName}\"] {CompareType} {Delimiter}{v.Replace("\\","~").Replace("~","\\\\")}{Delimiter}")));

        public override string ToString()
        {
            return Expression;
        }

        public static MetaMetricsFilter FromMeasurement(params string[] measurements)
        {
            return new MetaMetricsFilter() { FieldName = "_measurement", CompareValues = measurements };
        }

        public static MetaMetricsFilter FromMeasurementItems(params string[] measurements)
        {
            return new MetaMetricsFilter() { FieldName = "_measurement", CompareValues = measurements.Select(s=>s+"__items").ToArray() };
        }
        
        public static MetaMetricsFilter FromSublicense(params string[] sublicense)
        {
            return new MetaMetricsFilter() { FieldName = "sublicense", CompareValues = sublicense };
        }
        
        public static MetaMetricsFilter FromField(string fieldname)
        {
            return new MetaMetricsFilter() { FieldName = "_field", CompareValue = fieldname };
        }

        public static MetaMetricsFilter FromApp(string app)
        {
            return new MetaMetricsFilter() { FieldName = "app", CompareValue = app };
        }

        public static MetaMetricsFilter FromLicense(string license)
        {
            return new MetaMetricsFilter() { FieldName = "license", CompareValue = license };
        }

        public static MetaMetricsFilter FromVersion(string version)
        {
            return new MetaMetricsFilter() { FieldName = "version", CompareValue = version };
        }

        public static MetaMetricsFilter FromEnvironment(string env)
        {
            return new MetaMetricsFilter() { FieldName = "env", CompareValue = env };
        }

        public static MetaMetricsFilter FromProject(string project)
        {
            return new MetaMetricsFilter() { FieldName = "project", CompareValue = project };
        }
        public static MetaMetricsFilter FromServer(string server)
        {
            return new MetaMetricsFilter() { FieldName = "server", CompareValue = server };
        }

        public static MetaMetricsFilter FromItems(string server)
        {
            return new MetaMetricsFilter() { FieldName = "server", CompareValue = server };
        }
    }
}