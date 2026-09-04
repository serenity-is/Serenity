using System.Data.Common;

namespace Serenity.Data;

public class DapperCoreAsyncTests
{
    [Fact]
    public async Task EnsureOpenAsync_Opens_ClosedConnection()
    {
        using var connection = new MockDbConnection();
        var result = await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        Assert.Same(connection, result);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task EnsureOpenAsync_DoesNotOpen_AlreadyOpenConnection()
    {
        using var connection = new MockDbConnection();
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
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.True(connection.OpenedOnce);
        Assert.Equal(1, actual.OpenCalls);
    }

    [Fact]
    public async Task ExecuteAsync_Executes_AndReturnsRowsAffected()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(command => 5);

        var result = await connection.ExecuteAsync("UPDATE [Table] SET [X] = 1", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(5, result);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public async Task QueryAsync_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

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
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        var result = await connection.QueryAsync<TestRow>("SELECT * FROM [Table]", cancellationToken: TestContext.Current.CancellationToken);
        var list = result.ToList();
        Assert.Single(list);
        Assert.Equal(1, list[0].Id);
        Assert.Equal("Test", list[0].Name);
    }

    [Fact]
    public async Task QueryAsync_ISqlQuery_Returns_DynamicObjects()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

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
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 1, Name = "Test" }));

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

    [Fact]
    public async Task EnsureOpenAsync_Opens_WrappedConnection()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        await connection.EnsureOpenAsync(TestContext.Current.CancellationToken);
        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.True(connection.OpenedOnce);
    }

    [Fact]
    public void CreateCommand_AsIDbConnection_Works()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        var command = ((IDbConnection)connection).CreateCommand();
        Assert.IsType<MockDbCommand>(command);
    }

    [Fact]
    public void CreateCommand_AsDbConnection_Works()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        var command = connection.CreateCommand();
        Assert.IsType<MockDbCommand>(command);
    }

    [Fact]
    public void DbProviderFactory_ReturnsNull_ForDbConnectionActual_WithNoFactory()
    {
        using var actual = new MockDbConnection();
        using var connection = new WrappedConnection(actual, SqlServer2012Dialect.Instance);
        Assert.Null(DbProviderFactories.GetFactory(connection));
    }

    private class TestRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}