namespace Serenity.Data.Mapping;

/// <summary>
/// Replace expression attribute, which replaces all occurrences of a string in another one like REPLACE of SQL Server.
/// </summary>
/// <param name="expression">An expression that returns the string to search in.</param>
/// <param name="search">An expression that returns the string to search for.</param>
/// <param name="replacement">An expression that returns the string to replace it with.</param>
/// <exception cref="ArgumentNullException">One of the expressions is null.</exception>
public class ReplaceAttribute(object expression, object search, object replacement) : BaseExpressionAttribute
{
    /// <inheritdoc/>
    public override string Translate(ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        var arguments = new[] { Expression, Search, Replacement }.Select(x => ToString(x, dialect));
        if (dialect.ServerType == nameof(ServerType.Firebird))
            arguments = arguments.Select(CastParameterForFirebird);

        return "REPLACE(" + string.Join(", ", arguments) + ")";
    }

    /// <summary>
    /// String expression.
    /// </summary>
    public object Expression { get; } = expression ?? throw new ArgumentNullException(nameof(expression));

    /// <summary>
    /// Search expression.
    /// </summary>
    public object Search { get; } = search ?? throw new ArgumentNullException(nameof(search));

    /// <summary>
    /// Replacement expression.
    /// </summary>
    public object Replacement { get; } = replacement ?? throw new ArgumentNullException(nameof(replacement));
}
