namespace Serenity.Tests.Data;

public class UnitOfWorkTests
{

    [Fact]
    public void DefaultCtor_Requires_ConnectionArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(connection: null));
    }

    [Fact]
    public void DefaultCtor_AutoOpens_And_StartsTransaction()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection); 
    }

    [Fact]
    public void DefaultCtor_StartsTransactionOnly_IfAlreadyOpen()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
    }

    [Fact]
    public void DeferStartCtor_Requires_ConnectionArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(connection: null, deferStart: false));
    }

    [Fact]
    public void DeferStartCtor_AutoOpens_And_StartsTransaction_ForDeferStartFalse()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, deferStart: false);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
    }

    [Fact]
    public void DeferStartCtor_StartsTransactionOnly_IfAlreadyOpen_ForDeferStartFalse()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, deferStart: false);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
    }

    [Fact]
    public void DeferStartCtor_DoesNotAutoOpen_IfDeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Null(connection.Transaction);
    }

    [Fact]
    public void DeferStartCtor_DoesNotAutoOpen_IfDeferStartTrue_WithStateChange()
    {
        using var connection = new UnitOfWorkTestConnectionWithStateChange();
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Null(connection.Transaction);
    }

    [Fact]
    public void DeferStartCtor_StartsTransaction_IfAlreadyOpen_And_DeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
    }

    [Fact]
    public void DeferStartCtor_StartsTransaction_IfAlreadyOpen_And_DeferStartTrue_WithStateChangeEvent()
    {
        using var connection = new UnitOfWorkTestConnectionWithStateChange();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
    }

    [Fact]
    public void DeferStartCtor_AutoOpens_And_StartsTransaction_WhenConnectionRead_IfDeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void DeferStartCtor_DoesNot_AutoOpen_IfDeferStartTrue_WithStateChangeEvent()
    {
        using var connection = new UnitOfWorkTestConnectionWithStateChange();
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Null(connection.Transaction);
        connection.stateChange(connection, new(ConnectionState.Closed, ConnectionState.Connecting));
        Assert.NotNull(connection.stateChange);
        connection.Open();
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.Transaction);
        connection.stateChange(connection, new(ConnectionState.Closed, ConnectionState.Open));
        Assert.Null(connection.stateChange);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
        Assert.Null(connection.stateChange);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void DeferStartCtor_StartsTransactionOnly_WhenConnectionRead_IfAlreadyOpen_AndDeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
    }

    [Fact]
    public void DeferStartCtor_StartsTransactionOnly_WhenConnectionRead_IfAlreadyOpen_AndDeferStartTrue_WithStateChange()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
    }

    [Fact]
    public void IsolationLevelCtor_Requires_ConnectionArgument_WithIsolationLevelConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitOfWork(connection: null, IsolationLevel.ReadCommitted));
    }

    [InlineData(IsolationLevel.ReadCommitted)]
    [InlineData(IsolationLevel.Serializable)]
    [InlineData(IsolationLevel.RepeatableRead)]
    [Theory]
    public void IsolationLevelCtor_Uses_TheIsolationLevel_Argument(IsolationLevel isolationLevel)
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, isolationLevel);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Equal(isolationLevel, connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    [Fact]
    public void IsolationLevelCtor_Ignores_IsolationLevel_Unspecified()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, IsolationLevel.Unspecified);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    [Fact]
    public void IsolationLevelCtor_AutoOpens_And_StartsTransaction_ForDeferStartFalse()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, IsolationLevel.ReadCommitted, deferStart: false);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Equal(IsolationLevel.ReadCommitted, connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    [Fact]
    public void IsolationLevelCtor_StartsTransactionOnly_IfAlreadyOpen_ForDeferStartFalse()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, IsolationLevel.ReadCommitted, deferStart: false);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Equal(IsolationLevel.ReadCommitted, connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    [Fact]
    public void IsolationLevelCtor_DoesNotAutoOpen_IfDeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, IsolationLevel.ReadCommitted, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Null(connection.Transaction);
    }

    [Fact]
    public void IsolationLevelCtor_StartsTransaction_IfAlreadyOpen_And_DeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        connection.State = ConnectionState.Open;
        using var uow = new UnitOfWork(connection, IsolationLevel.ReadCommitted, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Equal(IsolationLevel.ReadCommitted, connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
    }

    [Fact]
    public void IsolationLevelCtor_AutoOpens_And_StartsTransaction_WhenConnectionRead_IfDeferStartTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        using var uow = new UnitOfWork(connection, IsolationLevel.ReadCommitted, deferStart: true);
        Assert.Equal(0, connection.OpenCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
        Assert.Equal(IsolationLevel.ReadCommitted, connection.BeginTransactionLevel);
        Assert.NotNull(connection.Transaction);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void WhenDeferIsTrue_DoesNot_TryToOpenConnection_After_DisposeIsCalled()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection, deferStart: true);
        try
        {
            Assert.Equal(0, connection.OpenCalls);
        }
        finally
        {
            uow.Dispose();
        }
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Null(connection.Transaction);
        Assert.NotNull(uow.Connection);
        Assert.Equal(0, connection.OpenCalls);
    }

    [Fact]
    public void WhenDeferIsFalse_DoesNot_TryToOpenConnection_After_DisposeIsCalled()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection, deferStart: false);
        try
        {
            Assert.Equal(1, connection.OpenCalls);
            Assert.NotNull(connection.Transaction);
            Assert.Null(connection.BeginTransactionLevel);
        }
        finally
        {
            uow.Dispose();
        }
        connection.State = ConnectionState.Closed;
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.NotNull(uow.Connection);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void DoesNot_TryToDispose_Transaction_Twice()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        try
        {
            Assert.Equal(1, connection.OpenCalls);
            Assert.NotNull(connection.Transaction);
            Assert.Null(connection.BeginTransactionLevel);
        }
        finally
        {
            uow.Dispose();
        }
        Assert.Equal(1, connection.Transaction.DisposeCalls);
        uow.Dispose();
        Assert.Equal(1, connection.Transaction.DisposeCalls);
    }

    [Fact]
    public void DoesNot_Call_OnCommit_When_Disposed_WithoutCommit()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int onCommitCalls = 0;
        try
        {
            uow.OnCommit += () => onCommitCalls++;
            Assert.Equal(1, connection.OpenCalls);
            Assert.NotNull(connection.Transaction);
            Assert.Null(connection.BeginTransactionLevel);
        }
        finally
        {
            uow.Dispose();
        }
        Assert.Equal(0, onCommitCalls);
        uow.Dispose();
        Assert.Equal(0, onCommitCalls);
    }

    [Fact]
    public void DoesNot_Call_OnCommit_When_Commit_NotCalled()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int onCommitCalls = 0;
        uow.OnCommit += () => onCommitCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        uow = null;
        GC.Collect();
        Assert.Equal(0, onCommitCalls);
    }

    [Fact]
    public void DoesNotCall_Rollback_When_Commit_NotCalled_And_GarbageCollected_WithoutDispose()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int rollbackCalls = 0;
        uow.OnRollback += () => rollbackCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        uow = null;
        GC.Collect();
        Assert.Equal(0, rollbackCalls);
    }

    [Fact]
    public void Calls_Rollback_When_Disposed_But_NotAgain_When_GarbageCollected()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int rollbackCalls = 0;
        uow.OnRollback += () => rollbackCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        uow.Dispose();
        Assert.Equal(1, rollbackCalls);
        uow = null;
        GC.Collect();
        Assert.Equal(1, rollbackCalls);
    }

    [Fact]
    public void Dispose_Calls_Rollback_Even_When_Transaction_Dispose_Raises_An_Error()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int rollbackCalls = 0;
        uow.OnRollback += () => rollbackCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        connection.Transaction.ThrowOnDispose = true;
        connection.Transaction.ThrowOnRollback = true;
        Assert.Throws<NotImplementedException>(() => uow.Dispose());
        Assert.Equal(1, rollbackCalls);
        Assert.Equal(1, connection.Transaction.DisposeCalls);
        Assert.Equal(0, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        uow.Dispose();
        Assert.Equal(1, rollbackCalls);
    }

    [Fact]
    public void Commit_Raises_Error_If_Already_Committed()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        uow.Commit();
        Assert.Equal(1, connection.Transaction?.CommitCalls);
        Assert.Throws<InvalidOperationException>(() => uow.Commit());
        Assert.Equal(1, connection.Transaction?.CommitCalls);
    }

    [Fact]
    public void Commit_DoesNot_Raise_Error_If_DeferStartTrue_AndTransactionNotStarted()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection, deferStart: true);
        uow.Commit();
        Assert.Null(connection.Transaction);
    }

    [Fact]
    public void Commit_Calls_OnCommit_EvenIf_TransactionNotStarted_WhenDeferStartIsTrue()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection, deferStart: true);
        var commitCalls = 0;
        var rollbackCalls = 0;
        uow.OnCommit += () => commitCalls++;
        uow.OnRollback += () => rollbackCalls++;
        uow.Commit();
        Assert.Null(connection.Transaction);
        Assert.Equal(1, commitCalls);
        Assert.Equal(0, rollbackCalls);
        uow.Dispose();
        Assert.Equal(1, commitCalls);
        Assert.Equal(0, rollbackCalls);
    }

    [Fact]
    public void Commit_DoesNotCall_OnCommit_When_Transaction_Commit_Raises_An_Error()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int commitCalls = 0;
        int rollbackCalls = 0;
        uow.OnCommit += () => commitCalls++;
        uow.OnRollback += () => rollbackCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        connection.Transaction.ThrowOnCommit = true;
        Assert.Throws<NotImplementedException>(() => uow.Commit());
        Assert.Equal(0, commitCalls);
        Assert.Equal(0, rollbackCalls);
        Assert.Equal(1, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.DisposeCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        uow.Dispose();
        Assert.Equal(0, commitCalls);
        Assert.Equal(1, rollbackCalls);
        Assert.Equal(1, connection.Transaction.CommitCalls);
        Assert.Equal(1, connection.Transaction.DisposeCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    [Fact]
    public void Commit_DoesNotCall_OnRollback_When_OnCommitCallback_Throws_An_Error()
    {
        using var connection = new UnitOfWorkTestConnection();
        var uow = new UnitOfWork(connection);
        int rollbackCalls = 0;
        uow.OnCommit += () => throw new NotImplementedException();
        uow.OnRollback += () => rollbackCalls++;
        Assert.Equal(1, connection.OpenCalls);
        Assert.NotNull(connection.Transaction);
        Assert.Null(connection.BeginTransactionLevel);
        Assert.Throws<NotImplementedException>(() => uow.Commit());
        Assert.Equal(0, rollbackCalls);
        Assert.Equal(1, connection.Transaction.CommitCalls);
        Assert.Equal(0, connection.Transaction.DisposeCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
        uow.Dispose();
        Assert.Equal(0, rollbackCalls);
        Assert.Equal(1, connection.Transaction.CommitCalls);
        Assert.Equal(1, connection.Transaction.DisposeCalls);
        Assert.Equal(0, connection.Transaction.RollbackCalls);
    }

    private class UnitOfWorkTestConnection : IDbConnection
    {
        public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ConnectionTimeout => throw new NotImplementedException();
        public string Database => throw new NotImplementedException();
        public ConnectionState State { get; set; } = ConnectionState.Closed;

        public IDbTransaction BeginTransaction()
        {
            if (State != ConnectionState.Open)
                throw new InvalidOperationException("BeginTransaction called for a closed connection!");

            if (Transaction != null)
                throw new InvalidOperationException("BeginTransaction called multiple times!");

            Transaction ??= new UnitOfWorkTestTransaction();
            return Transaction;
        }

        public IsolationLevel? BeginTransactionLevel { get; private set; }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (State != ConnectionState.Open)
                throw new InvalidOperationException("BeginTransaction called for a closed connection!");

            if (Transaction != null)
                throw new InvalidOperationException("BeginTransaction called multiple times!");

            BeginTransactionLevel = il;
            Transaction ??= new UnitOfWorkTestTransaction();
            return Transaction;
        }

        public UnitOfWorkTestTransaction Transaction { get; set; }

        public void Open()
        {
            if (State == ConnectionState.Open)
                throw new InvalidOperationException("Open called for an open connection!");

            State = ConnectionState.Open;
            OpenCalls++;
        }

        public int OpenCalls { get; set; }

        public void ChangeDatabase(string databaseName) => throw new NotImplementedException();
        public void Close() => throw new NotImplementedException();
        public void Dispose() {}
        public IDbCommand CreateCommand() => throw new NotImplementedException();
        public void Commit() => throw new NotImplementedException();
    }

    private class UnitOfWorkTestConnectionWithStateChange : UnitOfWorkTestConnection, IHasConnectionStateChange
    {
        public StateChangeEventHandler stateChange;

        public event StateChangeEventHandler StateChange
        {
            add { stateChange += value; }
            remove { stateChange -= value; }
        }
    }

    private class UnitOfWorkTestTransaction : IDbTransaction, IDisposable
    {
        public IDbConnection Connection => throw new NotImplementedException();
        public IsolationLevel IsolationLevel => throw new NotImplementedException();

        public void Commit()
        {
            CommitCalls++;
            if (ThrowOnCommit)
                throw new NotImplementedException();
        }

        public int CommitCalls { get; set; }
        public bool ThrowOnCommit { get; set; }

        public void Dispose()
        {
            DisposeCalls++;
            if (ThrowOnDispose)
                throw new NotImplementedException();
        }

        public int DisposeCalls { get; set; }
        public bool ThrowOnDispose { get; set; }

        public int RollbackCalls { get; set; }

        public bool ThrowOnRollback { get; set; }

        public void Rollback()
        {
            RollbackCalls++;
            if (ThrowOnRollback) 
                throw new NotImplementedException();
        }
    }
}