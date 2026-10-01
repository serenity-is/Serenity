namespace Serenity.Data;

/// <summary>
/// Removes T0. references in SQL expression.
/// </summary>
public static class T0ReferenceRemover
{
    /// <summary>
    /// Removes the "t0." aliases in SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The expression with T0 references removed.</returns>
    /// <exception cref="ArgumentNullException">expression is null.</exception>
    public static string RemoveT0Aliases(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var sb = new StringBuilder();

        int startIdent = -1;
        for (var index = 0; index < expression.Length; index++)
        {
            var start = index;
            if (expression[index] == '[')
            {
                var end = index;
                if (SqlRegionScanner.TryReadBracketedIdentifier(expression, ref end, out var bracketed))
                {
                    if (bracketed != null && end + 1 < expression.Length && expression[end + 1] == '.' &&
                        string.Equals(bracketed, "T0", StringComparison.OrdinalIgnoreCase))
                    {
                        index = end + 1;
                        startIdent = -1;
                        continue;
                    }

                    sb.Append(expression, start, end - start + 1);
                    index = end;
                    startIdent = -1;
                    continue;
                }
            }

            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index))
            {
                sb.Append(expression, start, index - start + 1);
                startIdent = -1;
                continue;
            }

            var c = expression[index];
            sb.Append(c);

            if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                if (startIdent < 0)
                    startIdent = index;
            }
            else if (c >= '0' && c <= '9')
            {
            }
            else if (c == '.')
            {
                if (startIdent >= 0 &&
                    startIdent < index &&
                    index - startIdent == 2 &&
                    expression[startIdent + 1] == '0' &&
                    char.ToLowerInvariant(expression[startIdent]) == 't')
                {
                    sb.Length -= 3;
                }

                startIdent = -1;
            }
            else
                startIdent = -1;
        }

        return sb.ToString();
    }
}
