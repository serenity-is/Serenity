namespace Serenity.Data;

public class EntityConnectionExtensions_ById_Full_Tests
{
    private static MockDbConnection Reader(params object[] items) =>
        new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(items));

    [Fact]
    public void ById_ReturnsRow_WhenFound()
    {
        using var connection = Reader(new { ID = 777, Name = "Test" });

        var row = connection.ById<IdNameRow>(777);

        Assert.Equal("Test", row.Name);
    }

    [Fact]
    public void ById_ThrowsValidationError_WhenNotFound()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        var ex = Assert.Throws<ValidationError>(() => connection.ById<IdNameRow>(777));

        Assert.Equal("RecordNotFound", ex.ErrorCode);
    }

    [Fact]
    public void ById_WithEditQuery_ReturnsRow()
    {
        using var connection = Reader(new { ID = 777, Name = "Test" });

        var edited = new List<string>();
        var row = connection.ById<IdNameRow>(777, q =>
        {
            q.Select("ID");
            edited.Add("called");
        });

        Assert.Contains("called", edited, StringComparer.Ordinal);
        Assert.Equal(777, row.ID);
    }

    [Fact]
    public void ById_WithEditQuery_ThrowsValidationError_WhenNotFound()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.Throws<ValidationError>(() => connection.ById<IdNameRow>(777, q => q.Select("1")));
    }

    [Fact]
    public void ById_Interceptor_FindsRow()
    {
        var row = new IdNameRow { ID = 5, Name = "IC" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(args => new OptionalValue<IRow>(row));

        Assert.Same(row, connection.ById<IdNameRow>(5));
        Assert.Equal(0, connection.DbCommandCallCount);
    }

    [Fact]
    public void TryById_EditQueryInterceptor()
    {
        SqlQuery editedQuery = null;
        var row = new IdNameRow { ID = 6, Name = "IE" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(args =>
            {
                editedQuery = new SqlQuery();
                args.EditQuery(editedQuery);
                return new OptionalValue<IRow>(row);
            });

        var result = connection.TryById<IdNameRow>(6, q => q.Select("ID"));

        Assert.Same(row, result);
        Assert.Equal(0, connection.DbCommandCallCount);
    }

    [Fact]
    public async Task ByIdAsync_ReturnsRow()
    {
        using var connection = Reader(new { ID = 777, Name = "Test" });

        var row = await connection.ByIdAsync<IdNameRow>(777, TestContext.Current.CancellationToken);

        Assert.Equal("Test", row.Name);
    }

    [Fact]
    public async Task ByIdAsync_ThrowsValidationError_WhenNotFound()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        await Assert.ThrowsAsync<ValidationError>(() => connection.ByIdAsync<IdNameRow>(777, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryByIdAsync_ReturnsNull_WhenNotFound()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.Null(await connection.TryByIdAsync<IdNameRow>(777, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryByIdAsync_WithEditQuery_ReturnsRow()
    {
        using var connection = Reader(new { ID = 777, Name = "Test" });

        var row = await connection.TryByIdAsync<IdNameRow>(777, q => q.Select("ID"), TestContext.Current.CancellationToken);

        Assert.Equal(777, row.ID);
    }

    [Fact]
    public async Task TryByIdAsync_Interceptor_Authenticates()
    {
        var row = new IdNameRow { ID = 8, Name = "IX" };
        using var connection = new MockDbConnection()
            .InterceptFindRow(args => new OptionalValue<IRow>(row));

        Assert.Same(row, await connection.TryByIdAsync<IdNameRow>(8, TestContext.Current.CancellationToken));
        Assert.Equal(0, connection.DbCommandCallCount);
    }
}
