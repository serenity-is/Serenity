namespace Serenity.Data;

public class SqlQuery_CloneTests
{
    [Fact]
    public void Clone_Copies_Full_Query()
    {
        var query = new SqlQuery()
            .Dialect(new SqlServer2012Dialect())
            .From("TestTable")
            .Select("A", "B")
            .Where("C = 1")
            .OrderBy("A")
            .GroupBy("B")
            .Having("Count(*) > 1")
            .Skip(5)
            .Take(10)
            .Distinct(true);

        var clone = query.Clone();

        Assert.Equal(query.ToString(), clone.ToString());
        Assert.Equal(query.ParamCount, clone.ParamCount);
    }

    [Fact]
    public void Clone_Copies_Params_For_Root_Query()
    {
        var query = new SqlQuery().From("T").Where("A = @p1");
        query.AddParam("@p1", 1);

        var clone = query.Clone();

        Assert.Equal(1, clone.ParamCount);
        Assert.Equal(query.ToString(), clone.ToString());
    }

    [Fact]
    public void Clone_Copies_SubQuery_With_Parent()
    {
        var query = new SqlQuery().From("Parent");
        var sub = query.SubQuery()
            .Select("A")
            .From("SubTable")
            .Where("B = 1");

        var clone = sub.Clone();

        Assert.Equal(sub.ToString(), clone.ToString());
    }

    [Fact]
    public void Clone_Copies_Alias_Expressions_And_AliasWithJoins()
    {
        var query = new SqlQuery().From("Base").Select("x")
            .Join(new LeftJoin("T1Table", "T1", null))
            .LeftJoin(IdNameRow.Fields, null);

        var clone = query.Clone();

        Assert.Equal(query.ToString(), clone.ToString());
    }
}
