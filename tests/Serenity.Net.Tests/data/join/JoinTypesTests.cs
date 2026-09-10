namespace Serenity.Data;

public class JoinTypesTests
{
    [Fact]
    public void CrossApply_Wraps_SubQuery_And_Keyword()
    {
        var join = new CrossApply("SELECT 1", "ca");

        Assert.Equal("CROSS APPLY", join.GetKeyword());
        Assert.Equal("(SELECT 1)", join.Table);
        Assert.Equal("ca", join.Name);
    }

    [Fact]
    public void CrossApply_Keeps_Empty_SubQuery_As_Is()
    {
        var join = new CrossApply("", "ca");
        Assert.Equal("", join.Table);
    }

    [Fact]
    public void CrossApply_Registers_In_Joins_Dictionary()
    {
        var joins = new Dictionary<string, Join>();
        _ = new CrossApply(joins, "SELECT 1", "ca");

        Assert.True(joins.ContainsKey("ca"));
    }

    [Fact]
    public void RightJoin_Keyword_And_Properties()
    {
        var join = new RightJoin("Table", "T1", null);

        Assert.Equal("RIGHT JOIN", join.GetKeyword());
        Assert.Equal("Table", join.Table);
        Assert.Equal("T1", join.Name);
    }

    [Fact]
    public void RightJoin_Registers_In_Joins_Dictionary()
    {
        var joins = new Dictionary<string, Join>();
        _ = new RightJoin(joins, "Table", "T1", null);

        Assert.True(joins.ContainsKey("T1"));
    }
}
