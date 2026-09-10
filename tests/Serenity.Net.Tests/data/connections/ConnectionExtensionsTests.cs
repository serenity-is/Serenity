namespace Serenity.Data;

public class ConnectionExtensionsTests
{
    [Fact]
    public void EnsureOpen_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).EnsureOpen());
    }

    [Fact]
    public void EnsureOpen_ClosedConnection_OpensIt()
    {
        using var connection = new MockDbConnection();

        Assert.Same(connection, connection.EnsureOpen());
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void EnsureOpen_AlreadyOpen_DoesNotOpenAgain()
    {
        using var connection = new MockDbConnection();
        connection.EnsureOpen();
        connection.EnsureOpen();

        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void EnsureOpen_OpenedOnce_WrappedConnection_ThrowsInvalidOperationException()
    {
        var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);
        wrapped.Open();
        wrapped.Close();

        Assert.Throws<InvalidOperationException>(() => wrapped.EnsureOpen());
        connection.Dispose();
    }

    [Fact]
    public async Task EnsureOpenAsync_OpenAsync_UsesOpenAsync()
    {
        using var connection = new MockDbConnection();

        var result = await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);

        Assert.Same(connection, (object?)result ?? connection);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task EnsureOpenAsync_AlreadyOpen_DoesNotOpen()
    {
        using var connection = new MockDbConnection();
        await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void GetCurrentActualTransaction_ReturnsActualTransaction()
    {
        var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        Assert.Null(wrapped.GetCurrentActualTransaction());

        var transaction = wrapped.BeginTransaction();
        Assert.Same(((IHasActualTransaction)transaction).ActualTransaction,
            wrapped.GetCurrentActualTransaction());

        transaction.Dispose();
        connection.Dispose();
    }

    [Fact]
    public void GetCurrentActualTransaction_ForPlainConnection_ReturnsNull()
    {
        using var connection = new MockDbConnection();

        Assert.Null(connection.GetCurrentActualTransaction());
    }

    [Fact]
    public void SetCommandTimeout_ForWrappedConnection_SetsValue()
    {
        var connection = new MockDbConnection();
        using var wrapped = new WrappedConnection(connection, SqlServer2012Dialect.Instance);

        wrapped.SetCommandTimeout(20);
        Assert.Equal(20, wrapped.CommandTimeout);

        connection.Dispose();
    }

    [Fact]
    public void SetCommandTimeout_ForPlainConnection_ThrowsArgumentOutOfRange()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentOutOfRangeException>(() => connection.SetCommandTimeout(15));
    }

    [Fact]
    public void GetDialect_UsesMockDialectIfAssigned()
    {
        var connection = new MockDbConnection { Dialect = OracleDialect.Instance };

        Assert.Equal(OracleDialect.Instance, connection.GetDialect());
    }

    [Fact]
    public void GetDialect_NoDialectAssigned_FallsBackToDefault()
    {
        using var connection = new MockDbConnection();

        Assert.Equal(SqlSettings.DefaultDialect, connection.GetDialect());
    }

    [Fact]
    public void GetDialect_ConnectionWithoutIHasDialect_FallsBackToDefault()
    {
        var connection = new NoDialectConnection();

        Assert.Equal(SqlSettings.DefaultDialect, connection.GetDialect());
    }

    [Fact]
    public void GetLogger_ReturnsLoggerIfAvailable()
    {
        var connection = new MockDbConnection();

        Assert.Null(connection.GetLogger());
    }

    private class NoDialectConnection : IDbConnection
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int ConnectionTimeout => 0;
        public string Database => string.Empty;
        public ConnectionState State => ConnectionState.Closed;
        public IDbTransaction BeginTransaction() => throw new NotImplementedException();
        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();
        public void ChangeDatabase(string databaseName) { }
        public IDbCommand CreateCommand() => throw new NotImplementedException();
        public void Open() { }
        public void Close() { }
        public void Dispose() { }
    }
}
