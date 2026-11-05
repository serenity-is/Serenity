namespace Serenity.Data.Mapping;

/// <summary>
/// DateDiff expression attribute, which returns the number of date part boundaries crossed between two dates like
/// DATEDIFF of SQL Server, e.g. one day from 23:00 to 01:00 of the next day, and one year from December 31 to
/// January 1, rather than the number of complete units between them.
/// </summary>
/// <remarks>
/// Creates a new instance
/// </remarks>
/// <param name="part">Datepart like "year", "month" etc.</param>
/// <param name="start">An expression that returns a date value</param>
/// <param name="end">An expression that returns a date value</param>
/// <exception cref="ArgumentNullException">One of expressions is null</exception>
public class DateDiffAttribute(DateParts part, object start, object end) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        var start = ToString(Start, dialect);
        var end = ToString(End, dialect);

        if ((Part == DateParts.Year || Part == DateParts.Month) && (
             dialect.ServerType == nameof(ServerType.Sqlite) ||
             dialect.ServerType == nameof(ServerType.Postgres) ||
             dialect.ServerType == nameof(ServerType.Oracle) ||
             dialect.ServerType == nameof(ServerType.MySql)))
        {
            var years = $"({new DatePartAttribute(DateParts.Year, end).ToString(dialect)} - " +
                new DatePartAttribute(DateParts.Year, start).ToString(dialect) + ")";

            if (Part == DateParts.Year)
                return years;

            return $"({years} * 12 + " +
                new DatePartAttribute(DateParts.Month, end).ToString(dialect) + " - " +
                new DatePartAttribute(DateParts.Month, start).ToString(dialect) + ")";
        }

        // the dates are truncated to the part, so the difference only counts the boundaries crossed
        switch (dialect.ServerType)
        {
            case nameof(ServerType.Sqlite):
                {
                    var format = Part switch
                    {
                        DateParts.Day => "%Y-%m-%d",
                        DateParts.Hour => "%Y-%m-%d %H:00:00",
                        DateParts.Minute => "%Y-%m-%d %H:%M:00",
                        DateParts.Second => "%Y-%m-%d %H:%M:%S",
                        _ => throw new InvalidOperationException(nameof(Part))
                    };

                    string seconds(string date) => $"CAST(STRFTIME('%s', STRFTIME('{format}', {date})) AS INTEGER)";
                    return $"(({seconds(end)} - {seconds(start)}) / {SecondsOf(Part)})";
                }

            case nameof(ServerType.Postgres):
                {
                    var unit = Part.GetName().ToLowerInvariant();
                    return $"ROUND(EXTRACT(EPOCH FROM (DATE_TRUNC('{unit}', {end}::timestamp) - " +
                        $"DATE_TRUNC('{unit}', {start}::timestamp))) / {SecondsOf(Part)})";
                }

            case nameof(ServerType.Oracle):
                {
                    return Part switch
                    {
                        DateParts.Day => $"(TRUNC(CAST({end} AS DATE)) - TRUNC(CAST({start} AS DATE)))",
                        DateParts.Hour => $"ROUND((TRUNC(CAST({end} AS DATE), 'HH') - TRUNC(CAST({start} AS DATE), 'HH')) * 24)",
                        DateParts.Minute => $"ROUND((TRUNC(CAST({end} AS DATE), 'MI') - TRUNC(CAST({start} AS DATE), 'MI')) * 1440)",
                        DateParts.Second => $"ROUND((CAST({end} AS DATE) - CAST({start} AS DATE)) * 86400)",
                        _ => throw new InvalidOperationException(nameof(Part))
                    };
                }

            case nameof(ServerType.MySql):
                {
                    if (Part == DateParts.Day)
                        return $"DATEDIFF({end}, {start})";

                    var format = Part switch
                    {
                        DateParts.Hour => "%Y-%m-%d %H:00:00",
                        DateParts.Minute => "%Y-%m-%d %H:%i:00",
                        DateParts.Second => "%Y-%m-%d %H:%i:%s",
                        _ => throw new InvalidOperationException(nameof(Part))
                    };

                    return $"TIMESTAMPDIFF({Part.GetName().ToUpperInvariant()}, DATE_FORMAT({start}, '{format}'), " +
                        $"DATE_FORMAT({end}, '{format}'))";
                }

            default:
                return $"DATEDIFF({Part.GetName().ToUpperInvariant()}, {start}, {end})";
        };
    }

    private static int SecondsOf(DateParts part) => part switch
    {
        DateParts.Day => 86400,
        DateParts.Hour => 3600,
        DateParts.Minute => 60,
        DateParts.Second => 1,
        _ => throw new InvalidOperationException(nameof(part))
    };

    /// <summary>
    /// Date part.
    /// </summary>
    public DateParts Part { get; } = part;

    /// <summary>
    /// Date expression 1.
    /// </summary>
    public object Start { get; } = start ?? throw new ArgumentNullException(nameof(start));

    /// <summary>
    /// Date expression 2.
    /// </summary>
    public object End { get; } = end ?? throw new ArgumentNullException(nameof(end));
}
