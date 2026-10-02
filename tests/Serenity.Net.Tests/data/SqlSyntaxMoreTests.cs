namespace Serenity.Data;

public class SqlSyntaxMoreTests
{
    [Fact]
    public void IndexParam_Uses_Cache_And_Fallback()
    {
        Assert.Equal("@p0", 0.IndexParam());
        Assert.Equal("@p999", 999.IndexParam());
        Assert.Equal("@p2000", 2000.IndexParam());
    }

    [Fact]
    public void TableAlias_Uses_Cache_And_Fallback()
    {
        Assert.Equal("T0", 0.TableAlias());
        Assert.Equal("T99", 99.TableAlias());
        Assert.Equal("T200", 200.TableAlias());
        Assert.Equal("T0.", 0.TableAliasDot());
        Assert.Equal("T99.", 99.TableAliasDot());
        Assert.Equal("T200.", 200.TableAliasDot());
    }

    [Fact]
    public void Caches_Return_Same_Instance_Under_Concurrency()
    {
        const int count = 64;
        var params1 = new string[count];
        var aliases = new string[count];
        var aliasDots = new string[count];

        Parallel.For(0, count, i =>
        {
            params1[i] = 5.IndexParam();
            aliases[i] = 5.TableAlias();
            aliasDots[i] = 5.TableAliasDot();
        });

        Assert.All(params1, x => Assert.Same(params1[0], x));
        Assert.All(aliases, x => Assert.Same(aliases[0], x));
        Assert.All(aliasDots, x => Assert.Same(aliasDots[0], x));
    }

    [Fact]
    public void IsReservedKeywordForAny_Checks_All_Dialects()
    {
        Assert.False(SqlSyntax.IsReservedKeywordForAny(null!));
        Assert.False(SqlSyntax.IsReservedKeywordForAny(""));
        Assert.True(SqlSyntax.IsReservedKeywordForAny("select"));
        Assert.False(SqlSyntax.IsReservedKeywordForAny("NotAKeywordForSureXYZ"));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("1abc", false)]
    [InlineData("abc", true)]
    [InlineData("_a1", true)]
    public void IsValidIdentifier_Validates(string? s, bool expected)
        => Assert.Equal(expected, SqlSyntax.IsValidIdentifier(s));

    [Fact]
    public void IsQuoted_And_Unquote_Work()
    {
        Assert.False(SqlSyntax.IsQuoted(null!));
        Assert.False(SqlSyntax.IsQuoted(""));
        Assert.False(SqlSyntax.IsQuoted("ab"));
        Assert.True(SqlSyntax.IsQuoted("[ab]"));
        Assert.True(SqlSyntax.IsQuoted("\"ab\""));
        Assert.True(SqlSyntax.IsQuoted("`ab`"));
        Assert.Equal("ab", SqlSyntax.Unquote("[ab]"));
        Assert.Equal("abc", SqlSyntax.Unquote("abc"));
    }

    [Fact]
    public void IsValidQuotedIdentifier_Validates()
    {
        Assert.True(SqlSyntax.IsValidQuotedIdentifier("[a b]"));
        Assert.False(SqlSyntax.IsValidQuotedIdentifier("[ a]"));
        Assert.False(SqlSyntax.IsValidQuotedIdentifier("[a!b]"));
        Assert.True(SqlSyntax.IsValidQuotedIdentifier("abc"));
    }

    [Fact]
    public void AutoBracket_Brackets_Only_When_Needed()
    {
        Assert.Equal("[abc]", SqlSyntax.AutoBracket("abc", SqlServer2012Dialect.Instance));
        Assert.Equal("[select]", SqlSyntax.AutoBracket("select", SqlServer2012Dialect.Instance));
        Assert.Equal("[abc]", SqlSyntax.AutoBracket("[abc]", SqlServer2012Dialect.Instance));
        Assert.Null(SqlSyntax.AutoBracket(null!, SqlServer2012Dialect.Instance));

        var old = SqlSettings.SetLocalDialect(new MockSqlDialect(autoQuotedIdentifiers: false));
        try
        {
            Assert.Equal("abc", SqlSyntax.AutoBracket("abc", null));
            Assert.Equal("[select]", SqlSyntax.AutoBracket("select", null));
            Assert.Equal("[Order Details]", SqlSyntax.AutoBracket("Order Details", null));
            Assert.Equal("OrderDetails", SqlSyntax.AutoBracket("OrderDetails", null));
        }
        finally
        {
            SqlSettings.SetLocalDialect(old);
        }
    }

    [Fact]
    public void AutoBracketValid_Checks_Identifier_Validity()
    {
        var old = SqlSettings.SetLocalDialect(new MockSqlDialect(autoQuotedIdentifiers: false));
        try
        {
            Assert.Equal("abc", SqlSyntax.AutoBracketValid("abc", null));
            Assert.Equal("[select]", SqlSyntax.AutoBracketValid("select", null));

            SqlSettings.SetLocalDialect(new MockSqlDialect(autoQuotedIdentifiers: true));
            Assert.Equal("[abc]", SqlSyntax.AutoBracketValid("abc", null));
            Assert.Equal("1abc", SqlSyntax.AutoBracketValid("1abc", null));
            Assert.Equal("Order Details", SqlSyntax.AutoBracketValid("Order Details", null));
            Assert.Equal("[Order Details]", SqlSyntax.AutoBracketValid("[Order Details]", null));
        }
        finally
        {
            SqlSettings.SetLocalDialect(old);
        }
    }
}
