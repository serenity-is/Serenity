namespace Serenity.Data.Mapping;

/// <summary>
/// DateAdd expression attribute, which adds a number of date parts to a date like DATEADD of SQL Server. Adding months
/// or years to the end of a month gives the last day of the target month when it is shorter, e.g. January 31 plus one
/// month is February 28.
/// </summary>
/// <param name="part">The date part to add, like year or month.</param>
/// <param name="number">An expression that returns the number of parts to add, which can be negative.</param>
/// <param name="expression">An expression that returns a date value.</param>
/// <exception cref="ArgumentNullException">One of the expressions is null.</exception>
public class DateAddAttribute(DateParts part, object number, object expression) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        var number = ToString(Number, dialect);
        var date = ToString(Expression, dialect);
        var partName = Part.GetName().ToUpperInvariant();

        switch (dialect.ServerType)
        {
            case nameof(ServerType.Postgres):
                return $"({date} + ({number}) * INTERVAL '1 {partName}')";

            case nameof(ServerType.MySql):
                return $"DATE_ADD({date}, INTERVAL ({number}) {partName})";

            case nameof(ServerType.Oracle):
                {
                    if (Part is not (DateParts.Year or DateParts.Month))
                        return $"(CAST({date} AS DATE) + NUMTODSINTERVAL({number}, '{partName}'))";

                    // ADD_MONTHS moves the last day of a month to the last day of the target month, e.g. February 28
                    // to March 31, so the months are added to the first day, then the day is clamped to the month
                    var months = Part == DateParts.Year ? $"({number}) * 12" : number;
                    var day = $"CAST({date} AS DATE)";
                    var firstDay = $"ADD_MONTHS(TRUNC({day}, 'MM'), {months})";
                    return $"({firstDay} + (LEAST(EXTRACT(DAY FROM {day}), EXTRACT(DAY FROM LAST_DAY({firstDay}))) - 1) + " +
                        $"({day} - TRUNC({day})))";
                }

            case nameof(ServerType.Sqlite):
                {
                    // floor (SQLite 3.46+) gives the last day of a shorter month instead of overflowing into the next
                    var floor = Part is DateParts.Year or DateParts.Month ? ", 'floor'" : "";
                    return $"DATETIME({date}, ({number}) || ' {partName.ToLowerInvariant()}s'{floor})";
                }

            default:
                return $"DATEADD({partName}, {number}, {date})";
        }
    }

    /// <summary>
    /// Date part.
    /// </summary>
    public DateParts Part { get; } = part;

    /// <summary>
    /// The number of parts to add.
    /// </summary>
    public object Number { get; } = number ?? throw new ArgumentNullException(nameof(number));

    /// <summary>
    /// Date expression.
    /// </summary>
    public object Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));
}
