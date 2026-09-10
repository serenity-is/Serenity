namespace Serenity.Data;

public class SqlHelperExecuteScalarTests
{
    [Fact]
    public void ExecuteScalar_InterceptorReturnsValue_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => 42)
            .OnDbCommandExecuteScalar(_ => throw new InvalidOperationException("should not execute"));

        var result = SqlHelper.ExecuteScalar(connection, "SELECT COUNT(*) FROM [Table]");

        Assert.Equal(42, result);
        var call = Assert.Single(connection.ExecuteScalarCalls);
        Assert.Equal("SELECT COUNT(*) FROM [Table]", call.CommandText);
        Assert.Null(call.Query);
        Assert.Equal(0, connection.DbCommandExecuteScalarCallCount);
    }

    [Fact]
    public void ExecuteScalar_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => default)
            .OnDbCommandExecuteScalar(_ => 7);

        var result = SqlHelper.ExecuteScalar(connection, "SELECT COUNT(*) FROM [Table]");

        Assert.Equal(7, result);
        Assert.Equal(1, connection.DbCommandExecuteScalarCallCount);
    }

    [Fact]
    public void ExecuteScalar_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => "value");

        var result = SqlHelper.ExecuteScalar(connection, "SELECT [X] FROM [Table]");

        Assert.Equal("value", result);
    }

    [Fact]
    public void ExecuteScalar_WithParams_AddsParametersToCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => 1);

        var result = SqlHelper.ExecuteScalar(connection, "SELECT [X] FROM [Table] WHERE Id = @p1",
            new Dictionary<string, object?> { ["@p1"] = 1 });

        Assert.Equal(1, result);
    }

    [Fact]
    public void ExecuteScalar_WithQuery_InterceptorReceivesQuery()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => 42);

        var query = new SqlQuery().From("Table").Select("COUNT(*)");
        var result = SqlHelper.ExecuteScalar(connection, query);

        Assert.Equal(42, result);
        var call = Assert.Single(connection.ExecuteScalarCalls);
        Assert.Same(query, call.Query);
    }

    [Fact]
    public void ExecuteScalar_WithQuery_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => default)
            .OnDbCommandExecuteScalar(_ => 9);

        var query = new SqlQuery().From("Table").Select("COUNT(*)");
        var result = SqlHelper.ExecuteScalar(connection, query);

        Assert.Equal(9, result);
    }

    [Fact]
    public void ExecuteScalar_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentNullException>(() =>
            SqlHelper.ExecuteScalar(connection, (SqlQuery)null!));
    }

    [Fact]
    public async Task ExecuteScalarAsync_InterceptorReturnsValue_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => 42)
            .OnDbCommandExecuteScalar(_ => throw new InvalidOperationException("should not execute"));

        var result = await SqlHelper.ExecuteScalarAsync(connection, "SELECT COUNT(*) FROM [Table]",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        Assert.Equal(0, connection.DbCommandExecuteScalarCallCount);
    }

    [Fact]
    public async Task ExecuteScalarAsync_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => default)
            .OnDbCommandExecuteScalar(_ => 7);

        var result = await SqlHelper.ExecuteScalarAsync(connection, "SELECT COUNT(*) FROM [Table]",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(7, result);
        Assert.Equal(1, connection.DbCommandExecuteScalarCallCount);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WithQuery_InterceptorReceivesQuery()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteScalar(_ => 42);

        var query = new SqlQuery().From("Table").Select("COUNT(*)");
        var result = await SqlHelper.ExecuteScalarAsync(connection, query,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        var call = Assert.Single(connection.ExecuteScalarCalls);
        Assert.Same(query, call.Query);
        Assert.True(call.IsAsync);
    }

    [Fact]
    public async Task ExecuteScalarAsync_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            SqlHelper.ExecuteScalarAsync(connection, (SqlQuery)null!,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ExecuteScalar_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => throw new InvalidOperationException("boom"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            SqlHelper.ExecuteScalar(connection, "SELECT COUNT(*) FROM [Table]"));

        Assert.Equal("SELECT COUNT(*) FROM [Table]", ex.Data["sql_command_text"]);
    }

    [Fact]
    public async Task ExecuteScalarAsync_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteScalar(_ => throw new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            SqlHelper.ExecuteScalarAsync(connection, "SELECT COUNT(*) FROM [Table]",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("SELECT COUNT(*) FROM [Table]", ex.Data["sql_command_text"]);
    }
}
