using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Serenity;

/// <summary>
///   This static class contains some helper functions that operate on <see cref="string"/> objects.</summary>
public static partial class StringHelper
{
    /// <summary>
    ///   <p>Returns true if <see cref="string"/> is <c>null</c> or empty (zero length)</p></summary>
    /// <remarks>
    ///   <p>This function might be useful if an empty string is assumed to be <c>null</c>.</p>
    ///   <p>This is an extension method, so it can be called directly as <c>str.IsNullOrEmpty()</c>.</p></remarks>
    /// <param name="str">
    ///   String.</param>
    /// <returns>
    ///   If <paramref name="str"/> is <c>null</c> or empty, <c>true</c></returns>
    public static bool IsEmptyOrNull([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    ///   <p>Returns true if <see cref="string"/> is <c>null</c> or empty (zero length)</p></summary>
    /// <remarks>
    ///   <p>This function might be useful if an empty string is assumed to be <c>null</c>.</p>
    ///   <p>This is an extension method, so it can be called directly as <c>str.IsNullOrEmpty()</c>.</p></remarks>
    /// <param name="str">
    ///   String.</param>
    /// <returns>
    ///   If <paramref name="str"/> is <c>null</c> or empty, <c>true</c></returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    ///   Checks if a string <see cref="string"/> is <c>null</c>, empty or just contains whitespace
    ///   characters.</summary>
    /// <remarks>
    ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
    ///   To see a list see <see cref="string.Trim()" /> function.</p>
    ///   <p>This is an extension method, so it can be called directly as <c>str.IsTrimmedEmpty()</c>.</p></remarks>
    /// <param name="str">
    ///   String.</param>
    /// <returns>
    ///   If string is null, empty or contains only white space, <c>true</c></returns>
    public static bool IsTrimmedEmpty([NotNullWhen(false)] this string? str)
    {
        return TrimToNull(str) == null;
    }

    /// <summary>
    ///   <p>Removes whitespace characters in the left or right of the <see cref="string"/> string,
    ///   and if resulting string is empty or null, returns null.</p></summary>
    /// <remarks>
    ///   <p>Generally, when a user entered string is going to be saved to database, if user entered an
    ///   empty string, <c>null</c> or a string of whitespaces, it is stored as <c>null</c>, otherwise
    ///   it is expected to remove whitespace at start and end only.</p>
    ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
    ///   To see a list see <see cref="string.Trim()" /> function.</p>
    ///   <p>This is an extension method, so it can be called directly as <c>str.TrimToNull()</c>.</p></remarks>
    /// <param name="str">
    ///   String to be trimmed.</param>
    /// <returns>
    ///   Trimmed string, result is null if empty.</returns>
    public static string? TrimToNull(this string? str)
    {
        if (str.IsNullOrEmpty())
            return null;

        str = str.Trim();

        if (str.Length == 0)
            return null;
        return str;
    }

    /// <summary>
    ///   <p>Removes whitespace characters in the left or right of the <see cref="string"/> string,
    ///   and if resulting string is empty or null, returns empty.</p></summary>
    /// <remarks>
    ///   <p>Generally, when a user entered string is going to be saved to database, if user entered an
    ///   empty string, <c>null</c> or a string of whitespaces, it is stored as empty string, otherwise
    ///   it is expected to remove whitespace at start and end only.</p>
    ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
    ///   To see a list see <see cref="string.Trim()" /> function.</p>
    ///   <p>This is an extension method, so it can be called directly as <c>str.TrimToEmpty()</c>.</p></remarks>
    /// <param name="str">
    ///   String to be trimmed.</param>
    /// <returns>
    ///   Trimmed string (result won't be null).</returns>
    public static string TrimToEmpty(this string? str)
    {
        if (str.IsNullOrEmpty())
            return string.Empty;
        return str.Trim();
    }

    /// <summary>
    ///   Compares two strings ignoring whitespace at the left or right.</summary>
    /// <remarks>
    ///   <p><c>null</c> is considered to be empty.</p>
    ///   <p><b>Warning:</b> "\n" (line end), "\t" (tab) and some other are also considered as whitespace). 
    ///   To see a list see <see cref="string.Trim()" /> function.</p>
    ///   <p>This function can be used to compare a string entered by user to the value in the database
    ///   for equality.</p></remarks>
    /// <param name="string1">
    ///   String 1.</param>
    /// <param name="string2">
    ///   String 2.</param>
    /// <returns>
    ///   If two strings are same trimmed, true</returns>
    public static bool IsTrimmedSame(this string? string1, string? string2)
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
    ///   Maximum length for the resulting string. If given as 0, or <paramref name="str"/> is shorter
    ///   than this value, string returns as is. Otherwise <paramref name="str"/> 
    ///   it is trimmed to be under this limit in length including "the three dots".</param>
    /// <returns>
    ///   <paramref name="str"/> itself, or trimmed and three dotted string</returns>
    public static string ThreeDots(this string? str, int maxLength)
    {
        if (str == null)
            return string.Empty;

        if (maxLength == 0 ||
            str.Length <= maxLength)
            return str;

        if (maxLength > 3)
            maxLength -= 3;
        else
            return "...";

        return str[..maxLength] + "...";
    }

    /// <summary>
    /// Converts the string to single line by replacing line endings with space.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>Single lined string.</returns>
    public static string ToSingleLine(this string? str)
    {
        return str.TrimToEmpty().Replace("\r\n", " ").Replace("\n", " ").Trim();
    }

    /// <summary>
    /// Converts the string to its single quoted representation.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>Single quoted string.</returns>
    public static string ToSingleQuoted(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return emptySingleQuote;

        StringBuilder sb = new();
        QuoteString(str, sb, false);
        return sb.ToString();
    }

    /// <summary>
    /// Converts the string to its double quoted representation.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>Double quoted string.</returns>
    public static string ToDoubleQuoted(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return emptyDoubleQuote;

        StringBuilder sb = new();
        QuoteString(str, sb, true);
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
        if (string.IsNullOrEmpty(s))
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
                        sb.Append(@"\'");
                    else
                        sb.Append(c);
                    break;
                case '\"':
                    if (doubleQuote)
                        sb.Append(@"\""");
                    else
                        sb.Append(c);
                    break;
                case '\\':
                    sb.Append(@"\\");
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

    /// <summary>
    /// Determines whether the collection is empty or null.
    /// </summary>
    /// <param name="collection">The collection.</param>
    /// <returns>
    ///   <c>true</c> if the collection is empty or null; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmptyOrNull(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }

    /// <summary>
    /// A substring function that doesn't raise out of bound errors or null reference exception.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <returns>Substring or empty string.</returns>
    public static string SafeSubstring(this string? value, int startIndex, int maxLength)
    {
        if (value.IsNullOrEmpty())
            return string.Empty;

        var len = value.Length;
        if (startIndex >= len || maxLength <= 0)
            return string.Empty;

        if (startIndex + maxLength > len)
            return value[startIndex..];

        return value.Substring(startIndex, maxLength);
    }

    /// <summary>
    /// A regex to remove invalid file name characters
    /// </summary>
    public static readonly Regex InvalidFilenameCharsRegex = 
        new ($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
            RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Sanitizes the filename by removing diacritics, ı with i and replacing any
    /// invalid filename characters with underscore.
    /// </summary>
    /// <param name="filename">The string.</param>
    /// <param name="replacement">Replacement string for invalid characters</param>
    /// <param name="removeDiacritics">True to remove diacritics</param>
    /// <exception cref="ArgumentNullException">s is null</exception>
    public static string SanitizeFilename(string filename, 
        string replacement = "_", bool removeDiacritics = true)
    {
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));

        if (removeDiacritics)
        {
            RemoveDiacritics(filename);
            filename = filename.Replace("ı", "i");
        }

        filename = InvalidFilenameCharsRegex.Replace(filename, replacement);

        return filename.TrimToEmpty();
    }

    /// <summary>
    /// A regex to remove invalid file path characters
    /// </summary>
    public static readonly Regex InvalidPathCharsRegex =
        new($"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]",
            RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Sanitizes the path by removing diacritics, ı with i and replacing any
    /// invalid file path characters with underscore.
    /// </summary>
    /// <param name="filename">The string.</param>
    /// <param name="replacement">Replacement string for invalid characters</param>
    /// <param name="removeDiacritics">True to remove diacritics</param>
    /// <exception cref="ArgumentNullException">s is null</exception>
    public static string SanitizeFilePath(string filename,
        string replacement = "_", bool removeDiacritics = true)
    {
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));

        if (removeDiacritics)
        {
            RemoveDiacritics(filename);
            filename = filename.Replace("ı", "i");
        }

        filename = InvalidPathCharsRegex.Replace(filename, replacement);

        return filename.TrimToEmpty();
    }

    /// <summary>
    /// Removes the diacritic characters from string by replacing them with ASCII versions.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>String with diacritics replaced.</returns>
    public static string RemoveDiacritics(string s)
    {
        var normalizedString = s.Normalize(NormalizationForm.FormKD);
        var stringBuilder = new StringBuilder();

        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                if (c == 'ı')
                    c = 'i';

                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Formats a nullable struct
    /// </summary>
    /// <param name="source"></param>
    /// <param name="format">The format string 
    /// If <c>null</c> use the default format defined for the type of the IFormattable implementation.</param>
    /// <param name="provider">The format provider 
    /// If <c>null</c> the default provider is used</param>
    /// <param name="empty">The string to show when the source is <c>null</c>. 
    /// If <c>null</c> an empty string is returned</param>
    /// <returns>The formatted string or the default value if the source is <c>null</c></returns>
    public static string ToStringDefault<T>(this T? source, string? format = null, IFormatProvider? provider = null, string? empty = null)
        where T : struct, IFormattable
    {
        if (source.HasValue)
            return source.Value.ToString(format, provider);

        return empty ?? "";
    }

    /// <summary>
    /// Formats a nullable object
    /// </summary>
    /// <param name="source"></param>
    /// <param name="format">The format string 
    /// If <c>null</c> use the default format defined for the type of the IFormattable implementation.</param>
    /// <param name="provider">The format provider 
    /// If <c>null</c> the default provider is used</param>
    /// <param name="empty">The string to show when the source is <c>null</c>. 
    /// If <c>null</c> an empty string is returned</param>
    /// <returns>The formatted string or the default value if the source is <c>null</c></returns>
    public static string ToStringDefault<T>(this T source, string? format = null, IFormatProvider? provider = null, string? empty = null)
        where T : class, IFormattable
    {
        if (source != null)
            return source.ToString(format, provider);

        return empty ?? "";
    }

    /// <summary>
    /// Joins two strings conditionally, by putting separator between if both are non empty
    /// </summary>
    public static string Join(string a, string separator, string b)
    {
        if (a.IsNullOrEmpty() && b.IsNullOrEmpty())
            return "";

        if (a.IsNullOrEmpty())
            return b ?? "";

        if (b.IsNullOrEmpty())
            return a ?? "";

        return a + separator + b;
    }

    /// <summary>
    /// Joins strings conditionally, by putting separator between if both are non empty or null
    /// </summary>
    public static string JoinNonEmpty(string separator, params string[] values)
    {
        return string.Join(separator, values.Where(x => !string.IsNullOrEmpty(x)));
    }

    /// <summary>
    /// Joins strings conditionally, by putting separator between if both are non empty or null
    /// </summary>
    public static string JoinNonEmpty(string separator, IEnumerable<string> values)
    {
        return string.Join(separator, values.Where(x => !string.IsNullOrEmpty(x)));
    }
}