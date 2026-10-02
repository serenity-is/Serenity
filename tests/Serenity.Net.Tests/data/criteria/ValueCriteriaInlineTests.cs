namespace Serenity.Data;

public class ValueCriteriaInlineTests
{
    private enum TestEnum
    {
        A = 1, B, C, D, E, F, G, H, I, J, K
    }

    private enum LargeUlongEnum : ulong
    {
        First = ulong.MaxValue
    }

    private static object[] LongList => [.. Enumerable.Range(1, 11).Cast<object>()];

    private static void UseNearlyExhaustedSqlServerBudget(SqlQuery query)
    {
        for (int i = 0; i < 1590; i++)
            query.AddParam("@existing" + i, i);
    }

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
        UseNearlyExhaustedSqlServerBudget(query);
        var criteria = new ValueCriteria(LongList);

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Equal(1590, query.Params.Count);
    }

    [Theory]
    [InlineData(typeof(PostgresDialect))]
    [InlineData(typeof(MySqlDialect))]
    public void ToString_MoreThanTenIntegers_StaysParameterized_On_LargeBudget_Dialects(Type dialectType)
    {
        var query = new SqlQuery().Dialect((ISqlDialect)Activator.CreateInstance(dialectType)!);
        var criteria = new ValueCriteria(LongList);

        Assert.Equal("(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)", criteria.ToString(query));
        Assert.Equal(11, query.Params.Count);
    }

    [Fact]
    public void ToString_MoreThanTenLongs_InlinesValues()
    {
        var query = new SqlQuery();
        UseNearlyExhaustedSqlServerBudget(query);
        var criteria = new ValueCriteria(Enumerable.Range(1, 11).Select(i => (long)i).Cast<object>().ToArray());

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Equal(1590, query.Params.Count);
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
        UseNearlyExhaustedSqlServerBudget(query);

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
        Assert.Equal(1590, query.Params.Count);
    }

    [Fact]
    public void ToString_LargeUlong_DoesNotOverflow()
    {
        var query = new SqlQuery();
        UseNearlyExhaustedSqlServerBudget(query);
        var values = Enumerable.Repeat(ulong.MaxValue, 11).Cast<object>().ToArray();

        Assert.Equal("(" + string.Join(",", Enumerable.Repeat("18446744073709551615", 11)) + ")",
            new ValueCriteria(values).ToString(query));
        Assert.Equal(1590, query.Params.Count);
    }

    [Fact]
    public void ToString_MoreThanTenEnums_InlinesConvertedValues()
    {
        var query = new SqlQuery();
        UseNearlyExhaustedSqlServerBudget(query);
        var criteria = new ValueCriteria(Enum.GetValues<TestEnum>().Cast<object>().ToArray());

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", criteria.ToString(query));
        Assert.Equal(1590, query.Params.Count);
    }

    [Fact]
    public void ToString_MoreThanTenUlongBackedEnums_DoesNotOverflow()
    {
        var query = new SqlQuery();
        UseNearlyExhaustedSqlServerBudget(query);
        var values = Enumerable.Repeat((object)LargeUlongEnum.First, 11).ToArray();
        var literal = "18446744073709551615";

        Assert.Equal("(" + string.Join(",", Enumerable.Repeat(literal, 11)) + ")",
            new ValueCriteria(values).ToString(query));
        Assert.Equal(1590, query.Params.Count);
    }

    [Fact]
    public void ToString_MoreThanTenStrings_InlinesLiterals_On_SqlServer()
    {
        var query = new SqlQuery();
        UseNearlyExhaustedSqlServerBudget(query);
        var values = Enumerable.Range(1, 11).Select(i => "v" + i).Cast<object>().ToArray();

        var result = new ValueCriteria(values).ToString(query);

        Assert.StartsWith("(N'v1',", result);
        Assert.EndsWith(")", result);
        Assert.DoesNotContain("@p", result);
        Assert.Equal(1590, query.Params.Count);
    }

    [Fact]
    public void ToString_MixedList_StaysParameterized_While_Budget_Allows()
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
        // Safe literal types remain parameterized while there is budget.
        Assert.Contains("@p8", result);
        int paramCount = query.Params.Count;
        Assert.Equal(11, paramCount);
    }

    [Fact]
    public void ToString_MixedValues_LessThanTen_UsesParamsForAllExceptEnumInlineOnlyOverTen()
    {
        var query = new SqlQuery();
        var criteria = new ValueCriteria(new object[] { 1, 2, 3 });

        Assert.Equal("(@p1,@p2,@p3)", criteria.ToString(query));
        Assert.Equal(3, query.Params.Count);
    }

    [Theory]
    [InlineData(typeof(PostgresDialect))]
    [InlineData(typeof(MySqlDialect))]
    public void ToString_InlinesSafeValues_When_ReservedParameterBudget_Is_Exceeded(Type dialectType)
    {
        var query = new SqlQuery().Dialect((ISqlDialect)Activator.CreateInstance(dialectType)!);
        for (int i = 0; i < 65025; i++)
            query.AddParam("@existing" + i, i);
        var values = Enumerable.Range(1, 11).Cast<object>().ToArray();

        Assert.Equal("(1,2,3,4,5,6,7,8,9,10,11)", new ValueCriteria(values).ToString(query));
        Assert.Equal(65025, query.Params.Count);
    }

    [Fact]
    public void ToString_InlinesStringAndGuidLists_When_ReservedBudget_Is_Exceeded()
    {
        var query = new SqlQuery().Dialect(PostgresDialect.Instance);
        for (int i = 0; i < 65025; i++)
            query.AddParam("@existing" + i, i);
        var strings = Enumerable.Range(1, 11).Select(i => "v" + i).ToArray();
        var guids = Enumerable.Range(1, 11).Select(i => new Guid(i, 0, 0, new byte[8])).ToArray();

        Assert.StartsWith("('v1','v2'", new ValueCriteria(strings).ToString(query));
        Assert.StartsWith("('" + guids[0].ToString("D") + "'", new ValueCriteria(guids).ToString(query));
        Assert.Equal(65025, query.Params.Count);
    }

    [Fact]
    public void ToString_Oracle_InlinesLists_Over_1000_Values()
    {
        var query = new SqlQuery().Dialect(OracleDialect.Instance);
        var values = Enumerable.Range(1, 1001).Cast<object>().ToArray();

        var result = new ValueCriteria(values).ToString(query);

        Assert.StartsWith("(1,2,3,", result);
        Assert.EndsWith(",1001)", result);
        Assert.Null(query.Params);
    }

    // Note: above cases cover long / lazy lists and their supported literal types.
}
