using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MetaMetrics.Api;
using MetaMetricsViewer.Wpf.Annotations;

namespace MetaMetricsViewer.Wpf
{
    public partial class DetailWindow : Window, INotifyPropertyChanged
    {
        private MetaMetricsMeasurementGroup _datagroup;
        private List<MetaMetricsItemTimeValues> _measureItems;
        private int _hoursIntervall = 24;

        public DetailWindow()
        {
            InitializeComponent();
        }

        public MetaMetricsMeasurementGroup DataGroup
        {
            get => _datagroup;
            set
            {
                if (ReferenceEquals(_datagroup, value))
                    return;
                _datagroup = value;
                OnPropertyChanged();
                if (_datagroup != null)
                {
                    Title = $"{_datagroup?.TimeLine.Name} - {_datagroup?.MeasurementDisplay}";
                    SparkLine.HoursIntervall = HoursInterval;
                    SparkLine.DataGroup = _datagroup;
                    if (_datagroup.ItemTimeValues.Any())
                    {
                        MeasureItems = _datagroup.ItemTimeValues.OrderBy(n=>n.Key).Select(n=>n.Value).ToList();
                    }
                }
            }
        }    
        
        public MetaMetricsMeasurementGroup DataGroupItems
        {
            get => _datagroup;
            set
            {
                if (value != null)
                {
                    if (value.ItemTimeValues.Any())
                    {
                        MeasureItems = value.ItemTimeValues.OrderBy(n=>n.Key).Select(n=>n.Value).ToList();
                    }
                }
            }
        }

        public List<MetaMetricsItemTimeValues> MeasureItems
        {
            get => _measureItems;
            set
            {
                if (ReferenceEquals(_measureItems, value))
                    return;
                _measureItems = value;
                if (_measureItems != null)
                {
                    
                }
                OnPropertyChanged();
            }
        }     
        
        public string HoursIntervalString
        {
            get
            {
                return HoursInterval.ToString();
            }
            set
            {
                var hours = value.ToInt();
                if (hours > 0)
                {
                    HoursInterval = hours;
                    SparkLine.HoursIntervall = HoursInterval;
                    OnPropertyChanged();
                }
            }
        }

        public int HoursInterval
        {
            set
            {
                if (_hoursIntervall == value)
                    return;
                _hoursIntervall = value;
                OnPropertyChanged();
            }
            get => _hoursIntervall;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}