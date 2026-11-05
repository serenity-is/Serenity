namespace Serenity.Data.Mapping;

/// <summary>
/// Trim expression attribute, which removes the spaces at the start, at the end, or both, of a string, like TRIM,
/// LTRIM and RTRIM of SQL Server.
/// </summary>
/// <param name="expression">An expression that returns a string.</param>
/// <exception cref="ArgumentNullException">The expression is null.</exception>
public class TrimAttribute(object expression) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        var expression = ToString(Expression, dialect);
        if (!Leading && !Trailing)
            return expression;

        if (dialect.ServerType == nameof(ServerType.Firebird))
        {
            // Firebird has no LTRIM / RTRIM
            var side = Leading && Trailing ? "" : Leading ? "LEADING FROM " : "TRAILING FROM ";
            return $"TRIM({side}{CastParameterForFirebird(expression)})";
        }

        if (Leading && Trailing)
        {
            // TRIM is only available since SQL Server 2017
            return dialect.ServerType == nameof(ServerType.SqlServer) ?
                $"LTRIM(RTRIM({expression}))" : $"TRIM({expression})";
        }

        return (Leading ? "LTRIM(" : "RTRIM(") + expression + ")";
    }

    /// <summary>
    /// String expression.
    /// </summary>
    public object Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>
    /// Whether to remove the spaces at the start, true by default.
    /// </summary>
    public bool Leading { get; set; } = true;

    /// <summary>
    /// Whether to remove the spaces at the end, true by default.
    /// </summary>
    public bool Trailing { get; set; } = true;
}
