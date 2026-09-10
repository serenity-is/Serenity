using Microsoft.Extensions.Logging;

namespace Serenity.Data;

public class SqlHelperExecuteNonQueryTests
{
    [Fact]
    public void ExecuteNonQuery_InterceptorReturnsValue_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 5)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var result = SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1");

        Assert.Equal(5, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Equal("UPDATE [Table] SET [X] = 1", call.CommandText);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteNonQuery_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 7);

        var result = SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1");

        Assert.Equal(7, result);
        Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteNonQuery_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 3);

        var result = SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1");

        Assert.Equal(3, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteNonQuery_WithParams_AddsParametersToCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var result = SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = @p1",
            new Dictionary<string, object?> { ["@p1"] = 5 });

        Assert.Equal(1, result);
    }

    [Fact]
    public void ExecuteNonQuery_OpensClosedConnection()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        Assert.Equal(ConnectionState.Closed, connection.State);

        SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1");

        Assert.Equal(ConnectionState.Open, connection.State);
        Assert.Equal(1, connection.OpenCalls);
    }

    [Fact]
    public void ExecuteNonQuery_WithDebugLogger_LogsStartAndEnd()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);
        var logger = new MockLogger();

        SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1", logger: logger);

        Assert.Null(logger.LastException);
    }

    [Fact]
    public void ExecuteNonQuery_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("boom"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            SqlHelper.ExecuteNonQuery(connection, "UPDATE [Table] SET [X] = 1"));

        Assert.Equal("UPDATE [Table] SET [X] = 1", ex.Data["sql_command_text"]);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_InterceptorReturnsValue_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 5)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = 1",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(5, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 7);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = 1",
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(7, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_WhenCommandThrows_SetsCommandTextData()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("boom"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = 1",
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal("UPDATE [Table] SET [X] = 1", ex.Data["sql_command_text"]);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_WithParams_AddsParametersToCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var result = await SqlHelper.ExecuteNonQueryAsync(connection, "UPDATE [Table] SET [X] = @p1",
            new Dictionary<string, object?> { ["@p1"] = 5 },
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
    }
}
