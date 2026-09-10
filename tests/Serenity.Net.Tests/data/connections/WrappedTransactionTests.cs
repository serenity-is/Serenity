namespace Serenity.Data;

public class WrappedTransactionTests
{
    private static (WrappedConnection, MockDbConnection) GetWrapped()
    {
        var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance };
        var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);
        return (wrapped, connection);
    }

    [Fact]
    public void Commit_DetachesTransaction()
    {
        var (wrapped, _) = GetWrapped();
        var transaction = wrapped.BeginTransaction();

        Assert.Same(transaction, wrapped.CurrentTransaction);
        transaction.Commit();
        Assert.Null(wrapped.CurrentTransaction);
    }

    [Fact]
    public void Rollback_DetachesTransaction()
    {
        var (wrapped, _) = GetWrapped();

        var transaction = wrapped.BeginTransaction();
        transaction.Rollback();

        Assert.Null(wrapped.CurrentTransaction);
    }

    [Fact]
    public void IsolationLevel_DelegatesToActual()
    {
        var (wrapped, connection) = GetWrapped();

        var transaction = (WrappedTransaction)wrapped.BeginTransaction(IsolationLevel.ReadCommitted);

        Assert.Equal(transaction.ActualTransaction.IsolationLevel, transaction.IsolationLevel);
    }

    [Fact]
    public void Dispose_DetachesActualTransaction()
    {
        var (wrapped, connection) = GetWrapped();

        var transaction = (WrappedTransaction)wrapped.BeginTransaction();
        var actualTransaction = transaction.ActualTransaction;

        transaction.Dispose();

        Assert.Null(wrapped.GetCurrentActualTransaction());
        connection.Dispose();
        wrapped.Dispose();
    }

    [Fact]
    public void DisposeAsync_DetachesActualTransaction()
    {
        var (wrapped, connection) = GetWrapped();

        var transaction = wrapped.BeginTransaction();
        transaction.DisposeAsync();

        Assert.Null(wrapped.GetCurrentActualTransaction());
        connection.Dispose();
        wrapped.Dispose();
    }

    [Fact]
    public void CommitAsync_OnDbTransactionDetector()
    {
        var (wrapped, connection) = GetWrapped();

        var transaction = wrapped.BeginTransaction();
        transaction.CommitAsync();

        Assert.Null(wrapped.CurrentTransaction);
        connection.Dispose();
        wrapped.Dispose();
    }

    [Fact]
    public void RollbackAsync_Works()
    {
        var (wrapped, connection) = GetWrapped();

        var transaction = wrapped.BeginTransaction();
        transaction.RollbackAsync();

        Assert.Null(wrapped.CurrentTransaction);
        connection.Dispose();
        wrapped.Dispose();
    }
}
