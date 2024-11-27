namespace Serenity.Data;

/// <summary>
/// Locates alias references in an SQL expression
/// </summary>
public class JoinAliasLocator
{
    /// <summary>
    /// Locates the aliases in specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">expression is null</exception>
    public static HashSet<string> Locate(string expression)
    {
        if (expression == null)
            throw new ArgumentNullException("expression");

        HashSet<string> aliases = null;
        EnumerateAliases(expression, s =>
        {
            aliases ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            aliases.Add(s);
        });

        return aliases;
    }

    /// <summary>
    /// Locates the aliases in a SQL expression, returning first alias in an out parameter.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="singleAlias">The single alias.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">expression is null</exception>
    public static HashSet<string> LocateOptimized(string expression, out string singleAlias)
    {
        if (expression == null)
            throw new ArgumentNullException("expression");

        HashSet<string> aliases = null;
        string alias = null;
        EnumerateAliases(expression, s =>
        {
            if (aliases == null && (alias == null || (aliases == null && alias == s)))
                alias = s;
            else if (aliases == null)
            {
                aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { alias, s };
                alias = null;
            }
            else
                aliases.Add(s);
        });

        singleAlias = alias;
        return aliases;
    }

    /// <summary>
    /// Enumerates the aliases in an SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="alias">The alias handler action.</param>
    /// <returns></returns>
    public static bool EnumerateAliases(string expression, Action<string> alias)
    {
        bool inQuote = false;
        int startIdent = -1;
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];

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
                        startIdent = i;
                }
                else if (c >= '0' && c <= '9')
                {
                }
                else if (c == '.')
                {
                    if (startIdent >= 0 && startIdent < i)
                    {
                        alias(expression[startIdent..i]);
                    }
                    startIdent = -1;
                }
                else
                    startIdent = -1;
            }
        }

        return true;
    }

    /// <summary>
    /// Replaces the aliases in an SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="replace">The replace function.</param>
    /// <returns></returns>
    public static string ReplaceAliases(string expression, Func<string, string> replace)
    {
        bool inQuote = false;
        int startIdent = -1;
        var sb = new StringBuilder();
        for (var i = 0; i < expression.Length; i++)
        {
            var c = expression[i];
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
                        startIdent = i;
                }
                else if (c >= '0' && c <= '9')
                {
                }
                else if (c == '.')
                {
                    if (startIdent >= 0 && startIdent < i)
                    {
                        var alias = expression[startIdent..i];
                        var replaced = replace(alias);
                        if (alias != replaced)
                        {
                            sb.Length -= alias.Length + 1;
                            sb.Append(replaced);
                            sb.Append(".");
                        }
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