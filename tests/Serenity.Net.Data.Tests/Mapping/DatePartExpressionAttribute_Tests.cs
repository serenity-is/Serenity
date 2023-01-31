namespace Serenity.Tests.Data;

using Attr = DatePartAttribute;

public class DatePartExpressionAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_DateExpression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Attr(DateParts.Year, null));
    }

    [InlineData(typeof(MySqlDialect))]
    [InlineData(typeof(OracleDialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Uses_Extract_For_MySql_Oracle_And_Firebird(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);
        Assert.Equal("EXTRACT(YEAR FROM TEST)", new Attr(DateParts.Year, "TEST").ToString(dialect));
    }

    [Fact]
    public void Uses_DatePart_For_Postgres()
    {
        Assert.Equal("DATEPART(TEST, YEAR)", new Attr(DateParts.Year, "TEST").ToString(PostgresDialect.Instance));
    }

    [Fact]
    public void Uses_DatePart_For_SqlServer()
    {
        var dialect = SqlServer2012Dialect.Instance;
        Assert.Equal("DATEPART(YEAR, TEST)", new Attr(DateParts.Year, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(MONTH, TEST)", new Attr(DateParts.Month, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(DAY, TEST)", new Attr(DateParts.Day, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(HOUR, TEST)", new Attr(DateParts.Hour, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(MINUTE, TEST)", new Attr(DateParts.Minute, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(SECOND, TEST)", new Attr(DateParts.Second, "TEST").ToString(dialect));
    }

    [Fact]
    public void Uses_StrFTime_For_Sqlite()
    {
        var dialect = SqliteDialect.Instance;
        Assert.Equal("CAST(STRFTIME('%Y', TEST) AS INTEGER)", new Attr(DateParts.Year, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%m', TEST) AS INTEGER)", new Attr(DateParts.Month, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%d', TEST) AS INTEGER)", new Attr(DateParts.Day, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%H', TEST) AS INTEGER)", new Attr(DateParts.Hour, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%M', TEST) AS INTEGER)", new Attr(DateParts.Minute, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%S', TEST) AS INTEGER)", new Attr(DateParts.Second, "TEST").ToString(dialect));
    }

}