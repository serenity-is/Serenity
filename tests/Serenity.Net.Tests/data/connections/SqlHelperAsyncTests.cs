namespace Serenity.Data;

public class SqlHelperAsyncTests
{
    [Fact]
    public async Task ExecuteNonQueryAsync_Intercepts()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 5);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = 1", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(5, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Equal("UPDATE [Table] SET [X] = 1", call.CommandText);
    }

    [Fact]
    public async Task ExecuteReaderAsync_Intercepts()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 1, Name = "Test" }));

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM [Table]", null, cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt32(0));
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal("SELECT * FROM [Table]", call.CommandText);
    }

    [Fact]
    public async Task ExecuteReaderAsync_Executes_OnDbConnection()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 2, Name = "Real" }));

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM [Table]", null, cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt32(0));
        Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_SqlUpdate_Intercepts()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 1);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");
        var result = await query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task ExistsAsync_Intercepts()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new { Id = 1 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = await query.ExistsAsync(connection, cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(result);
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Same(query, call.Query);
    }

    [Fact]
    public async Task ExistsAsync_Executes_OnDbConnection()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(command => new MockDbDataReader(new { Id = 3 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = await query.ExistsAsync(connection, cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(result);
        Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_Executes_OnDbConnection()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(command => 7);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = 1", cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(7, result);
    }
}