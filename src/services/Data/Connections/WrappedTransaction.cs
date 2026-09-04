using System.Data.Common;

namespace Serenity.Data;

/// <summary>
/// Wraps a transaction instance to add current transaction support for the wrapped connection.
/// </summary>
/// <seealso cref="DbTransaction" />
public class WrappedTransaction : DbTransaction, IHasActualTransaction
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
    ///   Returns the actual transaction.</summary>
    public IDbTransaction ActualTransaction => actualTransaction;

    /// <summary>
    ///   Returns the transaction isolation level</summary>
    public override IsolationLevel IsolationLevel => actualTransaction.IsolationLevel;

    /// <summary>
    ///   Returns the connection associated with this transaction.</summary>
    protected override DbConnection DbConnection => wrappedConnection;

    /// <summary>
    ///   Commits actual transaction and sets wrapped transaction for related connection to null.</summary>
    public override void Commit()
    {
        actualTransaction.Commit();
        DetachConnection();
    }

    /// <summary>
    ///   Commits actual transaction asynchronously and sets wrapped transaction for related connection to null.</summary>
    public override async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (actualTransaction is DbTransaction dbTransaction)
            await dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        else
            actualTransaction.Commit();
        DetachConnection();
    }

    /// <summary>
    ///   Rollbacks actual transaction and sets wrapped transaction for related connection to null.</summary>
    public override void Rollback()
    {
        actualTransaction.Rollback();
        DetachConnection();
    }

    /// <summary>
    ///   Rollbacks actual transaction asynchronously and sets wrapped transaction for related connection to null.</summary>
    public override async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (actualTransaction is DbTransaction dbTransaction)
            await dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        else
            actualTransaction.Rollback();
        DetachConnection();
    }

    /// <summary>
    ///   Disposes actual transaction and sets wrapped transaction for related connection to null.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            actualTransaction.Dispose();
            DetachConnection();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    ///   Disposes actual transaction asynchronously and sets wrapped transaction for related connection to null.</summary>
    public override async ValueTask DisposeAsync()
    {
        if (actualTransaction is DbTransaction dbTransaction)
            await dbTransaction.DisposeAsync().ConfigureAwait(false);
        else
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