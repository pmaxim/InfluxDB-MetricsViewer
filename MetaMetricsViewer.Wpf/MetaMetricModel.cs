using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using MetaMetrics.Api;
using MetaMetricsViewer.Wpf.Annotations;

namespace MetaMetricsViewer.Wpf
{
    public class MetaMetricsModel : INotifyPropertyChanged
    {
        private List<MetaMetricsInstallationTimeLine> _installations = new List<MetaMetricsInstallationTimeLine>();
        public string Title { set; get; } = "Anton";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MetaMetricsRangeInfo[] RangePossibilities { set; get; } = new[]
        {
            new MetaMetricsRangeInfo() { Display ="Today", Days  = 0},
            new MetaMetricsRangeInfo() { Display ="Yesterday", Days  = 1},
            new MetaMetricsRangeInfo() { Display ="2d", Days  = 2},
            new MetaMetricsRangeInfo() { Display ="3d", Days  = 3},
            new MetaMetricsRangeInfo() { Display ="4d", Days  = 4},
            new MetaMetricsRangeInfo() { Display ="1w", Weeks  = 1, Default = true},
            new MetaMetricsRangeInfo() { Display ="2w", Weeks  = 2},
            new MetaMetricsRangeInfo() { Display ="4w", Weeks  = 4},
            new MetaMetricsRangeInfo() { Display ="1m", Monthes  = 1},
            new MetaMetricsRangeInfo() { Display ="2m", Monthes  = 2},
            new MetaMetricsRangeInfo() { Display ="3m", Monthes  = 3},
            new MetaMetricsRangeInfo() { Display ="4m", Monthes  = 4},
            new MetaMetricsRangeInfo() { Display ="6m", Monthes  = 6},
            new MetaMetricsRangeInfo() { Display ="1y", Years  = 1},
            new MetaMetricsRangeInfo() { Display ="Current week", CurrentWeek = true},
            new MetaMetricsRangeInfo() { Display ="Current month", CurrentMonth = true},
            new MetaMetricsRangeInfo() { Display ="Current quarter", CurrentQuarter = true},
            new MetaMetricsRangeInfo() { Display ="Current year", CurrentYear = true},
            new MetaMetricsRangeInfo() { Display ="Last week", LastWeek = true},
            new MetaMetricsRangeInfo() { Display ="Last month", LastMonth = true},
            new MetaMetricsRangeInfo() { Display ="Last quarter", LastQuarter = true},
            new MetaMetricsRangeInfo() { Display ="Last year", LastYear = true},
        };

        private MetaMetricsRangeInfo _selectedRange;
        public MetaMetricsRangeInfo SelectedRange
        {
            set
            {
                if (ReferenceEquals(_selectedRange, value))
                    return;
                _selectedRange = value;
                OnPropertyChanged();
            }
            get => _selectedRange??(_selectedRange = RangePossibilities.FirstOrDefault(s=>s.Default)??RangePossibilities.FirstOrDefault());
        }
        
        private int _eachHours = 24;
        public string EachHourString
        {
            get
            {
                return EachHours.ToString();
            }
            set
            {
                var hours = value.ToInt();
                if (hours > 0)
                {
                    EachHours = hours;
                    OnPropertyChanged();
                }
            }
        }

        private string _sortPathFreeColumn;
        public string SortPathFreeColumn
        {
            get
            {
                return _sortPathFreeColumn??"Name";
            }
            set
            {
                if (_sortPathFreeColumn == value)
                    return;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    _sortPathFreeColumn = value;
                    FilterOnMeasure = true;
                }
                else
                {
                    //FilterOnMeasure = false;
                }
            }
        }

