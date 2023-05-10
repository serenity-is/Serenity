namespace Serenity.Tests.Data;

public class SqlQuery_SubQuery_Tests
{
    [Fact]
    public void SubQueryShouldBeEnclosedInParen()
    {
        var sub = new SqlQuery().SubQuery()
            .Select("TestColumn")
            .From("TestTable");

        Assert.Equal(
            Normalize.Sql(
                "(SELECT TestColumn FROM TestTable)"),
            Normalize.Sql(
                sub.ToString()));
    }

    [Fact]
    public void SubQuerySharesParameters()
    {
        var query = new SqlQuery();
        Assert.Equal(0, query.ParamCount);

        var sub = query.SubQuery();
        sub.AddParam("@px1", "value");
        Assert.Equal(1, query.ParamCount);
        Assert.Equal("value", (string)query.Params["@px1"]);
    }

    [Fact]
    public void SubQueryCanBeUsedAsCriteriaUsingVar()
    {
        var query = new SqlQuery()
            .From("ParentTable")
            .Select("ParentColumn");

        query.Where(new Criteria(query.SubQuery()
            .From("SubTable")
            .Take(1)
            .Select("SubColumn")) >= 1);

        Assert.Equal(
            Normalize.Sql(
                "SELECT ParentColumn FROM ParentTable WHERE " +
                    "((SELECT TOP 1 SubColumn FROM SubTable) >= @p1)"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SubQueryCanBeUsedAsCriteriaUsingWith()
    {
        var query = new SqlQuery()
            .From("ParentTable")
            .Select("ParentColumn")
            .With(me => me.Where(new Criteria(me.SubQuery()
                .From("SubTable")
                .Take(1)
                .Select("SubColumn")) >= 1));

        Assert.Equal(
            Normalize.Sql(
                "SELECT ParentColumn FROM ParentTable WHERE " +
                    "((SELECT TOP 1 SubColumn FROM SubTable) >= @p1)"),
            Normalize.Sql(
                query.ToString()));
    }
}