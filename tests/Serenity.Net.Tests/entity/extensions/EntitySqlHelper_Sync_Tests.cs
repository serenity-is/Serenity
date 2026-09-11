namespace Serenity.Data;

public class EntitySqlHelper_Sync_Tests
{
    private static SqlQuery CityQuery(CityRow row) => new SqlQuery().From(row).SelectTableFields();

    [Fact]
    public void GetFirst_Loads_Row()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }));

        Assert.True(CityQuery(row).GetFirst(connection));
        Assert.Equal(1, row.CityId);
        Assert.Equal("A", row.CityName);
    }

    [Fact]
    public void GetFirst_Returns_False_When_No_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.False(CityQuery(row).GetFirst(connection));
    }

    [Fact]
    public void GetSingle_Loads_Row()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }));

        Assert.True(CityQuery(row).GetSingle(connection));
        Assert.Equal("A", row.CityName);
    }

    [Fact]
    public void GetSingle_Returns_False_When_No_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(_ => new MockDbDataReader());

        Assert.False(CityQuery(row).GetSingle(connection));
    }

    [Fact]
    public void GetSingle_Throws_When_Multiple_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 },
                new { CityId = 2, CityName = "B", CountryId = 3 }));

        Assert.Throws<InvalidOperationException>(() => CityQuery(row).GetSingle(connection));
    }

    [Fact]
    public async Task GetFirstAsync_And_GetSingleAsync_Load_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }));
        var token = TestContext.Current.CancellationToken;

        Assert.True(await CityQuery(row).GetFirstAsync(connection, token));
        Assert.True(await CityQuery(row).GetSingleAsync(connection, token));
    }

    [Fact]
    public void ForEach_Invokes_Callback_Per_Row()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 },
                new { CityId = 2, CityName = "B", CountryId = 3 }));

        var seen = new List<string>();
        var count = CityQuery(row).ForEach(connection, () => seen.Add(row.CityName));

        Assert.Equal(0, count);
        Assert.Equal(["A", "B"], seen);
    }

    [Fact]
    public void ForEach_NonMultipleResultsets_Without_Count()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection { Dialect = MySqlDialect.Instance }
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }));

        var seen = new List<string>();
        var count = CityQuery(row).ForEach(connection, () => seen.Add(row.CityName));

        Assert.Equal(0, count);
        Assert.Equal(["A"], seen);
    }

    [Fact]
    public void ForEach_NonMultipleResultsets_With_Count()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection { Dialect = MySqlDialect.Instance }
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }))
            .InterceptExecuteScalar(_ => 42);

        var query = CityQuery(row).Dialect(MySqlDialect.Instance);
        query.CountRecords = true;

        var seen = new List<string>();
        var count = query.ForEach(connection, () => seen.Add(row.CityName));

        Assert.Equal(42, count);
        Assert.Equal(["A"], seen);
    }

    [Fact]
    public void List_Returns_Cloned_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 },
                new { CityId = 2, CityName = "B", CountryId = 3 }));

        var list = EntitySqlHelper.List<CityRow>(CityQuery(row), connection);

        Assert.Equal(2, list.Count);
        Assert.Equal("A", list[0].CityName);
        Assert.NotSame(row, list[0]);
    }

    [Fact]
    public void List_Throws_When_No_LoaderRow()
    {
        using var connection = new MockDbConnection();
        Assert.Throws<ArgumentNullException>(() => EntitySqlHelper.List<CityRow>(new SqlQuery(), connection));
    }

    [Fact]
    public void GetFromReader_Wraps_Conversion_Error()
    {
        var row = new CityRow();
        using var reader = new MockDbDataReader(new { CityId = "not-an-int", CityName = "A", CountryId = 2 });

        var ex = Assert.Throws<InvalidOperationException>(() => CityQuery(row).GetFromReader(reader));
        Assert.Contains("CityId", ex.Message);
    }

    [Fact]
    public async Task ListAsync_Returns_Cloned_Rows()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection()
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }));

        var list = await CityQuery(row).ListAsync<CityRow>(connection, null,
            TestContext.Current.CancellationToken);

        Assert.Single(list);
    }

    [Fact]
    public async Task ForEachAsync_NonMultipleResultsets_With_Count()
    {
        var row = new CityRow();
        using var connection = new MockDbConnection { Dialect = MySqlDialect.Instance }
            .InterceptExecuteReader(args => args.ToMockReader(
                new { CityId = 1, CityName = "A", CountryId = 2 }))
            .InterceptExecuteScalar(_ => 42);

        var query = CityQuery(row).Dialect(MySqlDialect.Instance);
        query.CountRecords = true;

        var seen = new List<string>();
        var count = await query.ForEachAsync(connection, () => seen.Add(row.CityName),
            TestContext.Current.CancellationToken);

        Assert.Equal(42, count);
    }

    [Fact]
    public void GetFromReader_Skips_Invalid_Into_Entries()
    {
        var row = new CityRow();
        var query = CityQuery(row);
        using var reader = new MockDbDataReader(new { CityId = 1 });

        query.GetFromReader(reader, []);
        query.GetFromReader(reader, [new()]);
    }

    [Fact]
    public void GetFromReader_Uses_DictionaryData_For_Unmatched_Columns()
    {
        var city = new CityRow();
        var other = new IdNameRow();
        using var reader = new MockDbDataReader(
            new { CityId = 1, CityName = (string?)null, CountryId = 2 });
        Assert.True(reader.Read());

        CityQuery(city).GetFromReader(reader, [other]);

        Assert.Equal(1, ((IRow)other).GetDictionaryData("CityId"));
        Assert.Null(((IRow)other).GetDictionaryData("CityName"));
    }

    [Fact]
    public void GetFromReader_Wraps_Named_Field_Conversion_Error()
    {
        var city = new CityRow();
        var other = new OtherCityRow();
        using var reader = new MockDbDataReader(
            new { CityId = "not-an-int", CityName = "A", CountryId = 2 });
        Assert.True(reader.Read());

        var ex = Assert.Throws<InvalidOperationException>(() =>
            CityQuery(city).GetFromReader(reader, [other]));
        Assert.Contains("CityId", ex.Message);
    }
}

[TableName("OtherCities")]
public class OtherCityRow : Row<OtherCityRow.RowFields>, IIdRow
{
    [IdProperty]
    public int? CityId { get => fields.CityId[this]; set => fields.CityId[this] = value; }

    public class RowFields : RowFieldsBase
    {
#pragma warning disable CS0649
        public Int32Field CityId;
#pragma warning restore CS0649
    }
}
