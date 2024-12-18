namespace Serenity.Data;

public class EntityConnectionExtensions_ById_Tests
{
    [Fact]
    public void Throws_Exception_If_Record_Not_Found()
    {
        using var connection = new MockDbConnection().InterceptFindRow(args => null);
        Assert.Throws<ValidationError>(() => connection.ById<IdNameRow>(777));
    }

    [Fact]
    public void Returned_Row_Has_Track_With_Checks_True()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => new MockDbDataReader(new
            {
                ID = 777,
                Name = "Test"
            }));

        var row = connection.ById<IdNameRow>(777);
        Assert.True(((IRow)row).TrackWithChecks);
    }

    [Fact]
    public void Passes_The_RecordId_To_WhereStatement()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader((InterceptExecuteReaderArgs args) => new MockDbDataReader(new
            {
                ID = 777,
                Name = "Test"
            }));

        connection.ById<IdNameRow>(777);
        var call = Assert.Single(connection.ExecuteReaderCalls);
        Assert.Equal(@"SELECT 
T0.[ID] AS [ID],
T0.[Name] AS [Name] 
FROM [IdName] T0 
WHERE (T0.[ID] = @p1)".NormalizeSql(), call.CommandText.NormalizeSql());
        var param = Assert.Single(call.Parameters);
        Assert.Equal(777, param.Value);
    }

    [Fact]
    public void Only_Loads_The_Table_Fields_By_Default()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull())
                    .Columns.Select(x => x.ColumnName);
                Assert.Equal(columns, CityRow.Fields
                    .Where(x => x.IsTableField()).Select(x => x.Name));
                return new MockDbDataReader(new
                {
                    CityId = 777,
                    CityName = "Test",
                    CountryId = 999
                });
            });

        var row = connection.ById<CityRow>(777);
        Assert.Equal(777, row.CityId);
        Assert.Equal("Test", row.CityName);
        Assert.Equal(999, row.CountryId);
        Assert.Throws<InvalidOperationException>(() => row.CountryName);
        Assert.Throws<InvalidOperationException>(() => row.FullName);
    }

    [Fact]
    public void Only_Loads_The_Fields_Selected_By_Query_Callback()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                var columns = ((ISqlQueryExtensible)args.Query.AssertNotNull())
                                    .Columns.Select(x => x.ColumnName);
                Assert.Equal(columns, [CityRow.Fields.CountryName.Name]);
                return new MockDbDataReader(new
                {
                    CountryName = "TestCountry"
                });
            });

        var row = connection.ById<CityRow>(777, query => query
            .Select(CityRow.Fields.CountryName));
        Assert.Equal("TestCountry", row.CountryName);
        Assert.Throws<InvalidOperationException>(() => row.CityId);
        Assert.Throws<InvalidOperationException>(() => row.CityName);
        Assert.Throws<InvalidOperationException>(() => row.CountryId);
        Assert.Throws<InvalidOperationException>(() => row.FullName);
    }

    [Fact]
    public void Exception_Thrown_If_Multiple_Results_Returned()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                return new MockDbDataReader(new
                {
                    CityId = 777,
                    CityName = "Test City",
                    CountryId = 999
                },                    
                new
                {
                    CityId = 777,
                    CityName = "Another City",
                    CountryId = 333
                });
            });

        Assert.Throws<InvalidOperationException>(() => connection.ById<CityRow>(777));
    }

    [Fact]
    public void Exception_Thrown_If_Multiple_Results_Returned_With_Edit_Query()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                return new MockDbDataReader(new
                {
                    CityId = 777,
                    CityName = "Test City",
                    CountryId = 999
                },
                new
                {
                    CityId = 777,
                    CityName = "Another City",
                    CountryId = 333
                });
            });

        Assert.Throws<InvalidOperationException>(() => 
            connection.ById<CityRow>(777, query => query.Select(CityRow.Fields.CityName)));
    }
}