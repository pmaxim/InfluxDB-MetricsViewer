using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MetaMetrics.Api;
using MetaMetricsViewer.Wpf.Annotations;
using Syncfusion.UI.Xaml.Charts;

namespace MetaMetricsViewer.Wpf
{
    public partial class MetaMetricsSparkLine : UserControl, INotifyPropertyChanged
    {
        public MetaMetricsSparkLine()
        {
            InitializeComponent();
        }
        
        public static readonly DependencyProperty SparklineHeightProperty = DependencyProperty.Register(
            nameof(SparklineHeight),
            typeof(double),
            typeof(MetaMetricsSparkLine),new PropertyMetadata(0d,OnSparklineHeightChanged)
        );
        
        public static readonly DependencyProperty MeasurementTypeProperty = DependencyProperty.Register(
            nameof(MeasurementType),
            typeof(MetaMetricsMeasurementType),
            typeof(MetaMetricsSparkLine),new PropertyMetadata(MetaMetricsMeasurementType.Unknown,OnMeasurementTypeChanged)
        );

        public static readonly DependencyProperty DataItemProperty = DependencyProperty.Register(
            nameof(DataItem),
            typeof(object),
            typeof(MetaMetricsSparkLine),new PropertyMetadata(null, OnDataItemTypeChanged)
        );
        
        
        public static readonly DependencyProperty HoursIntervallProperty = DependencyProperty.Register(
            nameof(HoursIntervall),
            typeof(int),
            typeof(MetaMetricsSparkLine),new PropertyMetadata(0, OnHoursIntervallChanged)
        );

