using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Contains external functions in Site.Script.Externals.js
        /// </summary>
        [Imported, IgnoreNamespace, ScriptName("Q")]
        public static class Externals
        {
            /// <summary>
            /// External AlertDialog function
            /// </summary>
            /// <param name="message">Message</param>
            /// <param name="options">Alert options</param>
            [ScriptName("alert")]
            public static void AlertDialog(string message, AlertOptions options)
            {
            }

            /// <summary>
            /// External ConfirmDialog function
            /// </summary>
            /// <param name="message">Message</param>
            /// <param name="options">Confirm options</param>
            [ScriptName("confirm")]
            public static void ConfirmDialog(string message, Action onYes, ConfirmOptions options)
            {
            }

            /// <summary>
            /// External ConfirmDialog function
            /// </summary>
            /// <param name="message">Message</param>
            /// <param name="options">Confirm options</param>
            [ScriptName("iframeDialog")]
            public static void IFrameDialog(dynamic options)
            {
            }

            /// <summary>
            /// External format number method, using a simplified version of jQuery.NumberFormatter.js
            /// </summary>
            /// <remarks>
            /// https://code.google.com/p/jquery-numberformatter/
            /// </remarks>
            public static string FormatNumber(double number, string format, string dec, string group)
            {
                return null;
            }

            /// <summary>
            /// Converts a numeric or string value to its ID representation (integer or string for large numbers)
            /// </summary>
            /// <remarks>
            /// https://code.google.com/p/jquery-numberformatter/
            /// </remarks>
            public static Int64? ToId(object value)
            {
                return null;
            }

            public static void ValidatorAbortHandler(jQueryValidator validator)
            {
            }

            public static Double ParseDecimal(string value)
            {
                return 0;
            }

            public static JsDate ParseDate(string value)
            {
                return null;
            }

            public static JsDate ParseISODateTime(string value)
            {
                return null;
            }

            public static JsDictionary<string, string> ParseQueryString()
            {
                return null;
            }

            public static void PostToService(PostToServiceOptions options)
            {
            }

            public static void PostToUrl(PostToUrlOptions options)
            {
            }

            public static jQueryValidatorOptions ValidateOptions(jQueryValidatorOptions options)
            {
                return null;
            }

            public static int TurkishLocaleCompare(string a, string b)
            {
                return 0;
            }

            public static string TurkishLocaleToUpper(string a)
            {
                return null;
            }

            public static string FormatISODateTimeUTC(JsDate date)
            {
                return null;
            }

            [InlineCode("Select2.util.stripDiacritics({input})")]
            public static string StripDiacritics(string input)
            {
                return null;
            }
        }
    }
}
