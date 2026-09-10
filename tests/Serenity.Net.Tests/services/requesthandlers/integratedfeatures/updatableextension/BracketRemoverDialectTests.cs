namespace Serenity.Services;

public class BracketRemoverDialectTests
{
    [Fact]
    public void Quotes_Return_Input_Unchanged()
    {
        var dialect = BracketRemoverDialect.Instance;

        Assert.Equal('\x1', dialect.OpenQuote);
        Assert.Equal('\x1', dialect.CloseQuote);
        Assert.Equal("abc", dialect.QuoteIdentifier("abc"));
        Assert.Equal("abc", dialect.QuoteColumnAlias("abc"));
    }

    [Fact]
    public void Unimplemented_Members_Throw()
    {
        var dialect = BracketRemoverDialect.Instance;

        Assert.Throws<NotImplementedException>(() => dialect.CanUseOffsetFetch);
        Assert.Throws<NotImplementedException>(() => dialect.CanUseRowNumber);
        Assert.Throws<NotImplementedException>(() => dialect.CanUseSkipKeyword);
        Assert.Throws<NotImplementedException>(() => dialect.ConcatOperator);
        Assert.Throws<NotImplementedException>(() => dialect.DateFormat);
        Assert.Throws<NotImplementedException>(() => dialect.DateTimeFormat);
        Assert.Throws<NotImplementedException>(() => dialect.IsLikeCaseSensitive);
        Assert.Throws<NotImplementedException>(() => dialect.MultipleResultsets);
        Assert.Throws<NotImplementedException>(() => dialect.NeedsBoolWorkaround);
        Assert.Throws<NotImplementedException>(() => dialect.NeedsExecuteBlockStatement);
        Assert.Throws<NotImplementedException>(() => dialect.OffsetFetchFormat);
        Assert.Throws<NotImplementedException>(() => dialect.OffsetFormat);
        Assert.Throws<NotImplementedException>(() => dialect.ParameterPrefix);
        Assert.Throws<NotImplementedException>(() => dialect.ScopeIdentityExpression);
        Assert.Throws<NotImplementedException>(() => dialect.ServerType);
        Assert.Throws<NotImplementedException>(() => dialect.SkipKeyword);
        Assert.Throws<NotImplementedException>(() => dialect.TakeKeyword);
        Assert.Throws<NotImplementedException>(() => dialect.TimeFormat);
        Assert.Throws<NotImplementedException>(() => dialect.UnionKeyword(SqlUnionType.Union));
        Assert.Throws<NotImplementedException>(() => dialect.UseDateTime2);
        Assert.Throws<NotImplementedException>(() => dialect.UseReturningIdentity);
        Assert.Throws<NotImplementedException>(() => dialect.UseReturningIntoVar);
        Assert.Throws<NotImplementedException>(() => dialect.UseRowNum);
        Assert.Throws<NotImplementedException>(() => dialect.UseScopeIdentity);
        Assert.Throws<NotImplementedException>(() => dialect.UseTakeAtEnd);
        Assert.Throws<NotImplementedException>(() => dialect.QuoteUnicodeString("x"));
    }
}
