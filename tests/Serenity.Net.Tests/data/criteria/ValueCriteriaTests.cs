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
        Assert.Equal("test", query.Params!["@p1"]);
    }

    [Fact]
    public void ToString_IntValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(5);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Equal(5, query.Params!["@p1"]);
    }

    [Fact]
    public void ToString_DateTimeValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var value = new DateTime(2023, 1, 15, 10, 30, 0);
        var criteria = new ValueCriteria(value);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Equal(value, query.Params!["@p1"]);
    }

    [Fact]
    public void ToString_NullValue_AddsParamToQuery()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(null);

        Assert.Equal("@p1", criteria.ToString(query));
        Assert.Null(query.Params!["@p1"]);
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
        Assert.Equal("a", query.Params!["@p1"]);
        Assert.Equal("b", query.Params!["@p2"]);
    }

    [Fact]
    public void ToString_ManyIntegers_AreInlined()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 15).ToArray());

        // Collections with more than 10 items inline integer values instead of parameters.
        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11,12,13,14,15)", criteria.ToString(query));
        Assert.Null(query.Params);
    }

    [Fact]
    public void ToString_ManyEnums_AreInlined()
    {
        var query = new SqlQuery();
        var values = Enumerable.Range(0, 15).Select(i => (SampleEnum)(i % 3)).ToArray();
        var criteria = new ValueCriteria(values);

        // Enum items are inlined through their Int64 value when count is above 10.
        Assert.Equal("(0,1,2,0,1,2,0,1,2,0,1,2,0,1,2)", criteria.ToString(query));
        Assert.Null(query.Params);
    }

    [Fact]
    public void ToString_ManyStrings_StillUseParams()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 15).Select(i => "s" + i).ToArray());

        // Only integer and enum items are inlined; strings are always parameterized.
        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
            criteria.ToString(query));
        Assert.Equal(15, query.Params!.Count);
    }

    [Fact]
    public void ToString_ShortIntCollection_UsesParams()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new[] { 1, 2, 3, 4, 5 });

        // Collections with 10 or fewer items are always parameterized.
        Assert.Equal("(@p1,@p2,@p3,@p4,@p5)", criteria.ToString(query));
        Assert.Equal(5, query.Params!.Count);
    }

    [Fact]
    public void ToString_WithoutQuery_ThrowsInvalidOperation()
    {
        // BaseCriteria.ToString() uses a no-params checker that rejects parameterized criteria.
        Assert.Throws<InvalidOperationException>(() => new ValueCriteria(5).ToString());
    }
}