        private static void OnDataItemTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spark = d as MetaMetricsSparkLine;
            if (e.NewValue is MetaMetricsInstallationTimeLine)
            {
                spark.DataItem = e.NewValue as MetaMetricsInstallationTimeLine;
            }
            else if (e.NewValue is MetaMetricsMeasurementGroup)
            {
                spark.DataGroup = e.NewValue as MetaMetricsMeasurementGroup;
            }
            else if (e.NewValue is MetaMetricsItemTimeValues)
            {
                spark.MeasureItem = e.NewValue as MetaMetricsItemTimeValues;
            }
        }
        private static void OnSparklineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spark = d as MetaMetricsSparkLine;
            if (e.NewValue is double)
                spark.SparklineHeight = (double)e.NewValue;
            else if (e.NewValue is float)
                spark.SparklineHeight = (float)e.NewValue;
            else if (e.NewValue is int)
                spark.SparklineHeight = (int)e.NewValue;
        }
        
        private static void OnMeasurementTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spark = d as MetaMetricsSparkLine;
            spark.MeasurementType = (MetaMetricsMeasurementType)e.NewValue;
        }
        
        private static void OnHoursIntervallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spark = d as MetaMetricsSparkLine;
            if (e.NewValue is int)
            {
                spark.HoursIntervall = (int)e.NewValue;
            }
        }

        public double SparklineHeight
        {
            set
            {
                if (_sparklineHeight == value)
                    return;
                _sparklineHeight = value;
                OnPropertyChanged();
            }
            get => _sparklineHeight<=0 ? 32 :_sparklineHeight;
        }


        private MetaMetricsMeasurementType _measurementType;
        public string MeasurementName
        {
            get
            {
                return MeasurementType.ToString();
            }
        }

        private int _hoursIntervall;
        public int HoursIntervall 
        {
            set
            {
                if (_hoursIntervall == value)
                    return;
                _hoursIntervall = value;
                OnPropertyChanged();
                if (DataGroup != null)
                    BuildView(DataGroup);
                else if (MeasureItem != null)
                    BuildView(MeasureItem);
                else if (DataItem != null)
                    Refresh();
            }
            get => _hoursIntervall;
        }

        private Visibility _hasData;
        public Visibility HasData
        {
            set
            {
                if (_hasData == value)
                    return;
                _hasData = value;
                OnPropertyChanged();
            }
            get => _hasData;
        }
        
        public List<IMetaMetricsTimeValue> Items
        {
            set
            {
                //if (ReferenceEquals(_items, value))
                    //return;
                _items = value;
                OnPropertyChanged();
                HasData = _items?.Any() ?? false ? Visibility.Visible : Visibility.Collapsed;
            }
            get => _items?? new List<IMetaMetricsTimeValue>();
        }

        public MetaMetricsMeasurementType MeasurementType
        {
            set
            {
                if (_measurementType == value)
                    return;
                _measurementType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MeasurementName));
                Refresh();
            }
            get => _measurementType;
        }

        private MetaMetricsInstallationTimeLine _dataItem;
        private string _contentValues;
        private List<IMetaMetricsTimeValue> _items;

        public MetaMetricsInstallationTimeLine DataItem
        {
            set
            {
                //if (ReferenceEquals(_dataItem, value))
                    //return;
                _dataItem = value;
                OnPropertyChanged();
                Refresh();
            }
            get
            {
                return _dataItem;
            }
        }

        private MetaMetricsItemTimeValues _measureItem;
        public MetaMetricsItemTimeValues MeasureItem
        {
            set
            {
                if (ReferenceEquals(_measureItem, value))
                    return;
                _measureItem = value;
                OnPropertyChanged();
                BuildView(_measureItem);
            }
            get
            {
                return _measureItem;
            }
        }

        private MetaMetricsMeasurementGroup _dataGroup;
        public MetaMetricsMeasurementGroup DataGroup
        {
            set
            {
                if (ReferenceEquals(_dataGroup, value))
                   return;
                _dataGroup = value;
                OnPropertyChanged();
                BuildView(_dataGroup);
            }
            get
            {
                return _dataGroup;
            }
        }

        public string ContentValues
        {
            set
            {
                if (_contentValues == value)
                    return;
                _contentValues = value;
                OnPropertyChanged();
            }
            get => _contentValues;
        }
        
        private string _avg;
        public string Avg
        {
            set
            {
                if (_avg == value)
                    return;
                _avg = value;
                OnPropertyChanged();
            }
            get => _avg;
        } 
        private string _max;
        public string Max
        {
            set
            {
                if (_max == value)
                    return;
                _max = value;
                OnPropertyChanged();
            }
            get => _max;
        } 
        private string _sum;
        public string Sum
        {
            set
            {
                if (_sum == value)
                    return;
                _sum = value;
                OnPropertyChanged();
            }
            get => _sum;
        } 
        
        private string _firstTime;
        public string FirstTime
        {
            set
            {
                if (_firstTime == value)
                    return;
                _firstTime = value;
                OnPropertyChanged();
            }
            get => _firstTime;
        }  
        
        private string _lastTime;
        public string LastTime
        {
            set
            {
                if (_lastTime == value)
                    return;
                _lastTime = value;
                OnPropertyChanged();
            }
            get => _lastTime;
        }
        
        
        private string _name;
        public string ItemName
        {
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        void Refresh()
        {
            if (_measurementType != MetaMetricsMeasurementType.Unknown && _dataItem != null)
            {
                var data = _dataItem.TimeValues.FirstOrDefault(n => n.Measurement == _measurementType);
                if (data != null)
                {
                    BuildView(data);
                }
                else
                {
                    ClearView();
                }
            }
            else
            {
                ClearView();
            }
        }

        public DateTime StartTime { set; get; }

        private void BuildView(MetaMetricsItemTimeValues data)
        {
            ContentValues = string.Join(" ", data.ItemTimeValues.Select(n => n.Value));
            var items = new Dictionary<string,IMetaMetricsTimeValue>();
            each = data.Query.EveryHourCombine;
            if (HoursIntervall > 0)
            {
                each = HoursIntervall;
            }
            StartTime = data.Query.RequestStartTime.Group(each);
            var time = StartTime;
            while (time<data.Query.RealEndTime)
            {
                var till = time.AddHours(each).AddMinutes(-1);
                if (till > data.Query.RealEndTime)
                    till = data.Query.RealEndTime;
                items.Add(time.ToString("yyyyMMddHHmm"),new MetaMetricsItemTimeValue(){From = time, Till = till, Value = 0});
                time = time.AddHours(each);
            }
            foreach (var item in data.ItemTimeValues)
            {
                var t = item.From.Group(each);
                var it = items[t.ToString("yyyyMMddHHmm")];
                it.Value+=item.Value;
                it.Exists = true;
            }
            Items = items.Values.ToList();
            ItemName = data.ItemName;
            FirstTime = data.FirstTime.ToString("dd.MM. HH:mm");
            LastTime = data.LastTillTime.ToString("dd.MM. HH:mm");            
            Avg = items.Where(n=>n.Value.Exists).Average(n=>n.Value.Value).ToString("#,##0.0");
            Max = items.Max(n=>n.Value.Value).ToString("#,##0");
            Sum = items.Sum(n=>n.Value.Value).ToString("#,##0");
        }

        private int counter = 0;
        private void BuildView(MetaMetricsMeasurementGroup data)
        {
            var lastcounter = Interlocked.Increment(ref counter);
            
            var eachHour = data.Query.EveryHourCombine;
            if (HoursIntervall > 0)
            {
                eachHour = HoursIntervall;
            }
            var startTime = data.Query.RequestStartTime.Group(eachHour);
            
            ThreadPool.QueueUserWorkItem((s) =>
            {
                var items = new Dictionary<string, IMetaMetricsTimeValue>();
                var time = startTime;
                while (time < data.Query.RealEndTime)
                {
                    var till = time.AddHours(eachHour).AddMinutes(-1);
                    if (till > data.Query.RealEndTime)
                        till = data.Query.RealEndTime;
                    items.Add(time.ToString("yyyyMMddHHmm"), new MetaMetricsItemTimeValue() { From = time, Till = till, Value = 0 });
                    time = time.AddHours(eachHour);
                }

                foreach (var item in data.TimeValues)
                {
                    var t = item.From.Group(eachHour);
                    var it = items[t.ToString("yyyyMMddHHmm")];
                    it.Value += item.Value;
                    it.Exists = true;
                }
                
                Dispatcher.InvokeAsync(() =>
                {
                    if (lastcounter == counter)
                    {

                        each = eachHour;
                        StartTime = startTime;
                        ContentValues = string.Join(" ", data.TimeValues.Select(n => n.Value));
                        Items = items.Values.ToList();
                        ItemName = data.TimeLine.Name + " / " + data.Measurement.ToString();

                        FirstTime = data.FirstTime.ToString("dd.MM. HH:mm");
                        LastTime = data.LastTillTime.ToString("dd.MM. HH:mm");

                        Avg = items.Where(n => n.Value.Exists).Average(n => n.Value.Value).ToString("#,##0.0");
                        Max = items.Max(n => n.Value.Value).ToString("#,##0");
                        Sum = items.Sum(n => n.Value.Value).ToString("#,##0");
                    }
                });
            });
           
        }

        private int each = 1;
        void ClearView()
        {
            ContentValues = "";
            Items = new List<IMetaMetricsTimeValue>();
            ItemName = "";
            FirstTime = "";
            LastTime = "";
            Avg = "";
            Max = "";
            Sum = "";
            StartTime = DateTime.MinValue;
            each = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ContentPresenter info;
        private double _sparklineHeight = 32;

        private void MySparkLine_OnOnSparklineMouseMove(object sender, SparklineMouseMoveEventArgs args)
        {
            /*
            if (!args.RootPanel.Children.Contains(info))
            {
                info=new ContentPresenter();
                args.RootPanel.Children.Add(info);
                TextBlock.SetForeground(info, new SolidColorBrush(Colors.Red)); 
                
                TextBlock.SetFontSize(info, 8); 
            }
            */
            if (StartTime != DateTime.MinValue && !double.IsNaN(args.Value.Y))
            {
                var from = StartTime.AddHours(args.Value.X * each);
                var till = from;
                var value = "n.a.";
                var index = (int)args.Value.X;
                if (index < Items.Count)
                {
                    from = Items[index].From;
                    till = Items[index].Till;
                    if (Items[index].Exists)
                        value = $"{Items[index].Value.ToString("#,##0")}";
                }

                TrackBall.Text = $"{from:ddd dd.MM. HH:mm}-{till:HH:mm}: {value}";
                TrackBallLine.X1 = args.Coordinate.X;
                TrackBallLine.X2 = args.Coordinate.X;
                TrackBallLine.Y1 = 0;
                TrackBallLine.Y2 = 32;
                TrackBallLine.Visibility = Visibility.Visible;
            }
            else
            {
                TrackBallLine.Visibility = Visibility.Collapsed;
            }

        }
    }
}