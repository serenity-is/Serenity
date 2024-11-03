namespace Serenity.Tests.Data;

public class SqlQuery_SkipTake_Tests
{
    [Fact]
    public void SkipThrowsExceptionIfNoOrderByForSql2000Dialect()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .Skip(10);

        Assert.Throws<InvalidOperationException>(delegate
        {
            query.ToString();
        });
    }

    [Fact]
    public void SkipUsesWorkAroundWithOneOrderByForSql2000Dialect()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .Skip(10);

        Assert.Equal(
            Normalize.Sql(
                "DECLARE @Value0 SQL_VARIANT;" +
                "SELECT TOP 10 @Value0 = x FROM t ORDER BY x;" +
                "SELECT c FROM t WHERE ((((x IS NOT NULL AND @Value0 IS NULL) OR (x > @Value0)))) ORDER BY x"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SkipUsesWorkAroundWithTwoOrderByForSql2000Dialect()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .Where("c > 2")
            .OrderBy("x")
            .OrderBy("y")

            .Skip(100)
            .Take(50);

        Assert.Equal(
            Normalize.Sql(
                "DECLARE @Value0 SQL_VARIANT;" +
                "DECLARE @Value1 SQL_VARIANT;" +
                "SELECT TOP 100 @Value0 = x,@Value1 = y FROM t WHERE c > 2 ORDER BY x, y;" +
                "SELECT TOP 50 c FROM t WHERE c > 2 AND " +
                    "((((x IS NOT NULL AND @Value0 IS NULL) OR (x > @Value0))) " +
                        "OR (((x IS NULL AND @Value0 IS NULL) OR (x = @Value0)) " +
                        "AND ((y IS NOT NULL AND @Value1 IS NULL) OR (y > @Value1)))) ORDER BY x, y"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SkipUsesRowNumberForSql2005Dialect()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2005Dialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .Skip(10)
            .Take(20);

        Assert.Equal(
            Normalize.Sql(
                "SELECT * FROM (\n" +
                    "SELECT TOP 30 c, ROW_NUMBER() OVER (ORDER BY x) AS x__num__ FROM t ORDER BY x) x__results__ " +
                "WHERE x__num__ > 10"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void TakeUsesCorrectSyntaxForSqliteDialect()
    {
        var query = new SqlQuery()
            .Dialect(SqliteDialect.Instance)
            .Select("c")
            .From("t")
            .Take(10);

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t LIMIT 10"),
            Normalize.Sql(
                query.ToString()));

    }

    [Fact]
    public void SkipTakeUsesCorrectSyntaxForOracleDialect()
    {
        var query = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t")
            .Take(20)
            .Skip(50);

        Assert.Equal(
            Normalize.Sql(
                "SELECT * FROM ( SELECT c, ROWNUM AS x__rownum__ FROM t) WHERE x__rownum__ > 50 AND ROWNUM <= 20"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void SkipTakeWithOrderByUsesCorrectSyntaxForOracleDialect()
    {
        var query = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .Take(20)
            .Skip(50);

        Assert.Equal(
            Normalize.Sql(
                "SELECT * FROM ( SELECT c, ROW_NUMBER() OVER (ORDER BY x) AS x__rownum__ FROM t ORDER BY x) WHERE x__rownum__ > 50 AND ROWNUM <= 20"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void TakeUsesCorrectSyntaxForOracleDialect()
    {
        var query = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t")
            .Take(10);

        Assert.Equal(
            Normalize.Sql(
                "SELECT * FROM ( SELECT c, ROWNUM AS x__rownum__ FROM t) WHERE x__rownum__ > 0 AND ROWNUM <= 10"),
            Normalize.Sql(
                query.ToString()));
    }

    [Fact]
    public void WithoutTakeUsesCorrectSyntaxForOracleDialect()
    {
        var query = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t");

        Assert.Equal(
            Normalize.Sql(
                "SELECT c FROM t"),
            Normalize.Sql(
                query.ToString()));
    }
}