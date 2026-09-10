namespace Serenity.Data;

public class SqlQuery_ToStringTests
{
    private sealed class QueryToStringDialect : SqlServer2012Dialect, ISqlQueryToString
    {
        public string ToString(ISqlQuery sqlQuery)
        {
            return "CUSTOM";
        }
    }

    [Fact]
    public void ToString_Uses_ISqlQueryToString_Dialect_If_Available()
    {
        var query = new SqlQuery()
            .Dialect(new QueryToStringDialect())
            .Select("c")
            .From("t");

        Assert.Equal("CUSTOM", query.ToString());
    }

    [Fact]
    public void ToString_Throws_For_Null_Query()
    {
        Assert.Throws<ArgumentNullException>(() => SqlQuery.ToString(null!, SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void ToString_Appends_ForXml()
    {
        var sql = new SqlQuery().Select("c").From("t").ForXml("RAW").ToString();
        Assert.Contains("FOR XML RAW", sql);
    }

    [Fact]
    public void ToString_Appends_ForJson()
    {
        var sql = new SqlQuery().Select("c").From("t").ForJson().ToString();
        Assert.Contains("FOR JSON AUTO", sql);

        var sql2 = new SqlQuery().Select("c").From("t").ForJson("PATH").ToString();
        Assert.Contains("FOR JSON PATH", sql2);
    }

    [Fact]
    public void ToString_Distinct_With_Multiple_Columns()
    {
        var sql = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("a")
            .Select("b")
            .From("t")
            .Distinct(true)
            .ToString();

        Assert.Contains("SELECT DISTINCT", sql);
    }

    [Fact]
    public void ToString_SkipKeyword_For_Firebird()
    {
        var sql = new SqlQuery()
            .Dialect(FirebirdDialect.Instance)
            .Select("c")
            .From("t")
            .Skip(5)
            .ToString();

        Assert.Contains("SKIP 5", sql);
    }

    [Fact]
    public void ToString_Offset_Without_Take_For_Postgres()
    {
        var sql = new SqlQuery()
            .Dialect(PostgresDialect.Instance)
            .Select("c")
            .From("t")
            .Skip(5)
            .ToString();

        Assert.Contains("OFFSET 5", sql);
    }

    [Fact]
    public void ToString_Sql2000_Second_Query_With_Distinct()
    {
        var sql = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .Distinct(true)
            .OrderBy("x")
            .Skip(10)
            .ToString();

        Assert.Contains("SELECT DISTINCT", sql);
    }

    [Fact]
    public void ToString_Sql2000_Second_Query_With_Descending_Multiple_OrderBy()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2000Dialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x DESC")
            .OrderBy("y")
            .OrderBy("z")
            .Skip(10);

        var sql = query.ToString();

        Assert.Contains("DECLARE @Value2 SQL_VARIANT", sql);
        Assert.Contains("@Value0 = x", sql);
        Assert.Contains("x < @Value0", sql);
        Assert.Contains("x = @Value0", sql);
    }

    [Fact]
    public void ToString_Oracle_RowNumber_With_Multiple_OrderBy()
    {
        var sql = new SqlQuery()
            .Dialect(OracleDialect.Instance)
            .Select("c")
            .From("t")
            .OrderBy("x")
            .OrderBy("y")
            .Take(10)
            .Skip(5)
            .ToString();

        Assert.Contains("ROW_NUMBER() OVER (ORDER BY x, y)", sql);
    }

    [Fact]
    public void ToString_CountRecords_With_MultipleResultsets()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t");
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains(";\n", sql);
        Assert.Contains("SELECT count(*)", sql);
    }

    [Fact]
    public void ToString_CountRecords_Without_MultipleResultsets()
    {
        var query = new SqlQuery()
            .Dialect(FirebirdDialect.Instance)
            .Select("c")
            .From("t");
        query.CountRecords = true;

        Assert.Contains("\n---\n", query.ToString());
    }

    [Fact]
    public void ToString_CountRecords_With_Distinct()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t")
            .Distinct(true);
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains("SELECT DISTINCT", sql);
        Assert.Contains(") x__alias__", sql);
    }

    [Fact]
    public void ToString_CountRecords_With_GroupBy()
    {
        var query = new SqlQuery()
            .Dialect(SqlServer2012Dialect.Instance)
            .Select("c")
            .From("t")
            .GroupBy("c");
        query.CountRecords = true;

        var sql = query.ToString();

        Assert.Contains("1 as x__alias__x", sql);
        Assert.Contains(") x__alias__", sql);
    }
}
