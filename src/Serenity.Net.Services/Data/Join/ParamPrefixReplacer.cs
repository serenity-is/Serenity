namespace Serenity.Data;

/// <summary>
/// Param prefix replacer
/// </summary>
public static class ParamPrefixReplacer
{
    /// <summary>
    /// Replaces the param prefixes in specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="paramPrefix">The parameter prefix.</param>
    /// <returns></returns>
    public static string Replace(string expression, char paramPrefix)
    {
        if (expression == null)
            return null;

        bool inQuote = false;
        var sb = new StringBuilder(expression.Length);
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

            if (inQuote)
            {
                sb.Append(c);
                if (c == '\'')
                    inQuote = false;
            }
            else if (c == '\'')
            {
                sb.Append(c);
                inQuote = true;
            }
            else if (c == '@')
            {
                sb.Append(paramPrefix);
            }
            else
                sb.Append(c);
        }

        return sb.ToString();
    }
}