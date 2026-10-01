namespace Serenity.Data;

public partial class SqlQuery_OrderBy_Tests
{
    private sealed class AliasWithJoins(string table, string name)
        : Alias(table, name), IHaveJoins
    {
        public IDictionary<string, Join> Joins { get; } = new Dictionary<string, Join>();
    }

    [Fact]
    public void OrderByWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(null));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(String.Empty));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(null, "x"));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderBy(new Alias("x"), null));
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
                "SELECT TestColumn FROM [TestTable] ORDER BY TestColumn, TestColumn2"),
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
    public void OrderBy_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.OrderBy("T1.Value", desc: true),
            "ORDER BY T1.Value DESC");
    }

    [Fact]
    public void OrderBy_Alias_Overload_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.OrderBy(new Alias("T1"), "Value"),
            "ORDER BY T1.Value");
    }

    [Fact]
    public void OrderByFirst_Ensures_Referenced_Joins()
    {
        AssertClauseEnsuresJoin(
            query => query.OrderByFirst("T1.Value", desc: true),
            "ORDER BY T1.Value DESC");
    }

    private static void AssertClauseEnsuresJoin(Func<SqlQuery, SqlQuery> addClause, string expectedClause)
    {
        var root = new AliasWithJoins("Base", "T0");
        _ = new LeftJoin(root.Joins, "Related", "T1", null);

        var query = addClause(new SqlQuery().From(root).Select("T0.Id"));
        var sql = Normalize.Sql(query.ToString());

        Assert.Contains("LEFT JOIN [Related] T1", sql);
        Assert.Contains(Normalize.Sql(expectedClause), sql);
    }

    [Fact]
    public void OrderByFirstWithEmptyOrNullArgumentsThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst(null, desc: false));
        Assert.Throws<ArgumentNullException>(() => new SqlQuery().OrderByFirst(null, desc: true));
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
                "SELECT a FROM [TestTable] ORDER BY c, a, b"),
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
                "SELECT a FROM [TestTable] ORDER BY b, a, c"),
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
                "SELECT a FROM [TestTable] ORDER BY b, a, c"),
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
                "SELECT a FROM [TestTable] ORDER BY b DESC, a, c"),
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
                "SELECT TestColumn FROM [TestTable] ORDER BY TestColumn, SecondColumn"),
            Normalize.Sql(
                query.ToString()));
    }
}
