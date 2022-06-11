using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using InfluxDB.Client;
using MetaMetrics.Api;
using MK.Classification;
using Syncfusion.Licensing;
using Syncfusion.UI.Xaml.Charts;
using DataGrid = System.Windows.Controls.DataGrid;

namespace MetaMetricsViewer.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            Model = new MetaMetricsModel() { Title = "Fritz" };
            InitializeComponent();
            Loaded += async (sender, args) =>
            {
                await InitModel();
            };
        }

        private async Task InitModel()
        {
            Client = await MetaMetricsConnector.Main();
            Model.InQuery = true;
            await Task.Run(async () =>
            {
                var apps = await MetaMetricsConnector.QueryApps(Client, (ex, q) => { });
                var measurements = await MetaMetricsConnector.QueryMeasurements(Client, (ex, q) => { });
                var environments = await MetaMetricsConnector.QueryEnvironments(Client, (ex, q) => { });
                var projects = await MetaMetricsConnector.QueryProjects(Client, (ex, q) => { });
                var versions = await MetaMetricsConnector.QueryVersions(Client, (ex, q) => { });
                var licenses = await MetaMetricsConnector.QueryLicenses(Client, (ex, q) => { });
                var allsubLicenses = await MetaMetricsConnector.QuerySubLicenses(Client, (ex, q) => { });
                var server = await MetaMetricsConnector.QueryServers(Client, (ex, q) => { });
                var items = await MetaMetricsConnector.QueryItems(Client, (ex, q) => { });
                var subLicenses = new Dictionary<string,List<string>>();
                foreach (var s in allsubLicenses)
                {
                    if (MetaMetricsInstallationService.Instance.Installations.ContainsKey(s.ToInstallation()) && !MetaMetricsInstallationService.Instance.Installations.ContainsKey(s))
                    {
                        var orginst = MetaMetricsInstallationService.Instance[s.ToInstallation()];
                        MetaMetricsInstallationService.Instance.Installations.Remove(s.ToInstallation());
                        orginst.Key = s;
                        MetaMetricsInstallationService.Instance.Installations[orginst.Key] = orginst;
                    }
                    var inst = MetaMetricsInstallationService.Instance[s];
                    if (!string.IsNullOrEmpty(inst.MapTo))
                    {
                        inst = MetaMetricsInstallationService.Instance[inst.MapTo];
                    }
                    if (inst.Ignore)
                    {
                        continue;
                    }

                    List<string> comb;
                    if (!subLicenses.TryGetValue(inst.Key, out comb))
                    {
                        subLicenses[inst.Key] = comb = new List<string>();
                        comb.Add(inst.Key);
                    }
                    if (inst.Key != s)
                        comb.Add(s);
                }
                foreach (var expected in MetaMetricsInstallationService.Instance.Installations.Values)
                {
                    if (string.IsNullOrEmpty(expected.MapTo) && !expected.Ignore)
                    {
                        if (!subLicenses.ContainsKey(expected.Key))
                        {
                            subLicenses[expected.Key] =  new List<string>(new []{expected.Key});
                        }
                    }
                }

                ThreadPool.QueueUserWorkItem(async (stat) =>
                {
                    var data = await QueryData(subLicenses.Values, (ex, q) => { Dispatcher.Invoke(() => { Model.LastError = $"{q}: {ex}"; }); });
                    Dispatcher.Invoke(() =>
                    {
                        //Model.AllProdu = new[] { "" }.Concat(data.SelectMany(n=>n.Products.Split(',',';')).Where(n=>!string.IsNullOrEmpty(n)).GroupBy(n=>n.Trim(), StringComparer.InvariantCultureIgnoreCase).Select(n=>n.Key).OrderBy(n => n)).ToArray();
                        File.WriteAllLines("unknown.txt", data.SelectMany(n => n.TimeValues.Select(t => t.MeasurementName)).GroupBy(n => n).Select(n => new MetaMetricsMeasureInfo(n.Key)).OrderBy(n => n.MeasurementType == MetaMetricsMeasurementType.Unknown).ThenBy(n => n.MeasurementType).ThenBy(n => n.MeasurementName).Where(n => n.MeasurementType == MetaMetricsMeasurementType.Unknown).Select(s => s.MeasurementName));
                        Model.Installations = null;
                        Model.Installations = data;
                        Model.InQuery = false;
                    });

                });
            });
        }

        private async Task<List<MetaMetricsInstallationTimeLine>> QueryData(Action<Exception, string> onError)
        {
            var query = new MetaMetricsQuery()
                .OffsetHours(0)
                .FromLastDays(Model.DaysBack)
                .To()
                .Each(Model.EachHours)
                //.Measurement(MetaMetricsQueryExtension.AllCounters())
                .OnValue();
            return await MetaMetricsConnector.QueryMeasurement(Client, query, true, onError);
        }
        
        private async Task<List<MetaMetricsInstallationTimeLine>> QueryData(IEnumerable<List<string>> subLicenses, Action<Exception, string> onError, bool addExected = true)
        {   
            Dispatcher.Invoke(() =>
            {
                Model.CurrentAction = $"Querying starting";
            });
            var res = new List<MetaMetricsInstallationTimeLine>();
            MetaMetricsQuery lastquery = null;
            foreach (var subLicense in subLicenses)
            {
                Console.WriteLine($"Querying {string.Join(", ",subLicense)}");
                Dispatcher.Invoke(() =>
                {
                    Model.CurrentAction = $"Querying {string.Join(", ",subLicense)}";
                });
                var query = lastquery = new MetaMetricsQuery()
                    .OffsetHours(0)
                    .FromLastDays(Model.DaysBack)
                    .To()
                    .Each(Model.EachHours)
                    .Sublicense(subLicense.ToArray())
                    //.Measurement(MetaMetricsQueryExtension.AllCounters())
                    .OnValue();
                var sub = await MetaMetricsConnector.QueryMeasurement(Client, query, false, onError);
                if (sub.Count==0)
                {
                    var expected = MetaMetricsInstallationService.Instance[subLicense.First()];
                    sub.Add(new MetaMetricsInstallationTimeLine(lastquery, expected));
                }
                res.AddRange(sub);
            }
            Dispatcher.Invoke(() =>
            {
                Model.CurrentAction = $"Querying complete";
            });
            if (true)
            {
                var dupKey = res.GroupBy(n => n.Installation.Key).Where(n => n.Count() > 1).ToArray();
                var dupName = res.GroupBy(n => n.Installation.Fullname).Where(n => n.Count() > 1).ToArray();
                /*
           
                return timeLines.Values.OrderBy(n => n.Name).ToList();
                */
            }
            return res;
        }

        public InfluxDBClient Client { get; set; }

        public MetaMetricsModel Model { set; get; }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Model.InQuery)
            {
                Model.InQuery = true;
                Model.LastError = "";
                Task.Run(async () =>
                {
                    var items = await QueryData((ex, q) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Model.LastError = $"{q}: {ex}";
                        });
                    });
                    Dispatcher.Invoke(() =>
                    {
                        Model.Installations = null;
                        Model.Installations = items;
                        Model.InQuery = false;
                    });
                });
            }
        }

        private void AddInstallation_OnClick(object sender, RoutedEventArgs e)
        {
            
            var wnd = new EditInstallation() {  };   
            wnd.OnSave = (entry) =>
            {
                MetaMetricsInstallationService.Instance[entry.Key] = entry; 
                MetaMetricsInstallationService.Instance.Save();
            };
            wnd.ShowDialog();
        }
        
        private void EditInstallation(MetaMetricsInstallationTimeLine timeLine)
        {
            var entry = timeLine.Installation;
            var wnd = new EditInstallation() { Installation = entry };
            wnd.OnSave = (e) =>
            {
                entry.MapTo = e.MapTo;
                entry.Display = e.Display;
                entry.Group = e.Group;
                entry.Server = e.Server;
                entry.Zusatz = e.Zusatz;
                entry.KIS = e.KIS;
                entry.Interface = e.Interface;
                entry.IK = e.IK;
                entry.Standort = e.Standort;
                entry.Standorte = e.Standorte;
                entry.Hospitals = e.Hospitals;
                entry.ProjectID = e.ProjectID;
                //entry.Products = e.Products;
                entry.Ignore = e.Ignore;
                timeLine.Name = entry.Fullname; 
                timeLine.IK = entry.IK; 
                timeLine.Standort = entry.Standort; 
                timeLine.Standorte = entry.Standorte; 
                timeLine.Hospitals = entry.Hospitals; 
                timeLine.Server = entry.Server; 
                timeLine.KIS = entry.KIS; 
                timeLine.Interface = entry.Interface; 
                timeLine.ProjectID = entry.ProjectID; 
                timeLine.Products = entry.Products; 
                MetaMetricsInstallationService.Instance.Save();
            };
            wnd.OnDelete = (e) =>
            {
                MetaMetricsInstallationService.Instance.Remove(e?.Key);
                MetaMetricsInstallationService.Instance.Save();
            };
            wnd.ShowDialog();
        }
        
        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (InstallationsGrid.CurrentColumn?.Header?.ToString()?.ToUpper() == "NAME")
            {
                EditInstallation(Model.SelectedInstallation);
            }
            else if (Model.SelectedInstallation != null)
            {
                if (!Model.InQuery)
                {
                    var fromDate = DateTime.Today.AddDays(-30);
                    if (Model.SelectedRange != null)
                    {
                        fromDate = Model.SelectedRange.StartDate();
                    }
                    Model.InQuery = true;
                    Model.LastError = "";
                    var sublicense = Model.SelectedInstallation.Installation.Key;
                    Model.Measures = null;
                    Task.Run(async () =>
                    {
                        var query = new MetaMetricsQuery()
                            .Sublicense(sublicense)
                            .OffsetHours(0)
                            .From(fromDate)
                            .To()
                            .Each(Model.EachHours)
                            //.Measurement(measure)
                            .OnValue();
                        var q = query.Query;
                        var items = await MetaMetricsConnector.QueryMeasurement(Client, query, false, (ex, qq) =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Model.LastError = $"{q}: {ex}";
                            });
                        });
                        this.Dispatcher.Invoke(() => { Model.InQuery = false; });
                        if (items.Any())
                        {
                            var values = items.First().TimeValues;
                            this.Dispatcher.Invoke(() =>
                            {
                                Model.Measures = values;
                            });
                        }
                    });
                }
            }
        }

        private void Details_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Model.SelectedMeasures != null)
            {
                if (!Model.InQuery)
                {  
                    var fromDate = DateTime.Today.AddDays(-30);
                    if (Model.SelectedRange != null)
                    {
                        fromDate = Model.SelectedRange.StartDate();
                    }
                    Model.InQuery = true;
                    Model.LastError = "";
                    var measure = Model.SelectedMeasures.MeasurementName;
                    var sublicense = Model.SelectedMeasures.TimeLine.Installation.Key;
                    Task.Run(async () =>
                    {
                        var query = new MetaMetricsQuery()
                            .Sublicense(sublicense)
                            .OffsetHours(0)
                            .From(fromDate)
                            .To()
                            .Each(1)
                            .Measurement(measure)
                            .OnValue();
                        var q = query.Query;
                        var items = await MetaMetricsConnector.QueryMeasurement(Client, query, false, (ex, qq) =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Model.LastError = $"{qq}: {ex}";
                            });
                        });
                        
                        var queryItems = new MetaMetricsQuery()
                            .Sublicense(sublicense)
                            .OffsetHours(0)
                            .FromLastDays(30)
                            .To()
                            .Each(1)
                            .MeasurementItems(measure)
                            .OnTotal();
                        var qitems = queryItems.Query;
                        var itemsItems = await MetaMetricsConnector.QueryMeasurement(Client, queryItems, false, (ex, qq) =>
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Model.LastError = $"{qq}: {ex}";
                            });
                        });
                        
                        this.Dispatcher.Invoke(() =>
                        {
                            Model.InQuery = false;
                        });
                        if (items.Any())
                        {
                            var values = items.First().TimeValues;
                            var valuesItems = itemsItems.FirstOrDefault()?.TimeValues;
                            if (values.Any())
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    var wnd = new DetailWindow();
                                    wnd.Owner = this;
                                    wnd.DataGroup = values.First();
                                    wnd.DataGroupItems = valuesItems?.FirstOrDefault();
                                    wnd.Show();
                                });
                            }
                        }
                    });
                }
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Model.SelectedMeasure?.MeasurementName))
            {
                var col = InstallationsGrid.Columns.Skip(9).First();
                col.Header = Model.SelectedMeasure.MeasurementDisplay;
                Model.SortPathFreeColumn = $".[{Model.SelectedMeasure.MeasurementType.ToString()}].Avg";
                col.SortMemberPath = Model.SortPathFreeColumn;
            }
            else
            {
                //Model.SortPathFreeColumn = null;
            }
        }

        private void MapOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var map = new MetaMetricsOnMap(Model);
            map.Show();
        }

        private void InstallationsGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            /*
            var grid = (DataGrid)sender;
            var c = grid.Items.Count;
            for (var n = 0; n < c; n++)
            {
                var tl = grid.[n] as MetaMetricsInstallationTimeLine;
                if (tl != null)
                {
                    tl.Pos = n;
                }
            }
            */
        }
    }
    
    public class SelectMeasureDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as MetaMetricsMeasureInfo)?.MeasurementDisplay ?? "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class SelectMeasureTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as MetaMetricsMeasureInfo)?.MeasurementType ?? MetaMetricsMeasurementType.Unknown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class RowToIndexConverter :  IValueConverter
    {
        static RowToIndexConverter converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            //var n = row?.AlternationIndex??-1;
            if (row != null)
                return row.GetIndex()+1;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}