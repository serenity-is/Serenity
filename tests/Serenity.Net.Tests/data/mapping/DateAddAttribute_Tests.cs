namespace Serenity.Data.Mapping;

using DateAdd = DateAddAttribute;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
public class DateAddAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Number_Or_Date_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DateAdd(DateParts.Day, null, "D"));
        Assert.Throws<ArgumentNullException>(() => new DateAdd(DateParts.Day, "N", null));
    }

    [InlineData(typeof(SqlServer2012Dialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Firebird_SqlServer_Syntax(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType)!;

        Assert.Equal("DATEADD(YEAR, N, D)", new DateAdd(DateParts.Year, "N", "D").ToString(dialect));
        Assert.Equal("DATEADD(MONTH, N, D)", new DateAdd(DateParts.Month, "N", "D").ToString(dialect));
        Assert.Equal("DATEADD(SECOND, N, D)", new DateAdd(DateParts.Second, "N", "D").ToString(dialect));
    }

    [Fact]
    public void Postgres_Syntax()
    {
        Assert.Equal("(D + (N) * INTERVAL '1 DAY')", new DateAdd(DateParts.Day, "N", "D").ToString(PostgresDialect.Instance));
        Assert.Equal("(D + (N) * INTERVAL '1 MONTH')", new DateAdd(DateParts.Month, "N", "D").ToString(PostgresDialect.Instance));
    }

    [Fact]
    public void MySql_Syntax()
    {
        Assert.Equal("DATE_ADD(D, INTERVAL (N) HOUR)", new DateAdd(DateParts.Hour, "N", "D").ToString(MySqlDialect.Instance));
    }

    [Fact]
    public void Sqlite_Syntax_KeepsTheEndOfMonthForMonthsAndYears()
    {
        Assert.Equal("DATETIME(D, (N) || ' days')", new DateAdd(DateParts.Day, "N", "D").ToString(SqliteDialect.Instance));
        Assert.Equal("DATETIME(D, (N) || ' months', 'floor')", new DateAdd(DateParts.Month, "N", "D").ToString(SqliteDialect.Instance));
        Assert.Equal("DATETIME(D, (N) || ' years', 'floor')", new DateAdd(DateParts.Year, "N", "D").ToString(SqliteDialect.Instance));
    }

    [Fact]
    public void Oracle_Syntax()
    {
        Assert.Equal("(CAST(D AS DATE) + NUMTODSINTERVAL(N, 'MINUTE'))",
            new DateAdd(DateParts.Minute, "N", "D").ToString(OracleDialect.Instance));

        Assert.Equal("(ADD_MONTHS(TRUNC(CAST(D AS DATE), 'MM'), (N) * 12) + " +
            "(LEAST(EXTRACT(DAY FROM CAST(D AS DATE)), EXTRACT(DAY FROM LAST_DAY(ADD_MONTHS(TRUNC(CAST(D AS DATE), 'MM'), (N) * 12)))) - 1) + " +
            "(CAST(D AS DATE) - TRUNC(CAST(D AS DATE))))",
            new DateAdd(DateParts.Year, "N", "D").ToString(OracleDialect.Instance));
    }

    [Fact]
    public void Accepts_Other_Expression_Attributes()
    {
        Assert.Equal("DATEADD(DAY, 1, SYSDATETIME())",
            new DateAdd(DateParts.Day, 1, typeof(SqlNowAttribute)).ToString(SqlServer2012Dialect.Instance));
    }
}
