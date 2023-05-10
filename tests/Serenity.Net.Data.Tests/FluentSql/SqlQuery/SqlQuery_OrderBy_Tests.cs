namespace Serenity.Tests.Data;

public partial class SqlQuery_OrderBy_Tests
{
    [Fact]
    public void OrderByWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy((Alias)null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(new Alias("x"), (string)null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(new Alias("x"), String.Empty));
    }

    [Fact]
    public void OrderByWithExpressionWorks()
    {
        var query = new SqlQuery()
            .Select("TestColumn")
            .From("TestTable")
            .OrderBy("TestColumn")
            .OrderBy("TestColumn2");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable ORDER BY TestColumn, TestColumn2"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByWithAliasAndFieldnameWorks()
    {
        var query = new SqlQuery()
            .Select("u.TestColumn")
            .From("TestTable u")
            .OrderBy(new Alias("u"), "TestColumn")
            .OrderBy(new Alias("u"), "TestColumn2");

        Assert.Equal(
            Normalize.Sql(
                "SELECT u.TestColumn FROM TestTable u ORDER BY u.TestColumn, u.TestColumn2"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByAppendsDescKeywordWhenDescArgumentIsTrue()
    {
        var query = new SqlQuery()
            .Select("u.TestColumn")
            .From("TestTable u")
            .OrderBy(new Alias("u"), "TestColumn", desc: true)
            .OrderBy("TestColumn2", desc: true);

        Assert.Equal(
            Normalize.Sql(
                "SELECT u.TestColumn FROM TestTable u ORDER BY u.TestColumn DESC, TestColumn2 DESC"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByFirstWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst((string)null, desc: false));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst((string)null, desc: true));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst("", desc: false));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst("", desc: true));
    }

    [Fact]
    public void OrderByFirstInsertsExpressionToStart()
    {
        var query = new SqlQuery()
            .From("TestTable")
            .Select("a")
            .OrderBy("a")
            .OrderBy("b")
            .OrderByFirst("c");

        Assert.Equal(
            Normalize.Sql(
                "SELECT a FROM TestTable ORDER BY c, a, b"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByFirstMovesExpressionToStartIfAlreadyInStatement()
    {
        var query = new SqlQuery()
            .From("TestTable")
            .Select("a")
            .OrderBy("a")
            .OrderBy("b")
            .OrderBy("c")
            .OrderByFirst("b");

        Assert.Equal(
            Normalize.Sql(
                "SELECT a FROM TestTable ORDER BY b, a, c"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByFirstHandlesDescWhileMovingExpressionToFirst()
    {
        var query1 = new SqlQuery()
            .From("TestTable")
            .Select("a")
            .OrderBy("a")
            .OrderBy("b", desc: true)
            .OrderBy("c")
            .OrderByFirst("b");

        Assert.Equal(
            Normalize.Sql(
                "SELECT a FROM TestTable ORDER BY b, a, c"),
            Normalize.Sql(
                query1.ToString()));

        var query2 = new SqlQuery()
            .From("TestTable")
            .Select("a")
            .OrderBy("a")
            .OrderBy("b")
            .OrderBy("c")
            .OrderByFirst("b", desc: true);

        Assert.Equal(
            Normalize.Sql(
                "SELECT a FROM TestTable ORDER BY b DESC, a, c"),
            Normalize.Sql(
                query2.ToString()));
    }

    [Fact]
    public void OrderByFirstAppendsDescKeywordWhenDescArgumentIsTrue()
    {
        var query = new SqlQuery()
            .Select("u.TestColumn")
            .From("TestTable u")
            .OrderBy(new Alias("u"), "TestColumn", desc: true)
            .OrderByFirst("TestColumn2", desc: true);

        Assert.Equal(
            Normalize.Sql(
                "SELECT u.TestColumn FROM TestTable u ORDER BY TestColumn2 DESC, u.TestColumn DESC"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void OrderByFirstWorksProperlyWhenNoOrderByExists()
    {
        var query = new SqlQuery()
            .Select("TestColumn")
            .From("TestTable")
            .OrderByFirst("TestColumn")
            .OrderBy("SecondColumn");

        Assert.Equal(
            Normalize.Sql(
                "SELECT TestColumn FROM TestTable ORDER BY TestColumn, SecondColumn"),
            Normalize.Sql(
                query.ToString()));
    }
}