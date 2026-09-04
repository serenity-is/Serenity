using System.Data.Common;

namespace Serenity.Data;

public class DapperCoreAsyncTests
{
    [Fact]
    public async Task EnsureOpenAsync_Opens_ClosedConnection()
    {
        using var connection = new AsyncTestConnection();
        await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task EnsureOpenAsync_Opens_ClosedConnection_WithNonDbConnection()
    {
        using var connection = new MockDbConnection();
        var result = await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        Assert.Same(connection, result);
    }

    [Fact]
    public async Task EnsureOpenAsync_DoesNotOpen_AlreadyOpenConnection()
    {
        using var connection = new AsyncTestConnection();
        connection.Open();
        await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task EnsureOpenAsync_Throws_ForNullConnection()
    {
        IDbConnection connection = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => connection.EnsureOpenAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task EnsureOpenAsync_Throws_ForOpenedOnceConnection()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        connection.Open();
        connection.Close();
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.EnsureOpenAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WrappedConnection_OpenAsync_Opens_ActualConnection()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        Assert.True(connection.OpenedOnce);
    }

    [Fact]
    public async Task WrappedConnection_OpenAsync_Opens_ActualDbConnection()
    {
        using var actual = new AsyncTestConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        await connection.OpenAsync(TestContext.Current.CancellationToken);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.True(connection.OpenedOnce);
        Assert.Equal(1, actual.OpenCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Executes_AndReturnsRowsAffected()
    {
        using var connection = new AsyncTestConnection()
            .OnExecuteNonQuery(sql => 5);

        var result = await connection.ExecuteAsync("UPDATE [Table] SET [X] = 1", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(5, result);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task QueryAsync_Returns_DynamicObjects()
    {
        using var connection = new AsyncTestConnection()
            .OnExecuteReader(sql => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var result = await connection.QueryAsync("SELECT * FROM [Table]", cancellationToken: TestContext.Current.CancellationToken);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
        Assert.Equal("Test", (string)list[0].Name);
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public async Task QueryAsync_T_Returns_TypedObjects()
    {
        using var connection = new AsyncTestConnection()
            .OnExecuteReader(sql => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var result = await connection.QueryAsync<TestRow>("SELECT * FROM [Table]", cancellationToken: TestContext.Current.CancellationToken);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
        Assert.Equal("Test", list[0].Name);
    }

    [Fact]
    public async Task QueryAsync_ISqlQuery_Returns_DynamicObjects()
    {
        using var connection = new AsyncTestConnection()
            .OnExecuteReader(sql => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var query = new SqlQuery()
            .From("Table")
            .Select("Id")
            .Select("Name")
            .Where("Id = @Id");
        query.AddParam("@Id", 1);

        var result = await connection.QueryAsync(query, cancellationToken: TestContext.Current.CancellationToken);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(1, (int)list[0].Id);
        Assert.Equal("Test", (string)list[0].Name);
    }

    [Fact]
    public async Task QueryAsync_T_ISqlQuery_Returns_TypedObjects()
    {
        using var connection = new AsyncTestConnection()
            .OnExecuteReader(sql => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var query = new SqlQuery()
            .From("Table")
            .Select("Id")
            .Select("Name")
            .Where("Id = @Id");
        query.AddParam("@Id", 1);

        var result = await connection.QueryAsync<TestRow>(query, cancellationToken: TestContext.Current.CancellationToken);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
        Assert.Equal("Test", list[0].Name);
    }

    private class TestRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class AsyncTestConnection : DbConnection
    {
        private ConnectionState state = ConnectionState.Closed;
        public override string ConnectionString { get; set; }
        public override string Database => "Test";
        public override string DataSource => "Test";
        public override string ServerVersion => "1.0";
        public override ConnectionState State => state;
        public int OpenCalls { get; private set; }
        public Func<string, DbDataReader> ReaderCallback { get; private set; }
        public Func<string, int> NonQueryCallback { get; private set; }

        public AsyncTestConnection OnExecuteReader(Func<string, DbDataReader> func)
        {
            ReaderCallback = func;
            return this;
        }

        public AsyncTestConnection OnExecuteNonQuery(Func<string, int> func)
        {
            NonQueryCallback = func;
            return this;
        }

        public override void ChangeDatabase(string databaseName) { }
        public override void Close() => state = ConnectionState.Closed;
        public override void Open()
        {
            state = ConnectionState.Open;
            OpenCalls++;
        }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotImplementedException();
        protected override DbCommand CreateDbCommand() => new AsyncTestCommand(this);
        protected override DbProviderFactory DbProviderFactory => throw new NotImplementedException();
    }

    private class AsyncTestCommand : DbCommand
    {
        private readonly AsyncTestConnection connection;
        public AsyncTestCommand(AsyncTestConnection connection)
        {
            this.connection = connection;
        }
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get => connection; set => throw new NotImplementedException(); }
        protected override DbParameterCollection DbParameterCollection { get; } = new MockDbParameterCollection();
        protected override DbTransaction DbTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override void Cancel() { }
        public override int ExecuteNonQuery() => connection.NonQueryCallback?.Invoke(CommandText) ?? 0;
        public override object ExecuteScalar() => throw new NotImplementedException();
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => new MockDbParameter();
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            connection.ReaderCallback?.Invoke(CommandText) ?? throw new NotImplementedException();
    }
}