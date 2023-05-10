namespace Serenity.Data;

/// <summary>
/// Selects field expressions based on dialect
/// </summary>
public class DialectExpressionSelector
{
    private readonly string dialectServerType;
    private readonly string dialectTypeName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialectExpressionSelector"/> class.
    /// </summary>
    /// <param name="dialect">The dialect.</param>
    /// <exception cref="ArgumentNullException">dialect</exception>
    public DialectExpressionSelector(ISqlDialect dialect)
    {
        Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
        dialectServerType = dialect.ServerType;
        dialectTypeName = dialect.GetType().Name;
    }

    private bool IsMatch(string dialect)
    {
        if (dialect[0] == '!')
        {
            dialect = dialect[1..];
            return dialect.Length > 0 && !IsMatch(dialect);
        }

        return dialectServerType.StartsWith(dialect, StringComparison.OrdinalIgnoreCase) ||
            dialectTypeName.StartsWith(dialect, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the best match.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <param name="attributes">The expressions.</param>
    /// <param name="getDialect">The get dialect.</param>
    /// <returns></returns>
    public TAttribute GetBestMatch<TAttribute>(IEnumerable<TAttribute> attributes, 
        Func<TAttribute, string> getDialect)
        where TAttribute: class
    {
        var st = dialectServerType;
        var tn = dialectTypeName;

        var matches = attributes.Select(attr =>
        {
            var d = getDialect(attr);

            if (string.IsNullOrEmpty(getDialect(attr)))
                return (attr, 0);

            if (!d.Contains(',', StringComparison.Ordinal))
                return IsMatch(d) ? (attr, d.Length) : (null, 0);

            bool negate = d.StartsWith('!');
            if (negate)
                d = d[1..];

            var m = d.Split(comma, StringSplitOptions.RemoveEmptyEntries)
                .Select(z => z.Trim())
                .Where(IsMatch);

            if (negate)
            {
                if (m.Any())
                    return (null, 0);

                return (attr, d.Length + 1);
            }

            return m.OrderByDescending(z => z.Length)
                .Select(z => (attr, z.Length))
                .FirstOrDefault();

        }).Where(x => x.attr != null);

        var count = matches.Count();

        if (count == 0)
            return null;

        if (count == 1)
            return matches.First().attr;

        if (matches.Select(x => getDialect(x.attr)).Distinct().Count() == count)
            return matches.OrderByDescending(x => x.Item2).First().attr;

        var duplicate = matches.GroupBy(x => getDialect(x.attr))
            .Where(x => x.Count() > 1)
            .First();

        throw new AmbiguousMatchException(string.Format(
            "There are multiple attributes matching the dialect: {0}",
            duplicate.Key.TrimToNull() ?? "<null>"));
    }

    private static readonly char[] comma = new char[] { ',' };

    /// <summary>
    /// Gets the dialect used for this expression selector
    /// </summary>
    public ISqlDialect Dialect { get; }
}