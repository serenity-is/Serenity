namespace Serenity.Data;

public class DialectExpressionSelectorTests
{
    [AttributeUsage(AttributeTargets.All)]
    private class TestAttr : Attribute
    {
        public string? Dialect { get; set; }
    }

    private static TestAttr Attr(string? dialect) => new() { Dialect = dialect };

    [Fact]
    public void Constructor_NullDialect_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DialectExpressionSelector(null!));
    }

    [Fact]
    public void Dialect_PropertyReturnsConstructorArgument()
    {
        var dialect = SqlServer2012Dialect.Instance;
        var selector = new DialectExpressionSelector(dialect);

        Assert.Same(dialect, selector.Dialect);
    }

    [Fact]
    public void GetBestMatch_NullDialect_MatchesWithZeroScore()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr(null);

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_EmptyDialect_MatchesWithZeroScore()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr(string.Empty);

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_SingleTokenMatchingServerType_IsSelected()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("SqlServer");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_SingleTokenMatchingDialectTypeName_IsSelected()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("SqlServer2012");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_IsCaseInsensitive()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("sqlserver");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_SingleTokenNotMatching_ReturnsNull()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        Assert.Null(selector.GetBestMatch([Attr("MySql")], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_CommaSeparatedList_MatchesAnyToken()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("MySql,SqlServer");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_CommaSeparatedList_TrimsTokens()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("MySql, SqlServer");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_Negation_NonMatchingDialect_IsIncluded()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("!MySql");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_Negation_MatchingDialect_IsExcluded()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        Assert.Null(selector.GetBestMatch([Attr("!SqlServer")], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_NegatedList_NoMatchingToken_IsIncluded()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var attr = Attr("!MySql,Oracle");

        Assert.Same(attr, selector.GetBestMatch([attr], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_NegatedList_MatchingToken_IsExcluded()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        Assert.Null(selector.GetBestMatch([Attr("!MySql,SqlServer")], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_MultipleMatches_LongestTokenWins()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var shortMatch = Attr("Sql");
        var longMatch = Attr("SqlServer");

        var result = selector.GetBestMatch([shortMatch, longMatch], x => x.Dialect);

        Assert.Same(longMatch, result);
    }

    [Fact]
    public void GetBestMatch_DuplicateDialectStrings_ThrowsAmbiguousMatchException()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        var exception = Assert.Throws<AmbiguousMatchException>(() =>
            selector.GetBestMatch([Attr("SqlServer"), Attr("SqlServer")], x => x.Dialect));

        Assert.Contains("There are multiple attributes matching the dialect", exception.Message);
    }

    [Fact]
    public void GetBestMatch_NoMatches_ReturnsNull()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);

        Assert.Null(selector.GetBestMatch([Attr("MySql"), Attr("Oracle")], x => x.Dialect));
    }

    [Fact]
    public void GetBestMatch_Mixed_NegatedExcluded_PositiveMatchSelected()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var negated = Attr("!SqlServer");
        var positive = Attr("SqlServer");

        var result = selector.GetBestMatch([negated, positive], x => x.Dialect);

        Assert.Same(positive, result);
    }

    [Fact]
    public void GetBestMatch_Mixed_NegatedIncluded_OtherExcluded()
    {
        var selector = new DialectExpressionSelector(SqlServer2012Dialect.Instance);
        var negated = Attr("!MySql");
        var other = Attr("Oracle");

        var result = selector.GetBestMatch([negated, other], x => x.Dialect);

        Assert.Same(negated, result);
    }

    [Fact]
    public void GetBestMatch_MySqlDialect_SelectsMySqlMatch()
    {
        var selector = new DialectExpressionSelector(MySqlDialect.Instance);
        var mySql = Attr("MySql");
        var sqlServer = Attr("SqlServer");

        var result = selector.GetBestMatch([mySql, sqlServer], x => x.Dialect);

        Assert.Same(mySql, result);
    }

    [Fact]
    public void GetBestMatch_SqliteDialect_SelectsSqliteMatch()
    {
        var selector = new DialectExpressionSelector(SqliteDialect.Instance);
        var sqlite = Attr("Sqlite");
        var sqlServer = Attr("SqlServer");

        var result = selector.GetBestMatch([sqlite, sqlServer], x => x.Dialect);

        Assert.Same(sqlite, result);
    }
}
