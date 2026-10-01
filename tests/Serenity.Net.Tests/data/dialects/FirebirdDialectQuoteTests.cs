namespace Serenity.Data;

public class FirebirdDialectQuoteTests
{
    [Theory]
    [InlineData("Order-Detail", "\"Order-Detail\"")]
    [InlineData("my.table", "\"my.table\"")]
    [InlineData("123col", "\"123col\"")]
    [InlineData("_col", "\"_col\"")]
    public void QuoteIdentifier_Quotes_Invalid_Unquoted_Identifiers(string identifier, string expected)
    {
        Assert.Equal(expected, FirebirdDialect.Instance.QuoteIdentifier(identifier));
    }

    [Fact]
    public void QuoteIdentifier_Leaves_Plain_Identifier_Untouched()
    {
        Assert.Equal("MyColumn", FirebirdDialect.Instance.QuoteIdentifier("MyColumn"));
    }

    [Fact]
    public void QuoteIdentifier_Escapes_Double_Quote_In_Identifier()
    {
        Assert.Equal("\"my\"\"column\"", FirebirdDialect.Instance.QuoteIdentifier("my\"column"));
    }
}
