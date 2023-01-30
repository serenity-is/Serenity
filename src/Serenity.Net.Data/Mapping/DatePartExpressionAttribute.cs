namespace Serenity.Data.Mapping;

/// <summary>
/// DatePart expression attribute
/// </summary>
public class DatePartExpressionAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="part">Datepart like "year", "month" etc.</param>
    /// <param name="expression">An expression that returns a date value</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public DatePartExpressionAttribute(string expression, DatePart part)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Part = part;
    }

    private static string ToSqliteSpecifier(DatePart datePart)
    {
        return datePart switch
        {
            DatePart.Year => "%Y",
            DatePart.Month => "%m",
            DatePart.Day => "%d",
            DatePart.Hour => "%H",
            DatePart.Minute => "%M",
            DatePart.Second => "%S",
            _ => throw new ArgumentOutOfRangeException(nameof(datePart))
        };
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect sqlDialect)
    {
        string datePart = Enum.GetName(typeof(DatePart), Part).ToUpperInvariant();
        return sqlDialect.ServerType switch
        {
            nameof(ServerType.MySql) or
            nameof(ServerType.Oracle) or
            nameof(ServerType.Firebird) =>
                "EXTRACT(" + datePart + " FROM " + Expression + ")",

            nameof(ServerType.Postgres) =>
                "DATEPART(" + Expression + ", " + datePart + ")",

            nameof(ServerType.Sqlite) =>
                "CAST(STRFTIME('" + ToSqliteSpecifier(Part) + "', " +
                    Expression + ") AS INTEGER)",

            _ => "DATEPART(" + datePart + ", " + Expression + ")"
        };
    }

    /// <summary>
    /// Date part
    /// </summary>
    public DatePart Part { get; }

    /// <summary>
    /// Date expression
    /// </summary>
    public string Expression { get; }
}