namespace Serenity.Data;

/// <summary>
/// An interface to implement unit of work pattern, e.g. a transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>
    /// The connection.
    /// </value>
    IDbConnection Connection { get; }

    /// <summary>
    /// Occurs when transaction is committed.
    /// </summary>
    event Action OnCommit;

    /// <summary>
    /// Occurs when transaction is rolled back.
    /// </summary>
    event Action OnRollback;
}