using System;
using System.Collections;
using System.Text;
using Serenity.Data;
using System.Globalization;

namespace Serenity
{
    /// <summary>
    ///   This static class contains some helper functions that operate on <see cref="String"/> objects.</summary>
    public static partial class StringHelper
    {
        /// <summary>
        ///   <p>Returns <c>null</c>, if <see cref="String"/> is <c>null</c> or empty (zero length),
        ///   otherwise string as is.</p></summary>
        /// <remarks>
        ///   <p>This function might be useful if an empty string is assumed to be <c>null</c>.</p>
        ///   <p>This is an extension method, so it can be called directly as <c>str.EmptyToNull()</c>.</p></remarks>
        /// <param name="str">
        ///   String (can be null).</param>
        /// <returns>
        ///   If <paramref name="str"/> is <c>null</c> or empty <c>null</c>, otherwise string itself</returns>
        public static string EmptyToNull(this string str)
        {
            if (str == null ||
                str.Length == 0)
                return null;
            else
                return str;
        }

        /// <summary>
        ///   <p>Returns true if <see cref="String"/> is <c>null</c> or empty (zero length)</p></summary>
        /// <remarks>
        ///   <p>This function might be useful if an empty string is assumed to be <c>null</c>.</p>
        ///   <p>This is an extension method, so it can be called directly as <c>str.IsEmptyOrNull()</c>.</p></remarks>
        /// <param name="str">
        ///   String.</param>
        /// <returns>
        ///   If <paramref name="str"/> is <c>null</c> or empty, <c>true</c></returns>
        public static bool IsEmptyOrNull(this string str)
        {
            return str == null || str.Length == 0;
        }

        /// <summary>
        ///   Checks if a string <see cref="String"/> is <c>null</c>, empty or just contains whitespace
        ///   characters.</summary>
        /// <remarks>
        ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        ///   To see a list see <see cref="String.Trim()" /> function.</p>
        ///   <p>This is an extension method, so it can be called directly as <c>str.IsTrimmedEmpty()</c>.</p></remarks>
        /// <param name="str">
        ///   String.</param>
        /// <returns>
        ///   If string is null, empty or contains only white space, <c>true</c></returns>
        public static bool IsTrimmedEmpty(this string str)
        {
            return TrimToNull(str) == null;
        }

        /// <summary>
        ///   <p>Removes whitespace characters in the left or right of the <see cref="String"/> string,
        ///   and if resulting string is empty or null, returns null.</p></summary>
        /// <remarks>
        ///   <p>Generally, when a user entered string is going to be saved to database, if user entered an
        ///   empty string, <c>null</c> or a string of whitespaces, it is stored as <c>null</c>, otherwise
        ///   it is expected to remove whitespace at start and end only.</p>
        ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        ///   To see a list see <see cref="String.Trim()" /> function.</p>
        ///   <p>This is an extension method, so it can be called directly as <c>str.TrimToNull()</c>.</p></remarks>
        /// <param name="str">
        ///   String to be trimmed.</param>
        /// <returns>
        ///   Trimmed string, result is null if empty.</returns>
        public static string TrimToNull(this string str)
        {
            if (str == null || str.Length == 0)
                return null;
            else
            {
                str = str.Trim();
                if (str.Length == 0)
                    return null;
                else
                    return str;
            }
        }

        /// <summary>
        ///   <p>Removes whitespace characters in the left or right of the <see cref="String"/> string,
        ///   and if resulting string is empty or null, returns empty.</p></summary>
        /// <remarks>
        ///   <p>Generally, when a user entered string is going to be saved to database, if user entered an
        ///   empty string, <c>null</c> or a string of whitespaces, it is stored as empty string, otherwise
        ///   it is expected to remove whitespace at start and end only.</p>
        ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        ///   To see a list see <see cref="String.Trim()" /> function.</p>
        ///   <p>This is an extension method, so it can be called directly as <c>str.TrimToEmpty()</c>.</p></remarks>
        /// <param name="str">
        ///   String to be trimmed.</param>
        /// <returns>
        ///   Trimmed string (result won't be null).</returns>
        public static string TrimToEmpty(this string str)
        {
            if (str == null || str.Length == 0)
                return String.Empty;
            else
                return str.Trim();
        }

