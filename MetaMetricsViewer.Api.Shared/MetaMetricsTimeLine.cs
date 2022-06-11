using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MK.Classification;
using NodaTime;

namespace MetaMetrics.Api
{
    public class MetaMetricsInstallationTimeLine : IComparer<MetaMetricsMeasurementGroup>, INotifyPropertyChanged
    {
        private string _ik;
        private string _standort;
        private string _group;
        private string _kis;
        private string _server;
        private string _license;
        private string _version;
        private string _name;
        private string _interface;
        private string _projectId;
        private string _products;
        private string _hospitals;
        private MKStandortVerzeichnisKH _hospital;
        private string _hospitalName;
        private string _hospitalTraeger;
        private string _hospitalSitz;
        private string _hospitalStrasse;
        private string _hospitalPlz;
        private string _hospitalOrt;
        private double _hospitalLatitude;
        private double _hospitalLongitude;
        private int _hospitalBetten;
        private int _hospitalFaelle;
        private MKStandortVerzeichnisStandort _hospitalStandort;
        private string[] _standorte;
        public MetaMetricsQuery Query { private set; get; }
        public MetaMetricsInstallationMapEntry Installation { private set; get; }

        public MetaMetricsInstallationTimeLine(MetaMetricsQuery query, MetaMetricsInstallationMapEntry installation)
        {
            Query = query;
            Installation = installation;
            Group = installation.Group;
            Name = installation.Fullname;
            Server = installation.Server;
            License = installation.License;
            Version = installation.Version;
            KIS = installation.KIS;
            Hospitals = installation.Hospitals;
            Interface = installation.Interface;
            IK = installation.IK;
            Standort = installation.Standort;
            Standorte = installation.Standorte;
            ProjectID = installation.ProjectID;
            Products = installation.Products;
            Sublicense = installation.Sublicense;
        }

        public string Sublicense { get; set; }

        public int Pos { set; get; }

        public MKStandortVerzeichnisStandort HospitalStandort
        {
            set
            {
                if (ReferenceEquals(_hospitalStandort, value))
                    return;
                _hospitalStandort = value;
                
                HospitalOrt = _hospitalStandort?.Ort;
                HospitalPlz = _hospitalStandort?.Plz;
                HospitalStrasse = _hospitalStandort?.Strasse;
                HospitalName = _hospitalStandort?.Bezeichnung;
            }
            get => _hospitalStandort;
        }

        public MKStandortVerzeichnisKH Hospital
        {
            get => _hospital;
            set
            {
                if (ReferenceEquals(_hospital, value))
                    return;
                _hospital = value;
                HospitalName = _hospital?.Bezeichnung;
                HospitalTraeger = _hospital?.Traeger;
                HospitalSitz = _hospital?.Sitz;
                HospitalLatitude = _hospital?.Latitude??0d;
                HospitalLongitude = _hospital?.Longitude??0d;
                HospitalBetten = _hospital?.Betten??0;
                HospitalFaelle = _hospital?.Faelle??0;
                if (HospitalStandort == null)
                    HospitalStandort = _hospital?.Standorte?.FirstOrDefault();
            }
        }

        public string HospitalName
        {
            set
            {
                if (_hospitalName == value)
                    return;
                _hospitalName = value;
                OnPropertyChanged();
            }
            get => _hospitalName;
        }

        public string HospitalTraeger
        {
            set
            {
                if (value == _hospitalTraeger) return;
                _hospitalTraeger = value;
                OnPropertyChanged();
            }
            get => _hospitalTraeger;
        }

        public string HospitalSitz
        {
            set
            {
                if (value == _hospitalSitz) return;
                _hospitalSitz = value;
                OnPropertyChanged();
            }
            get => _hospitalSitz;
        }

        public string HospitalStrasse
        {
            set
            {
                if (value == _hospitalStrasse) return;
                _hospitalStrasse = value;
                OnPropertyChanged();
            }
            get => _hospitalStrasse;
        }

        public string HospitalPlz
        {
            set
            {
                if (value == _hospitalPlz) return;
                _hospitalPlz = value;
                OnPropertyChanged();
            }
            get => _hospitalPlz;
        }

        public string HospitalOrt
        {
            set
            {
                if (value == _hospitalOrt) return;
                _hospitalOrt = value;
                OnPropertyChanged();
            }
            get => _hospitalOrt;
        }

        public double HospitalLatitude
        {
            set
            {
                if (value.Equals(_hospitalLatitude)) return;
                _hospitalLatitude = value;
                OnPropertyChanged();
            }
            get => _hospitalLatitude;
        }

        public double HospitalLongitude
        {
            set
            {
                if (value.Equals(_hospitalLongitude)) return;
                _hospitalLongitude = value;
                OnPropertyChanged();
            }
            get => _hospitalLongitude;
        }

        public int HospitalBetten
        {
            set
            {
                if (value == _hospitalBetten) return;
                _hospitalBetten = value;
                OnPropertyChanged();
            }
            get => _hospitalBetten;
        }

        public int HospitalFaelle
        {
            set
            {
                if (value == _hospitalFaelle) return;
                _hospitalFaelle = value;
                OnPropertyChanged();
            }
            get => _hospitalFaelle;
        }

