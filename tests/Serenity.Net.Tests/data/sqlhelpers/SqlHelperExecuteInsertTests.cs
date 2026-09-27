namespace Serenity.Data;

public class SqlHelperExecuteInsertTests
{
    [Fact]
    public void Execute_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.Equal(ExpectedRows.One, args.ExpectedRows);
                Assert.False(args.GetNewId);
                return 1;
            })
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("X", "@p1");
        query.SetParam("@p1", 1);
        var parameters = new Dictionary<string, object?> { ["@p1"] = 2 };
        query.Execute(connection, parameters);

        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Same(query, call.Query);
        Assert.Equal(2, call.Parameters!["@p1"]);
        Assert.Equal(1, query.Params!["@p1"]);
        Assert.Single(parameters);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void Execute_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("X", "1");
        query.Execute(connection);

        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void Execute_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("X", "1");
        query.Execute(connection);

        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 1)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("X", "@p1");
        query.SetParam("@p1", 1);
        var parameters = new Dictionary<string, object?> { ["@p1"] = 2 };
        await query.ExecuteAsync(connection, parameters,
            cancellationToken: TestContext.Current.CancellationToken);

        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Same(query, call.Query);
        Assert.True(call.IsAsync);
        Assert.Equal(2, call.Parameters!["@p1"]);
        Assert.Equal(1, query.Params!["@p1"]);
        Assert.Single(parameters);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("X", "1");
        await query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }
}
