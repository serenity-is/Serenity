namespace Serenity.Data;

/// <summary>
/// Removes T0. references in SQL expression
/// </summary>
public static class T0ReferenceRemover
{
    /// <summary>
    /// Removes the "t0." aliases in SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">expression</exception>
    public static string RemoveT0Aliases(string expression)
    {
        if (expression == null)
            throw new ArgumentNullException("expression");

        var sb = new StringBuilder();

        bool inQuote = false;
        int startIdent = -1;
        for (var index = 0; index < expression.Length; index++)
        {
            var c = expression[index];
            sb.Append(c);

            if (inQuote)
            {
                if (c == '\'')
                {
                    inQuote = false;
                }
            }
            else
            {
                if (c == '\'')
                {
                    inQuote = true;
                    startIdent = -1;
                }
                else if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
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
        }

        return sb.ToString();
    }
}