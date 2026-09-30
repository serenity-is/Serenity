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

        char? quoteChar = null;
        bool inLineComment = false;
        bool inBlockComment = false;
        int startBracket = -1;
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];
            sb.Append(c);

            if (inBlockComment)
            {
                // Block comments end only at */; brackets and quotes inside
                // them are comment text, not SQL.
                if (c == '*' && i + 1 < expression.Length && expression[i + 1] == '/')
                {
                    sb.Append(expression[++i]);
                    inBlockComment = false;
                }
            }
            else if (inLineComment)
            {
                // Line comments run to the newline; brackets and quotes inside
                // them are comment text, not SQL. Only '\n' ends the comment.
                if (c == '\n')
                    inLineComment = false;
            }
            else if (quoteChar != null)
            {
                if (c == quoteChar)
                    quoteChar = null;
            }
            else if (c == '\'' || c == '"' || c == '`')
            {
                quoteChar = c;
                startBracket = -1;
            }
            else if (c == '-' && i + 1 < expression.Length && expression[i + 1] == '-')
            {
                // A "--" outside a quoted string starts a line comment. An
                // unclosed bracket before it is invalid SQL anyway; drop it so
                // the bracket can't pair across the comment boundary.
                sb.Append(expression[++i]);
                startBracket = -1;
                inLineComment = true;
            }
            else if (c == '/' && i + 1 < expression.Length && expression[i + 1] == '*')
            {
                // A "/*" outside quoted strings and comments starts a block
                // comment. Drop a pending bracket for the same reason as above;
                // a lone '/' (division) falls through untouched.
                sb.Append(expression[++i]);
                startBracket = -1;
                inBlockComment = true;
            }
            else if (c == '[')
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
    /// "--" line comments and "/* */" block comments are skipped (like
    /// ParamPrefixReplacer): comment detection only applies outside quoted
    /// strings, quote tracking is suspended inside comments, and block
    /// comments don't nest. A "--" inside a block comment (or vice versa)
    /// is plain comment text.
    /// </remarks>
    [return:NotNullIfNotNull(nameof(expression))]
    public static string? ReplaceBrackets(string? expression, ISqlDialect dialect)
    {
        if (expression == null)
            return null;

        char? quoteChar = null;
        bool inLineComment = false;
        bool inBlockComment = false;
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inBlockComment)
            {
                // Block comments end only at */; brackets and quotes inside
                // them are comment text, not SQL.
                sb.Append(c);
                if (c == '*' && i + 1 < expression.Length && expression[i + 1] == '/')
                {
                    sb.Append(expression[++i]);
                    inBlockComment = false;
                }
            }
            else if (inLineComment)
            {
                // Line comments run to the newline; brackets and quotes inside
                // them are comment text, not SQL. Only '\n' ends the comment.
                sb.Append(c);
                if (c == '\n')
                    inLineComment = false;
            }
            else if (quoteChar != null)
            {
                if (c == quoteChar)
                    quoteChar = null;

                sb.Append(c);
            }
            else if (c == '\'' || c == '"' || c == '`')
            {
                quoteChar = c;
                sb.Append(c);
            }
            else if (c == '-' && i + 1 < expression.Length && expression[i + 1] == '-')
            {
                // A "--" outside a quoted string starts a line comment, so a "["
                // inside it must not pair forward into real SQL.
                sb.Append(c);
                sb.Append(expression[++i]);
                inLineComment = true;
            }
            else if (c == '/' && i + 1 < expression.Length && expression[i + 1] == '*')
            {
                // A "/*" outside quoted strings and comments starts a block
                // comment. A lone '/' (division) falls through untouched.
                sb.Append(c);
                sb.Append(expression[++i]);
                inBlockComment = true;
            }
            else if (c == '[')
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