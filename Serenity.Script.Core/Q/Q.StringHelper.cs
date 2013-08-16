using System;
using System.Text.RegularExpressions;

namespace Serenity
{
    public static partial class Q
    {
        /// <summary>
        /// Trims a text
        /// </summary>
        /// <param name="text">Text to trim</param>
        /// <returns>Trimmed text without space characters at start or end</returns>
        public static string Trim(string text)
        {
            return (text ?? "").Replace(new Regex(@"^\s+|\s+$", "g"), "");
        }

        /// <summary>
        /// <p>Returns true if <see cref="String"/> is <c>null</c> or empty (zero length)</p>
        /// </summary>
        /// <remarks>
        /// <p>This function might be useful if an empty string is assumed to be <c>null</c>.</p>
        /// <p>This is an extension method, so it can be called directly as <c>str.IsEmptyOrNull()</c>.</p>
        /// </remarks>
        /// <param name="str">String.</param>
        /// <returns>If <paramref name="str"/> is <c>null</c> or empty, <c>true</c></returns>
        public static bool IsEmptyOrNull(this string str)
        {
            return str == null || str.Length == 0;
        }

        /// <summary>
        /// Checks if a string <see cref="String"/> is <c>null</c>, empty or just contains whitespace
        /// characters.
        /// </summary>
        /// <remarks>
        /// <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        /// To see a list see <see cref="String.Trim()" /> function.</p>
        /// <p>This is an extension method, so it can be called directly as <c>str.IsTrimmedEmpty()</c>.</p></remarks>
        /// <param name="str">
        /// String.</param>
        /// <returns>
        /// If string is null, empty or contains only white space, <c>true</c></returns>
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
        /// <p>Removes whitespace characters in the left or right of the <see cref="String"/> string,
        /// and if resulting string is empty or null, returns empty.</p></summary>
        /// <remarks>
        /// <p>Generally, when a user entered string is going to be saved to database, if user entered an
        /// empty string, <c>null</c> or a string of whitespaces, it is stored as empty string, otherwise
        /// it is expected to remove whitespace at start and end only.</p>
        /// <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
        /// To see a list see <see cref="String.Trim()" /> function.</p>
        /// <p>This is an extension method, so it can be called directly as <c>str.TrimToEmpty()</c>.</p></remarks>
        /// <param name="str">
        /// String to be trimmed.</param>
        /// <returns>
        /// Trimmed string (result won't be null).</returns>
        public static string TrimToEmpty(this string str)
        {
            if (str == null || str.Length == 0)
                return String.Empty;
            else
                return str.Trim();
        }

        /// <summary>
        /// Replaces line end characters with spaces in a given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>String with line ends replaced.</returns>
        public static string ToSingleLine(this string str)
        {
            return str.TrimToEmpty().Replace("\r\n", " ").Replace("\n", " ").Trim();
        }
    }
}