        private string _filterText;
        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (_filterText == value)
                    return;
                _filterText = value;
                OnPropertyChanged();
                RunFilter(true);
            }
        }

        private Func<MetaMetricsInstallationTimeLine, bool> FilterItems = f => true;

        private int filtercounter = 0;

        private Timer filterTime;

        void RunFilter(bool delayed = false)
        {
            filterTime?.Dispose();
            filterTime = null;
            var filterText = FilterText;
            if (delayed)
            {
                filterTime = new Timer((s) => RunFilterNow(filterText),null,new TimeSpan(0,0,0,0,600),new TimeSpan(0,0,0,0,600));
            }
            else
            {
                RunFilterNow(filterText);
            }
        }

        void RunFilterNow(string filtertext)
        {
            filterTime?.Dispose();
            filterTime = null;
            var version = Interlocked.Increment(ref filtercounter);
            if (!string.IsNullOrEmpty(filtertext))
            {
                ThreadPool.QueueUserWorkItem((s) =>
                {
                    var fi = filtertext.Searchable().Trim().Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                    var filtered = Installations.Where(FilterItems).Where(n=>$"{n.Name.Searchable()}, #{n.IK}#, #{n.ProjectID}#, #{n.Server.Searchable()}#, #{n.Version}#, #{n.License.Searchable()}#".Contains(fi)).ToList();
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        if (version == filtercounter)
                            FilteredInstallations = filtered;
                    });
                });
            }
            else
            {
                FilteredInstallations = Installations.Where(FilterItems).ToList();
            }
        }
        
        public int EachHours
        {
            set
            {
                if (_eachHours == value)
                    return;
                _eachHours = value;
                OnPropertyChanged();
            }
            get => _eachHours;
        }

        private int _hourInterval = 4;
        public int HourInterval
        {
            set
            {
                if (_hourInterval == value)
                    return;
                _hourInterval = value;
                OnPropertyChanged();
            }
            get => _hourInterval;
        }
        
        private int _daysBack = 6;
        private MetaMetricsInstallationTimeLine _selectedInstallation;

        public int DaysBack
        {
            set
            {
                if (_daysBack == value)
                    return;
                _daysBack = value;
                OnPropertyChanged();
            }
            get => _daysBack;
        }

        public MetaMetricsInstallationTimeLine SelectedInstallation
        {
            set
            {
                if (ReferenceEquals(_selectedInstallation, value))
                    return;
                _selectedInstallation = value;
                if (_selectedInstallation != null)
                {
                    Measures = _selectedInstallation.TimeValues;
                }
                else
                {
                    Measures = null;
                }
                OnPropertyChanged();
            }
            get => _selectedInstallation;
        }
        

        private MetaMetricsMeasurementGroup _selectedMeasures;
        public MetaMetricsMeasurementGroup SelectedMeasures
        {
            set
            {
                if (ReferenceEquals(_selectedMeasures, value))
                    return;
                _selectedMeasures = value;
                if (_selectedMeasures != null)
                {

                }
                OnPropertyChanged();
            }
            get => _selectedMeasures;
        }

        private List<MetaMetricsMeasurementGroup> _measures;
        public List<MetaMetricsMeasurementGroup> Measures
        {
            set
            {
                if (ReferenceEquals(_measures, value))
                    return;
                _measures = value;
                if (_measures != null)
                {

                }
                OnPropertyChanged();
            }
            get => _measures ?? (_measures = new List<MetaMetricsMeasurementGroup>());
        }

        public List<MetaMetricsInstallationTimeLine> Installations
        {
            set
            {
                _installations = value;
                OnPropertyChanged();
                RunFilter();
            }
            get
            {
                return _installations ?? (_installations = new List<MetaMetricsInstallationTimeLine>());
            }
        }

        private List<MetaMetricsInstallationTimeLine> _filteredInstallations;
        private bool _inQuery;

        public List<MetaMetricsInstallationTimeLine> FilteredInstallations
        {
            set
            {
                _filteredInstallations = value;
                OnPropertyChanged();
                if (value != null)
                {
                    AllMeasures = value.SelectMany(n => n.TimeValues.Select(t => t.MeasurementName)).GroupBy(n => n).Select(n => new MetaMetricsMeasureInfo(n.Key)).OrderBy(n => n.MeasurementType == MetaMetricsMeasurementType.Unknown).ThenBy(n => n.MeasurementType).ThenBy(n => n.MeasurementName).Where(n => n.MeasurementType != MetaMetricsMeasurementType.Ignore).ToArray();
                    AllGroups = new[] { "" }.Concat(value.Select(n => n.Group).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllKIS = new[] { "" }.Concat(value.SelectMany(n => (n.KIS ?? "").Split(new[] { ',', '+', ' ' }, StringSplitOptions.RemoveEmptyEntries)).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllInterface = new[] { "" }.Concat(value.SelectMany(n => (n.Interface ?? "").Split(new[] { ',', '+', ' ' }, StringSplitOptions.RemoveEmptyEntries)).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllVersion = new[] { "" }.Concat(value.Select(n => n.Version).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderByDescending(n => n)).ToArray();
                    AllLicense = new[] { "" }.Concat(value.Select(n => n.License).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllServer = new[] { "" }.Concat(value.Select(n => n.Server).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllDates = new[] { "" }.Concat(value.Select(n => n.LastTillTime.ToString("yyyy-MM-dd HH:mm")).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderByDescending(n => n)).ToArray();
                    AllIK = new[] { "" }.Concat(value.Select(n => n.IK).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                    AllProduct = new[] { "" }.Concat(value.SelectMany(n => (n.Products ?? "").Split(new[] { ',', '+', ' ' }, StringSplitOptions.RemoveEmptyEntries)).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).OrderBy(n => n)).ToArray();
                
                    Count = _filteredInstallations.Count;
                    HospitalCount = _filteredInstallations.Sum(s=>s.Hospitals.ToInt());
                    var GroupsCount = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.Group)).GroupBy(n => n.Group, StringComparer.InvariantCultureIgnoreCase).Count();
                    var IKCount = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.IK)).GroupBy(n => n.IK, StringComparer.InvariantCultureIgnoreCase).Count();
                    var StandortCount = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.Standort)).GroupBy(n => n.Standort, StringComparer.InvariantCultureIgnoreCase).Count();
                    var ServerCount = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.Server)).GroupBy(n => n.Server, StringComparer.InvariantCultureIgnoreCase).Count();
                    var LicenseCount = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.License)).GroupBy(n => n.License, StringComparer.InvariantCultureIgnoreCase).Count();
                    var VersionsUsed = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.Version)).GroupBy(n => n.Version, StringComparer.InvariantCultureIgnoreCase).OrderByDescending(n=>n.Key).Select(n=>$"Version {n.Key} = {n.Count()}");
                    var KISUsed = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.KIS)).GroupBy(n => n.KIS, StringComparer.InvariantCultureIgnoreCase).OrderByDescending(n=>n.Count()).Select(n=>$"{n.Key} = {n.Count()}");
                    var InterfacesUsed = _filteredInstallations.Where(n => !string.IsNullOrEmpty(n.Interface)).GroupBy(n => $"{n.KIS}: {n.Interface}", StringComparer.InvariantCultureIgnoreCase).OrderByDescending(n=>n.Count()).Select(n=>$"{n.Key} = {n.Count()}");
                    Counters = string.Join("\n", new string[]
                    {
                        $"Installationen = {Count}",
                        $"Krankenhäuser = {HospitalCount}",
                        $"Lizenzen = {LicenseCount}",
                        $"IK's = {IKCount}",
                        $"Standorte = {StandortCount}",
                        $"KH-Gruppen = {GroupsCount}",
                        $"Server = {ServerCount}",
                    }.Concat(VersionsUsed).Concat(KISUsed).Concat(InterfacesUsed));
                }
            }
            get
            {
                return _filteredInstallations ?? (_filteredInstallations = new List<MetaMetricsInstallationTimeLine>());
            }
        }

        public bool InQuery
        {
            get => _inQuery;
            set
            {
                if (_inQuery == value)
                    return;
                _inQuery = value;
                OnPropertyChanged();
            }
        }
        
        private string _lastQuery;
        public string LastQuery
        {
            get => _lastQuery;
            set
            {
                if (_lastQuery == value)
                    return;
                _lastQuery = value;
                OnPropertyChanged();
            }
        }

        private string _lastError;
        private MetaMetricsMeasureInfo[] _allMeasures;
        private MetaMetricsMeasureInfo _selectedMeasure;
        private string _rangeString = "";
        private bool _filterOnMeasure;

        public MetaMetricsModel()
        {
        }

        public string LastError
        {
            get => _lastError;
            set
            {
                if (_lastError == value)
                    return;
                _lastError = value;
                OnPropertyChanged();
            }
        }

        public MetaMetricsMeasureInfo SelectedMeasure
        {
            set
            {
                if (ReferenceEquals(_selectedMeasure, value))
                    return;
                _selectedMeasure = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
            get => _selectedMeasure;
        }

        public MetaMetricsMeasureInfo[] AllMeasures
        {
            get => _allMeasures??Array.Empty<MetaMetricsMeasureInfo>();
            set
            {
                if (ReferenceEquals(_allMeasures,value))
                    return;
                _allMeasures = value;
                /*
                if (_allMeasures?.Any() == true)
                {
                    SelectedMeasure = _allMeasures.First();
                }
                */
                OnPropertyChanged();
            }
        }

        private string[] _allKIS;
        public string[] AllKIS
        {
            get => _allKIS ?? Array.Empty<string>();
            
            set
            {
                if (ReferenceEquals(_allKIS,value))
                    return;
                _allKIS = value;
                OnPropertyChanged();
            }
        }
        private string[] _allInterface;
        public string[] AllInterface
        {
            get => _allInterface ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allInterface,value))
                    return;
                _allInterface = value;
                OnPropertyChanged();
            }
        }
        private string[] _allVersion;
        public string[] AllVersion
        {
            get => _allVersion ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allVersion,value))
                    return;
                _allVersion = value;
                OnPropertyChanged();
            }
        }
        private string[] _allGroups;
        public string[] AllGroups
        {
            get => _allGroups ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allGroups,value))
                    return;
                _allGroups = value;
                OnPropertyChanged();
            }
        }
        private string[] _allIK;
        public string[] AllIK
        {
            get => _allIK ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allIK,value))
                    return;
                _allIK = value;
                OnPropertyChanged();
            }
        }
        private string[] _allServer;
        public string[] AllServer
        {
            get => _allServer ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allServer,value))
                    return;
                _allServer = value;
                OnPropertyChanged();
            }
        }
        private string[] _allDates;
        public string[] AllDates
        {
            get => _allDates ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allDates,value))
                    return;
                _allDates = value;
                OnPropertyChanged();
            }
        }
        private string[] _allLicense;
        public string[] AllLicense
        {
            get => _allLicense ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allLicense,value))
                    return;
                _allLicense = value;
                OnPropertyChanged();
            }
        }
        private string[] _allProduct;
        public string[] AllProduct
        {
            get => _allProduct ?? Array.Empty<string>();
            set
            {
                if (ReferenceEquals(_allProduct,value))
                    return;
                _allProduct = value;
                OnPropertyChanged();
            }
        }
        
        
        private int _count;

        public int Count
        {
            get => _count;
            set
            {
                if (_count == value)
                    return;
                _count = value;
                OnPropertyChanged();
            }
        }
        
         
        private string _counters;

        public string Counters
        {
            get => _counters;
            set
            {
                if (_counters == value)
                    return;
                _counters = value;
                OnPropertyChanged();
            }
        }
        
        private int _hospCount;

        public int HospitalCount
        {
            get => _hospCount;
            set
            {
                if (_hospCount == value)
                    return;
                _hospCount = value;
                OnPropertyChanged();
            }
        }
        private string _selectedDate;

        public string SelectedDate
        {
            get => _selectedDate ?? "";
            set
            {
                if (_selectedDate == value)
                    return;
                _selectedDate = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }
        private string _selectedGroup;

        public string SelectedGroup
        {
            get => _selectedGroup ?? "";
            set
            {
                if (_selectedGroup == value)
                    return;
                _selectedGroup = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }

        private string _selectedKIS;

        public string SelectedKIS
        {
            get => _selectedKIS ?? "";
            set
            {
                if (_selectedKIS == value)
                    return;
                _selectedKIS = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }

        private string _selectedInterface;

        public string SelectedInterface
        {
            get => _selectedInterface ?? "";
            set
            {
                if (_selectedInterface == value)
                    return;
                _selectedInterface = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }
        
        private string _selectedVersion;

        public string SelectedVersion
        {
            get => _selectedVersion ?? "";
            set
            {
                if (_selectedVersion == value)
                    return;
                _selectedVersion = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }
        private string _selectedServer;

        public string SelectedServer
        {
            get => _selectedServer ?? "";
            set
            {
                if (_selectedServer == value)
                    return;
                _selectedServer = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }
        private string _selectedLicense;

        public string SelectedLicense
        {
            get => _selectedLicense ?? "";
            set
            {
                if (_selectedLicense == value)
                    return;
                _selectedLicense = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }
        
        private string _selectedProduct;
        private string _currentAction;

        public string SelectedProduct
        {
            get => _selectedProduct ?? "";
            set
            {
                if (_selectedProduct == value)
                    return;
                _selectedProduct = value;
                OnPropertyChanged();
                SelectedMeasureChanged();
            }
        }


        public bool FilterOnMeasure
        {
            set
            {
                if (_filterOnMeasure == value)
                    return;
                _filterOnMeasure = value;
                
                SelectedMeasureChanged();
            }
            get => _filterOnMeasure;
        }

        public string CurrentAction
        {
            get => _currentAction;
            set
            {
                if (_currentAction == value)
                    return;
                _currentAction = value;
                OnPropertyChanged();
            }
        }

        void SelectedMeasureChanged()
        {
            //FilterItems = f => true;
            var list = new List<Func<MetaMetricsInstallationTimeLine, bool>>();
            if (_filterOnMeasure && _selectedMeasure != null && _selectedMeasure.MeasurementType != MetaMetricsMeasurementType.Unknown)
            {
                var _selMeasure = _selectedMeasure.MeasurementType;
                list.Add( f => f[_selMeasure] != null);
            }
            if (!string.IsNullOrEmpty(SelectedDate))
            {
                var date = SelectedDate;
                list.Add( f => f.LastTillTime.ToString("yyyy-MM-dd HH:mm") == date);
            }
            if (!string.IsNullOrEmpty(SelectedGroup))
            {
                var group = SelectedGroup.Searchable();
                list.Add( f => f.Group.Searchable() == group);
            }
            if (!string.IsNullOrEmpty(SelectedKIS))
            {
                var kis = SelectedKIS.Searchable();
                list.Add( f => (f.KIS??"").Searchable().Split(new []{' ' ,'+',','}).Contains(kis));
            }
            if (!string.IsNullOrEmpty(SelectedInterface))
            {
                var interf = SelectedInterface.Searchable();
                list.Add( f => (f.Interface??"").Searchable().Split(new []{' ' ,'+',','}).Contains(interf));
            }
            if (!string.IsNullOrEmpty(SelectedVersion))
            {
                var version = SelectedVersion;
                list.Add( f => f.Version == version);
            }
            if (!string.IsNullOrEmpty(SelectedLicense))
            {
                var license = SelectedLicense;
                list.Add( f => f.License == license);
            }
            if (!string.IsNullOrEmpty(SelectedProduct))
            {
                var product = SelectedProduct.Searchable();
                list.Add( f => (f.Products??"").Searchable().Split(new []{' ' ,'+',','}).Contains(product));
            }
            if (!string.IsNullOrEmpty(SelectedServer))
            {
                var server = SelectedServer.Searchable();
                list.Add( f => (f.Server??"").Searchable() == server);
            }

            if (list.Count == 0)
            {
                FilterItems = f => true;
            }
            else if (list.Count == 0)
            {
                FilterItems = list[0];
            }
            else 
            {
                FilterItems = f=>list.All(l=>l(f));
            }
            RunFilter();
        }
    }

    public class MetaMetricsRangeInfo
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
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek==0?6:((int)basedate.DayOfWeek-1)));
            else if (CurrentMonth)
                basedate = new DateTime(basedate.Year, basedate.Month, 1);
            else if (CurrentQuarter)
                basedate = new DateTime(basedate.Year, ((basedate.Month-1)/3)*3+1, 1);
            else if (CurrentYear)
                basedate = new DateTime(basedate.Year, 1, 1); 
            else if (LastWeek)
            {
                basedate = basedate.AddDays(-7);
                basedate = basedate.AddDays(-((int)basedate.DayOfWeek==0?6:((int)basedate.DayOfWeek-1)));
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
    }
    
    public class VisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility? res = null;
            if (targetType == typeof(Visibility))
            {
                if (value == null)
                {
                    res = Visibility.Collapsed;
                }
                else if (value is bool)
                {
                    res = ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value is int)
                {
                    res = ((int)value)!=0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value is string)
                {
                    res = !(string.IsNullOrEmpty((string)value)) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            if (parameter is string && res != null)
            {
                if (parameter.ToString().ToLower().Trim() == "not")
                {
                    res = res == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}