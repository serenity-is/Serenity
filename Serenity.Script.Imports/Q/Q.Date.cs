using System;
using System.Runtime.CompilerServices;
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
        [InlineCode("Q.formatDate({date}, {format})")]
        public static string FormatDate(this JsDate date, string format = null)
        {
            return null;
        }

        [InlineCode("Q.formatDate({date}, {format})")]
        public static string FormatDate(string date, string format = null)
        {
            return null;
        }

        [InlineCode("Q.formatISODateTimeUTC({date})")]
        public static string FormatISODateTimeUTC(JsDate date)
        {
            return null;
        }

        [InlineCode("Q.parseDate({value})")]
        public static JsDate ParseDate(string value)
        {
            return null;
        }

        [InlineCode("Q.parseISODateTime({value})")]
        public static JsDate ParseISODateTime(string value)
        {
            return null;
        }
    }
}