        public MetaMetricsMeasurementGroup this[MetaMetricsMeasurementType meas]
        {
            get
            {
               // var meas = txt.ToEnum(MetaMetricsMeasurementType.Unknown);
                return TimeValues.FirstOrDefault(n=>n.Measurement == meas);
            }
        }
        
        public int Nr { set; get; }

        public string Name
        {
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
            get => _name;
        }

        public string KIS
        {
            set
            {
                if (value == _kis) return;
                _kis = value;
                OnPropertyChanged();
            }
            get => _kis;
        }

        public string Hospitals
        {
            set
            {
                if (value == _hospitals) return;
                _hospitals = value;
                OnPropertyChanged();
            }
            get => _hospitals;
        }

        public string Server
        {
            set
            {
                if (value == _server) return;
                _server = value;
                OnPropertyChanged();
            }
            get => _server;
        }

        public string License
        {
            set
            {
                if (value == _license) return;
                _license = value;
                OnPropertyChanged();
            }
            get => _license;
        }

        public string Version
        {
            set
            {
                if (value == _version) return;
                _version = value;
                OnPropertyChanged();
            }
            get => _version;
        }

        public string Interface
        {
            set
            {
                if (value == _interface) return;
                _interface = value;
                OnPropertyChanged();
            }
            get => _interface;
        }

        
        public string Group
        {
            set
            {
                if (value == _group) return;
                _group = value;
                OnPropertyChanged();
            }
            get => _group;
        }

        public string IK
        {
            set
            {
                if (_ik == value)
                    return;
                _ik = value;
                var nik = IK.ToInt();
                //var nst = Standort.ToInt();
                var standorte = MKStandortVerzeichnisFactory.Instance[""];
                if (standorte.CouldLoad)
                {
                    Hospital = standorte.GetKH(nik);
                    //HospitalStandort = standorte.GetStandort(nst);
                }

                OnPropertyChanged();
            }
            get => _ik;
        }
        
        public string Standort
        {
            set
            {
                if (value == _standort) return;
                _standort = value;
                OnPropertyChanged();
                var nst = Standort.ToInt();
                var standorte = MKStandortVerzeichnisFactory.Instance[""];
                if (standorte.CouldLoad)
                {
                    HospitalStandort = standorte.GetStandort(nst);
                }
            }
            get => _standort;
        }

        public string[] Standorte
        {
            set
            {
                if (ReferenceEquals(_standorte, value))
                    _standorte = value;
                OnPropertyChanged();
            }
            get => _standorte;
        }

        public string ProjectID
        {
            set
            {
                if (value == _projectId) return;
                _projectId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProjectIDInt));
            }
            get => _projectId;
        }

        public int ProjectIDInt => (ProjectID ?? "").Split(' ', ',').First().ToInt();

        public string Products
        {
            set
            {
                if (value == _products) return;
                _products = value;
                OnPropertyChanged();
            }
            get => _products;
        }

        public List<MetaMetricsMeasurementGroup> TimeValues { get; } = new List<MetaMetricsMeasurementGroup>();
        public DateTime FirstTime => TimeValues.Any()?TimeValues.Min(n => n.FirstTime):DateTime.MinValue;

       
        public DateTime LastData
        {
            get
            {
                var data = TimeValues.Any() ? TimeValues.Min(n => n.FirstTime) : DateTime.MinValue;
                if (data > Installation.LastData)
                    return data;
                return Installation.LastData;
            }
        }

        public DateTime LastFromTime => TimeValues.Any()?TimeValues.Max(n => n.LastFromTime):DateTime.MinValue;
        public DateTime LastTillTime => TimeValues.Any()?TimeValues.Max(n => n.LastTillTime):DateTime.MinValue;
        public long Sum => TimeValues.Sum(n => n.Sum);

        
        public string[] Produkte { get; set; }

        //public Point LongLat => new Point(HospitalLongitude, HospitalLatitude);
        /*
        public double Distance(double longitude, double latitude)
        {
            //return LongLat.Distance(longitude, latitude);
            return 0;
               var p = Math.PI/180;  
            var a = 0.5 - Math.Cos((latitude - Latitude) * p) / 2 +
                    Math.Cos(Latitude * p) * Math.Cos(latitude * p) *
                    (1 - Math.Cos((longitude - Longitude) * p)) / 2;

            return 12742 * Math.Asin(Math.Sqrt(a));
        }
       */

        public override string ToString()
        {
            return $"{Name} ({TimeValues.Count}: {Sum}) [{FirstTime} - {LastTillTime}]";
        }

        public void Add(string measurement, string item, Instant time, long value, int combine, int offset, DateTime maxDate)
        {
            var group = new MetaMetricsMeasurementGroup(this, measurement);
            
            var index = TimeValues.BinarySearch(group, this);
            if (index < 0)
            {
                index = ~index;
                TimeValues.Insert(index, group);
            }
            else
            {
                group = TimeValues[index];
            }
         
            group.Add(item, time,  value, combine, offset, maxDate);
        }

        public int Compare(MetaMetricsMeasurementGroup x, MetaMetricsMeasurementGroup y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Measurement.CompareTo(y.Measurement);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}