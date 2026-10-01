namespace Serenity.Data;

/// <summary>
/// An interface to implement the unit of work pattern, e.g. a transaction.
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
    /// Occurs when Commit completes without error. This signals operation
    /// success, not necessarily a database transaction commit: when no
    /// underlying transaction exists (e.g. a deferred unit of work whose
    /// connection was never opened, or implementations without a real
    /// transaction) the event still fires. Check <see cref="HasStartedTransaction"/>
    /// when it matters whether work actually ran transactionally.
    /// </summary>
    event Action OnCommit;

    /// <summary>
    /// Occurs when the unit of work is disposed without a prior Commit.
    /// Like <see cref="OnCommit"/>, this is tied to the operation outcome, not
    /// to a database transaction: it also fires when no underlying transaction
    /// was ever started.
    /// </summary>
    event Action OnRollback;

    /// <summary>
    /// Gets whether the unit of work's transaction was actually started.
    /// A normal unit of work starts its transaction right away, so the default
    /// implementation returns true. Implementations with deferred or no
    /// transaction override this with the real state: it stays false until the
    /// transaction is successfully started, e.g. for a deferred unit of work
    /// whose connection was never opened. Once true it stays true, including
    /// after commit, rollback and dispose, so it can reliably be read from
    /// OnCommit / OnRollback handlers and after the fact.
    /// </summary>
    bool HasStartedTransaction => true;
}