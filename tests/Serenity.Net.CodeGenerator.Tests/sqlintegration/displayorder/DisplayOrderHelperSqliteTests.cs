using System.Data.Common;

namespace Serenity.Data;

public class DisplayOrderHelperSqliteTests
{
    [TableName("DispOrderIT")]
    public class OrderRow : Row<OrderRow.RowFields>, IIdRow, IDisplayOrderRow
    {
        [IdProperty]
        public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

        public int? Order { get => fields.Order[this]; set => fields.Order[this] = value; }

        public Int32Field DisplayOrderField => fields.Order;

        public class RowFields : RowFieldsBase
        {
            public Int32Field Id = null!;
            public Int32Field Order = null!;
        }
    }

    private static OrderRow.RowFields Fields => new OrderRow().GetFields();

    private static WrappedConnection OpenConnection()
    {
        var connection = SqlIntegrationConnections.CreateConnection("Sqlite");
        connection.Open();
        return new WrappedConnection(connection, SqliteDialect.Instance);
    }

    private static void CreateTable(DbConnection connection, string table, bool uniqueOrder = false)
    {
        SqlIntegrationConnections.ExecuteSql(connection,
            $"CREATE TABLE [{table}] ([Id] INTEGER PRIMARY KEY, [Order] INTEGER NOT NULL);");

        if (uniqueOrder)
            SqlIntegrationConnections.ExecuteSql(connection, $"CREATE UNIQUE INDEX [UQ_{table}] ON [{table}] ([Order]);");
    }

    private static void InsertOrders(DbConnection connection, string table, params (int Id, int Order)[] rows)
    {
        foreach (var (id, order) in rows)
            SqlIntegrationConnections.ExecuteSql(connection, $"INSERT INTO [{table}] ([Id], [Order]) VALUES ({id}, {order});");
    }

    private static List<(int Id, int Order)> ReadOrders(DbConnection connection, string table)
    {
        var result = new List<(int, int)>();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT [Id], [Order] FROM [{table}] ORDER BY [Order], [Id]";
        using var reader = command.ExecuteReader();
        while (reader.Read())
            result.Add((Convert.ToInt32(reader.GetValue(0)), Convert.ToInt32(reader.GetValue(1))));
        return result;
    }

    private static string NewTable(string suffix) => "DispOrder_" + suffix + "_" + Guid.NewGuid().ToString("N");

    [Fact]
    public void GetNextValue_Returns_Max_Plus_One()
    {
        using var connection = OpenConnection();
        var table = NewTable("Next");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 5), (2, 9));

        Assert.Equal(10, DisplayOrderHelper.GetNextValue(connection, table, Fields.Order, null));
    }

    [Fact]
    public void ReorderValues_Normalizes_Values_To_Consecutive_By_Default()
    {
        using var connection = OpenConnection();
        var table = NewTable("Norm");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 10), (2, 20), (3, 30), (4, 40));

        var changed = DisplayOrderHelper.ReorderValues(connection, table, Fields.Id, Fields.Order,
            recordID: 2, newDisplayOrder: 4);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 1, 3, 4, 2 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 1, 2, 3, 4 }, rows.Select(x => x.Order));
    }

    [Fact]
    public void ReorderValues_PreserveGaps_Keeps_Existing_Values()
    {
        using var connection = OpenConnection();
        var table = NewTable("Gaps");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 10), (2, 20), (3, 30), (4, 40));

        var changed = DisplayOrderHelper.ReorderValues(connection, table, Fields.Id, Fields.Order,
            recordID: 2, newDisplayOrder: 1, preserveGaps: true);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 2, 1, 3, 4 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 10, 20, 30, 40 }, rows.Select(x => x.Order));
    }

    [Fact]
    public void ReorderValues_With_Sentinel_Id_Closes_Gaps()
    {
        using var connection = OpenConnection();
        var table = NewTable("Close");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 1), (2, 3), (3, 4));

        var changed = DisplayOrderHelper.ReorderValues(connection, table, Fields.Id, Fields.Order,
            recordID: -1, newDisplayOrder: 1);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 1, 2, 3 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 1, 2, 3 }, rows.Select(x => x.Order));
    }

    [Fact]
    public void ReorderValues_Normalizes_Duplicate_Values()
    {
        using var connection = OpenConnection();
        var table = NewTable("Dup");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 0), (2, 0), (3, 0));

        var changed = DisplayOrderHelper.ReorderValues(connection, table, Fields.Id, Fields.Order,
            recordID: 2, newDisplayOrder: 2);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 1, 2, 3 }, rows.Select(x => x.Order));
    }

    [Fact]
    public void ReorderValues_With_Unique_Constraint_Does_Not_Violate_It()
    {
        using var connection = OpenConnection();
        var table = NewTable("Unique");
        CreateTable(connection, table, uniqueOrder: true);
        InsertOrders(connection, table, (1, 1), (2, 2), (3, 3));

        var changed = DisplayOrderHelper.ReorderValues(connection, table, Fields.Id, Fields.Order,
            recordID: 1, newDisplayOrder: 3, hasUniqueConstraint: true);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 2, 3, 1 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 1, 2, 3 }, rows.Select(x => x.Order));
    }

    [Fact]
    public async Task ReorderValuesAsync_Normalizes_Values_To_Consecutive()
    {
        using var connection = OpenConnection();
        var table = NewTable("Async");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 10), (2, 20), (3, 30));

        var changed = await DisplayOrderHelper.ReorderValuesAsync(connection, table, Fields.Id, Fields.Order,
            recordID: 3, newDisplayOrder: 1, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 3, 1, 2 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 1, 2, 3 }, rows.Select(x => x.Order));
    }

    [Fact]
    public async Task ReorderValuesAsync_PreserveGaps_Keeps_Existing_Values()
    {
        using var connection = OpenConnection();
        var table = NewTable("AsyncGaps");
        CreateTable(connection, table);
        InsertOrders(connection, table, (1, 5), (2, 15), (3, 25));

        var changed = await DisplayOrderHelper.ReorderValuesAsync(connection, table, Fields.Id, Fields.Order,
            recordID: 3, newDisplayOrder: 1, preserveGaps: true,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(changed);
        var rows = ReadOrders(connection, table);
        Assert.Equal(new[] { 3, 1, 2 }, rows.Select(x => x.Id));
        Assert.Equal(new[] { 5, 15, 25 }, rows.Select(x => x.Order));
    }
}

