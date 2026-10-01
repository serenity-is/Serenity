namespace Serenity.Data;

public class OracleDialectQuoteTests
{
    // Oracle folds unquoted identifiers to UPPER, while ORDER BY / WHERE fragments are
    // raw strings appended verbatim. Quoted identifiers must therefore be upper-cased so
    // that folded raw references (e.g. ORDER BY AliasA -> ALIASA) keep matching the
    // quoted SELECT aliases (e.g. "ALIASA"). See the note on QuoteColumnAlias.

    [Fact]
    public void QuoteIdentifier_Upper_Cases_Keyword()
    {
        Assert.Equal("\"SELECT\"", OracleDialect.Instance.QuoteIdentifier("select"));
    }

    [Fact]
    public void QuoteIdentifier_Upper_Cases_Name_With_Space()
    {
        Assert.Equal("\"MY COLUMN\"", OracleDialect.Instance.QuoteIdentifier("My Column"));
    }

    [Fact]
    public void QuoteIdentifier_Leaves_Plain_Identifier_Untouched()
    {
        Assert.Equal("MyColumn", OracleDialect.Instance.QuoteIdentifier("MyColumn"));
    }

    [Theory]
    [InlineData("Order-Detail", "\"ORDER-DETAIL\"")]
    [InlineData("my.table", "\"MY.TABLE\"")]
    [InlineData("123col", "\"123COL\"")]
    [InlineData("_col", "\"_COL\"")]
    public void QuoteIdentifier_Quotes_Invalid_Unquoted_Identifiers(string identifier, string expected)
    {
        Assert.Equal(expected, OracleDialect.Instance.QuoteIdentifier(identifier));
    }

    [Fact]
    public void QuoteColumnAlias_Upper_Cases_Alias()
    {
        Assert.Equal("\"MYALIAS\"", OracleDialect.Instance.QuoteColumnAlias("MyAlias"));
    }

    [Fact]
    public void QuoteIdentifier_Leaves_Quoted_Identifier_Untouched()
    {
        Assert.Equal("\"MyColumn\"", OracleDialect.Instance.QuoteIdentifier("\"MyColumn\""));
    }
}
