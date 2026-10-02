namespace Serenity.Data;

public class DialectQuoteEscapingTests
{
    [Fact]
    public void QuoteIdentifier_Escapes_Closing_Delimiter()
    {
        Assert.Equal("[a]]b]", SqlServer2000Dialect.Instance.QuoteIdentifier("a]b"));
        Assert.Equal("\"a]b\"", SqliteDialect.Instance.QuoteIdentifier("a]b"));
        Assert.Equal("`a``b`", MySqlDialect.Instance.QuoteIdentifier("a`b"));
    }

    [Fact]
    public void QuoteIdentifier_Escapes_Double_Quote()
    {
        Assert.Equal("\"a\"\"b\"", PostgresDialect.Instance.QuoteIdentifier("a\"b"));
        Assert.Equal("\"a\"\"b\"", FirebirdDialect.Instance.QuoteIdentifier("a\"b"));
        Assert.Equal("\"A\"\"B\"", OracleDialect.Instance.QuoteIdentifier("a\"b"));
    }

    [Fact]
    public void QuoteIdentifier_Does_Not_Treat_Single_Quote_As_Already_Quoted()
    {
        Assert.Equal("[]]]", SqlServer2000Dialect.Instance.QuoteIdentifier("]"));
        Assert.Equal("\"]\"", SqliteDialect.Instance.QuoteIdentifier("]"));
        Assert.Equal("````", MySqlDialect.Instance.QuoteIdentifier("`"));
        Assert.Equal("\"\"\"\"", PostgresDialect.Instance.QuoteIdentifier("\""));
        Assert.Equal("\"\"\"\"", FirebirdDialect.Instance.QuoteIdentifier("\""));
        Assert.Equal("\"\"\"\"", OracleDialect.Instance.QuoteIdentifier("\""));
    }
}
