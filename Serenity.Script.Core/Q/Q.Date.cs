using System;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Converts a date to string according to a specified format
        /// </summary>
        /// <param name="date">Date to format, can be null</param>
        /// <param name="format">Format specifier</param>
        /// <returns>Formatted date, or empty string if date is null</returns>
        public static string FormatDate(this JsDate date, string format = null)
        {
            if (date == null)
                return "";

            if (format == null)
                format = Q.Culture.DateFormat;

            Func<int, string> pad = i => i.ToString().PadLeft(2, '0');

            return format.Replace(new Regex("dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|fff|zz?z?|\\/", "g"),
                delegate(string fmt)
                {
                    switch (fmt)
                    {
                        case "/":
                            return Q.Culture.DateSeparator;
                        case "hh":
                            return pad(date.GetHours() < 13 ? date.GetHours() : (date.GetHours() - 12));
                        case "h":
                            return (date.GetHours() < 13 ? date.GetHours() : (date.GetHours() - 12)).As<string>();
                        case "HH":
                            return pad(date.GetHours());
                        case "H":
                            return date.GetHours().As<string>();
                        case "mm":
                            return pad(date.GetMinutes());
                        case "m":
                            return date.GetMinutes().As<string>();
                        case "ss":
                            return pad(date.GetSeconds());
                        case "s":
                            return date.GetSeconds().As<string>();
                        case "yyyy":
                            return date.GetFullYear().As<string>();
                        case "yy":
                            return date.GetFullYear().ToString().Substring(2, 4);
                        case "dddd":
                            return date.As<dynamic>().GetDayName();
                        case "ddd":
                            return date.As<dynamic>().GetDayName(true);
                        case "dd":
                            return pad(date.GetDate());
                        case "d":
                            return date.GetDate().ToString();
                        case "MM":
                            return pad((date.GetMonth() + 1));
                        case "M":
                            return (date.GetMonth() + 1).As<string>();
                        case "t":
                            return (date.GetHours() < 12 ? "A" : "P");
                        case "tt":
                            return (date.GetHours() < 12 ? "AM" : "PM");
                        case "fff":
                            return (date.GetMilliseconds()).ToString().PadLeft(3, '0');
                        case "zzz":
                        case "zz":
                        case "z":
                            return "";
                        default:
                            return fmt;
                    }
                });
        }
    }
}