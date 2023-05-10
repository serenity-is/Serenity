namespace Serenity.Data;

/// <summary>
/// Wraps a transaction instance to add current transaction support for the wrapped connection.
/// </summary>
/// <seealso cref="IDbTransaction" />
public class WrappedTransaction : IDbTransaction, IHasActualTransaction
{
    private WrappedConnection wrappedConnection;
    private readonly IDbTransaction actualTransaction;

    /// <summary>
    ///   Creates a new WrappedTransaction instance.</summary>
    /// <param name="wrappedConnection">Wrapped connection</param>
    /// <param name="actualTransaction">The actual transaction, this wrapped transaction is created for.</param>
    internal WrappedTransaction(WrappedConnection wrappedConnection, IDbTransaction actualTransaction)
    {
        this.wrappedConnection = wrappedConnection;
        this.actualTransaction = actualTransaction;
    }

    /// <summary>
    ///   Returns the connection associated with this transaction.</summary>
    public IDbConnection Connection => wrappedConnection;

    /// <summary>
    ///   Returns the actual transaction.</summary>
    public IDbTransaction ActualTransaction => actualTransaction;

    /// <summary>
    ///   Returns the transaction isolation level</summary>
    public IsolationLevel IsolationLevel => actualTransaction.IsolationLevel;

    /// <summary>
    ///   Commits actual transaction and sets wrapped transaction for related connection to null.</summary>
    public void Commit()
    {
        actualTransaction.Commit();
        DetachConnection();
    }

    /// <summary>
    ///   Rollbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
    public void Rollback()
    {
        actualTransaction.Rollback();
        DetachConnection();
    }

    /// <summary>
    ///   Rolbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
    public void Dispose()
    {
        actualTransaction.Dispose();
        DetachConnection();
    }

    private void DetachConnection()
    {
        if (wrappedConnection != null)
        {
            wrappedConnection.Release(this);
            wrappedConnection = null;
        }
    }
}