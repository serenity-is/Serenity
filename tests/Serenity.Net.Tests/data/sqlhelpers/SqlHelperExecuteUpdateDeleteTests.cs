namespace Serenity.Data;

public class SqlHelperExecuteUpdateDeleteTests
{
    [Fact]
    public void ExecuteUpdate_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.Equal(ExpectedRows.One, args.ExpectedRows);
                return 1;
            })
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");
        var result = query.Execute(connection);

        Assert.Equal(1, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Same(query, call.Query);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpdate_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");
        var result = query.Execute(connection);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpdate_WithExpectedRowsOne_AndZeroRows_Throws()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 0);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");

        var ex = Assert.Throws<InvalidOperationException>(() => query.Execute(connection));

        Assert.Contains("Query affected 0 rows while 1 expected", ex.Message);
    }

    [Fact]
    public void ExecuteUpdate_WithExpectedRowsIgnore_ReturnsAffectedRows()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 5);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");
        var result = query.Execute(connection, ExpectedRows.Ignore);

        Assert.Equal(5, result);
    }

    [Fact]
    public void ExecuteUpdate_WithExpectedRowsZeroOrOne_AndTwoRows_Throws()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 2);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");

        Assert.Throws<InvalidOperationException>(() =>
            query.Execute(connection, ExpectedRows.ZeroOrOne));
    }

    [Fact]
    public async Task ExecuteUpdateAsync_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 1)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");
        var result = await query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteUpdateAsync_WithExpectedRowsOne_AndZeroRows_Throws()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 0);

        var query = new SqlUpdate("Table").SetTo("X", "1").Where("Id = 1");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ExecuteDelete_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.Equal(ExpectedRows.One, args.ExpectedRows);
                return 1;
            })
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlDelete("Table").Where("Id = 1");
        var result = query.Execute(connection);

        Assert.Equal(1, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Same(query, call.Query);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteDelete_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlDelete("Table").Where("Id = 1");
        var result = query.Execute(connection);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteDelete_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentNullException>(() =>
            SqlHelper.Execute((SqlDelete)null, connection));
    }

    [Fact]
    public void ExecuteDelete_WithExpectedRowsOne_AndZeroRows_Throws()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 0);

        var query = new SqlDelete("Table").Where("Id = 1");

        Assert.Throws<InvalidOperationException>(() => query.Execute(connection));
    }

    [Fact]
    public async Task ExecuteDeleteAsync_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 1)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlDelete("Table").Where("Id = 1");
        var result = await query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteDeleteAsync_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlDelete("Table").Where("Id = 1");
        var result = await query.ExecuteAsync(connection, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }
}
