namespace MetaMetricsViewer.Console.Models
{
    public class MetaMetricsRangeInfo
    {
        public string? Display { set; get; }
        public int Days { set; get; }
        public int Weeks { set; get; }
        public int Monthes { set; get; }
        public int Years { set; get; }
        public bool Default { set; get; }
        public bool CurrentWeek { set; get; }
        public bool LastWeek { set; get; }
        public bool CurrentMonth { set; get; }
        public bool LastMonth { set; get; }
        public bool CurrentQuarter { set; get; }
        public bool LastQuarter { set; get; }
        public bool CurrentYear { set; get; }
        public bool LastYear { set; get; }

        public DateTime StartDate()
        {
            var basedate = DateTime.Today;
            if (Days > 0)
                basedate = basedate.AddDays(-Days);
            else if (Weeks > 0)
                basedate = basedate.AddDays(-Weeks*7);
            else if (Monthes > 0)
                basedate = basedate.AddMonths(-Monthes);
            else if (Years > 0)
                basedate = basedate.AddYears(-Years);
            else if (CurrentWeek)
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek==0 ? 6 : ((int)basedate.DayOfWeek-1)));
            else if (CurrentMonth)
                basedate = new DateTime(basedate.Year, basedate.Month, 1);
            else if (CurrentQuarter)
                basedate = new DateTime(basedate.Year, ((basedate.Month-1)/3)*3+1, 1);
            else if (CurrentYear)
                basedate = new DateTime(basedate.Year, 1, 1);
            else if (LastWeek)
            {
                basedate = basedate.AddDays(-7);
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek==0 ? 6 : ((int)basedate.DayOfWeek-1)));
            }
            else if (LastMonth)
            {
                basedate = basedate.AddMonths(-1);
                basedate = new DateTime(basedate.Year, basedate.Month, 1);
            }
            else if (LastQuarter)
            {
                basedate = basedate.AddMonths(-3);
                basedate = new DateTime(basedate.Year, ((basedate.Month-1)/3)*3+1, 1);
            }
            else if (LastYear)
            {
                basedate = basedate.AddYears(-1);
                basedate = new DateTime(basedate.Year, 1, 1);
            }

            return basedate;
        }

        public static MetaMetricsRangeInfo[] InitRanges()
        {
            return new[]
            {
                new MetaMetricsRangeInfo() { Display ="Today", Days  = 0 },
                new MetaMetricsRangeInfo() { Display ="Yesterday", Days  = 1 },
                new MetaMetricsRangeInfo() { Display ="2d", Days  = 2 },
                new MetaMetricsRangeInfo() { Display ="3d", Days  = 3 },
                new MetaMetricsRangeInfo() { Display ="4d", Days  = 4 },
                new MetaMetricsRangeInfo() { Display ="1w", Weeks  = 1, Default = true },
                new MetaMetricsRangeInfo() { Display ="2w", Weeks  = 2 },
                new MetaMetricsRangeInfo() { Display ="4w", Weeks  = 4 },
                new MetaMetricsRangeInfo() { Display ="1m", Monthes  = 1 },
                new MetaMetricsRangeInfo() { Display ="2m", Monthes  = 2 },
                new MetaMetricsRangeInfo() { Display ="3m", Monthes  = 3 },
                new MetaMetricsRangeInfo() { Display ="4m", Monthes  = 4 },
                new MetaMetricsRangeInfo() { Display ="6m", Monthes  = 6 },
                new MetaMetricsRangeInfo() { Display ="1y", Years  = 1 },
                new MetaMetricsRangeInfo() { Display ="Current week", CurrentWeek = true },
                new MetaMetricsRangeInfo() { Display ="Current month", CurrentMonth = true },
                new MetaMetricsRangeInfo() { Display ="Current quarter", CurrentQuarter = true },
                new MetaMetricsRangeInfo() { Display ="Current year", CurrentYear = true },
                new MetaMetricsRangeInfo() { Display ="Last week", LastWeek = true },
                new MetaMetricsRangeInfo() { Display ="Last month", LastMonth = true },
                new MetaMetricsRangeInfo() { Display ="Last quarter", LastQuarter = true },
                new MetaMetricsRangeInfo() { Display ="Last year", LastYear = true },
            };
        }
    }
}
