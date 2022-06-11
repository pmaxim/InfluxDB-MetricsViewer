using System;

namespace MetaMetrics.Api
{
    public class MetaMetricsRangeInfoDto
    {
        public string Display { set; get; }
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
        public string Every { get; set; }

        public DateTime StartDate()
        {
            var basedate = DateTime.Today;
            if (Days > 0)
                basedate = basedate.AddDays(-Days);
            else if (Weeks > 0)
                basedate = basedate.AddDays(-Weeks * 7);
            else if (Monthes > 0)
                basedate = basedate.AddMonths(-Monthes);
            else if (Years > 0)
                basedate = basedate.AddYears(-Years);
            else if (CurrentWeek)
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek == 0 ? 6 : ((int)basedate.DayOfWeek - 1)));
            else if (CurrentMonth)
                basedate = new DateTime(basedate.Year, basedate.Month, 1);
            else if (CurrentQuarter)
                basedate = new DateTime(basedate.Year, ((basedate.Month - 1) / 3) * 3 + 1, 1);
            else if (CurrentYear)
                basedate = new DateTime(basedate.Year, 1, 1);
            else if (LastWeek)
            {
                basedate = basedate.AddDays(-7);
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek == 0 ? 6 : ((int)basedate.DayOfWeek - 1)));
            }
            else if (LastMonth)
            {
                basedate = basedate.AddMonths(-1);
                basedate = new DateTime(basedate.Year, basedate.Month, 1);
            }
            else if (LastQuarter)
            {
                basedate = basedate.AddMonths(-3);
                basedate = new DateTime(basedate.Year, ((basedate.Month - 1) / 3) * 3 + 1, 1);
            }
            else if (LastYear)
            {
                basedate = basedate.AddYears(-1);
                basedate = new DateTime(basedate.Year, 1, 1);
            }

            return basedate;
        }
    }
}
