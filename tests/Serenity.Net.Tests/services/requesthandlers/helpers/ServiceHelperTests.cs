namespace Serenity.Services;

public class ServiceHelperTests
{
    [TableName("ServiceHelperTest")]
    private class TestRow : Row<TestRow.RowFields>, IIdRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? Number { get => fields.Number[this]; set => fields.Number[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public Int32Field Number;
#pragma warning restore CS0649
        }
    }

    private static TestRow.RowFields Fields => new TestRow().GetFields();

    [Fact]
    public void SetSkipTakeTotal_Sets_TotalCount_When_Take_Is_Zero()
    {
        var response = new ListResponse<TestRow> { Entities = { new TestRow() } };
        var query = new SqlQuery().Skip(2).Take(0);

        response.SetSkipTakeTotal(query);

        Assert.Equal(2, response.Skip);
        Assert.Equal(0, response.Take);
        Assert.Equal(3, response.TotalCount);
    }

    [Fact]
    public void SetSkipTakeTotal_Does_Not_Set_TotalCount_When_Take_Is_Set()
    {
        var response = new ListResponse<TestRow> { Entities = { new TestRow() } };
        var query = new SqlQuery().Skip(2).Take(5);

        response.SetSkipTakeTotal(query);

        Assert.Equal(2, response.Skip);
        Assert.Equal(5, response.Take);
        Assert.Equal(0, response.TotalCount);
    }

    [Fact]
    public void CheckParentNotDeleted_Throws_When_Record_Exists()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { X = 1 }));

        Assert.Throws<ValidationError>(() =>
            ServiceHelper.CheckParentNotDeleted(connection, "T", q => q.Where("1 = 1"), new MockTextLocalizer()));
    }

    [Fact]
    public void CheckParentNotDeleted_Passes_When_No_Record()
    {
        var filterCalled = false;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        ServiceHelper.CheckParentNotDeleted(connection, "T", q => filterCalled = true, new MockTextLocalizer());

        Assert.True(filterCalled);
    }

    [Fact]
    public async Task CheckParentNotDeletedAsync_Throws_When_Record_Exists()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { X = 1 }));

        await Assert.ThrowsAsync<ValidationError>(() =>
            ServiceHelper.CheckParentNotDeletedAsync(connection, "T", q => { }, new MockTextLocalizer(),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CheckParentNotDeletedAsync_Passes_When_No_Record()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        await ServiceHelper.CheckParentNotDeletedAsync(connection, "T", q => { }, new MockTextLocalizer(),
            TestContext.Current.CancellationToken);
    }

    [Fact]
    public void IsUniqueIndexException_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceHelper.IsUniqueIndexException(null, new Exception("x"), null, null, new TestRow(), Fields.Number));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceHelper.IsUniqueIndexException(new MockDbConnection(), null, null, null, new TestRow(), Fields.Number));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceHelper.IsUniqueIndexException(new MockDbConnection(), new Exception("x"), null, null, new TestRow()));
    }

    [Fact]
    public void IsUniqueIndexException_Returns_False_When_IndexName_Not_In_Message()
    {
        using var connection = new MockDbConnection();
        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("other"),
            "UQ_Test", null, new TestRow { Number = 1 }, Fields.Number);
        Assert.False(result);
    }

    [Fact]
    public void IsUniqueIndexException_Returns_False_When_Old_Row_Not_Different()
    {
        using var connection = new MockDbConnection();
        var oldRow = new TestRow { Number = 1 };
        var newRow = new TestRow { Number = 1 };

        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("x"),
            null, oldRow, newRow, Fields.Number);

        Assert.False(result);
    }

    [Fact]
    public void IsUniqueIndexException_Returns_False_When_No_Matching_Row()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("UQ_Test"),
            "UQ_Test", null, new TestRow { Number = 1 }, Fields.Number);

        Assert.False(result);
    }

    [Fact]
    public void IsUniqueIndexException_Returns_True_When_Different_Id_Found()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 99 }));

        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("UQ_Test"),
            "UQ_Test", null, new TestRow { ID = 5, Number = 1 }, Fields.Number);

        Assert.True(result);
    }

    [Fact]
    public void IsUniqueIndexException_Returns_False_When_Same_Id_Found()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 5 }));

        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("UQ_Test"),
            "UQ_Test", null, new TestRow { ID = 5, Number = 1 }, Fields.Number);

        Assert.False(result);
    }

    [Fact]
    public void IsUniqueIndexException_Checks_Old_Row_Difference()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { ID = 99 }));

        var oldRow = new TestRow { ID = 5, Number = 1 };
        var newRow = new TestRow { ID = 5, Number = 2 };

        var result = ServiceHelper.IsUniqueIndexException(connection, new Exception("UQ_Test"),
            "UQ_Test", oldRow, newRow, Fields.Number);

        Assert.True(result);
    }
}
