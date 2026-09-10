namespace Serenity.Data;

public class SqlHelperExecuteAndGetIDTests
{
    [Fact]
    public void ExecuteAndGetID_InterceptorReturnsValue_DoesNotExecute()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args =>
            {
                Assert.True(args.GetNewId);
                Assert.Equal(ExpectedRows.One, args.ExpectedRows);
                return 42;
            })
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = query.ExecuteAndGetID(connection);

        Assert.Equal(42, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.True(call.GetNewId);
    }

    [Fact]
    public void ExecuteAndGetID_WithScopeIdentityDialect_ReadsIdentityFromReader()
    {
        string? commandText = null;
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteReader(cmd =>
            {
                commandText = cmd.CommandText;
                return new MockDbDataReader(new { IDCOLUMNVALUE = (long?)123 });
            });

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = query.ExecuteAndGetID(connection);

        Assert.Equal(123, result);
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
        Assert.Contains("SCOPE_IDENTITY() AS IDCOLUMNVALUE", commandText);
    }

    [Fact]
    public void ExecuteAndGetID_WithScopeIdentityDialect_EmptyReader_ReturnsNull()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = query.ExecuteAndGetID(connection);

        Assert.Null(result);
    }

    [Fact]
    public void ExecuteAndGetID_WithScopeIdentityDialect_NullValue_ReturnsNull()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { IDCOLUMNVALUE = (long?)null }));

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = query.ExecuteAndGetID(connection);

        Assert.Null(result);
    }

    [Fact]
    public void ExecuteAndGetID_WithReturningIdentityDialect_ExecutesReturningCommand()
    {
        string? commandText = null;
        using var connection = new MockDbConnection { Dialect = PostgresDialect.Instance }
            .OnDbCommandExecuteNonQuery(cmd =>
            {
                commandText = cmd.CommandText;
                // a real provider sets the output parameter value after execution
                cmd.Parameters["Id"].Value = 1L;
                return 1;
            });

        var query = new SqlInsert("Table").SetTo("X", "1").IdentityColumn("Id");
        var result = query.ExecuteAndGetID(connection);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
        Assert.Contains("RETURNING", commandText);
    }

    [Fact]
    public void ExecuteAndGetID_WithReturningIdentityDialect_MissingIdentityColumn_Throws()
    {
        using var connection = new MockDbConnection { Dialect = PostgresDialect.Instance }
            .OnDbCommandExecuteNonQuery(_ => 1);

        var query = new SqlInsert("Table").SetTo("X", "1");

        Assert.Throws<ArgumentNullException>(() => query.ExecuteAndGetID(connection));
    }

    [Fact]
    public void ExecuteAndGetID_WithReturningIntoVarDialect_ExecutesReturningCommand()
    {
        string? commandText = null;
        using var connection = new MockDbConnection { Dialect = OracleDialect.Instance }
            .OnDbCommandExecuteNonQuery(cmd =>
            {
                commandText = cmd.CommandText;
                cmd.Parameters["Id"].Value = 1L;
                return 1;
            });

        var query = new SqlInsert("Table").SetTo("X", "1").IdentityColumn("Id");
        var result = query.ExecuteAndGetID(connection);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
        Assert.Contains("RETURNING", commandText);
        Assert.Contains(" INTO :Id", commandText);
    }

    [Fact]
    public void ExecuteAndGetID_WithUnsupportedDialect_ThrowsNotImplemented()
    {
        using var connection = new MockDbConnection { Dialect = new NoIdentityDialect() };

        var query = new SqlInsert("Table").SetTo("X", "1");

        Assert.Throws<NotImplementedException>(() => query.ExecuteAndGetID(connection));
    }

    [Fact]
    public async Task ExecuteAndGetIDAsync_InterceptorReturnsValue_DoesNotExecute()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteNonQuery(args => 42)
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = await query.ExecuteAndGetIDAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(42, result);
        var call = Assert.Single(connection.ExecuteNonQueryCalls);
        Assert.True(call.GetNewId);
        Assert.True(call.IsAsync);
    }

    [Fact]
    public async Task ExecuteAndGetIDAsync_WithScopeIdentityDialect_ReadsIdentityFromReader()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { IDCOLUMNVALUE = (long?)123 }));

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = await query.ExecuteAndGetIDAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(123, result);
    }

    [Fact]
    public async Task ExecuteAndGetIDAsync_WithScopeIdentityDialect_EmptyReader_ReturnsNull()
    {
        using var connection = new MockDbConnection { Dialect = SqlServer2012Dialect.Instance }
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        var query = new SqlInsert("Table").SetTo("X", "1");
        var result = await query.ExecuteAndGetIDAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAndGetIDAsync_WithReturningIdentityDialect_ExecutesReturningCommand()
    {
        using var connection = new MockDbConnection { Dialect = PostgresDialect.Instance }
            .OnDbCommandExecuteNonQuery(cmd =>
            {
                cmd.Parameters["Id"].Value = 1L;
                return 1;
            });

        var query = new SqlInsert("Table").SetTo("X", "1").IdentityColumn("Id");
        var result = await query.ExecuteAndGetIDAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task ExecuteAndGetIDAsync_WithUnsupportedDialect_ThrowsNotImplemented()
    {
        using var connection = new MockDbConnection { Dialect = new NoIdentityDialect() };

        var query = new SqlInsert("Table").SetTo("X", "1");

        await Assert.ThrowsAsync<NotImplementedException>(() => query.ExecuteAndGetIDAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken));
    }

    private class NoIdentityDialect : SqlServer2012Dialect
    {
        public override bool UseScopeIdentity => false;
    }
}
