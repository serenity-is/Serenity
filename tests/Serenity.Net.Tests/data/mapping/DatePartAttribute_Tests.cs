namespace Serenity.Tests.Data;

using DatePart = DatePartAttribute;

public class DatePartAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_DateExpression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DatePart(DateParts.Year, null));
    }

    [InlineData(typeof(MySqlDialect))]
    [InlineData(typeof(OracleDialect))]
    [InlineData(typeof(FirebirdDialect))]
    [Theory]
    public void Uses_Extract_For_MySql_Oracle_Firebird_And_Postgres(Type dialectType)
    {
        var dialect = (ISqlDialect)Activator.CreateInstance(dialectType);
        Assert.Equal("EXTRACT(YEAR FROM TEST)", new DatePart(DateParts.Year, "TEST").ToString(dialect));
    }

    [Fact]
    public void Uses_DatePart_For_SqlServer()
    {
        var dialect = SqlServer2012Dialect.Instance;
        Assert.Equal("DATEPART(YEAR, TEST)", new DatePart(DateParts.Year, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(MONTH, TEST)", new DatePart(DateParts.Month, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(DAY, TEST)", new DatePart(DateParts.Day, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(HOUR, TEST)", new DatePart(DateParts.Hour, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(MINUTE, TEST)", new DatePart(DateParts.Minute, "TEST").ToString(dialect));
        Assert.Equal("DATEPART(SECOND, TEST)", new DatePart(DateParts.Second, "TEST").ToString(dialect));
    }

    [Fact]
    public void Uses_StrFTime_For_Sqlite()
    {
        var dialect = SqliteDialect.Instance;
        Assert.Equal("CAST(STRFTIME('%Y', TEST) AS INTEGER)", new DatePart(DateParts.Year, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%m', TEST) AS INTEGER)", new DatePart(DateParts.Month, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%d', TEST) AS INTEGER)", new DatePart(DateParts.Day, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%H', TEST) AS INTEGER)", new DatePart(DateParts.Hour, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%M', TEST) AS INTEGER)", new DatePart(DateParts.Minute, "TEST").ToString(dialect));
        Assert.Equal("CAST(STRFTIME('%S', TEST) AS INTEGER)", new DatePart(DateParts.Second, "TEST").ToString(dialect));
    }

}