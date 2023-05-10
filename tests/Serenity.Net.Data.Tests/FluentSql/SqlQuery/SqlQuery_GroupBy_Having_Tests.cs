namespace Serenity.Tests.Data;

public class SqlQuery_GroupBy_Having_Tests
{
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
                "SELECT TestColumn FROM TestTable GROUP BY TestColumn, TestColumn2"),
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
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().Having((string)null));
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
                "SELECT TestColumn FROM TestTable GROUP BY TestColumn HAVING Count(*) > 5"),
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
                "SELECT c FROM t GROUP BY c HAVING count(*) > 2 AND sum(y) < 1000"),
            Normalize.Sql(
                query.ToString())
        );
    }
}