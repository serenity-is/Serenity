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

        // Quoted and comment contents are never translated.
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i, skipBracketIdentifiers: true))
            {
                sb.Append(expression, start, i - start + 1);
                continue;
            }

            var c = expression[i];

            if (c == '@')
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
