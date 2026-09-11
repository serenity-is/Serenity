using System.Data.Common;

namespace Serenity.Data;

public class WrappedConnectionTests_More
{
    private class StubTransaction : IDbTransaction
    {
        public IDbConnection? Connection { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public void Commit() { }
        public void Rollback() { }
        public void Dispose() => GC.SuppressFinalize(this);
    }

    private class TestIdbConnection : IDbConnection, IHasConnectionStateChange
    {
        public string ConnectionString { get; set; } = "";
        public int ConnectionTimeout => 0;
        public string Database { get; set; } = "TestDb";
        public ConnectionState State { get; set; } = ConnectionState.Closed;
        public IsolationLevel? BeginLevel { get; private set; }
        public StubTransaction Transaction { get; } = new();
        public bool Disposed { get; private set; }
        public bool CloseCalled { get; private set; }
        public Func<IDbCommand>? CommandFactory { get; set; }

        public event StateChangeEventHandler? StateChange;
        public void RaiseStateChange(StateChangeEventArgs e) => StateChange?.Invoke(this, e);

        public IDbTransaction BeginTransaction()
        {
            BeginLevel = null;
            return Transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            BeginLevel = il;
            Transaction.IsolationLevel = il;
            return Transaction;
        }

        public void ChangeDatabase(string databaseName) => Database = databaseName;
        public void Close() { CloseCalled = true; State = ConnectionState.Closed; }
        public IDbCommand CreateCommand() => CommandFactory?.Invoke() ?? new MockDbCommand();
        public void Open() => State = ConnectionState.Open;
        public void Dispose() { Disposed = true; GC.SuppressFinalize(this); }
    }

    private class ExposedWrappedConnection(IDbConnection connection, ISqlDialect dialect)
        : WrappedConnection(connection, dialect)
    {
        public DbProviderFactory? Factory => DbProviderFactory;
    }

    private class NullTransactionCommand : MockDbCommand
    {
        protected override DbTransaction DbTransaction
        {
            get => null!;
            set { }
        }
    }

    private class NullTransactionConnection : MockDbConnection
    {
        protected override DbCommand CreateDbCommand() => new NullTransactionCommand();
    }

    private class PlainIdbCommand : IDbCommand
    {
        public string CommandText { get; set; } = "";
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDbConnection? Connection { get; set; }
        public IDataParameterCollection Parameters => throw new NotSupportedException();
        public IDbTransaction? Transaction { get; set; }
        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Cancel() { }
        public IDbDataParameter CreateParameter() => throw new NotSupportedException();
        public void Dispose() => GC.SuppressFinalize(this);
        public int ExecuteNonQuery() => 0;
        public IDataReader ExecuteReader() => throw new NotSupportedException();
        public IDataReader ExecuteReader(CommandBehavior behavior) => throw new NotSupportedException();
        public object? ExecuteScalar() => null;
        public void Prepare() { }
    }

    [Fact]
    public void Constructor_NonDbConnectionWithStateChange_RaisesStateChange()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        var raised = false;
        wrapped.StateChange += (s, e) => raised = true;
        connection.RaiseStateChange(new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));

