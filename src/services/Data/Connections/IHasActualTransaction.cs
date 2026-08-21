namespace Serenity.Data;

/// <summary>
/// Interface for types that have an <see cref="ActualTransaction"/> property of type <see cref="IDbTransaction"/>.
/// </summary>
public interface IHasActualTransaction
{
    /// <summary>
    /// Gets the actual transaction.
    /// </summary>
    IDbTransaction ActualTransaction { get; }
}