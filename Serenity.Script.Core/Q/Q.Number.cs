using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Converts a number to string with .NET like formatting specifiers
        /// </summary>
        /// <param name="number">Number to be formatted. If null, return value is empty. 
        /// If it is NaN, return value is null.</param>
        /// <param name="format">Format to apply to the number. (Samples: '#,##0.00', '#0.##'...)</param>
        /// <returns>Formatted number string, or null</returns>
        public static string FormatNumber(double? number, string format)
        {
            if (!Script.IsValue(number))
                return "";

            return Q.Externals.FormatNumber(number.As<double>(), format, 
                dec: Q.Culture.DecimalSeparator, group: Q.Culture.GroupSeperator);
        }

        public static Double? ParseDecimal(string value)
        {
            if (value == null || Q.IsTrimmedEmpty(value))
                return null;

            return Q.Externals.ParseDecimal(value);
        }

    }
}
