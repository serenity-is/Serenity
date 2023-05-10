using Serenity.Tests.Entities;

namespace Serenity.Tests.Entity;

public class EntityConnectionExtensions_ById_Tests
{
    [Fact]
    public void Throws_Exception_If_Record_Not_Found()
    {
        using var connection = new MockDbConnection()
            .OnExecuteReader(command =>
            {
                return new MockDbDataReader();
            });

        Assert.Throws<ValidationError>(() => connection.ById<IdNameRow>(777));
    }

    [Fact]
    public void Returned_Row_Has_Track_With_Checks_True()
    {
        using var connection = new MockDbConnection()
            .OnExecuteReader(command =>
            {
                return new MockDbDataReader(new
                {
                    ID = 777,
                    Name = "Test"
                });
            });

        var row = connection.ById<IdNameRow>(777);
        Assert.True(((IRow)row).TrackWithChecks);
    }

    [Fact]
    public void Passes_The_RecordId_To_WhereStatement()
    {
        using var connection = new MockDbConnection()
            .OnExecuteReader(command =>
            {
                Assert.Collection(command.Parameters.OfType<IDbDataParameter>(),
                    x1 => Assert.Equal(777, x1.Value));

                Assert.Equal(@"SELECT 
T0.ID AS [ID],
T0.Name AS [Name] 
FROM IdName T0 
WHERE (T0.ID = @p1)".NormalizeSql(), command.CommandText.NormalizeSql());

                return new MockDbDataReader(new
                {
                    ID = 777,
                    Name = "Test"
                });
            });
                
        connection.ById<IdNameRow>(777);
    }

    [Fact]
    public void Only_Loads_The_Table_Fields_By_Default()
    {
        using var connection = new MockDbConnection()
            .OnExecuteReader(command =>
            {
                Assert.Equal(@"SELECT 
T0.CityId AS [CityId],
T0.CityName AS [CityName],
T0.CountryId AS [CountryId]
FROM Cities T0 
WHERE (T0.CityId = @p1)".NormalizeSql(), command.CommandText.NormalizeSql());

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
            .OnExecuteReader(command =>
            {
                Assert.Equal(@"SELECT 
jCountry.CountryName AS [CountryName]
FROM Cities T0 
LEFT JOIN Countries jCountry ON (jCountry.CountryId = T0.CountryId)
WHERE (T0.CityId = @p1)".NormalizeSql(), command.CommandText.NormalizeSql());

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
            .OnExecuteReader(command =>
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
            .OnExecuteReader(command =>
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