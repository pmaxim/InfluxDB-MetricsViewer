using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using MK.Classification;

namespace MetaMetrics.Api
{
    public class MetaMetricsInstallationService
    {
        private static MetaMetricsInstallationService instance;
        public static MetaMetricsInstallationService Instance => instance ?? (instance = new MetaMetricsInstallationService());
        public Dictionary<string, MetaMetricsInstallationMapEntry> Installations { set; get; } = new Dictionary<string, MetaMetricsInstallationMapEntry>(StringComparer.InvariantCultureIgnoreCase);

        public string Filename { set; get; } = Path.GetFullPath(Path.Combine("Installations.xml"));

        public MetaMetricsInstallationService()
        {
            Load();
        }

        private const string xml__entry = "Entry";
        private const string xml__installations = "Installations";

        public MetaMetricsInstallationMapEntry this[string key]
        {
            get
            {
                MetaMetricsInstallationMapEntry res;
                if (!Installations.TryGetValue(key, out res))
                {
                    res = new MetaMetricsInstallationMapEntry() { Key = key };
                    Installations[res.Key] = res;
                }
                return res;
            }
            set
            {
                if (!string.IsNullOrEmpty(key))
                {
                    if (value == null)
                        Installations.Remove(key);
                    else
                        Installations[key] = value;
                }
            }
        }
        
        public void Load()
        {
            if (File.Exists(Filename))
            {
                
                var xml = XElement.Load(Filename);
                foreach (var xmlentry in xml.Elements(xml__entry))
                {
                    var entry = MetaMetricsInstallationMapEntry.FromXml(xmlentry);
                    if (entry != null)
                    {
                        Installations[entry.Key] = entry;
                    }
                }
            }
        }

        public void Save()
        {
            var xml = new XElement(xml__installations);
            foreach (var entry in Installations.OrderBy(n=>n.Value.Ignore).ThenBy(n=>n.Value.Group??"").ThenBy(n=>n.Value.Display??n.Key).ThenBy(n=>n.Key).Select(n=>n.Value))
            {
                xml.Add(entry.ToXml(xml__entry));
            }
            xml.Save(Filename);
        }

        public void Remove(string objKey)
        {
            if (!string.IsNullOrEmpty(objKey))
            {
                this.Installations.Remove(objKey);
            }
        }
    }
    
    public class MetaMetricsInstallationMapEntry
    {
        public string Key { set; get; }
        public string MapTo { set; get; }
        public string Display { set; get; }
        public string Group { set; get; }
        public DateTime LastData { set; get; }
        public string Server { set; get; }
        public string License { set; get; }
        public string Version { set; get; }
        public string Sublicense { set; get; }
        public string Zusatz { set; get; }
        public string KIS { set; get; }
        public string Interface { set; get; }
        public string IK { set; get; }
        public string Standort { set; get; }
        public string[] Standorte { set; get; }
        public string Hospitals { set; get; }
        public string ProjectID { set; get; }
        public string Products => string.Join(", ", AllProducts.Keys.OrderBy(n => n));

        public Dictionary<string, bool> AllProducts { set; get; } = new Dictionary<string, bool>();
        public bool Ignore { set; get; }

        public string Fullname
        {
            get
            {
                var name = Key;
                if (!string.IsNullOrEmpty(Display))
                {
                    name = Display;
                }

                if (!string.IsNullOrEmpty(Group))
                {
                    name = Group + ", " + name;
                }

                if (!string.IsNullOrEmpty(Zusatz))
                {
                    name = name + " (" + Zusatz + ")";
                }

                if (!string.IsNullOrEmpty(KIS) || !string.IsNullOrEmpty(Interface) || !string.IsNullOrEmpty(Products))
                {
                    name = name + " [" + string.Join(", ", new[] { KIS, Interface, Products }.Where(n => !string.IsNullOrEmpty(n))) + "]";
                }

                return name;
            }
        }
        
