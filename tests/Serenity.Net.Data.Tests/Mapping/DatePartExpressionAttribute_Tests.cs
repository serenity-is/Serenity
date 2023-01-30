namespace Serenity.Tests.Data;

using Attr = DatePartExpressionAttribute;

public class DatePartExpressionAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_DateExpression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new Attr(null, DatePart.Year));
    }

    [InlineData(typeof(MySqlDialect))]
    [InlineData(typeof(OracleDialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Uses_Extract_For_MySql_Oracle_And_Firebird(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);
        Assert.Equal("EXTRACT(YEAR FROM TEST)", new Attr("TEST", DatePart.Year).ToString(dialect));
    }

    [Fact]
    public void Uses_DatePart_For_Postgres()
    {
        Assert.Equal("DATEPART(TEST, YEAR)", new Attr("TEST", DatePart.Year).ToString(PostgresDialect.Instance));
    }

    [Fact]
    public void Uses_DatePart_For_SqlServer()
    {
        var dialect = SqlServer2012Dialect.Instance;
        Assert.Equal("DATEPART(YEAR, TEST)", new Attr("TEST", DatePart.Year).ToString(dialect));
        Assert.Equal("DATEPART(MONTH, TEST)", new Attr("TEST", DatePart.Month).ToString(dialect));
        Assert.Equal("DATEPART(DAY, TEST)", new Attr("TEST", DatePart.Day).ToString(dialect));
        Assert.Equal("DATEPART(HOUR, TEST)", new Attr("TEST", DatePart.Hour).ToString(dialect));
        Assert.Equal("DATEPART(MINUTE, TEST)", new Attr("TEST", DatePart.Minute).ToString(dialect));
        Assert.Equal("DATEPART(SECOND, TEST)", new Attr("TEST", DatePart.Second).ToString(dialect));
    }

    [Fact]
    public void Uses_StrFTime_For_Sqlite()
    {
        var dialect = SqliteDialect.Instance;
        Assert.Equal("CAST(STRFTIME('%Y', TEST) AS INTEGER)", new Attr("TEST", DatePart.Year).ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%m', TEST) AS INTEGER)", new Attr("TEST", DatePart.Month).ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%d', TEST) AS INTEGER)", new Attr("TEST", DatePart.Day).ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%H', TEST) AS INTEGER)", new Attr("TEST", DatePart.Hour).ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%M', TEST) AS INTEGER)", new Attr("TEST", DatePart.Minute).ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%S', TEST) AS INTEGER)", new Attr("TEST", DatePart.Second).ToString(dialect));
    }

}