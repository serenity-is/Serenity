namespace Serenity.Data;

public class ValueCriteriaInlineTests
{
    private enum TestEnum
    {
        A = 1, B, C, D, E, F, G, H, I, J, K
    }

    private static object[] LongList => [.. Enumerable.Range(1, 11).Cast<object>()];

    [Fact]
    public void ToString_TenValues_UsesParams()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 10).Cast<object>().ToArray());

        var result = criteria.ToString(query);

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10)", result);
        Assert.NotEmpty(query.Params);
    }

    [Fact]
    public void ToString_MoreThanTenIntegers_InlinesValues()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(LongList);

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Null(query.Params);
    }

    [Fact]
    public void ToString_MoreThanTenLongs_InlinesValues()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enumerable.Range(1, 11).Select(i => (long)i).Cast<object>().ToArray());

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Null(query.Params);
    }

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(ulong))]
    public void ToString_MoreThanTenPrimitiveIntTypes_InlinesValues(Type type)
    {
        var query = new SqlQuery();

        object list = type.Name switch
        {
            "Byte" => Enumerable.Range(1, 11).Select(i => (byte)i).Cast<object>().ToArray(),
            "SByte" => [.. Enumerable.Range(1, 11).Select(i => (sbyte)i).Cast<object>()],
            "Int16" => [.. Enumerable.Range(1, 11).Select(i => (short)i).Cast<object>()],
            "UInt16" => [.. Enumerable.Range(1, 11).Select(i => (ushort)i).Cast<object>()],
            "UInt32" => [.. Enumerable.Range(1, 11).Select(i => (uint)i).Cast<object>()],
            _ => [.. Enumerable.Range(1, 11).Select(i => (ulong)i).Cast<object>()]
        };

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", new ValueCriteria(list).ToString(query));
        Assert.Null(query.Params);
    }

    [Fact]
    public void ToString_MoreThanTenEnums_InlinesConvertedValues()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(Enum.GetValues<TestEnum>().Cast<object>().ToArray());

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Null(query.Params);
    }

    [Fact]
    public void ToString_MoreThanTenStrings_UsesParams()
    {
        var query = new SqlQuery();
        var values = Enumerable.Range(1, 11).Select(i => "v" + i).Cast<object>().ToArray();

        var result = new ValueCriteria(values).ToString(query);

        Assert.StartsWith("(", result);
        Assert.EndsWith(")", result);
        Assert.Contains("@p1", result);
        Assert.Contains("@p11", result);
        Assert.True(query.Params.Count == 11);
    }

    [Fact]
    public void ToString_MoreThanTenMixedWithNull_UsesParamsForNullAndNonInlined()
    {
        var query = new SqlQuery();
        var values = new List<object>();
        for (int i = 1; i <= 6; i++)
            values.Add(i);
        values.Add(null);
        values.Add("s1");
        values.Add('c');
        values.Add(DateTime.MaxValue);
        values.Add(7.5);

        var result = new ValueCriteria(values.ToArray()).ToString(query);

        Assert.StartsWith("(", result);
        Assert.EndsWith(")", result);
        // integers among the 11 items are inlined; null, string, char, datetime and double become params
        int paramCount = query.Params.Count;
        Assert.Equal(5, paramCount);
    }

    [Fact]
    public void ToString_MixedValues_LessThanTen_UsesParamsForAllExceptEnumInlineOnlyOverTen()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new object[] { 1, 2, 3 });

        Assert.Equal("(@p1,@p2,@p3)", criteria.ToString(query));
        Assert.Equal(3, query.Params.Count);
    }

    // Note: dot above covers long/list and others
}


