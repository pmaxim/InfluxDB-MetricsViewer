using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MetaMetrics.Api;
using MetaMetricsViewer.Wpf.Annotations;
using MK.Classification;
using MK.CommonZip;

namespace MetaMetricsViewer.Wpf
{
    public partial class EditInstallation : Window , INotifyPropertyChanged
    {
        private MetaMetricsInstallationMapEntry _installation;
        private string _key;
        private string _mapTo;
        private string _group;
        private string _server;
        private string _hospitals;
        private string _zusatz;
        private string _kis;
        private string _interface;
        private string _ik;
        private string _standort;
        private string _projectID;
        private string _products;
        private string _display;
        private bool _ignore;

        public EditInstallation()
        {
            AlleStandorte = MKStandortVerzeichnisFactory.Instance[""].Standorte.Select(n => new ListBoxItem()
            {
                Content = $"{n.KH.Bezeichnung} #{n.IK}\n{n.Bezeichnung} #{n.ID}\n{n.KH.Sitz}\n{n.KH.Traeger}\n{n.Strasse}\n{n.Plz} {n.Ort}",
                Tag = n,
            }).ToList();
            
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MetaMetricsInstallationMapEntry Installation
        {
            set
            {
                if (ReferenceEquals(_installation, value))
                    return;
                _installation = value;
                OnPropertyChanged();
                Key = _installation?.Key;
                KIS = _installation?.KIS;
                Server = _installation?.Server;
                Interface = _installation?.Interface;
                Display = _installation?.Display;
                Group = _installation?.Group;
                Zusatz = _installation?.Zusatz;
                MapTo = _installation?.MapTo;
                IK = _installation?.IK;
                Standort = _installation?.Standort;
                Standorte = _installation?.Standorte;
                Hospitals = _installation?.Hospitals;
                ProjectID = _installation?.ProjectID;
                Products = _installation?.Products;
                Server = _installation?.Server;
                Ignore = _installation?.Ignore??false;
                Delete.Visibility = Visibility.Visible;
                KeyField.IsReadOnly = true;
            }
            get => _installation;
        }

        public bool Ignore
        {
            get => _ignore;
            set
            {
                if (value == _ignore) return;
                _ignore = value;
                OnPropertyChanged();
            }
        }

        public string Key
        {
            set
            {
                if (value == _key) return;
                _key = value?.Trim();
                OnPropertyChanged();
            }
            get => _key;
        }

        public string MapTo
        {
            set
            {
                if (value == _mapTo) return;
                _mapTo = value?.Trim();
                OnPropertyChanged();
            }
            get => _mapTo;
        }

        public string Group
        {
            set
            {
                if (value == _group) return;
                _group = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _group;
        }

        public string Display
        {
            set
            {
                if (value == _display) return;
                _display = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _display;
        }

        public string Server
        {
            set
            {
                if (value == _server) return;
                _server = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _server;
        }

        public string Hospitals
        {
            set
            {
                var h = value.ToInt();
                if (h < 1)
                    h = 1;
                if (h == _hospitals.ToInt()) return;
                _hospitals = h.ToString();
                OnPropertyChanged();
            }
            get => _hospitals;
        }

        public string KIS
        {
            set
            {
                if (value == _kis) return;
                _kis = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _kis;
        }
        
        public string Interface
        {
            set
            {
                if (value == _interface) return;
                _interface = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _interface;
        }

        public string IK
        {
            set
            {
                if (value == _ik) return;
                _ik = value?.Trim(' ',',');
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_ik))
                    FilterText = $"{IK} {Standort}";
            }
            get => _ik;
        }

        private string[] _standorte;
        public string[] Standorte
        {
            set
            {
                if (value == _standorte) return;
                _standorte = value;
                OnPropertyChanged();
            }
            get => _standorte;
        }
        
        public string Standort
        {
            set
            {
                if (value == _standort) return;
                _standort = value?.Trim();
                OnPropertyChanged();
                FilterText = $"{IK} {Standort}";
            }
            get => _standort;
        }
        
        public string ProjectID
        {
            set
            {
                if (value == _projectID) return;
                _projectID = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _projectID;
        }
        public string Products
        {
            set
            {
                if (value == _products) return;
                _products = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _products;
        }

        public string Zusatz
        {
            set
            {
                if (value == _zusatz) return;
                _zusatz = value?.Trim(' ',',');
                OnPropertyChanged();
            }
            get => _zusatz;
        }

        private string _filterText;
        private List<ListBoxItem> _alleStandorte;
        private List<ListBoxItem> _filteredStandorte;

        public string FilterText
        {
            set
            {
                if (value == _filterText) return;
                _filterText = value;
                OnPropertyChanged();
                RunFilter();
            }
            get => _filterText;
        }

        public List<ListBoxItem> AlleStandorte
        {
            set
            {
                _alleStandorte = value;
                OnPropertyChanged();
            }
            get => _alleStandorte ?? new List<ListBoxItem>();
        }
        
        public List<ListBoxItem> FilteredStandorte
        {
            set
            {
                _filteredStandorte = value;
                OnPropertyChanged();
            }
            get => _filteredStandorte ?? AlleStandorte;
        }
        
        
        private int filtercounter = 0;
        void RunFilter()
        {
            var version = Interlocked.Increment(ref filtercounter);
            if (!string.IsNullOrEmpty(FilterText))
            {
                var fi = FilterText.Searchable().Trim().Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                var filtered = AlleStandorte.Where(n=>$"{n.Content.ToString().Searchable()}".Contains(fi)).ToList();
                Task.Run(() =>
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        if (version == filtercounter)
                            FilteredStandorte = filtered;
                    });
                });
            }
            else
            {
                FilteredStandorte = AlleStandorte.ToList();
            }
        }

        public Action<MetaMetricsInstallationMapEntry> OnSave { get; set; }
        public Action<MetaMetricsInstallationMapEntry> OnDelete { get; set; }

        void SaveAction(MetaMetricsInstallationMapEntry entry)
        {
            if (!string.IsNullOrEmpty(Key))
            {
                OnSave?.Invoke(entry);
            }
        }
        
        void DeleteAction(MetaMetricsInstallationMapEntry entry)
        {
            OnDelete?.Invoke(entry);
        }

        private void Cancel_Button(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OK_Button(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            SaveAction(new MetaMetricsInstallationMapEntry()
            {
                Key = _key, 
                Display = Display, 
                Group = Group, 
                Zusatz = _zusatz, 
                MapTo = MapTo, 
                Server = Server, 
                KIS = KIS, 
                Interface = Interface, 
                IK = IK, 
                Standort = Standort, 
                Standorte = Standorte, 
                Hospitals = Hospitals, 
                ProjectID = ProjectID, 
                //Products = Products,
                Ignore = Ignore
            });
            this.Close();
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            DeleteAction(Installation);
            this.Close();
        }

        private void StandortListe_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var ik = ((((ListBox)sender)?.SelectedItem as ListBoxItem)?.Tag as MKStandortVerzeichnisStandort)?.IK??0;
            if (ik > 0)
                IK = ik.ToString();  
            var st = ((((ListBox)sender)?.SelectedItem as ListBoxItem)?.Tag as MKStandortVerzeichnisStandort)?.ID??0;
            if (st > 0)
                Standort = st.ToString();
        }
    }
}