        /// <summary>
        ///   Compares two strings ignoring whitespace at the left or right.</summary>
        /// <remarks>
        ///   <p><c>null</c> is considered to be empty.</p>
        ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        ///   To see a list see <see cref="String.Trim()" /> function.</p>
        ///   <p>This function can be used to compare a string entered by user to the value in the database
        ///   for equality.</p></remarks>
        /// <param name="string1">
        ///   String 1.</param>
        /// <param name="string2">
        ///   String 2.</param>
        /// <returns>
        ///   If two strings are same trimmed, true</returns>
        public static bool IsTrimmedSame(this string string1, string string2)
        {
            if ((string1 == null || string1.Length == 0) && 
                (string2 == null || string2.Length == 0))
                return true;
            else
                return TrimToNull(string1) == TrimToNull(string2);
        }

        /// <summary>
        ///   If the string's length is over a specified limit, trims its right and adds three points ("...").</summary>
        /// <remarks>
        ///   This is an extension method, so it can be called directly as <c>str.ThreeDots()</c>.</remarks> 
        /// <param name="str">
        ///   String.</param>
        /// <param name="maxLength">
        ///   Maksimum length for the resulting string. If given as 0, or <paramref name="str"/> is shorter
        ///   than this value, string returns as is. Otherwise <paramref name="str"/> 
        ///   it is trimmed to be under this limit in length including "the three dots".</param>
        /// <returns>
        ///   <paramref name="str"/> itself, or trimmed and three dotted string</returns>
        public static string ThreeDots(this string str, int maxLength)
        {
            if (str == null)
                return String.Empty;

            if (maxLength == 0 ||
                str.Length <= maxLength)
                return str;

            if (maxLength > 3)
                maxLength -= 3;
            else 
                return "...";

            return str.Substring(0, maxLength) + "...";
        }

        public static string ToSingleLine(this string str)
        {
            return str.TrimToEmpty().Replace("\r\n", " ").Replace("\n", " ").Trim();
        }

        public static string ToSingleQuoted(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return emptySingleQuote;

            StringBuilder sb = new StringBuilder();
            QuoteString(str, sb, false);
            return sb.ToString();
        }

        private const string emptySingleQuote = "''";
        private const string emptyDoubleQuote = "\"\"";

        /// <summary>
        ///   Quotes a string</summary>
        /// <param name="s">
        ///   String</param>
        /// <param name="sb">
        ///   StringBuilder</param>
        /// <param name="doubleQuote">
        ///   True to use double quotes</param>
        public static void QuoteString(string s, StringBuilder sb, bool doubleQuote)
        {
            if (String.IsNullOrEmpty(s))
            {
                if (doubleQuote)
                    sb.Append(emptyDoubleQuote);
                else
                    sb.Append(emptySingleQuote);
                return;
            }

            char c;
            int len = s.Length;

            sb.EnsureCapacity(sb.Length + (s.Length * 2));

            char quoteChar = doubleQuote ? '"' : '\'';
            sb.Append(quoteChar);

            for (int i = 0; i < len; i++)
            {
                c = s[i];

                switch (c)
                {
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    case '\t':
                        sb.Append(@"\t");
                        break;
                    case '\'':
                        if (!doubleQuote)
                            sb.Append(@"\'"); // IE doesn't understand \' in double quoted strings!!!
                        else
                            sb.Append(c);
                        break;
                    case '\"':
                        if (doubleQuote) // IE doesn't understand \" in single quoted strings!!!
                            sb.Append(@"\""");
                        else
                            sb.Append(c);
                        break;
                    case '\\':
                        sb.Append(@"\\");
                        break;
                    case '/':
                        sb.Append(@"\/");
                        break;
                    default:
                        if (c < ' ')
                        {
                            sb.Append(@"\u");
                            sb.Append(((int)c).ToString("X4", Invariants.NumberFormat));
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }

            sb.Append(quoteChar);
        }

        public static bool IsEmptyOrNull(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static string SafeSubstring(this string value, int startIndex, int maxLength)
        {
            if (value.IsEmptyOrNull())
                return String.Empty;

            var len = value.Length;
            if (startIndex >= len || maxLength <= 0)
                return String.Empty;

            if (startIndex + maxLength > len)
                return value.Substring(startIndex);

            return value.Substring(startIndex, maxLength);
        }

        public static String SanitizeFilename(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            s = RemoveDiacritics(s);
            s = s.Replace("/", "_");
            s = s.Replace(":", "_");
            s = s.Replace("&", "_");
            s = s.Replace("ý", "i");
            return s.TrimToEmpty();
        }

        public static String RemoveDiacritics(string s)
        {
            var normalizedString = s.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}