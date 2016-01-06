
using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Contains current regional settings
        /// </summary>
        public static class Culture
        {
            static Culture()
            {
                var s = J("script#ScriptCulture").GetHtml().TrimToNull();

                if (s != null)
                {
                    var sc = jQuery.ParseJson(s).As<ScriptCulture>();
                    DecimalSeparator = sc.DecimalSeparator ?? DecimalSeparator;
                    DateSeparator = sc.DateSeparator ?? DateSeparator;
                    DateOrder = sc.DateOrder ?? DateOrder;
                    DateFormat = sc.DateFormat ?? DateFormat;
                    DateTimeFormat = sc.DateTimeFormat ?? DateTimeFormat;
                }
            }

            /// <summary>
            /// Decimal char (dot)
            /// </summary>
            public static string DecimalSeparator = ".";

            /// <summary>
            /// Thousands (digit grouping) seperator that depends on decimal char
            /// </summary>
            public static string GroupSeperator
            {
                get { return DecimalSeparator == "," ? "." : ","; }
            }

            /// <summary>
            /// Date seperator
            /// </summary>
            public static string DateSeparator = "/";

            /// <summary>
            /// Order of date parts (date/month/year...)
            /// </summary>
            public static string DateOrder = "dmy";

            /// <summary>
            /// Default short date format
            /// </summary>
            public static string DateFormat = "dd/MM/yyyy";

            /// <summary>
            /// Default short date time format
            /// </summary>
            public static string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        }

        [Imported, Serializable, PreserveMemberCase]
        private class ScriptCulture
        {
            public string DateOrder { get; set; }
            public string DateFormat { get; set; }
            public string DateSeparator { get; set; }
            public string DateTimeFormat { get; set; }
            public string DecimalSeparator { get; set; }
            public string GroupSepearator { get; set; }
        }
    }
}