        public static MetaMetricsInstallationMapEntry FromXml(XElement xml)
        {
            if (xml != null)
            {
                var ret = new MetaMetricsInstallationMapEntry();
                ret.Key = xml.Get(nameof(Key),null)?.Trim();
                ret.Display = xml.Get(nameof(Display),null)?.Trim();
                ret.Group = xml.Get(nameof(Group),null)?.Trim();
                ret.Server = xml.Get(nameof(Server),null)?.Trim();
                ret.License = xml.Get(nameof(License),null)?.Trim();
                ret.Version = xml.Get(nameof(Version),null)?.Trim();
                ret.Zusatz = xml.Get(nameof(Zusatz),null)?.Trim();
                ret.MapTo = xml.Get(nameof(MapTo),null)?.Trim();
                ret.KIS = xml.Get(nameof(KIS),null)?.Trim();
                ret.Interface = xml.Get(nameof(Interface),null)?.Trim();
                ret.IK = xml.Get(nameof(IK),null)?.Trim();
                ret.Standort = xml.Get(nameof(Standort),null)?.Trim();
                ret.ProjectID = xml.Get(nameof(ProjectID),null)?.Trim();
                //ret.Products = xml.Get(nameof(Products),null)?.Trim();
                ret.Ignore = xml.Get(nameof(Ignore),"").ToBool();
                ret.Hospitals = xml.Get(nameof(Hospitals),null)?.Trim();
                ret.LastData = xml.Get(nameof(LastData),null)?.Trim().ToDateTime("yyyyMMddHHmm")??DateTime.MinValue;
                ret.Standorte = xml.Elements(nameof(Standorte)).Select(n => n.Value).SelectMany(n => n.Split(new[] { '\n', '\r',',',';',' ' })).Select(n => n.Trim()).Where(n => !string.IsNullOrEmpty(n)).GroupBy(n => n, StringComparer.InvariantCultureIgnoreCase).Select(n => n.Key).ToArray();
                if (ret.Hospitals.ToInt() < 1)
                    ret.Hospitals = "1";
                
                if (!string.IsNullOrEmpty(ret.Key))
                    return ret;
            }

            return null;
        }

        public XElement ToXml(string tag)
        {
            var xml = new XElement(tag);
            
            if (!string.IsNullOrEmpty(Group))
            {
                xml.Add(new XAttribute(nameof(Group),Group));
            }  
            if (!string.IsNullOrEmpty(Server))
            {
                xml.Add(new XAttribute(nameof(Server),Server));
            }
            if (!string.IsNullOrEmpty(Version))
            {
                xml.Add(new XAttribute(nameof(Version),Version));
            }
            if (!string.IsNullOrEmpty(License))
            {
                xml.Add(new XAttribute(nameof(License),License));
            }
            if (!string.IsNullOrEmpty(Display))
            {
                xml.Add(new XAttribute(nameof(Display),Display));
            }
            else
            {
                xml.Add(new XAttribute(nameof(Display),Key)); 
            }
            if (!string.IsNullOrEmpty(Zusatz))
            {
                xml.Add(new XAttribute(nameof(Zusatz),Zusatz));
            }
            
            if (!string.IsNullOrEmpty(KIS))
            {
                xml.Add(new XAttribute(nameof(KIS),KIS));
            }
            if (!string.IsNullOrEmpty(Interface))
            {
                xml.Add(new XAttribute(nameof(Interface),Interface));
            }
            if (!string.IsNullOrEmpty(IK))
            {
                xml.Add(new XAttribute(nameof(IK),IK));
            }
            if (!string.IsNullOrEmpty(Standort))
            {
                xml.Add(new XAttribute(nameof(Standort),Standort));
            }
            if (Hospitals.ToInt()>1)
            {
                xml.Add(new XAttribute(nameof(Hospitals),Hospitals));
            }

            if (Standorte != null)
            {
                foreach (var item in Standorte)
                {
                    xml.Add(new XElement(nameof(Standorte),item));
                }
            }

            if (!string.IsNullOrEmpty(ProjectID))
            {
                xml.Add(new XAttribute(nameof(ProjectID),ProjectID));
            }
            if (!string.IsNullOrEmpty(Products))
            {
                xml.Add(new XAttribute(nameof(Products),Products));
            }
            
            if (!string.IsNullOrEmpty(Key))
            {
                xml.Add(new XAttribute(nameof(Key),Key));
            }        
            if (!string.IsNullOrEmpty(MapTo))
            {
                xml.Add(new XAttribute(nameof(MapTo),MapTo));
            }
            if (LastData != DateTime.MinValue)
            {
                xml.Add(new XAttribute(nameof(LastData),LastData.ToString("yyyyMMddHHmm")));
            }
            if (Ignore)
            {
                xml.Add(new XAttribute(nameof(Ignore),Ignore));
            }
            return xml;
        }
    }
}