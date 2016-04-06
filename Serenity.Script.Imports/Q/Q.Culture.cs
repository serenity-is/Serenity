
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
        [Imported, ScriptNamespace("Q"), ScriptName("Culture")]
        public static class Culture
        {
            /// <summary>
            /// Decimal char (dot)
            /// </summary>
            public static string DecimalSeparator
            {
                [InlineCode("Q.Culture.decimalSeparator")]
                get;
                [InlineCode("Q.Culture.decimalSeparator = {value}")]
                set;
            }

            /// <summary>
            /// Thousands (digit grouping) seperator that depends on decimal char
            /// </summary>
            
            public static string GroupSeperator
            {
                [InlineCode("Q.Culture.get_groupSeparator()")]
                get { return null; }
            }

            /// <summary>
            /// Date seperator
            /// </summary>
            public static string DateSeparator
            {
                [InlineCode("Q.Culture.dateSeparator")]
                get;
                [InlineCode("Q.Culture.dateSeparator = {value}")]
                set;
            }

            /// <summary>
            /// Order of date parts (date/month/year...)
            /// </summary>
            public static string DateOrder
            {
                [InlineCode("Q.Culture.dateOrder")]
                get;
                [InlineCode("Q.Culture.dateOrder = {value}")]
                set;
            }

            /// <summary>
            /// Default short date format
            /// </summary>
            public static string DateFormat
            {
                [InlineCode("Q.Culture.dateFormat")]
                get;
                [InlineCode("Q.Culture.dateFormat = {value}")]
                set;
            }

            /// <summary>
            /// Default short date time format
            /// </summary>
            public static string DateTimeFormat
            {
                [InlineCode("Q.Culture.dateTimeFormat")]
                get;
                [InlineCode("Q.Culture.dateTimeFormat = {value}")]
                set;
            }
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