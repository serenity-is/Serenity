using System.Collections;

namespace Serenity.Data;

public class EntityConnectionExtensions_Coverage_Tests
{
    private static IRow Row(int id = 1, string name = "A") => new IdNameRow { ID = id, Name = name };

    private static MockDbConnection FindRowInterceptor(IRow? value) =>
        new MockDbConnection().InterceptFindRow(_ => new OptionalValue<IRow>(value!));

    private static MockDbConnection ListRowsInterceptor(IList? value) =>
        new MockDbConnection().InterceptListRows(_ => new OptionalValue<IList>(value!));

    private static MockDbConnection EmptyReader() =>
        new MockDbConnection().OnDbCommandExecuteReader(_ => new MockDbDataReader());

    private static readonly object[] OneRow = [new { ID = 1, Name = "A" }];

    private static MockDbConnection Reader(params object[] items) =>
        new MockDbConnection().OnDbCommandExecuteReader(_ => new MockDbDataReader(items));

    [Fact]
    public void TrySingle_Interceptor_Where_ReturnsRow()
    {
        var row = Row();
        using var connection = FindRowInterceptor(row);

        Assert.Same(row, connection.TrySingle<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public void TrySingle_Interceptor_EditQuery_ReturnsRow()
    {
        var row = Row();
        using var connection = FindRowInterceptor(row);

        Assert.Same(row, connection.TrySingle<IdNameRow>(q => q.Select("ID")));
    }

    [Fact]
    public void Count_Interceptor_ReturnsCount()
    {
        using var connection = ListRowsInterceptor(new ArrayList { 1, 2, 3 });

        Assert.Equal(3, connection.Count<IdNameRow>());
    }

    [Fact]
    public void ExistsById_Interceptor_ReturnsResult()
    {
        using var found = FindRowInterceptor(Row());
        Assert.True(found.ExistsById<IdNameRow>(1));

        using var notFound = FindRowInterceptor(null);
        Assert.False(notFound.ExistsById<IdNameRow>(1));
    }

    [Fact]
    public void Exists_Interceptor_ReturnsResult()
    {
        using var found = FindRowInterceptor(Row());
        Assert.True(found.Exists<IdNameRow>(new Criteria("Name") == "A"));

        using var notFound = FindRowInterceptor(null);
        Assert.False(notFound.Exists<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public void List_Interceptor_Where()
    {
        using var connection = ListRowsInterceptor(new List<IdNameRow> { (IdNameRow)Row() });

        Assert.Single(connection.List<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public void List_Interceptor_EditQuery()
    {
        using var connection = ListRowsInterceptor(new List<IdNameRow> { (IdNameRow)Row() });

        Assert.Single(connection.List<IdNameRow>(q => q.Select("ID")));
    }

    [Fact]
    public void First_WithEditQuery_NoRows_Throws()
    {
        using var connection = EmptyReader();

        Assert.Throws<ValidationError>(() => connection.First<IdNameRow>(q => q.Select("ID")));
    }

    [Fact]
    public void TryFirst_WithEditQuery_Fallback()
    {
        using var found = Reader(OneRow);
        Assert.NotNull(found.TryFirst<IdNameRow>(q => q.Select("ID")));

        using var notFound = EmptyReader();
        Assert.Null(notFound.TryFirst<IdNameRow>(q => q.Select("ID")));
    }

    [Fact]
    public async Task ByIdAsync_EditQuery_NoRows_Throws()
    {
        using var connection = EmptyReader();

        await Assert.ThrowsAsync<ValidationError>(() => connection.ByIdAsync<IdNameRow>(1,
            q => q.Select("ID"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task SingleAsync_EditQuery_NoRows_Throws()
    {
        using var connection = EmptyReader();

        await Assert.ThrowsAsync<ValidationError>(() => connection.SingleAsync<IdNameRow>(
            q => q.Select("ID"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task FirstAsync_EditQuery_NoRows_Throws()
    {
        using var connection = EmptyReader();

        await Assert.ThrowsAsync<ValidationError>(() => connection.FirstAsync<IdNameRow>(
            q => q.Select("ID"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExistsByIdAsync_Interceptor_ReturnsResult()
    {
        using var found = FindRowInterceptor(Row());
        Assert.True(await found.ExistsByIdAsync<IdNameRow>(1, TestContext.Current.CancellationToken));

        using var notFound = FindRowInterceptor(null);
        Assert.False(await notFound.ExistsByIdAsync<IdNameRow>(1, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExistsAsync_Interceptor_ReturnsResult()
    {
        using var found = FindRowInterceptor(Row());
        Assert.True(await found.ExistsAsync<IdNameRow>(new Criteria("Name") == "A",
            TestContext.Current.CancellationToken));

        using var notFound = FindRowInterceptor(null);
        Assert.False(await notFound.ExistsAsync<IdNameRow>(new Criteria("Name") == "A",
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListAsync_Interceptor_Where()
    {
        using var connection = ListRowsInterceptor(new List<IdNameRow> { (IdNameRow)Row() });

        Assert.Single(await connection.ListAsync<IdNameRow>(new Criteria("Name") == "A",
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListAsync_Interceptor_EditQuery()
    {
        using var connection = ListRowsInterceptor(new List<IdNameRow> { (IdNameRow)Row() });

        Assert.Single(await connection.ListAsync<IdNameRow>(q => q.Select("ID"),
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryByIdAsync_EditQuery_Interceptor_And_Null()
    {
        var row = Row(6, "B");
        using var intercepted = FindRowInterceptor(row);
        Assert.Same(row, await intercepted.TryByIdAsync<IdNameRow>(6, q => q.Select("ID"),
            TestContext.Current.CancellationToken));

        using var empty = EmptyReader();
        Assert.Null(await empty.TryByIdAsync<IdNameRow>(6, q => q.Select("ID"),
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryFirstAsync_Where_Interceptor()
    {
        var row = Row();
        using var connection = FindRowInterceptor(row);

        Assert.Same(row, await connection.TryFirstAsync<IdNameRow>(new Criteria("Name") == "A",
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TryFirstAsync_EditQuery_Interceptor_And_Null()
    {
        var row = Row(2, "B");
        using var intercepted = FindRowInterceptor(row);
        Assert.Same(row, await intercepted.TryFirstAsync<IdNameRow>(q => q.Select("ID"),
            TestContext.Current.CancellationToken));

        using var empty = EmptyReader();
        Assert.Null(await empty.TryFirstAsync<IdNameRow>(q => q.Select("ID"),
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TrySingleAsync_Where_Interceptor()
    {
        var row = Row();
        using var connection = FindRowInterceptor(row);

        Assert.Same(row, await connection.TrySingleAsync<IdNameRow>(new Criteria("Name") == "A",
            TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TrySingleAsync_EditQuery_Interceptor_And_Null()
    {
        var row = Row(3, "C");
        using var intercepted = FindRowInterceptor(row);
        Assert.Same(row, await intercepted.TrySingleAsync<IdNameRow>(q => q.Select("ID"),
            TestContext.Current.CancellationToken));

        using var empty = EmptyReader();
        Assert.Null(await empty.TrySingleAsync<IdNameRow>(q => q.Select("ID"),
            TestContext.Current.CancellationToken));
    }
}
