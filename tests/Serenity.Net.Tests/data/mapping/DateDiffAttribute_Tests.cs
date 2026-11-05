namespace Serenity.Data.Mapping;

using DateDiff = DateDiffAttribute;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
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

        Assert.Equal("((CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d', D2)) AS INTEGER) - " +
            "CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d', D1)) AS INTEGER)) / 86400)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("((CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:00:00', D2)) AS INTEGER) - " +
            "CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:00:00', D1)) AS INTEGER)) / 3600)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("((CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:%M:00', D2)) AS INTEGER) - " +
            "CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:%M:00', D1)) AS INTEGER)) / 60)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(SqliteDialect.Instance));

        Assert.Equal("((CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:%M:%S', D2)) AS INTEGER) - " +
            "CAST(STRFTIME('%s', STRFTIME('%Y-%m-%d %H:%M:%S', D1)) AS INTEGER)) / 1)",
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

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (DATE_TRUNC('day', D2::timestamp) - DATE_TRUNC('day', D1::timestamp))) / 86400)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (DATE_TRUNC('hour', D2::timestamp) - DATE_TRUNC('hour', D1::timestamp))) / 3600)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (DATE_TRUNC('minute', D2::timestamp) - DATE_TRUNC('minute', D1::timestamp))) / 60)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(PostgresDialect.Instance));

        Assert.Equal("ROUND(EXTRACT(EPOCH FROM (DATE_TRUNC('second', D2::timestamp) - DATE_TRUNC('second', D1::timestamp))) / 1)",
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

        Assert.Equal("(TRUNC(CAST(D2 AS DATE)) - TRUNC(CAST(D1 AS DATE)))",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((TRUNC(CAST(D2 AS DATE), 'HH') - TRUNC(CAST(D1 AS DATE), 'HH')) * 24)",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((TRUNC(CAST(D2 AS DATE), 'MI') - TRUNC(CAST(D1 AS DATE), 'MI')) * 1440)",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(OracleDialect.Instance));

        Assert.Equal("ROUND((CAST(D2 AS DATE) - CAST(D1 AS DATE)) * 86400)",
            new DateDiff(DateParts.Second, "D1", "D2").ToString(OracleDialect.Instance));
    }

    [Fact]
    public void MySql_Syntax()
    {
        Assert.Equal("(EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1))",
            new DateDiff(DateParts.Year, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("((EXTRACT(YEAR FROM D2) - EXTRACT(YEAR FROM D1)) * 12 + " +
            "EXTRACT(MONTH FROM D2) - EXTRACT(MONTH FROM D1))",
            new DateDiff(DateParts.Month, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("DATEDIFF(D2, D1)",
            new DateDiff(DateParts.Day, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(HOUR, DATE_FORMAT(D1, '%Y-%m-%d %H:00:00'), DATE_FORMAT(D2, '%Y-%m-%d %H:00:00'))",
            new DateDiff(DateParts.Hour, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(MINUTE, DATE_FORMAT(D1, '%Y-%m-%d %H:%i:00'), DATE_FORMAT(D2, '%Y-%m-%d %H:%i:00'))",
            new DateDiff(DateParts.Minute, "D1", "D2").ToString(MySqlDialect.Instance));

        Assert.Equal("TIMESTAMPDIFF(SECOND, DATE_FORMAT(D1, '%Y-%m-%d %H:%i:%s'), DATE_FORMAT(D2, '%Y-%m-%d %H:%i:%s'))",
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
