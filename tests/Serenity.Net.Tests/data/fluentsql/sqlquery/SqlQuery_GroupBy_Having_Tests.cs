namespace Serenity.Data;

public class SqlQuery_GroupBy_Having_Tests
{
    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    [Fact]
    public void GroupByWithExpressionWorks()
    {
        var query = new SqlQuery()
            .Select("TestColumn")
            .From("TestTable")
            .GroupBy("TestColumn")
            .GroupBy("TestColumn2");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM [TestTable] GROUP BY TestColumn, TestColumn2"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void GroupByWithAliasAndFieldnameWorks()
    {
        var query = new SqlQuery()
            .Select("u.TestColumn")
            .From("TestTable u")
            .GroupBy(new Alias("u"), "TestColumn")
            .GroupBy(new Alias("u"), "TestColumn2");

        Assert.Equal(
            Normalize.Sql(
                "SELECT u.TestColumn FROM TestTable u GROUP BY u.TestColumn, u.TestColumn2"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void HavingWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Having(null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Having(String.Empty));
    }

    [Fact]
    public void HavingWithExpressionWorks()
    {
        var query = new SqlQuery()
            .Select("TestColumn")
            .From("TestTable")
            .GroupBy("TestColumn")
            .Having("Count(*) > 5");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM [TestTable] GROUP BY TestColumn HAVING Count(*) > 5"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void HavingDoesAndWhenCalledMoreThanOnce()
    {
        var query = new SqlQuery()
            .From("t")
            .GroupBy("c")
            .Select("c")
            .Having("count(*) > 2")
            .Having("sum(y) < 1000");

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM [t] GROUP BY c HAVING count(*) > 2 AND sum(y) < 1000"),
            Normalize.Sql(
                query.ToString())
        );
    }

    [Fact]
    public void Having_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.Having("T1.Value > 0"),
            "HAVING T1.Value > 0");
    }

    [Fact]
    public void GroupBy_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.GroupBy("T1.Value"),
            "GROUP BY T1.Value");
    }

    [Fact]
    public void GroupBy_Alias_Overload_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.GroupBy(new Alias("T1"), "Value"),
            "GROUP BY T1.Value");
    }

    private static void AssertClauseEnsuresJoin(Func<SqlQuery, SqlQuery> addClause, string expectedClause)
    {
        var root = new AliasWithJoins("Base", "T0");
        _ = new LeftJoin(root.Joins, "Related", "T1", null);

        var query = addClause(new SqlQuery().From(root).Select("T0.Id"));

        Assert.Contains("LEFT JOIN [Related] T1", Normalize.Sql(query.ToString()));
        Assert.Contains(Normalize.Sql(expectedClause), Normalize.Sql(query.ToString()));
    }
}
