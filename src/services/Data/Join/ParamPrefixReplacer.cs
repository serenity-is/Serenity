using System.Diagnostics.CodeAnalysis;

namespace Serenity.Data;

/// <summary>
/// Replaces the parameter prefix character in SQL expressions.
/// </summary>
public static class ParamPrefixReplacer
{
    /// <summary>
    /// Replaces the param prefixes in specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="paramPrefix">The parameter prefix.</param>
    /// <returns>The expression with parameter prefixes replaced.</returns>
    [return:NotNullIfNotNull(nameof(expression))]
    public static string? Replace(string? expression, char paramPrefix)
    {
        if (expression == null)
            return null;

        // Same quoted-region tracking as BracketLocator: single quotes (strings
        // everywhere), double quotes (identifiers in Postgres/Oracle, strings
        // in MySQL/T-SQL) and backticks (MySQL identifiers). Contents are never
        // translated regardless of interpretation, and only the matching quote
        // ends a region, so an apostrophe inside "..." (or vice versa) is inert.
        char? quoteChar = null;
        bool inLineComment = false;
        bool inBlockComment = false;
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inBlockComment)
            {
                // Block comments end only at */ (newlines don't end them);
                // never translate inside them.
                sb.Append(c);
                if (c == '*' && i + 1 < expression.Length && expression[i + 1] == '/')
                {
                    sb.Append(expression[++i]);
                    inBlockComment = false;
                }
            }
            else if (inLineComment)
            {
                // Line comments run to the newline; never translate inside them.
                // Only '\n' ends the comment: a lone '\r' never ends a line on
                // its own and may appear stray mid-line.
                sb.Append(c);
                if (c == '\n')
                    inLineComment = false;
            }
            else if (quoteChar != null)
            {
                sb.Append(c);
                if (c == quoteChar)
                    quoteChar = null;
            }
            else if (c == '\'' || c == '"' || c == '`')
            {
                sb.Append(c);
                quoteChar = c;
            }
            else if (c == '-' && i + 1 < expression.Length && expression[i + 1] == '-')
            {
                // A "--" outside a quoted string starts a line comment. Quoted
                // strings are checked first, so 'a--b' is not misdetected, and an
                // apostrophe inside the comment can no longer corrupt the string
                // tracking for the rest of the statement.
                sb.Append(c);
                sb.Append(expression[++i]);
                inLineComment = true;
            }
            else if (c == '/' && i + 1 < expression.Length && expression[i + 1] == '*')
            {
                // A "/*" outside quoted strings and comments starts a block
                // comment (checked after both, so '/*', "-- /*" and nested "/*"
                // never trigger it). A lone '/' (division) falls through.
                sb.Append(c);
                sb.Append(expression[++i]);
                inBlockComment = true;
            }
            else if (c == '@')
            {
                // "@@" is a server-variable escape (e.g. T-SQL @@ROWCOUNT),
                // not a parameter: leave the pair untouched.
                if (i + 1 < expression.Length && expression[i + 1] == '@')
                {
                    sb.Append(c);
                    sb.Append(expression[++i]);
                }
                else
                    sb.Append(paramPrefix);
            }
            else
                sb.Append(c);
        }

        return sb.ToString();
    }
}