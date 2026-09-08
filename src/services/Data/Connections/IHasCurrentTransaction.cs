namespace Serenity.Data;

/// <summary>
/// Interface for types that have a <see cref="CurrentTransaction"/> property of type <see cref="IDbTransaction"/>.
/// </summary>
public interface IHasCurrentTransaction
{
    /// <summary>
    /// Gets the current transaction, if any.
    /// </summary>
    IDbTransaction? CurrentTransaction { get; }
}