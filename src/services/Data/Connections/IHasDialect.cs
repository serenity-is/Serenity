namespace Serenity.Data;

/// <summary>
/// Interface for types that have a <see cref="Dialect"/> property of type <see cref="ISqlDialect"/>.
/// </summary>
public interface IHasDialect
{
    /// <summary>
    /// Gets the SQL dialect.
    /// </summary>
    ISqlDialect Dialect { get; }
}