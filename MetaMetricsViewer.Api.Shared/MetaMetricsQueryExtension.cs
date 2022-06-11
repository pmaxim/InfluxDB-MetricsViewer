using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaMetrics.Api
{
    public static class MetaMetricsQueryExtension
    {
        public static MetaMetricsMeasurementType[] AllCounters()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                {
                    list.Add(measurement);
                }
            }
            return list.ToArray();
        }
        
        public static MetaMetricsMeasurementType[] AllItems()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsItemAttribute, MetaMetricsMeasurementType>().Any())
                {
                    list.Add(measurement);
                }
            }
            return list.ToArray();
        }
        
        public static MetaMetricsMeasurementType[] AllImportCounters()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                {
                    if (measurement.GetEnumAttribute<MetaMetricsImportAttribute, MetaMetricsMeasurementType>().Any())
                    {
                        list.Add(measurement);
                    }
                }
            }
            return list.ToArray();
        }
        
        public static MetaMetricsMeasurementType[] AllMetaKISImportCounters()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsMetaKISAttribute, MetaMetricsMeasurementType>().Any())
                {
                    if (measurement.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                    {
                        if (measurement.GetEnumAttribute<MetaMetricsImportAttribute, MetaMetricsMeasurementType>().Any())
                        {
                            list.Add(measurement);
                        }
                    }
                }
            }
            return list.ToArray();
        }
        
        public static MetaMetricsMeasurementType[] AllMetaTEXTImportCounters()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsMetaTEXTAttribute, MetaMetricsMeasurementType>().Any())
                {
                    if (measurement.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                    {
                        if (measurement.GetEnumAttribute<MetaMetricsImportAttribute, MetaMetricsMeasurementType>().Any())
                        {
                            list.Add(measurement);
                        }
                    }
                }
            }
            return list.ToArray();
        }
        
        public static MetaMetricsMeasurementType[] AllUsageCounters()
        {
            var list = new List<MetaMetricsMeasurementType>();
            foreach (MetaMetricsMeasurementType measurement in Enum.GetValues(typeof(MetaMetricsMeasurementType)))
            {
                if (measurement.GetEnumAttribute<MetaMetricsCounterAttribute, MetaMetricsMeasurementType>().Any())
                {
                    if (measurement.GetEnumAttribute<MetaMetricsUsageAttribute, MetaMetricsMeasurementType>().Any())
                    {
                        list.Add(measurement);
                    }
                }
            }
            return list.ToArray();
        }

        public static ATT[] GetEnumAttribute<ATT,T>(this T enu) where T : struct
        {
            var enumType = typeof(T);
            var memberInfos = enumType.GetMember(enu.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(ATT), false).Cast<ATT>().ToArray();
            return valueAttributes;
        }
        
        public static MetaMetricsQuery FromLastDays(this MetaMetricsQuery query, int lastdays)
        {
            query.LastDays = lastdays;
            return query;
        }
        
        public static MetaMetricsQuery OffsetHours(this MetaMetricsQuery query, int hours)
        {
            query.Offset = hours;
            return query;
        }
        
        public static MetaMetricsQuery Each(this MetaMetricsQuery query, int hours)
        {
            query.EveryHourCombine = hours;
            return query;
        }

        public static MetaMetricsQuery Every(this MetaMetricsQuery query, string every)
        {
            query.Every = every;
            return query;
        }

        public static MetaMetricsQuery CreateEmpty(this MetaMetricsQuery query, bool flag)
        {
            query.CreateEmpty = flag ? "true" : "false";
            return query;
        }

        public static MetaMetricsQuery From(this MetaMetricsQuery query, DateTime startTime)
        {
            query.StartTime = startTime;
            return query;
        }
        
        public static MetaMetricsQuery To(this MetaMetricsQuery query, DateTime? stopTime=null)
        {
            query.StopTime = stopTime;
            return query;
        }
        
        public static MetaMetricsQuery Measurement(this MetaMetricsQuery query, params string[] measurements)
        {
            if (measurements.Any())
                query.Filters.Add(MetaMetricsFilter.FromMeasurement(measurements));
            return query;
        }

        public static MetaMetricsQuery App(this MetaMetricsQuery query, string app)
        {
            if (!string.IsNullOrEmpty(app))
                query.Filters.Add(MetaMetricsFilter.FromApp(app));
            return query;
        }

        public static MetaMetricsQuery License(this MetaMetricsQuery query, string license)
        {
            if (!string.IsNullOrEmpty(license))
                query.Filters.Add(MetaMetricsFilter.FromLicense(license));
            return query;
        }

        public static MetaMetricsQuery Version(this MetaMetricsQuery query, string version)
        {
            if (!string.IsNullOrEmpty(version))
                query.Filters.Add(MetaMetricsFilter.FromVersion(version));
            return query;
        }

        public static MetaMetricsQuery Environment(this MetaMetricsQuery query, string env)
        {
            if (!string.IsNullOrEmpty(env))
                query.Filters.Add(MetaMetricsFilter.FromEnvironment(env));
            return query;
        }

        public static MetaMetricsQuery Project(this MetaMetricsQuery query, string project)
        {
            if (!string.IsNullOrEmpty(project))
                query.Filters.Add(MetaMetricsFilter.FromProject(project));
            return query;
        }

        public static MetaMetricsQuery Server(this MetaMetricsQuery query, string server)
        {
            if (!string.IsNullOrEmpty(server))
                query.Filters.Add(MetaMetricsFilter.FromServer(server));
            return query;
        }

        public static MetaMetricsQuery MeasurementItems(this MetaMetricsQuery query, params string[] measurements)
        {
            if (measurements.Any())
                query.Filters.Add(MetaMetricsFilter.FromMeasurementItems(measurements));
            return query;
        }       
        
        public static MetaMetricsQuery Sublicense(this MetaMetricsQuery query, params string[] license)
        {
            if (license.Any())
                query.Filters.Add(MetaMetricsFilter.FromSublicense(license));
            return query;
        }
        public static MetaMetricsQuery Measurement(this MetaMetricsQuery query, params MetaMetricsMeasurementType[] measurements)
        {
            if (measurements.Any())
                query.Filters.Add(MetaMetricsFilter.FromMeasurement(measurements.Select(n=>n.ToString()).ToArray()));
            return query;
        }

        public static DateTime Group(this DateTime date, int eachHour)
        {
            var dat = date;
            if (eachHour > 0 )
            {
                var hour = (date.Hour / eachHour) * eachHour;
                dat = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);//.AddHours(eachHour).AddMinutes(-1);
            }

            //if (dat > maxDate)
                //return maxDate;
            return dat;
        }
        
        public static MetaMetricsQuery OnField(this MetaMetricsQuery query, string fieldName=null)
        {
            if (!string.IsNullOrEmpty(fieldName))
                query.Filters.Add(MetaMetricsFilter.FromField(fieldName));
            return query;
        }

        public static MetaMetricsQuery OnTotal(this MetaMetricsQuery query)
        {
            query.Filters.Add(MetaMetricsFilter.FromField(MetaMetricsQuery.field__total));
            return query;
        }

        public static MetaMetricsQuery OnValue(this MetaMetricsQuery query)
        {
            query.Filters.Add(MetaMetricsFilter.FromField(MetaMetricsQuery.field__value));
            return query;
        }

        public static string ToInstallation(this string txt)
        {
            var res = txt;
            if (res != null)
            {
                res = res.Replace("\\ ", " ");
                res = res.Replace("\\,", ",");
            }
            return res;
        }
    }
}