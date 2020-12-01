using System.Globalization;
using System;

namespace Serenity
{
    public class ScriptCulture
    {
        public ScriptCulture()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public ScriptCulture(CultureInfo culture)
        {
            var order = DateHelper.DateElementOrderFor(culture.DateTimeFormat.ShortDatePattern);
            DateOrder = DateHelper.DateOrderString(order);
            DateFormat = DateHelper.DefaultDateFormat(order);
            DateTimeFormat = DateHelper.DefaultDateTimeFormat(order);
            DateSeparator = DateTime.MaxValue.ToString("yy/MM/dd", culture.DateTimeFormat)[2].ToString();
            DecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
            GroupSepearator = culture.NumberFormat.NumberGroupSeparator;
        }

        public string DateOrder { get; set; }
        public string DateFormat { get; set; }
        public string DateSeparator { get; set; }
        public string DateTimeFormat { get; set; }
        public string DecimalSeparator { get; set; }
        public string GroupSepearator { get; set; }
    }
}