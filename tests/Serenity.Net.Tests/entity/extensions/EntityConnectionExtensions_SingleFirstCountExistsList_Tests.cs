namespace Serenity.Data;

public class EntityConnectionExtensions_SingleFirstCountExistsList_Tests
{
    private static void OnReader(MockDbConnection connection, params object[] items) =>
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader(items));

    [Fact]
    public void Single_WithNoCriteria_ReturnsFirstRow()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" });

        var row = connection.Single<IdNameRow>(new Criteria("Name") == "A");

        Assert.Equal("A", row.Name);
    }

    [Fact]
    public async Task Single_WithNoRows_ThrowsValidationError()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader());

        await Assert.ThrowsAsync<ValidationError>(() =>
            connection.SingleAsync<IdNameRow>((ICriteria?)null));
    }

    [Fact]
    public void TrySingle_WithNoRows_ReturnsNull()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.Null(connection.TrySingle<IdNameRow>((ICriteria?)null));
    }

    [Fact]
    public void TrySingle_WithRow_ReturnsIt()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 5, Name = "B" });

        Assert.Equal(5, connection.TrySingle<IdNameRow>(new Criteria("Name") == "B")?.ID);
    }

    [Fact]
    public void Single_WithEditQuery_ReturnsRowFromEditedQuery()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(args =>
        {
            Assert.Contains("ID", args.CommandText, StringComparison.Ordinal);
            return new MockDbDataReader(new { ID = 3, Name = "C" });
        });

        connection.Single<IdNameRow>(q => q.Select("ID"));
    }

    [Fact]
    public void TrySingle_WithEditQuery_AndNoRow_ReturnsNull()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.Null(connection.TrySingle<IdNameRow>(q => q.Select("1")));
    }

    [Fact]
    public async Task TrySingleAsync_WithEditQuery_ReturnsRow()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 9, Name = "D" }));

        var row = await connection.TrySingleAsync<IdNameRow>(q => q.Select("ID"));

        Assert.Equal(9, row.ID);
    }

    [Fact]
    public void First_ExistingCriteria_ReturnsFirstRow()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" });

        var row = connection.First<IdNameRow>(new Criteria("Name") == "A");

        Assert.Equal("A", row.Name);
    }

    [Fact]
    public void TryFirst_WithNoRows_ReturnsNull()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.Null(connection.TryFirst<IdNameRow>(new Criteria("Name") == "X"));
    }

    [Fact]
    public void TryFirst_WithRows_ReturnsFirstRow()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" }, new { ID = 2, Name = "C" });

        Assert.Equal(1, connection.TryFirst<IdNameRow>(new Criteria("Name") == "A")?.ID);
    }

    [Fact]
    public async Task TryFirstAsync_WithEditQuery_ReturnsRow()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 2, Name = "B" }));

        Assert.Equal(2, (await connection.TryFirstAsync<IdNameRow>(q => q.Select("ID")))!.ID);
    }

    [Fact]
    public void Count_NoCriteria_UsesCountStar()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteScalar(_ => 12);

        Assert.Equal(12, connection.Count<IdNameRow>());
    }

    [Fact]
    public void Count_WithCriteria_IncludesWhere()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteScalar(_ => 3);

        Assert.Equal(3, connection.Count<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public async Task CountAsync_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptListRows(args => args.CountOnly ?
                new OptionalValue<System.Collections.IList>(new System.Collections.ArrayList()) : default);

        Assert.Equal(0, await connection.CountAsync<IdNameRow>());
    }

    [Fact]
    public async Task CountAsync_WithCriteria()
    {
        using var connection = new MockDbConnection();
        connection.OnDbCommandExecuteScalar(_ => 4);

        Assert.Equal(4, await connection.CountAsync<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public void ExistsById_Existing_ReturnsTrue()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 5 }));

        Assert.True(connection.ExistsById<IdNameRow>(5));
    }

    [Fact]
    public void ExistsById_NonExisting_ReturnsFalse()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        Assert.False(connection.ExistsById<IdNameRow>(5));
    }

    [Fact]
    public void Exists_WithCriteria()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { s = 1 }));

        Assert.True(connection.Exists<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public async Task ExistsAsync_WithCriteria()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { s = 1 }));

        Assert.True(await connection.ExistsAsync<IdNameRow>(new Criteria("Name") == "A"));
    }

    [Fact]
    public void List_ReturnsAllRows()
    {
        using var connection = new MockDbConnection();
        OnReader(connection,
            new { ID = 1, Name = "A" },
            new { ID = 2, Name = "B" });

        var rows = connection.List<IdNameRow>();

        Assert.Equal(2, rows.Count);
        Assert.Equal("A", rows[0].Name);
        Assert.Equal("B", rows[1].Name);
    }

    [Fact]
    public void List_WithCriteria_IncludesWhere()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" });

        var rows = connection.List<IdNameRow>(new Criteria("Name") == "A");

        Assert.Single(rows);
    }

    [Fact]
    public void List_WithEditQuery_SelectsSpecifiedFields()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1 });

        var rows = connection.List<IdNameRow>(q => q.Select("ID"));

        Assert.Single(rows);
        Assert.Equal(1, rows[0].ID);
    }

    [Fact]
    public async Task ListAsync_ReturnsRows()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" }, new { ID = 2, Name = "B" });

        var rows = await connection.ListAsync<IdNameRow>();

        Assert.Equal(2, rows.Count);
    }

    [Fact]
    public async Task ListAsync_WithCriteria()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 1, Name = "A" });

        var rows = await connection.ListAsync<IdNameRow>(new Criteria("Name") == "A");

        Assert.Single(rows);
    }

    [Fact]
    public async Task ListAsync_WithEditQuery()
    {
        using var connection = new MockDbConnection();
        OnReader(connection, new { ID = 44 });

        var rows = await connection.ListAsync<IdNameRow>(q => q.Select("ID"));

        Assert.Equal(44, rows[0].ID);
    }
}
