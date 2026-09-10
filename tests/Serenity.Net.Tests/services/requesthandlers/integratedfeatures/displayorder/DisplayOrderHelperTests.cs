namespace Serenity.Data;

public class DisplayOrderHelperTests
{
    [TableName("DisplayOrderTest")]
    private class TestRow : Row<TestRow.RowFields>, IIdRow, IDisplayOrderRow
    {
        [IdProperty]
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public int? Order { get => fields.Order[this]; set => fields.Order[this] = value; }

        public Int32Field DisplayOrderField => fields.Order;

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public Int32Field Order;
#pragma warning restore CS0649
        }
    }

    private static TestRow.RowFields Fields => new TestRow().GetFields();

    private static MockDbConnection ConnectionWith(params object[] rows)
    {
        return new MockDbConnection()
            .OnDbCommandExecuteReader(_ => rows.Length == 0 ? new MockDbDataReader() : new MockDbDataReader(rows))
            .OnDbCommandExecuteNonQuery(_ => 1);
    }

    [Fact]
    public void GetNextValue_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValue(null, "T", Fields.Order, null));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValue(new MockDbConnection(), null, Fields.Order, null));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValue(new MockDbConnection(), "", Fields.Order, null));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValue(new MockDbConnection(), "T", null, null));
    }

    [Fact]
    public void GetNextValue_Returns_Max_Plus_One()
    {
        using var connection = ConnectionWith(new { MAX = 5 });
        Assert.Equal(6, DisplayOrderHelper.GetNextValue(connection, "T", Fields.Order, null));
    }

    [Fact]
    public void GetNextValue_Returns_One_When_No_Rows()
    {
        using var connection = ConnectionWith();
        Assert.Equal(1, DisplayOrderHelper.GetNextValue(connection, "T", Fields.Order, null));
    }

    [Fact]
    public void GetNextValue_Returns_One_When_Null_Value()
    {
        using var connection = ConnectionWith(new { MAX = (int?)null });
        Assert.Equal(1, DisplayOrderHelper.GetNextValue(connection, "T", Fields.Order, null));
    }

    [Fact]
    public void GetNextValue_Row_Overload_Uses_Row()
    {
        using var connection = ConnectionWith(new { MAX = 2 });
        Assert.Equal(3, DisplayOrderHelper.GetNextValue(connection, new TestRow(), null));
    }

    [Fact]
    public async Task GetNextValueAsync_Throws_For_Nulls()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValueAsync(null, "T", Fields.Order, null, TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValueAsync(new MockDbConnection(), null, Fields.Order, null, TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValueAsync(new MockDbConnection(), "", Fields.Order, null, TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.GetNextValueAsync(new MockDbConnection(), "T", null, null, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetNextValueAsync_Returns_Max_Plus_One()
    {
        using var connection = ConnectionWith(new { MAX = 9 });
        Assert.Equal(10, await DisplayOrderHelper.GetNextValueAsync(connection, "T", Fields.Order, null, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetNextValueAsync_Returns_One_When_No_Rows()
    {
        using var connection = ConnectionWith();
        Assert.Equal(1, await DisplayOrderHelper.GetNextValueAsync(connection, "T", Fields.Order, null, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetNextValueAsync_Row_Overload_Uses_Row()
    {
        using var connection = ConnectionWith(new { MAX = 4 });
        Assert.Equal(5, await DisplayOrderHelper.GetNextValueAsync(connection, new TestRow(), null, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ReorderValues_Throws_For_Nulls()
    {
        var connection = new MockDbConnection();
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValues(null, "T", Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValues(connection, null, Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValues(connection, "", Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValues(connection, "T", null, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, null));
    }

    [Fact]
    public void ReorderValues_Moves_Record_Up()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 },
            new { ID = 3, Order = 3 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: 2, newDisplayOrder: 1);

        Assert.True(changed);
    }

    [Fact]
    public void ReorderValues_Moves_Record_Down_With_Clamping()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 },
            new { ID = 3, Order = 3 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: 1, newDisplayOrder: 100, descendingKeyOrder: true);

        Assert.True(changed);
    }

    [Fact]
    public void ReorderValues_Clamps_NonPositive_Order()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: null, newDisplayOrder: 0);

        Assert.False(changed);
    }

    [Fact]
    public void ReorderValues_With_String_Ids()
    {
        using var connection = ConnectionWith(
            new { ID = "a", Order = 1 },
            new { ID = "b", Order = 2 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: "b", newDisplayOrder: 1);

        Assert.True(changed);
    }

    [Fact]
    public void ReorderValues_With_Guid_Ids()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        using var connection = ConnectionWith(
            new { ID = id1, Order = 1 },
            new { ID = id2, Order = 2 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: id2, newDisplayOrder: 1);

        Assert.True(changed);
    }

    [Fact]
    public void ReorderValues_With_Unique_Constraint()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 });

        var changed = DisplayOrderHelper.ReorderValues(connection, "T", Fields.ID, Fields.Order,
            recordID: 1, newDisplayOrder: 2, hasUniqueConstraint: true);

        Assert.True(changed);
    }

    [Fact]
    public void ReorderValues_Row_Overload()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 });

        Assert.True(DisplayOrderHelper.ReorderValues(connection, new TestRow(),
            recordID: 2, newDisplayOrder: 1));
    }

    [Fact]
    public async Task ReorderValuesAsync_Throws_For_Nulls()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValuesAsync(null, "T", Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValuesAsync(new MockDbConnection(), null, Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValuesAsync(new MockDbConnection(), "T", null, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.ReorderValuesAsync(new MockDbConnection(), "T", Fields.ID, null, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ReorderValuesAsync_Moves_Record()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 },
            new { ID = 3, Order = 3 });

        var changed = await DisplayOrderHelper.ReorderValuesAsync(connection, "T", Fields.ID, Fields.Order,
            recordID: 3, newDisplayOrder: 1, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(changed);
    }

    [Fact]
    public async Task ReorderValuesAsync_Row_Overload()
    {
        using var connection = ConnectionWith(
            new { ID = 1, Order = 1 },
            new { ID = 2, Order = 2 });

        Assert.True(await DisplayOrderHelper.ReorderValuesAsync(connection, new TestRow(),
            recordID: 2, newDisplayOrder: 1, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public void UpdateOrders_Throws_For_Nulls()
    {
        var records = new List<DisplayOrderHelper.OrderRecord>();
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrders(null, records, "T", Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrders(new MockDbConnection(), records, null, Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrders(new MockDbConnection(), records, "", Fields.ID, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrders(new MockDbConnection(), records, "T", null, Fields.Order));
        Assert.Throws<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrders(new MockDbConnection(), records, "T", Fields.ID, null));
    }

    [Fact]
    public void UpdateOrders_Returns_False_For_Empty_List()
    {
        using var connection = ConnectionWith();
        Assert.False(DisplayOrderHelper.UpdateOrders(connection, [], "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public void UpdateOrders_Returns_False_When_No_Change()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 1, newOrder = 1 }
        };
        Assert.False(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public void UpdateOrders_Batches_Same_Difference()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 1, newOrder = 2 },
            new() { recordID = 2, oldOrder = 2, newOrder = 3 }
        };

        Assert.True(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public void UpdateOrders_Single_Update()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 5, newOrder = 1 }
        };

        Assert.True(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public void UpdateOrders_Unique_Constraint_Handles_Congestion()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 1, newOrder = 2 },
            new() { recordID = 2, oldOrder = 2, newOrder = 1 }
        };

        Assert.True(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order, true));
    }

    [Fact]
    public void UpdateOrders_With_ExecuteBlock_Dialect()
    {
        using var connection = new MockDbConnection { Dialect = FirebirdDialect.Instance };
        connection.OnDbCommandExecuteNonQuery(_ => 1);

        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 5, newOrder = 1 }
        };

        Assert.True(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public void UpdateOrders_Limits_Batch_To_100_Records()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>();
        for (int i = 1; i <= 105; i++)
            records.Add(new DisplayOrderHelper.OrderRecord { recordID = i, oldOrder = i, newOrder = i + 1 });

        Assert.True(DisplayOrderHelper.UpdateOrders(connection, records, "T", Fields.ID, Fields.Order));
    }

    [Fact]
    public async Task UpdateOrdersAsync_Throws_For_Nulls()
    {
        var records = new List<DisplayOrderHelper.OrderRecord>();
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrdersAsync(null!, records, "T", Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrdersAsync(new MockDbConnection(), records, null!, Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrdersAsync(new MockDbConnection(), records, "", Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrdersAsync(new MockDbConnection(), records, "T", null!, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            DisplayOrderHelper.UpdateOrdersAsync(new MockDbConnection(), records, "T", Fields.ID, null!, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateOrdersAsync_Returns_False_When_No_Change()
    {
        using var connection = ConnectionWith();
        Assert.False(await DisplayOrderHelper.UpdateOrdersAsync(connection, [], "T", Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task UpdateOrdersAsync_Updates_Records()
    {
        using var connection = ConnectionWith();
        var records = new List<DisplayOrderHelper.OrderRecord>
        {
            new() { recordID = 1, oldOrder = 5, newOrder = 1 }
        };

        Assert.True(await DisplayOrderHelper.UpdateOrdersAsync(connection, records, "T", Fields.ID, Fields.Order, cancellationToken: TestContext.Current.CancellationToken));
    }
}
