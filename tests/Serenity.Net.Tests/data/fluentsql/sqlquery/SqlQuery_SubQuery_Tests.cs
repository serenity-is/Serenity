namespace Serenity.Data;

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
                "(SELECT TestColumn FROM [TestTable])"),
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
    public void SubQueryInheritsParentDialect()
    {
        var query = new SqlQuery();
        var sub = query.SubQuery()
            .Select("SubColumn")
            .From("SubTable")
            .Take(1);

        query.Dialect(PostgresDialect.Instance);

        Assert.Same(PostgresDialect.Instance, ((IQueryWithParams)sub).Dialect);
        Assert.True(sub.IsDialectOverridden);
        Assert.Contains("LIMIT 1", sub.ToString());
        Assert.DoesNotContain("TOP 1", sub.ToString());
    }

    [Fact]
    public void Setting_Dialect_On_SubQuery_Changes_Root_And_Siblings()
    {
        var query = new SqlQuery();
        var sub = query.SubQuery();
        var sibling = query.SubQuery();

        sub.Dialect(PostgresDialect.Instance);

        Assert.Same(PostgresDialect.Instance, query.Dialect());
        Assert.Same(PostgresDialect.Instance, ((IQueryWithParams)sibling).Dialect);
        Assert.True(query.IsDialectOverridden);
        Assert.True(sibling.IsDialectOverridden);
    }

    [Fact]
    public void Parent_Dialect_Change_Invalidates_SubQuery_Sql_Cache()
    {
        var query = new SqlQuery();
        var sub = query.SubQuery().Select("SubColumn").From("SubTable").Take(1);

        Assert.Contains("TOP 1", sub.ToString());
        query.Dialect(PostgresDialect.Instance);

        Assert.Contains("LIMIT 1", sub.ToString());
        Assert.DoesNotContain("TOP 1", sub.ToString());
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
                "SELECT ParentColumn FROM [ParentTable] WHERE " +
                    "((SELECT TOP 1 SubColumn FROM [SubTable]) >= @p1)"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SubQueryCanBeUsedAsCriteriaUsingWithSelf()
    {
        var query = new SqlQuery()
            .From("ParentTable")
            .Select("ParentColumn")
            .WithSelf(out var me)
            .Where(new Criteria(me.SubQuery()
                .From("SubTable")
                .Take(1)
                .Select("SubColumn")) >= 1);

        Assert.Equal(
            Normalize.Sql(
                "SELECT ParentColumn FROM [ParentTable] WHERE " +
                    "((SELECT TOP 1 SubColumn FROM [SubTable]) >= @p1)"),
            Normalize.Sql(
                query.ToString()));
    }
}
