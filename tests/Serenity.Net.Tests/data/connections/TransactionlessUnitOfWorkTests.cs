namespace Serenity.Data;

public class TransactionlessUnitOfWorkTests
{
    private readonly MockDbConnection connection = new();

    [Fact]
    public void Connection_ThrowsOnNull() =>
        Assert.Throws<ArgumentNullException>(() => new TransactionlessUnitOfWork(null));

    [Fact]
    public void Commit_RaisesOnCommitEvent()
    {
        int commitCalled = 0;
        using var uow = new TransactionlessUnitOfWork(connection);
        uow.OnCommit += () => commitCalled++;

        uow.Commit();
        Assert.Equal(1, commitCalled);
    }

    [Fact]
    public void DoubleCommit_ThrowsInvalidOperationException()
    {
        using var uow = new TransactionlessUnitOfWork(connection);
        uow.Commit();

        Assert.Throws<InvalidOperationException>(uow.Commit);
    }

    [Fact]
    public void Dispose_RaisesOnRollbackEvent()
    {
        int rollbackCalled = 0;
        var uow = new TransactionlessUnitOfWork(connection);
        uow.OnRollback += () => rollbackCalled++;

        uow.Dispose();
        Assert.Equal(1, rollbackCalled);
    }

    [Fact]
    public void Dispose_AfterCommit_ClearedOnCommitEvent()
    {
        int commitCalled = 0;
        var uow = new TransactionlessUnitOfWork(connection);
        uow.OnCommit += () => commitCalled++;
        uow.Commit();
        // second commit event must not fire double
        Assert.Throws<InvalidOperationException>(uow.Commit);
        Assert.Equal(1, commitCalled);
    }

    [Fact]
    public void ExposesConnection()
    {
        using var uow = new TransactionlessUnitOfWork(connection);

        Assert.Same(connection, uow.Connection);
    }
}
