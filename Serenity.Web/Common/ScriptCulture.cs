using System.Globalization;
#if COREFX
using System;
#endif

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
#if COREFX
            DateSeparator = DateTime.MaxValue.ToString("/", culture.DateTimeFormat);
#else
            DateSeparator = culture.DateTimeFormat.DateSeparator;
#endif
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