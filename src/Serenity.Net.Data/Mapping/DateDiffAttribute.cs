namespace Serenity.Data.Mapping;

/// <summary>
/// DateDiff expression attribute
/// </summary>
public class DateDiffAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="part">Datepart like "year", "month" etc.</param>
    /// <param name="start">An expression that returns a date value</param>
    /// <param name="end">An expression that returns a date value</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public DateDiffAttribute(DateParts part, object start, object end)
    {
        Part = part;
        Start = start ?? throw new ArgumentNullException(nameof(start));
        End = end ?? throw new ArgumentNullException(nameof(end));
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        var start = ToString(Start, dialect);
        var end = ToString(End, dialect);

        string expr;

        if ((Part == DateParts.Year || Part == DateParts.Month) && (
             dialect.ServerType == nameof(ServerType.Sqlite) ||
             dialect.ServerType == nameof(ServerType.Postgres) ||
             dialect.ServerType == nameof(ServerType.Oracle)))
        {
            expr = $"({new DatePartAttribute(DateParts.Year, end).ToString(dialect)} - " +
                new DatePartAttribute(DateParts.Year, start).ToString(dialect) + ")";

            if (Part == DateParts.Year)
                return expr;

            return $"({expr} * 12 + " +
                new DatePartAttribute(DateParts.Month, end).ToString(dialect) + " - " +
                new DatePartAttribute(DateParts.Month, start).ToString(dialect) + ")";
        }

        string multiplier()
        {
            return Part switch
            {
                DateParts.Day => "",
                DateParts.Hour => " * 24",
                DateParts.Minute => " * 1440",
                DateParts.Second => " * 86400",
                _ => throw new InvalidOperationException(nameof(Part))
            };
        }

        switch (dialect.ServerType)
        {
            case nameof(ServerType.Sqlite):
                return $"ROUND((JULIANDAY({end}) - JULIANDAY({start})){multiplier()})";

            case nameof(ServerType.Oracle):
                return $"ROUND((CAST ({end} as DATE) - CAST ({start} as DATE)){multiplier()})";

            case nameof(ServerType.Postgres):
                expr = Part switch
                {
                    DateParts.Day => " / 86400",
                    DateParts.Hour => " / 3600",
                    DateParts.Minute => " / 60",
                    DateParts.Second => "",
                    _ => throw new InvalidOperationException(nameof(Part))
                };

                return $"ROUND(EXTRACT(EPOCH FROM ({end}::timestamp - {start}::timestamp)){expr})";

            case nameof(ServerType.MySql):
                return $"TIMESTAMPDIFF({Part.GetName().ToUpperInvariant()}, {start}, {end})";

            default:
                return $"DATEDIFF({Part.GetName().ToUpperInvariant()}, {start}, {end})";
        };
    }

    /// <summary>
    /// Date part
    /// </summary>
    public DateParts Part { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public object Start { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public object End { get; }
}