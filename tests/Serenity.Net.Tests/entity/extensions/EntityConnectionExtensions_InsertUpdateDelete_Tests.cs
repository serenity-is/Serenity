namespace Serenity.Data;

public class EntityConnectionExtensions_InsertUpdateDelete_Tests
{
    [Fact]
    public void Insert_ExecutesInsertCommand()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var row = new IdNameRow
        {
            Name = "X"
        };

        connection.Insert(row);
        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void Insert_Interceptor_ReturnsCommandNotExecuted()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(1))
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        connection.Insert(new IdNameRow { Name = "X" });

        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void InsertAndGetID_ReturnsNewId()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 42 }));

        var id = connection.InsertAndGetID(new IdNameRow { Name = "X" });

        Assert.Equal(42, id);
    }

    [Fact]
    public void InsertAndGetID_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(99));

        Assert.Equal(99, connection.InsertAndGetID(new IdNameRow { Name = "X" }));
        Assert.Equal(0, connection.DbCommandCallCount);
    }

    [Fact]
    public void UpdateById_NullId_ThrowsInvalidOperationException()
    {
        using var connection = new MockDbConnection();

        Assert.Throws<InvalidOperationException>(() =>
            connection.UpdateById(new IdNameRow()));
    }

    [Fact]
    public void UpdateById_ExecutesUpdate()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var result = connection.UpdateById(new IdNameRow
        {
            ID = 5,
            Name = "Y"
        });

        Assert.Equal(1, result);
    }

    [Fact]
    public void UpdateById_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(7));

        var result = connection.UpdateById(new IdNameRow { ID = 5, Name = "Y" });

        Assert.Equal(7, result);
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void DeleteById_ExecutesDelete()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        Assert.Equal(1, connection.DeleteById<IdNameRow>(5));
    }

    [Fact]
    public void DeleteById_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(3));

        Assert.Equal(3, connection.DeleteById<IdNameRow>(5));
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public void DeleteById_WithExpectedRowsMatch()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        Assert.Equal(1, connection.DeleteById<IdNameRow>(5, ExpectedRows.One));
    }

    [Fact]
    public void ToSqlInsert_WithAssignedFields()
    {
        var row = new IdNameRow { Name = "Z" };

        var insert = row.ToSqlInsert();

        Assert.Contains("INSERT INTO", insert.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToSqlInsert_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IRow)null).ToSqlInsert());
    }

    [Fact]
    public void ToSqlUpdateById_WithAssignedFields()
    {
        var row = new IdNameRow { ID = 3, Name = "Z" };

        var update = row.ToSqlUpdateById();

        Assert.Contains("UPDATE", update.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToSqlUpdateById_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IIdRow)null).ToSqlUpdateById());
    }
}

public class EntityConnectionExtensions_InsertUpdateDelete_Async_Tests
{
    [Fact]
    public async Task InsertAsync_Executes()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        await connection.InsertAsync(new IdNameRow { Name = "X" }, TestContext.Current.CancellationToken);

        Assert.Equal(1, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task InsertAsync_Interceptor_SkipsCommand()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(1))
            .OnDbCommandExecuteNonQuery(_ => throw new InvalidOperationException("should not execute"));

        await connection.InsertAsync(new IdNameRow { Name = "X" }, TestContext.Current.CancellationToken);

        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }

    [Fact]
    public async Task InsertAndGetIDAsync_ReturnsNewId()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 55 }));

        Assert.Equal(55, await connection.InsertAndGetIDAsync(new IdNameRow { Name = "X" }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task InsertAndGetIDAsync_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(101));

        Assert.Equal(101, await connection.InsertAndGetIDAsync(new IdNameRow { Name = "X" }, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateByIdAsync_NullId_ThrowsInvalidOperationException()
    {
        using var connection = new MockDbConnection();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            connection.UpdateByIdAsync(new IdNameRow(), ExpectedRows.One, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateByIdAsync_Executes()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        var result = await connection.UpdateByIdAsync(new IdNameRow { ID = 5, Name = "Y" }, ExpectedRows.One, TestContext.Current.CancellationToken);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task UpdateByIdAsync_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(7));

        Assert.Equal(7, await connection.UpdateByIdAsync(new IdNameRow { ID = 5, Name = "Y" }, ExpectedRows.One, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteByIdAsync_Executes()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteNonQuery(_ => 1);

        Assert.Equal(1, await connection.DeleteByIdAsync<IdNameRow>(5, ExpectedRows.One, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task DeleteByIdAsync_Interceptor()
    {
        using var connection = new MockDbConnection()
            .InterceptManipulateRow(args => new OptionalValue<long?>(3));

        Assert.Equal(3, await connection.DeleteByIdAsync<IdNameRow>(5, ExpectedRows.One, TestContext.Current.CancellationToken));
        Assert.Equal(0, connection.DbCommandExecuteNonQueryCallCount);
    }
}
