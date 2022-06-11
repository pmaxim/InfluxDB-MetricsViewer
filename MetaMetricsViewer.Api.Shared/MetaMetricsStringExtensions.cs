using System;
using System.Globalization;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

namespace MetaMetrics.Api
{
    public static class MetaMetricsStringExtensions
    {
        public static TEnum ToEnum<TEnum>(this string obj, TEnum def) where TEnum : struct
        {
            if (!string.IsNullOrEmpty(obj))
            {
                TEnum res = def;
                if (Enum.TryParse(obj.Trim(), true, out res))
                {
                    return res;
                }
            }
            return def;
        }

        public static string Searchable(this string txt)
        {
            if (txt != null)
            {
                var sb = new StringBuilder();
                foreach (var c in txt.Trim().ToLowerInvariant())
                {
                    switch (c)
                    {
                        case 'ß':
                            sb.Append("ss");
                            break;
                        case 'ä':
                            sb.Append("ae");
                            break;
                        case 'ö':
                            sb.Append("oe");
                            break;
                        case 'ü':
                            sb.Append("ue");
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                }

                return sb.ToString();
            }
            return txt;
        }

        public static int ToInt(this string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                int ret;
                if (int.TryParse(txt, NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out ret))
                    return ret;
            }
            return default(int);
        }

        public static DateTime ToDateTime(this string txt, string format)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                DateTime ret;
                if (DateTime.TryParseExact(txt, format, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out ret))
                {
                    return ret;
                }
            }
            return DateTime.MinValue;
        }

        public static bool ToBool(this string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                switch (txt.Trim().ToLower())
                {
                    case "1":
                    case "y":
                    case "yes":
                    case "j":
                    case "ja":
                    case "t":
                    case "true":
                    case "w":
                    case "wahr":
                        return true;
                    case "0":
                    case "n":
                    case "no":
                    case "nein":
                    case "f":
                    case "false":
                    case "falsch":
                        return false;
                    default:
                        if (txt.ToInt() != 0)
                            return true;
                        break;
                }
            }
            return false;
        }

        public static string Get(this XElement xml, string name, string def = "")
        {
            if (xml != null && !string.IsNullOrEmpty(name))
            {
                return xml.Attribute(name)?.Value ?? xml.Element(name)?.Value ?? def;
            }
            return def;
        }

        public static bool Contains(this string text, params string[] search)
        {
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var s in search)
                {
                    if (!text.Contains(s))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}