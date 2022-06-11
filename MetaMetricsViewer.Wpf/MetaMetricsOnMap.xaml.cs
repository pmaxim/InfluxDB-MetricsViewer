using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using C1.WPF.Maps;
using MetaMetrics.Api;
using MetaMetricsViewer.Api.Annotations;
using Syncfusion.UI.Xaml.Maps;

namespace MetaMetricsViewer.Wpf
{
    public partial class MetaMetricsOnMap : Window, INotifyPropertyChanged
    {
        private MetaMetricsModel Model;
        private MetricsMapPoint[] _allHospitals;

        public MetaMetricsOnMap(MetaMetricsModel model)
        {
            Model = model;
            _allHospitals = Model.Installations.Where(n => n.HospitalLatitude > 0 && n.HospitalLongitude > 0).Select(s => new MetricsMapPoint { Installation = s }).ToArray();
            InitializeComponent();
            myMaps.Source = null;

            Loaded += (sender, args) =>
            {
                FillHospitals();
            };
        }

        public MetricsMapPoint[] AllHospitals
        {
            get => _allHospitals??Array.Empty<MetricsMapPoint>();
            set
            {
                if (Equals(value, _allHospitals)) return;
                _allHospitals = value;
                OnPropertyChanged();
            }
        }

        void FillHospitals()
        {
            //allHospitalsLayer.Items = AllHospitals;
            
            foreach (var hospital in AllHospitals)
            {
                allHospitalsLayer.Items.Add(BuildMapItem(hospital));
                /*
                var el = new Ellipse(){Fill = new SolidColorBrush(Colors.Red), Width = 20, Height = 20};
                allHospitalsLayer.Items.Add(el);
                */
            }
            
        }

        UIElement BuildMapItem(MetricsMapPoint mapItem)
        {
            var stackPanel = new StackPanel() { Orientation = Orientation.Vertical, Cursor = System.Windows.Input.Cursors.Hand };
            stackPanel.SetValue(C1MapCanvas.LongLatProperty, mapItem.LongLat);
            stackPanel.SetValue(C1MapCanvas.PinpointProperty, new Point(10, 10));
            var imageName = "dot_purple";
            if (mapItem.Installation.LastTillTime.Date >= DateTime.Today)
            {
                imageName = "dot_green";
            }
            var imageSource =  new BitmapImage(new Uri($"pack://application:,,,/Resources/{imageName}.png"));
            var image = new Image() { Source = imageSource, Height = 20 };
            
            stackPanel.Children.Add(image);
            var tooltip = new ToolTip(){
                Background = new SolidColorBrush(Colors.Transparent),
                BorderBrush =  new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0),
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                Placement =PlacementMode.Top,
            };
            var border = new Border()
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                Padding = new Thickness(4),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4)
            };
            var tipStackPanel = new StackPanel();
            var tb = new TextBlock(){Text = mapItem.Installation.Name};
            tipStackPanel.Children.Add(tb);
            border.Child = tipStackPanel;
            tooltip.Content = border;
            ToolTipService.SetToolTip(stackPanel, tooltip);
            return stackPanel;
            /*
            <StackPanel Orientation="Vertical"  Cursor="Hand" PreviewMouseLeftButtonDown="Ellipse_MouseLeftButtonDown"  PreviewMouseRightButtonDown="SetFilterKoordinate"
            c1:C1MapCanvas.LongLat="{Binding LongLat}"
            c1:C1MapCanvas.Pinpoint="6, 6">
                <Image Source="Resources/dot_grey.png" Height="12" ></Image>
                <ToolTipService.ToolTip>
                <ToolTip Background="Transparent" BorderBrush="Transparent" BorderThickness="0" Padding="0" Margin="0">
                <Border Background="White" BorderThickness="1" CornerRadius="4" BorderBrush="Gray" Padding="4">
                <ContentControl ContentTemplate="{StaticResource krankenhausInfo}" Content="{Binding .}"></ContentControl>
                </Border>
                </ToolTip>
                </ToolTipService.ToolTip>
                </StackPanel>
                */
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MetricsMapPoint
    {
        public MetaMetricsInstallationTimeLine Installation { set; get; }

        public Point LongLat => new Point(Installation.HospitalLongitude, Installation.HospitalLatitude);
        public Point Pinpoint => new Point(10, 10);
    }

    public class ImageryLayerExt : ImageryLayer
    {
        protected override string GetUri(int X, int Y, int Scale)
        {
            var link = "http://mt1.google.com/vt/lyrs=y&x=" + X.ToString() + "&y=" + Y.ToString() + "&z=" + Scale.ToString();
            return link;
        }
    } 
    
    
    public class OfflineMapsSource : C1MultiScaleTileSource  
    {  
        private const string uriFormat = "ms-appx:/Tiles/{Z}/{X}/{Y}.png";  

        public OfflineMapsSource()  
            : base(0x8000000, 0x8000000, 0x100, 0x100, 0)  
        { }  

        protected override void GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY, IList<object> source)  
        {  
            if (tileLevel > 8)  
            {  
                var zoom = tileLevel - 8;  
                var uri = uriFormat;  

                uri = uri.Replace("{X}", tilePositionX.ToString());  
                uri = uri.Replace("{Y}", tilePositionY.ToString());  
                uri = uri.Replace("{Z}", zoom.ToString());  
                source.Add(new Uri(uri));  
            }  
        }  
    }  
}