namespace Serenity.Data.Mapping;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
public class TrimAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TrimAttribute(null!));
    }

    [Fact]
    public void Uses_LTrim_RTrim_For_SqlServer_Which_Has_Trim_Only_Since_2017()
    {
        Assert.Equal("LTRIM(RTRIM(S))", new TrimAttribute("S").ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("LTRIM(S)", new TrimAttribute("S") { Trailing = false }.ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("RTRIM(S)", new TrimAttribute("S") { Leading = false }.ToString(SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void Uses_Trim_For_Others()
    {
        Assert.Equal("TRIM(S)", new TrimAttribute("S").ToString(PostgresDialect.Instance));
        Assert.Equal("LTRIM(S)", new TrimAttribute("S") { Trailing = false }.ToString(OracleDialect.Instance));
    }

    [Fact]
    public void Uses_Trim_Leading_Trailing_For_Firebird_Which_Has_No_LTrim_RTrim()
    {
        Assert.Equal("TRIM(S)", new TrimAttribute("S").ToString(FirebirdDialect.Instance));
        Assert.Equal("TRIM(LEADING FROM S)", new TrimAttribute("S") { Trailing = false }.ToString(FirebirdDialect.Instance));
        Assert.Equal("TRIM(TRAILING FROM CAST(@s AS VARCHAR(8191)))", new TrimAttribute("@s") { Leading = false }.ToString(FirebirdDialect.Instance));
    }

    [Fact]
    public void Returns_The_Expression_When_Neither_Side_Is_Trimmed()
    {
        Assert.Equal("S", new TrimAttribute("S") { Leading = false, Trailing = false }.ToString(PostgresDialect.Instance));
    }
}
