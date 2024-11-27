namespace Serenity.Tests.Data;

using DateDiff = DateDiffAttribute;

public class DateDiffAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Start_Or_End_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DateDiff(DateParts.Year, null, null));
        Assert.Throws<ArgumentNullException>(() => new DateDiff(DateParts.Year, null, "TEST"));
        Assert.Throws<ArgumentNullException>(() => new DateDiff(DateParts.Year, "TEST", null));
    }

    [Fact]
    public void Sqlite_Syntax()
    {
        Assert.Equal("(CAST(STRFTIME('%Y', D2) AS INTEGER) - CAST(STRFTIME('%Y', D1) AS INTEGER))", 
            new DateDiff(DateParts.Year, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("((CAST(STRFTIME('%Y', D2) AS INTEGER) - CAST(STRFTIME('%Y', D1) AS INTEGER)) * 12 + " + 
            "CAST(STRFTIME('%m', D2) AS INTEGER) - CAST(STRFTIME('%m', D1) AS INTEGER))",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("ROUND((JULIANDAY(D2) - JULIANDAY(D1)))",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("ROUND((JULIANDAY(D2) - JULIANDAY(D1)) * 24)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("ROUND((JULIANDAY(D2) - JULIANDAY(D1)) * 1440)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("ROUND((JULIANDAY(D2) - JULIANDAY(D1)) * 86400)",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(SqliteDialect.Instance));
    }

    [Fact]
    public void Postgres_Syntax()
    {
        Assert.Equal("(EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1))",
            new DateDiff(DateParts.Year, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("((EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1)) * 12 + " +
            "EXTRACT(MONTH FROM D2) - EXTRACT(MONTH FROM D1))",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (D2::timestamp - D1::timestamp)) / 86400)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (D2::timestamp - D1::timestamp)) / 3600)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (D2::timestamp - D1::timestamp)) / 60)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (D2::timestamp - D1::timestamp)))",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(PostgresDialect.Instance));
    }

    [Fact]
    public void Oracle_Syntax()
    {
        Assert.Equal("(EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1))",
            new DateDiff(DateParts.Year, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("((EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1)) * 12 + " +
            "EXTRACT(MONTH FROM D2) - EXTRACT(MONTH FROM D1))",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((CAST (D2 as DATE) - CAST (D1 as DATE)))",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((CAST (D2 as DATE) - CAST (D1 as DATE)) * 24)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((CAST (D2 as DATE) - CAST (D1 as DATE)) * 1440)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((CAST (D2 as DATE) - CAST (D1 as DATE)) * 86400)",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(OracleDialect.Instance));
    }

    [Fact]
    public void MySql_Syntax()
    {
        Assert.Equal("TIMESTAMPDIFF(YEAR, D1, D2)",
            new DateDiff(DateParts.Year, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(MONTH, D1, D2)",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(DAY, D1, D2)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(HOUR, D1, D2)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(MINUTE, D1, D2)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(SECOND, D1, D2)",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(MySqlDialect.Instance));
    }

    [InlineData(typeof(SqlServer2012Dialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Firebird_SqlServer_Syntax(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);

        Assert.Equal("DATEDIFF(YEAR, D1, D2)",
            new DateDiff(DateParts.Year, "D1", "D2").ToString(dialect));

        Assert.Equal("DATEDIFF(MONTH, D1, D2)",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(dialect));

        Assert.Equal("DATEDIFF(DAY, D1, D2)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(dialect));

        Assert.Equal("DATEDIFF(HOUR, D1, D2)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(dialect));

        Assert.Equal("DATEDIFF(MINUTE, D1, D2)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(dialect));

        Assert.Equal("DATEDIFF(SECOND, D1, D2)",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(dialect));
    }
}