        Assert.True(raised);
    }

    [Fact]
    public void BeginTransaction_NonDbConnection_RecordsIsolationLevel()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        var transaction = wrapped.BeginTransaction();
        Assert.IsType<WrappedTransaction>(transaction);
        Assert.Null(connection.BeginLevel);

        transaction.Dispose();
        wrapped.BeginTransaction(IsolationLevel.Serializable);
        Assert.Equal(IsolationLevel.Serializable, connection.BeginLevel);
    }

    [Fact]
    public async Task BeginTransactionAsync_NonDbConnection_RecordsIsolationLevel()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);
        var token = TestContext.Current.CancellationToken;

        await wrapped.BeginTransactionAsync(IsolationLevel.ReadCommitted, token);
        Assert.Equal(IsolationLevel.ReadCommitted, connection.BeginLevel);

        var transaction = await wrapped.BeginTransactionAsync(token);
        Assert.IsType<WrappedTransaction>(transaction);
    }

    [Fact]
    public async Task BeginTransactionAsync_DbConnection_UsesAsyncApi()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance };
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        var transaction = await wrapped.BeginTransactionAsync(
            TestContext.Current.CancellationToken);

        Assert.IsType<WrappedTransaction>(transaction);
    }

    [Fact]
    public void Release_DetachesOnlyMatchingTransaction()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        var current = (WrappedTransaction)wrapped.BeginTransaction();
        var other = new WrappedTransaction(wrapped, connection.BeginTransaction());

        wrapped.Release(other);
        Assert.Same(current, wrapped.CurrentTransaction);

        wrapped.Release(current);
        Assert.Null(wrapped.CurrentTransaction);
    }

    [Fact]
    public async Task CloseAsync_NonDbConnection_ClosesActualConnection()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.CloseAsync();
        Assert.True(connection.CloseCalled);
    }

    [Fact]
    public async Task CloseAsync_DbConnection_DelegatesToActualConnection()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.CloseAsync();
        Assert.Equal(ConnectionState.Closed, wrapped.State);
    }

    [Fact]
    public async Task OpenAsync_NonDbConnection_OpensActualConnection()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.OpenAsync(TestContext.Current.CancellationToken);
        Assert.True(wrapped.OpenedOnce);
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public async Task OpenAsync_DbConnection_OpensActualConnection()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.OpenAsync(TestContext.Current.CancellationToken);
        Assert.True(wrapped.OpenedOnce);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void DataSource_AndServerVersion_NonDbConnection_Throw()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        Assert.Throws<NotSupportedException>(() => wrapped.DataSource);
        Assert.Throws<NotSupportedException>(() => wrapped.ServerVersion);
    }

    [Fact]
    public void DbProviderFactory_NonDbConnection_ReturnsNull()
    {
        var connection = new TestIdbConnection();
        using var wrapped = new ExposedWrappedConnection(connection, SqlServer2012Dialect.Instance);

        Assert.Null(wrapped.Factory);
    }

    [Fact]
    public async Task DisposeAsync_NonDbConnection_DisposesActualConnection()
    {
        var connection = new TestIdbConnection();
        var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.DisposeAsync();
        Assert.True(connection.Disposed);
    }

    [Fact]
    public async Task DisposeAsync_DbConnection_DisposesActualConnection()
    {
        var connection = new MockDbConnection();
        var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        await wrapped.DisposeAsync();
        Assert.Equal(ConnectionState.Closed, connection.State);
    }

    [Fact]
    public void CreateCommand_TransactionWithNullConnection_Throws()
    {
        var connection = new TestIdbConnection { CommandFactory = () => new MockDbCommand() };
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.BeginTransaction();
        Assert.Throws<Exception>(() => ((IDbConnection)wrapped).CreateCommand());
    }

    [Fact]
    public void CreateCommand_DefaultCommandTimeout_Applied()
    {
        var old = SqlSettings.SetLocalCommandTimeout(7);
        try
        {
            using var connection = new MockDbConnection();
            using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

            var command = ((IDbConnection)wrapped).CreateCommand();
            Assert.Equal(7, command.CommandTimeout);
        }
        finally
        {
            SqlSettings.SetLocalCommandTimeout(old);
        }
    }

    [Fact]
    public void Dialect_Setter_Updates()
    {
        using var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.Dialect = MySqlDialect.Instance;

        Assert.Same(MySqlDialect.Instance, wrapped.Dialect);
    }

    [Fact]
    public void CreateDbCommand_NonDbCommand_Throws()
    {
        var connection = new TestIdbConnection { CommandFactory = () => new PlainIdbCommand() };
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        Assert.Throws<NotSupportedException>(() => wrapped.CreateCommand());
    }

    [Fact]
    public void CreateDbCommand_TransactionNotSetOnCommand_Throws()
    {
        using var connection = new NullTransactionConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.BeginTransaction();

        Assert.Throws<Exception>(() => wrapped.CreateCommand());
    }
}
