namespace Serenity.Data;

/// <summary>
/// Interfaces for types that has an ActualTransaction property of type IDbTransaction
/// </summary>
public interface IHasActualTransaction
{
    /// <summary>
    /// Gets the actual transaction
    /// </summary>
    IDbTransaction ActualTransaction { get; }
}