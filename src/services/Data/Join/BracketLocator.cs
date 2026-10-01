using System.Diagnostics.CodeAnalysis;

namespace Serenity.Data;

/// <summary>
/// Contains helper methods for locating / replacing brackets in an SQL expression.
/// </summary>
public static class BracketLocator
{
    /// <summary>
    /// Replaces the bracket contents in SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="validChar1">An additional character allowed to be in brackets.</param>
    /// <param name="replace">The replace function.</param>
    /// <returns>The expression with bracket contents replaced.</returns>
    [return:NotNullIfNotNull(nameof(expression))]
    public static string? ReplaceBracketContents(string? expression, char validChar1, Func<string, string> replace)
    {
        if (expression == null)
            return null;

        int startBracket = -1;
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i, skipBracketIdentifiers: false))
            {
                // Brackets and quotes inside skip regions are comment text or
                // literal text, not SQL. An unclosed bracket before the region
                // is invalid SQL anyway; drop it so it can't pair across the
                // boundary.
                sb.Append(expression, start, i - start + 1);
                startBracket = -1;
                continue;
            }

            var c = expression[i];
            sb.Append(c);

            if (c == '[')
            {
                if (startBracket >= 0)
                {
                    // a nested bracket like "[a[b]]" can't be a valid bracketed
                    // identifier, skip replacement for the outer bracket
                    startBracket = -1;
                }
                else
                {
                    startBracket = i;
                }
            }
            else if (c == ']')
            {
                var wasStart = startBracket;
                startBracket = -1;
                if (wasStart >= 0 && wasStart < i - 1)
                {
                    var contents = expression.Substring(wasStart + 1, i - wasStart - 1);
                    var replaced = replace(contents);
                    if (contents != replaced)
                    {
                        sb.Length -= contents.Length + 1;
                        sb.Append(replaced);
                        sb.Append("]");
                    }
                }
            }
            else if (c == '_' || c == validChar1 || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
            }
            else if (startBracket >= 0)
            {
                startBracket = -1;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Replaces the brackets in an SQL expression with the dialect specific ones.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The expression with brackets replaced.</returns>
    /// <remarks>
    /// Quoted regions and comments are skipped via
    /// <see cref="SqlRegionScanner"/>, so a "[" inside them never pairs
    /// forward into real SQL. A lone '/' (division) falls through untouched.
    /// </remarks>
    [return:NotNullIfNotNull(nameof(expression))]
    public static string? ReplaceBrackets(string? expression, ISqlDialect dialect)
    {
        if (expression == null)
            return null;

        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i, skipBracketIdentifiers: false))
            {
                sb.Append(expression, start, i - start + 1);
                continue;
            }

            var c = expression[i];

            if (c == '[')
            {
                if (i > 0 &&
                    (char.IsLetterOrDigit(expression[i - 1]) ||
                     expression[i - 1] == '_'))
                {
                    // might be an array indexer expression like a[5]
                    sb.Append(c);
                    continue;
                }

                var end = expression.IndexOf(']', i + 1);
                if (end < 0)
                {
                    sb.Append(c);
                    continue;
                }

                if (end < expression.Length - 1 &&
                    (expression[end + 1] == '_' ||
                     char.IsLetterOrDigit(expression[end + 1])))
                {
                    sb.Append(c);
                    continue;
                }

                var sub = expression.SafeSubstring(i + 1, end - i - 1);
                if (sub.Length == 0 ||
                    sub.IndexOf('\'') >= 0 ||
                    sub.IndexOf('[') >= 0 ||
                    long.TryParse(sub, out _))
                {
                    sb.Append(c);
                    continue;
                }

                sb.Append(dialect.QuoteIdentifier(sub));
                i = end;
                continue;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
