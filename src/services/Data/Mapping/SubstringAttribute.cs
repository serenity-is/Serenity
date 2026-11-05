namespace Serenity.Data.Mapping;

/// <summary>
/// Substring expression attribute, which returns a part of a string like SUBSTRING of SQL Server.
/// </summary>
/// <param name="expression">An expression that returns a string.</param>
/// <param name="start">An expression that returns the position of the first character, starting from 1.</param>
/// <param name="length">An expression that returns the number of characters.</param>
/// <exception cref="ArgumentNullException">One of the expressions is null.</exception>
public class SubstringAttribute(object expression, object start, object length) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        var expression = ToString(Expression, dialect);
        var start = ToString(Start, dialect);
        var length = ToString(Length, dialect);

        return dialect.ServerType switch
        {
            // Firebird can't determine the types of parameters in SUBSTRING
            nameof(ServerType.Firebird) => $"SUBSTRING({CastParameterForFirebird(expression)} " +
                $"FROM {CastIntegerParameter(start)} FOR {CastIntegerParameter(length)})",
            nameof(ServerType.Oracle) or nameof(ServerType.Sqlite) => $"SUBSTR({expression}, {start}, {length})",
            _ => $"SUBSTRING({expression}, {start}, {length})"
        };
    }

    private static string CastIntegerParameter(string expression) =>
        IsParameter(expression) ? "CAST(" + expression + " AS INTEGER)" : expression;

    /// <summary>
    /// String expression.
    /// </summary>
    public object Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>
    /// Start position expression.
    /// </summary>
    public object Start { get; } = start ?? throw new ArgumentNullException(nameof(start));

    /// <summary>
    /// Length expression.
    /// </summary>
    public object Length { get; } = length ?? throw new ArgumentNullException(nameof(length));
}
