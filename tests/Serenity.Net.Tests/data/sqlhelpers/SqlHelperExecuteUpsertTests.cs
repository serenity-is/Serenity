namespace Serenity.Data;

public class SqlHelperExecuteUpsertTests
{
    [Fact]
    public void ExecuteUpsert_SetsDialectFromConnection()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .InterceptExecuteNonQuery(args => 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = query.ExecuteUpsert(connection, ["Id"]);

        Assert.Equal(1, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Contains("INSERT INTO", call.CommandText);
        Assert.Contains("NOT EXISTS", call.CommandText);
    }

    [Fact]
    public void ExecuteUpsert_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .InterceptExecuteNonQuery(args => 1)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = query.ExecuteUpsert(connection, ["Id"]);

        Assert.Equal(1, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpsert_InterceptorReturnsEmpty_ExecutesCommand()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .InterceptExecuteNonQuery(_ => default)
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = query.ExecuteUpsert(connection, ["Id"]);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpsert_WithExpectedRowsOne_AndZeroRows_Throws()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteNonQuery(_ => 0);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            query.ExecuteUpsert(connection, ["Id"], ExpectedRows.One));

        Assert.Contains("Query affected 0 rows while 1 expected", ex.Message);
    }

    [Fact]
    public void ExecuteUpsert_WithExpectedRowsZeroOrOne_AndTwoRows_Throws()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteNonQuery(_ => 2);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            query.ExecuteUpsert(connection, ["Id"], ExpectedRows.ZeroOrOne));

        Assert.Contains("Query affected 2 rows while 0 or 1 expected", ex.Message);
    }

    [Fact]
    public void ExecuteUpsert_WithUnsupportedDialect_FallsBackToUpdateThenInsert()
    {
        using var connection = new MockDbConnection { Dialect = new UnknownUpsertDialect() }
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = query.ExecuteUpsert(connection, ["Id"]);

        // update affects 1 row so insert is not executed
        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpsert_WithUnsupportedDialect_AndUpdateMissed_ExecutesInsert()
    {
        var calls = 0;
        using var connection = new MockDbConnection { Dialect = new UnknownUpsertDialect() }
            .OnDbCommandExecuteNonQuery(_ => ++calls <= 1 ? 0 : 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = query.ExecuteUpsert(connection, ["Id"]);

        // update affects 0 rows so insert is executed too
        Assert.Equal(1, result);
        Assert.Equal(2, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void ExecuteUpsert_NullQuery_ThrowsArgumentNullException()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<ArgumentNullException>(() =>
            SqlHelper.ExecuteUpsert(null, connection, ["Id"]));
    }

    [Fact]
    public void ExecuteUpsert_NullConnection_ThrowsArgumentNullException()
    {
        var query = new SqlInsert("Table").SetTo("Id", "1");

        Assert.Throws<ArgumentNullException>(() =>
            query.ExecuteUpsert(null, ["Id"]));
    }

    [Fact]
    public async Task ExecuteUpsertAsync_SetsDialectFromConnection()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .InterceptExecuteNonQuery(args => 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = await query.ExecuteUpsertAsync(connection, ["Id"],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.Contains("INSERT INTO", call.CommandText);
        Assert.Contains("NOT EXISTS", call.CommandText);
        Assert.True(call.IsAsync);
    }

    [Fact]
    public async Task ExecuteUpsertAsync_InterceptorHandles_DoesNotExecuteCommand()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .InterceptExecuteNonQuery(args => 1)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = await query.ExecuteUpsertAsync(connection, ["Id"],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteUpsertAsync_WithUnsupportedDialect_FallsBackToUpdateThenInsert()
    {
        using var connection = new MockDbConnection { Dialect = new UnknownUpsertDialect() }
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");
        var result = await query.ExecuteUpsertAsync(connection, ["Id"],
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteUpsertAsync_WithExpectedRowsOne_AndZeroRows_Throws()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteNonQuery(_ => 0);

        var query = new SqlInsert("Table").SetTo("Id", "1").SetTo("X", "2");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            query.ExecuteUpsertAsync(connection, ["Id"], ExpectedRows.One,
                cancellationToken: TestContext.Current.CancellationToken));
    }

    private class UnknownUpsertDialect : SqlServer2012Dialect
    {
        public override string ServerType => "UnknownServer";
    }
}
