namespace Serenity.Data;

/// <summary>
/// Locates alias references in an SQL expression.
/// </summary>
public class JoinAliasLocator
{
    /// <summary>
    /// Locates the aliases in specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The set of aliases found, or <c>null</c> if none are found.</returns>
    /// <exception cref="ArgumentNullException">expression is null.</exception>
    public static HashSet<string>? Locate(string expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        HashSet<string>? aliases = null;
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
    /// <returns>The set of aliases found, or <c>null</c> if none are found.</returns>
    /// <exception cref="ArgumentNullException">expression is null.</exception>
    public static HashSet<string>? LocateOptimized(string expression, out string? singleAlias)
    {
        ArgumentNullException.ThrowIfNull(expression);

        HashSet<string>? aliases = null;
        string? alias = null;
        EnumerateAliases(expression, s =>
        {
            if (aliases == null && (alias == null || (aliases == null && alias == s)))
                alias = s;
            else if (aliases == null)
            {
                aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { alias!, s };
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
    /// <returns><c>true</c> if the expression was processed successfully.</returns>
    public static bool EnumerateAliases(string expression, Action<string> alias)
    {
        // Skip regions via SqlRegionScanner. A '' inside a string is
        // parity-neutral for toggle scanning, so it needs no special case.
        int startIdent = -1;
        for (var i = 0; i < expression.Length; i++)
        {
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i))
            {
                startIdent = -1;
                continue;
            }

            var c = expression[i];

            if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
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

        return true;
    }

    /// <summary>
    /// Replaces the aliases in an SQL expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="replace">The replace function.</param>
    /// <returns>The expression with aliases replaced.</returns>
    public static string ReplaceAliases(string expression, Func<string, string> replace)
    {
        int startIdent = -1;
        var sb = new StringBuilder();
        for (var i = 0; i < expression.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i))
            {
                sb.Append(expression, start, i - start + 1);
                startIdent = -1;
                continue;
            }

            var c = expression[i];
            sb.Append(c);

            if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
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
                        sb.Append('.');
                    }
                }
                startIdent = -1;
            }
            else
                startIdent = -1;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Replaces aliases in the criteria tree while preserving its structure.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="replace">The alias replacement function.</param>
    /// <returns>The criteria with aliases replaced.</returns>
    /// <exception cref="ArgumentNullException">criteria or replace is null.</exception>
    public static BaseCriteria ReplaceAliases(BaseCriteria criteria, Func<string, string> replace)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(replace);

        return new CriteriaAliasReplacer(replace).Rewrite(criteria);
    }

    private sealed class CriteriaAliasReplacer(Func<string, string> replace) : BaseCriteriaVisitor
    {
        public BaseCriteria Rewrite(BaseCriteria criteria)
        {
            return Visit(criteria)!;
        }

        protected override BaseCriteria VisitCriteria(Criteria criteria)
        {
            var expression = JoinAliasLocator.ReplaceAliases(criteria.Expression, replace);
            return expression == criteria.Expression ? criteria : new Criteria(expression);
        }
    }
}