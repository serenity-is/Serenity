namespace Serenity.Data.Mapping;

/// <summary>
/// DatePart expression attribute
/// </summary>
public class DatePartAttribute : BaseExpressionAttribute
{
    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="expression">An expression that returns a date value</param>
    /// <param name="part">Datepart like "year", "month" etc.</param>
    /// <exception cref="ArgumentNullException">One of expressions is null</exception>
    public DatePartAttribute(DateParts part, object expression)
    {
        Part = part;
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    private static string ToSqliteSpecifier(DateParts datePart)
    {
        return datePart switch
        {
            DateParts.Year => "%Y",
            DateParts.Month => "%m",
            DateParts.Day => "%d",
            DateParts.Hour => "%H",
            DateParts.Minute => "%M",
            DateParts.Second => "%S",
            _ => throw new ArgumentOutOfRangeException(nameof(datePart))
        };
    }

    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        string datePart = Enum.GetName(typeof(DateParts), Part).ToUpperInvariant();
        string expression = ToString(Expression, dialect);
        return dialect.ServerType switch
        {
            nameof(ServerType.MySql) or
            nameof(ServerType.Oracle) or
            nameof(ServerType.Postgres) or
            nameof(ServerType.Firebird) =>
                "EXTRACT(" + datePart + " FROM " + expression + ")",

            nameof(ServerType.Sqlite) =>
                "CAST(STRFTIME('" + ToSqliteSpecifier(Part) + "', " +
                    expression + ") AS INTEGER)",

            _ => "DATEPART(" + datePart + ", " + expression + ")"
        };
    }

    /// <summary>
    /// Date part
    /// </summary>
    public DateParts Part { get; }

    /// <summary>
    /// Date expression
    /// </summary>
    public object Expression { get; }
}