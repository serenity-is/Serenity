namespace Serenity.Data;

public class FieldCriteriaOperatorsTests
{
    private readonly AllFieldsRow.RowFields f = [];

    private static string ToSql(BaseCriteria criteria) => criteria.ToString(new SqlQuery());

    [Fact]
    public void IsNull_IsNotNull()
    {
        Assert.Equal("T0.[AInt32] IS NULL", f.AInt32.IsNull().ToString());
        Assert.Equal("T0.[AInt32] IS NOT NULL", f.AInt32.IsNotNull().ToString());
    }

    [Fact]
    public void Like_WithAndWithoutUpper()
    {
        Assert.Equal("(T0.[AString] LIKE @p1)", f.AString.Like("%x%").ToString(new SqlQuery()));
        Assert.Equal("(UPPER(T0.[AString]) LIKE UPPER(@p1))", f.AString.Like("%x%", upper: true).ToString(new SqlQuery()));
    }

    [Fact]
    public void NotLike_NotContains()
    {
        Assert.Equal("(T0.[AString] NOT LIKE @p1)", f.AString.NotLike("%x%").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] NOT LIKE @p1)", f.AString.NotContains("x").ToString(new SqlQuery()));
    }

    [Fact]
    public void StartsWith_EndsWith_Contains()
    {
        Assert.Equal("(T0.[AString] LIKE @p1)", f.AString.StartsWith("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] LIKE @p1)", f.AString.EndsWith("x").ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] LIKE @p1)", f.AString.Contains("x").ToString(new SqlQuery()));
        Assert.Equal("(UPPER(T0.[AString]) LIKE UPPER(@p1))", f.AString.StartsWith("x", upper: true).ToString(new SqlQuery()));
    }

    [Fact]
    public void In_NotIn_RenderParameters()
    {
        Assert.Equal("(T0.[AInt32] IN (@p1,@p2))", f.AInt32.In(5, 7).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AString] NOT IN (@p1,@p2))", f.AString.NotIn("a", "b").ToString(new SqlQuery()));
    }

    private static readonly string self = "T0.[AInt32]";
    private static readonly string other = "T0.[AInt64]";

    private static void AssertOp(BaseCriteria criteria, string symbol)
    {
        Assert.Contains($"({self} {symbol} ", criteria.ToString(new SqlQuery()), StringComparison.Ordinal);
    }

    [Fact]
    public void EqualityOverloads_RenderEqual()
    {
        Field field = f.AInt32;

        AssertOp(field == new Criteria("x"), "=");
        AssertOp(field == new DateTime(2020, 1, 1), "=");
        AssertOp(field == 5.5m, "=");
        AssertOp(field == 5.5, "=");
        AssertOp(field == Guid.NewGuid(), "=");
        AssertOp(field == 5, "=");
        AssertOp(field == 5L, "=");
        AssertOp(field == new Parameter("@P0"), "=");
        AssertOp(field == "x", "=");
    }

    [Fact]
    public void InequalityOverloads_RenderNotEqual()
    {
        Field field = f.AInt32;

        AssertOp(field != new Criteria("x"), "!=");
        AssertOp(field != new DateTime(2020, 1, 1), "!=");
        AssertOp(field != 5.5m, "!=");
        AssertOp(field != 5.5, "!=");
        AssertOp(field != Guid.NewGuid(), "!=");
        AssertOp(field != 5, "!=");
        AssertOp(field != 5L, "!=");
        AssertOp(field != new Parameter("@P0"), "!=");
        AssertOp(field != "x", "!=");
    }

    [Fact]
    public void LessThanOverloads_RenderLess()
    {
        Field field = f.AInt32;

        AssertOp(field < new Criteria("x"), "<");
        AssertOp(field < new DateTime(2020, 1, 1), "<");
        AssertOp(field < 5.5m, "<");
        AssertOp(field < 5.5, "<");
        AssertOp(field < Guid.NewGuid(), "<");
        AssertOp(field < 5, "<");
        AssertOp(field < 5L, "<");
        AssertOp(field < new Parameter("@P0"), "<");
        AssertOp(field < "x", "<");
    }

    [Fact]
    public void LessOrEqualOverloads_RenderLessOrEqual()
    {
        Field field = f.AInt32;

        AssertOp(field <= new Criteria("x"), "<=");
        AssertOp(field <= new DateTime(2020, 1, 1), "<=");
        AssertOp(field <= 5.5m, "<=");
        AssertOp(field <= 5.5, "<=");
        AssertOp(field <= Guid.NewGuid(), "<=");
        AssertOp(field <= 5, "<=");
        AssertOp(field <= 5L, "<=");
        AssertOp(field <= new Parameter("@P0"), "<=");
        AssertOp(field <= "x", "<=");
    }

    [Fact]
    public void GreaterThanOverloads_RenderGreater()
    {
        Field field = f.AInt32;

        AssertOp(field > new Criteria("x"), ">");
        AssertOp(field > new DateTime(2020, 1, 1), ">");
        AssertOp(field > 5.5m, ">");
        AssertOp(field > 5.5, ">");
        AssertOp(field > Guid.NewGuid(), ">");
        AssertOp(field > 5, ">");
        AssertOp(field > 5L, ">");
        AssertOp(field > new Parameter("@P0"), ">");
        AssertOp(field > "x", ">");
    }

    [Fact]
    public void GreaterOrEqualOverloads_RenderGreaterOrEqual()
    {
        Field field = f.AInt32;

        AssertOp(field >= new Criteria("x"), ">=");
        AssertOp(field >= new DateTime(2020, 1, 1), ">=");
        AssertOp(field >= 5.5m, ">=");
        AssertOp(field >= 5.5, ">=");
        AssertOp(field >= Guid.NewGuid(), ">=");
        AssertOp(field >= 5, ">=");
        AssertOp(field >= 5L, ">=");
        AssertOp(field >= new Parameter("@P0"), ">=");
        AssertOp(field >= "x", ">=");
    }

    [Fact]
    public void FieldToField_ComparisonsRenderBothFields()
    {
        Field field = f.AInt32;
        Field field2 = f.AInt64;

        Assert.Equal($"({self} = {other})", (field == field2).ToString());
        Assert.Equal($"({self} != {other})", (field != field2).ToString());
        Assert.Equal($"({self} < {other})", (field < field2).ToString());
        Assert.Equal($"({self} <= {other})", (field <= field2).ToString());
        Assert.Equal($"({self} > {other})", (field > field2).ToString());
        Assert.Equal($"({self} >= {other})", (field >= field2).ToString());
    }

    [Fact]
    public void ConstantComparisons_RenderInline()
    {
        Field field = f.AInt32;

        Assert.Equal("(T0.[AInt32] = @p1)", (field == 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] = @p1)", (field == 5L).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] != @p1)", (field != 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] != @p1)", (field != 5L).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] < @p1)", (field < 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] < @p1)", (field < 5L).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] <= @p1)", (field <= 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] <= @p1)", (field <= 5L).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] > @p1)", (field > 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] > @p1)", (field > 5L).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] >= @p1)", (field >= 5).ToString(new SqlQuery()));
        Assert.Equal("(T0.[AInt32] >= @p1)", (field >= 5L).ToString(new SqlQuery()));
    }

    [Fact]
    public void ParameterCriteria_RendersParameterName()
    {
        Field field = f.AInt32;

        Assert.Equal("(T0.[AInt32] = @P0)", ToStringIgnoreParams(field == new Parameter("@P0")));
        Assert.Equal("(T0.[AInt32] != @P0)", ToStringIgnoreParams(field != new Parameter("@P0")));
        Assert.Equal("(T0.[AInt32] < @P0)", ToStringIgnoreParams(field < new Parameter("@P0")));
        Assert.Equal("(T0.[AInt32] <= @P0)", ToStringIgnoreParams(field <= new Parameter("@P0")));
        Assert.Equal("(T0.[AInt32] > @P0)", ToStringIgnoreParams(field > new Parameter("@P0")));
        Assert.Equal("(T0.[AInt32] >= @P0)", ToStringIgnoreParams(field >= new Parameter("@P0")));
    }

    private static string ToStringIgnoreParams(BaseCriteria criteria) => criteria.ToStringIgnoreParams();
}
