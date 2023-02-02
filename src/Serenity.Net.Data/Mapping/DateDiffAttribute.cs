namespace Serenity.Data.Mapping;

/// <summary>
/// DatePart expression attribute
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
    public DateDiffAttribute(DateParts part, string start, string end)
    {
        Part = part;
        Start = start ?? throw new ArgumentNullException(nameof(start));
        End = end ?? throw new ArgumentNullException(nameof(end));
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        string expr;

        if ((Part == DateParts.Year || Part == DateParts.Month) && (
             dialect.ServerType == nameof(ServerType.Sqlite) ||
             dialect.ServerType == nameof(ServerType.Postgres) ||
             dialect.ServerType == nameof(ServerType.Oracle)))
        {
            expr = $"({new DatePartAttribute(DateParts.Year, End).ToString(dialect)} - " +
                new DatePartAttribute(DateParts.Year, Start).ToString(dialect) + ")";

            if (Part == DateParts.Year)
                return expr;

            return $"({expr} * 12 + " +
                new DatePartAttribute(DateParts.Month, End).ToString(dialect) + " - " +
                new DatePartAttribute(DateParts.Month, Start).ToString(dialect) + ")";
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
                return $"ROUND((JULIANDAY({End}) - JULIANDAY({Start})){multiplier()})";

            case nameof(ServerType.Oracle):
                return $"ROUND((CAST ({End} as DATE) - CAST ({Start} as DATE)){multiplier()})";

            case nameof(ServerType.Postgres):
                expr = Part switch
                {
                    DateParts.Day => " / 86400",
                    DateParts.Hour => " / 3600",
                    DateParts.Minute => " / 60",
                    DateParts.Second => "",
                    _ => throw new InvalidOperationException(nameof(Part))
                };

                return $"ROUND(EXTRACT(EPOCH FROM ({End}::timestamp - {Start}::timestamp)){expr})";

            case nameof(ServerType.MySql):
                return $"TIMESTAMPDIFF({Part.GetName().ToUpperInvariant()}, {Start}, {End})";

            default:
                return $"DATEDIFF({Part.GetName().ToUpperInvariant()}, {Start}, {End})";
        };
    }

    /// <summary>
    /// Date part
    /// </summary>
    public DateParts Part { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public string Start { get; }

    /// <summary>
    /// Date expression 1
    /// </summary>
    public string End { get; }
}