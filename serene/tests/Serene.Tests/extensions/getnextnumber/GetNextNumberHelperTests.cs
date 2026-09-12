namespace Serenity.Extensions;

public class GetNextNumberHelperTests
{
    private class TestRow : Row<TestRow.RowFields>
    {
        public string? Serial { get => fields.Serial[this]; set => fields.Serial[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public StringField Serial = null!;
        }
    }

    [Fact]
    public void GetNextNumber_Returns_First_Number_When_No_Max()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader());

        var response = GetNextNumberHelper.GetNextNumber(connection,
            new GetNextNumberRequest { Prefix = "AB", Length = 6 }, TestRow.Fields.Serial);

        Assert.Equal(1, response.Number);
        Assert.Equal("AB0001", response.Serial);
    }

    [Fact]
    public void GetNextNumber_Increments_Max_Number()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Value = "AB0005" }));

        var response = GetNextNumberHelper.GetNextNumber(connection,
            new GetNextNumberRequest { Prefix = "AB", Length = 6 }, TestRow.Fields.Serial);

        Assert.Equal(6, response.Number);
        Assert.Equal("AB0006", response.Serial);
    }

    [Fact]
    public void GetNextNumber_Returns_First_Number_When_Max_Is_Not_Parsable()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Value = "ABxx" }));

        var response = GetNextNumberHelper.GetNextNumber(connection,
            new GetNextNumberRequest { Prefix = "AB", Length = 6 }, TestRow.Fields.Serial);

        Assert.Equal(1, response.Number);
        Assert.Equal("AB0001", response.Serial);
    }

    [Fact]
    public void GetNextNumber_Uses_Empty_Prefix_When_Null()
    {
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { Value = "0007" }));

        var response = GetNextNumberHelper.GetNextNumber(connection,
            new GetNextNumberRequest { Prefix = null, Length = 4 }, TestRow.Fields.Serial);

        Assert.Equal(8, response.Number);
        Assert.Equal("0008", response.Serial);
    }

    [Fact]
    public void GetNextNumberRequest_And_Response_Properties_Roundtrip()
    {
        var request = new GetNextNumberRequest { Prefix = "X", Length = 5 };
        Assert.Equal("X", request.Prefix);
        Assert.Equal(5, request.Length);

        var response = new GetNextNumberResponse { Number = 3, Serial = "S" };
        Assert.Equal(3, response.Number);
        Assert.Equal("S", response.Serial);
    }
}
