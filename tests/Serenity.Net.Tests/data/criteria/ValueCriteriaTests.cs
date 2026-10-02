using System.Globalization;

namespace Serenity.Data;

public class ValueCriteriaTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        Assert.Equal(5, new ValueCriteria(5).Value);
    }

    [Fact]
    public void Constructor_NullValue_SetsNull()
    {
        Assert.Null(new ValueCriteria(null).Value);
    }

    [Fact]
    public void ToString_StringValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria("test");

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Equal("test", query.Params["@p1"]);
    }

    [Fact]
    public void ToString_IntValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(5);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Equal(5, query.Params["@p1"]);
    }

    [Fact]
    public void ToString_DateTimeValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var value = new DateTime(2023, 1, 15, 10, 30, 0);
        var criteria = new ValueCriteria(value);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Equal(value, query.Params["@p1"]);
    }

    [Fact]
    public void ToString_NullValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(null);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Null(query.Params["@p1"]);
    }

    [Fact]
    public void ToString_ParamNamesIncrement()
    {
        var query = new SqlQuery();

        Assert.Equal("@p1", new ValueCriteria("a").ToString(query));
        Assert.Equal("@p2", new ValueCriteria("b").ToString(query));
    }

    [Fact]
    public void ToString_ShortCollection_UsesParams()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new object[] { "a", "b" });

        Assert.Equal("(@p1,@p2)", criteria.ToString(query));
        Assert.Equal("a", query.Params["@p1"]);
        Assert.Equal("b", query.Params["@p2"]);
    }

    [Fact]
    public void ToString_ManyIntegers_StayParameterized_While_Budget_Allows()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 15).ToArray());

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
            criteria.ToString(query));
        Assert.Equal(15, query.Params.Count);
    }

    [Fact]
    public void ToString_ManyEnums_StayParameterized_While_Budget_Allows()
    {
        var query = new SqlQuery();
        var values = Enumerable.Range(0, 15).Select(i => (SampleEnum)(i % 3)).ToArray();
        var criteria = new ValueCriteria(values);

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
            criteria.ToString(query));
        Assert.Equal(15, query.Params.Count);
    }

    [Fact]
    public void ToString_ManyStrings_StayParameterized_While_Budget_Allows()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 15).Select(i => "s" + i).ToArray());

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
            criteria.ToString(query));
        Assert.Equal(15, query.Params.Count);
    }

    [Theory]
    [InlineData(typeof(PostgresDialect))]
    [InlineData(typeof(MySqlDialect))]
    [InlineData(typeof(OracleDialect))]
    [InlineData(typeof(FirebirdDialect))]
    public void ToString_ManyStrings_StayParameterized_Where_Budget_Allows(Type dialectType)
    {
        // 65k+ budgets (or Oracle's expression cap, which inlining can't avoid):
        // keep reusable parameterized plans.
        var query = new SqlQuery().Dialect((ISqlDialect)Activator.CreateInstance(dialectType)!);
        var criteria = new ValueCriteria(Enumerable.Range(1, 15).Select(i => "s" + i).ToArray());

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
            criteria.ToString(query));
        Assert.Equal(15, query.Params.Count);
    }

    [Fact]
    public void ToString_ManyStrings_WithQuote_AreEscaped_On_SqlServer()
    {
        var query = new SqlQuery();
        AddNearlyExhaustedSqlServerBudget(query);
        var values = Enumerable.Range(1, 15).Select(i => "o'clock" + i).ToArray();

        var sql = new ValueCriteria(values).ToString(query);

        Assert.StartsWith("(N'o''clock1',", sql);
        Assert.Equal(1590, query.Params.Count);
    }

    [Theory]
    [InlineData(typeof(SqlServer2012Dialect), true)]
    [InlineData(typeof(SqliteDialect), true)]
    [InlineData(typeof(PostgresDialect), false)]
    [InlineData(typeof(MySqlDialect), false)]
    [InlineData(typeof(OracleDialect), false)]
    [InlineData(typeof(FirebirdDialect), false)]
    public void ToString_ManyGuids_Inline_Only_With_Standard_Literal(Type dialectType, bool inlined)
    {
        var guids = Enumerable.Range(1, 15)
            .Select(i => new Guid(i, 0, 0, new byte[8]))
            .ToArray();
        var query = new SqlQuery().Dialect((ISqlDialect)Activator.CreateInstance(dialectType)!);
        var expectedExistingCount = 0;
        if (inlined)
        {
            expectedExistingCount = dialectType == typeof(SqliteDialect) ? 32252 : 1590;
            for (int i = 0; i < expectedExistingCount; i++)
                query.AddParam("@existing" + i, i);
        }

        var sql = new ValueCriteria(guids).ToString(query);

        if (inlined)
        {
            Assert.StartsWith("('" + guids[0].ToString("D") + "'", sql);
            Assert.Equal(expectedExistingCount, query.Params.Count);
        }
        else
        {
            Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)", sql);
            Assert.Equal(15, query.Params.Count);
        }
    }

    [Fact]
    public void ToString_ShortIntCollection_UsesParams()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new[] { 1, 2, 3, 4, 5 });

        // Collections with 10 or fewer items are always parameterized.
        Assert.Equal("(@p1,@p2,@p3,@p4,@p5)", criteria.ToString(query));
        Assert.Equal(5, query.Params.Count);
    }

    [Fact]
    public void ToString_WithoutQuery_ThrowsInvalidOperation()
    {
        // BaseCriteria.ToString() uses a no-params checker that rejects parameterized criteria.
        Assert.Throws<InvalidOperationException>(() => new ValueCriteria(5).ToString());
    }

    [Fact]
    public void ToString_LazyEnumerable_EnumeratedOnce()
    {
        var enumerations = 0;
        IEnumerable<object> Lazy()
        {
            enumerations++;
            yield return 1;
            yield return 2;
            yield return 3;
        }

        var query = new SqlQuery();
        var criteria = new ValueCriteria(Lazy());

        // Previously the enumerable was walked twice (count, then render),
        // which throws or re-executes single-pass sources.
        Assert.Equal("(@p1,@p2,@p3)", criteria.ToString(query));
        Assert.Equal(1, enumerations);
    }

    [Fact]
    public void ToString_ManyIntegers_UsesInvariantDigits()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NativeDigits = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
        var current = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = culture;

            var query = new SqlQuery();
            AddNearlyExhaustedSqlServerBudget(query);
            var criteria = new ValueCriteria(Enumerable.Range(1, 15).ToArray());

            // Inlined values must use invariant digits, or the server can't parse them.
            Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15)", criteria.ToString(query));
            Assert.Equal(1590, query.Params.Count);
        }
        finally
        {
            CultureInfo.CurrentCulture = current;
        }
    }

    [Fact]
    public void ToString_ManyEnums_AreInlined_WithInvariantValue()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new object[] { SampleEnum.First, SampleEnum.Second });

        Assert.Equal("(@p1,@p2)", criteria.ToString(query));
        Assert.Equal(2, query.Params.Count);
    }

    private enum SampleEnum
    {
        First = 1,
        Second = 2
    }

    private static void AddNearlyExhaustedSqlServerBudget(SqlQuery query)
    {
        for (int i = 0; i < 1590; i++)
            query.AddParam("@existing" + i, i);
    }
}
