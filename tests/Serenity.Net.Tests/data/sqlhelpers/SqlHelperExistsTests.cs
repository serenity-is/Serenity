namespace Serenity.Data;

public class SqlHelperExistsTests
{
    [Fact]
    public void Exists_ReaderHasRow_ReturnsTrue()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = @p1");
        query.SetParam("@p1", 1);
        var parameters = new Dictionary<string, object?> { ["@p1"] = 2 };
        var result = query.Exists(connection, parameters);

        Assert.True(result);
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Same(query, call.Query);
        Assert.Equal(2, call.Parameters!["@p1"]);
        Assert.Equal(1, query.Params!["@p1"]);
    }

    [Fact]
    public void Exists_EmptyReader_ReturnsFalse()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = query.Exists(connection);

        Assert.False(result);
    }

    [Fact]
    public void Exists_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 2 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = query.Exists(connection);

        Assert.True(result);
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }

    [Fact]
    public async Task ExistsAsync_ReaderHasRow_ReturnsTrue()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Id = 1 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = @p1");
        query.SetParam("@p1", 1);
        var parameters = new Dictionary<string, object?> { ["@p1"] = 2 };
        var result = await query.ExistsAsync(connection, parameters,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(result);
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.True(call.IsAsync);
        Assert.Equal(2, call.Parameters!["@p1"]);
        Assert.Equal(1, query.Params!["@p1"]);
    }

    [Fact]
    public async Task ExistsAsync_EmptyReader_ReturnsFalse()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = await query.ExistsAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithoutInterceptor_ExecutesCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Id = 3 }));

        var query = new SqlQuery().From("Table").Select("Id").Where("Id = 1");
        var result = await query.ExistsAsync(connection,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(result);
        Assert.Equal(1, connection.DbCommandExecuteReaderCallCount);
    }
}
