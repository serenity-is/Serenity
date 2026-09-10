namespace Serenity.TestUtils;

public class MockDbDataReaderTests
{
    [Fact]
    public void Maps_Anonymous_Items_By_Column_Name_Ignoring_Order()
    {
        var reader = new MockDbDataReader(["Name", "ID"],
            [new { ID = 5, Name = "A" }]);

        Assert.True(reader.Read());
        Assert.Equal("A", reader.GetString(0));
        Assert.Equal(5, reader.GetInt32(1));
        Assert.Equal("Name", reader.GetName(0));
        Assert.Equal("ID", reader.GetName(1));
    }

    [Fact]
    public void Reads_Missing_Property_As_DBNull()
    {
        var reader = new MockDbDataReader(["ID", "Name"],
            [new { ID = 5 }]);

        Assert.True(reader.Read());
        Assert.False(reader.IsDBNull(0));
        Assert.True(reader.IsDBNull(1));
    }

    [Fact]
    public void Maps_Dictionary_Items_By_Key_Ignoring_Order()
    {
        var items = new List<IDictionary<string, object?>>
        {
            new Dictionary<string, object?> { ["Name"] = "A", ["ID"] = 5 }
        };

        var reader = new MockDbDataReader(["ID", "Name"], items);

        Assert.True(reader.Read());
        Assert.Equal(5, reader.GetInt32(0));
        Assert.Equal("A", reader.GetString(1));
    }

    [Fact]
    public void ToMockReader_Aligns_To_Query_Columns()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(new { Name = "A" }))
            .InterceptExecuteNonQuery(_ => 1);

        var handler = new ListRequestHandler<IdNameRow>(
            new NullRequestContext().WithPermissions(_ => true));

        var response = handler.List(connection, new ListRequest());

        var row = Assert.Single(response.Entities);
        Assert.Equal("A", row.Name);
        Assert.Null(row.ID);
    }

    [Fact]
    public void ToMockReader_Supports_Dictionaries()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new Dictionary<string, object?> { ["Name"] = "B" }))
            .InterceptExecuteNonQuery(_ => 1);

        var handler = new ListRequestHandler<IdNameRow>(
            new NullRequestContext().WithPermissions(_ => true));

        var response = handler.List(connection, new ListRequest());

        Assert.Equal("B", Assert.Single(response.Entities).Name);
    }
}
