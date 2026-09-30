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

        // Skip regions (quoted strings / identifiers and -- / /* */ comments) use
        // the same rules as JoinAliasLocator.EnumerateAliases: comment markers only
        // apply outside quotes, quotes don't span comments, block comments don't
        // nest, and only '\n' ends a line comment.
        char? quoteChar = null;
        bool inLineComment = false;
        bool inBlockComment = false;
        int startIdent = -1;
        for (var index = 0; index < expression.Length; index++)
        {
            var c = expression[index];
            sb.Append(c);

            if (inBlockComment)
            {
                if (c == '*' && index + 1 < expression.Length && expression[index + 1] == '/')
                {
                    sb.Append(expression[++index]);
                    inBlockComment = false;
                }
            }
            else if (inLineComment)
            {
                if (c == '\n')
                    inLineComment = false;
            }
            else if (quoteChar != null)
            {
                if (c == quoteChar)
                    quoteChar = null;
            }
            else
            {
                if (c == '\'' || c == '"' || c == '`')
                {
                    quoteChar = c;
                    startIdent = -1;
                }
                else if (c == '-' && index + 1 < expression.Length && expression[index + 1] == '-')
                {
                    sb.Append(expression[++index]);
                    startIdent = -1;
                    inLineComment = true;
                }
                else if (c == '/' && index + 1 < expression.Length && expression[index + 1] == '*')
                {
                    sb.Append(expression[++index]);
                    startIdent = -1;
                    inBlockComment = true;
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