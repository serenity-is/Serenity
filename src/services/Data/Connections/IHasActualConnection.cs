namespace Serenity.Data;

/// <summary>
/// Interface for types that have an <see cref="ActualConnection"/> property of type <see cref="IDbConnection"/>.
/// </summary>
public interface IHasActualConnection
{
    /// <summary>
    /// Gets the actual connection.
    /// </summary>
    IDbConnection ActualConnection { get; }
}