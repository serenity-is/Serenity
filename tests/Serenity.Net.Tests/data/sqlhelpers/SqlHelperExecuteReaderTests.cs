namespace Serenity.Data;

public class SqlHelperExecuteReaderTests
{
    [Fact]
    public void ExecuteReader_InterceptorReturnsReader_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { Id = 1 }))
            .OnDbCommandExecuteReader(_ => throw new InvalidOperationException("should not execute"));

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT * FROM [Table]", null);

        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt32(0));
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal("SELECT * FROM [Table]", call.CommandText);
        Assert.Null(call.Query);
        Assert.Equal(0, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public void ExecuteReader_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => default)
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 2 }));

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT * FROM [Table]", null);

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt32(0));
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public void ExecuteReader_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 3 }));

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT * FROM [Table]", null);

        Assert.True(reader.Read());
        Assert.Equal(3, reader.GetInt32(0));
    }

    [Fact]
    public void ExecuteReader_WithParams_AddsParametersToCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 1 }));

        using var reader = SqlHelper.ExecuteReader(connection, "SELECT * FROM [Table] WHERE Id = @p1",
            new Dictionary<string, object?> { ["@p1"] = 1 });

        Assert.True(reader.Read());
    }

    [Fact]
    public void ExecuteReader_WithQuery_InterceptorReceivesQuery()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { Id = 1 }));

        var query = new SqlQuery().From("Table").Select("Id");
        using var reader = query.ExecuteReader(connection);

        Assert.True(reader.Read());
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Same(query, call.Query);
    }

    [Fact]
    public void ExecuteReader_WithQuery_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => default)
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 4 }));

        var query = new SqlQuery().From("Table").Select("Id");
        using var reader = query.ExecuteReader(connection);

        Assert.True(reader.Read());
        Assert.Equal(4, reader.GetInt32(0));
    }

    [Fact]
    public void ExecuteReader_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentNullException>(() =>
            SqlHelper.ExecuteReader(null, connection));
    }

    [Fact]
    public async Task ExecuteReaderAsync_InterceptorReturnsReader_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { Id = 1 }))
            .OnDbCommandExecuteReader(_ => throw new InvalidOperationException("should not execute"));

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM [Table]", null,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        Assert.Equal(0, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExecuteReaderAsync_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => default)
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 2 }));

        using var reader = await SqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM [Table]", null,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExecuteReaderAsync_WithQuery_InterceptorReceivesQuery()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader(new { Id = 1 }));

        var query = new SqlQuery().From("Table").Select("Id");
        using var reader = await query.ExecuteReaderAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(reader.Read());
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Same(query, call.Query);
        Assert.True(call.IsAsync);
    }

    [Fact]
    public async Task ExecuteReaderAsync_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            SqlHelper.ExecuteReaderAsync(null, connection, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ExecuteReader_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => throw new InvalidOperationException("boom"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            SqlHelper.ExecuteReader(connection, "SELECT * FROM [Table]", null));

        Assert.Equal("SELECT * FROM [Table]", ex.Data["sql_command_text"]);
    }

    [Fact]
    public async Task ExecuteReaderAsync_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => throw new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            SqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM [Table]", null,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("SELECT * FROM [Table]", ex.Data["sql_command_text"]);
    }
}
