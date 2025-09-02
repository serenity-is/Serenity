namespace Serenity.Data;

/// <summary>
/// Interfaces for types that has a Dialect property of type ISqlDialect
/// </summary>
public interface IHasDialect
{
    /// <summary>
    /// Gets the sql dialect
    /// </summary>
    ISqlDialect Dialect { get; }
}