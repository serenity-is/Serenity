namespace Serenity.Data;

public class EntitySqlHelper_Async_Tests
{
    [Fact]
    public async Task ForEachAsync_Invocates_Callback_Once_Per_Row()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CityName]", args.CommandText);
                return new MockDbDataReader(
                    new { CityId = 101, CityName = "Amsterdam", CountryId = 111 },
                    new { CityId = 222, CityName = "Brussels", CountryId = 232 });
            });

        var row = new CityRow();
        var query = new SqlQuery().From(row)
            .SelectTableFields();

        var seen = new List<string>();
        var count = await query.ForEachAsync(connection, () => seen.Add(row.CityName),
            TestContext.Current.CancellationToken);

        Assert.Equal(0, count);
        Assert.Equal(["Amsterdam", "Brussels"], seen);
    }

    [Fact]
    public async Task ForEachAsync_Passes_Reader_To_Callback()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args =>
            {
                Assert.Contains("[CityName]", args.CommandText);
                return new MockDbDataReader(
                    new { CityId = 252, CityName = "Zurich", CountryId = 262 },
                    new { CityId = 272, CityName = "Geneva", CountryId = 282 });
            });

        var row = new CityRow();
        var query = new SqlQuery().From(row)
            .SelectTableFields();

        var citiesViaReader = new List<string>();
        var count = await query.ForEachAsync(connection, reader =>
        {
            citiesViaReader.Add(reader.IsDBNull(1) ? "" : reader.GetString(1));
        }, TestContext.Current.CancellationToken);

        Assert.Equal(0, count);
        Assert.Equal(["Zurich", "Geneva"], citiesViaReader);
    }

    [Fact]
    public async Task ForEachAsync_Handles_No_Rows()
    {
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        var row = new CityRow();
        var query = new SqlQuery().From(row)
            .SelectTableFields();

        var seen = new List<string>();
        var count = await query.ForEachAsync(connection, () => seen.Add(row.CityName),
            TestContext.Current.CancellationToken);

        Assert.Equal(0, count);
        Assert.Empty(seen);
    }
}
