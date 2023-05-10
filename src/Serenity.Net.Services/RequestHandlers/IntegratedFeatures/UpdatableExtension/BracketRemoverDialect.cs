namespace Serenity.Services;

internal class BracketRemoverDialect : ISqlDialect
{
    public static readonly BracketRemoverDialect Instance = new BracketRemoverDialect();

    public bool CanUseOffsetFetch => throw new NotImplementedException();

    public bool CanUseRowNumber => throw new NotImplementedException();

    public bool CanUseSkipKeyword => throw new NotImplementedException();

    public char CloseQuote => '\x1';

    public string ConcatOperator => throw new NotImplementedException();

    public string DateFormat => throw new NotImplementedException();

    public string DateTimeFormat => throw new NotImplementedException();

    public bool IsLikeCaseSensitive => throw new NotImplementedException();

    public bool MultipleResultsets => throw new NotImplementedException();

    public bool NeedsBoolWorkaround => throw new NotImplementedException();

    public bool NeedsExecuteBlockStatement => throw new NotImplementedException();

    public string OffsetFetchFormat => throw new NotImplementedException();

    public string OffsetFormat => throw new NotImplementedException();

    public char OpenQuote => '\x1';

    public char ParameterPrefix => throw new NotImplementedException();

    public string ScopeIdentityExpression => throw new NotImplementedException();

    public string ServerType => throw new NotImplementedException();

    public string SkipKeyword => throw new NotImplementedException();

    public string TakeKeyword => throw new NotImplementedException();

    public string TimeFormat => throw new NotImplementedException();

    public string UnionKeyword(SqlUnionType unionType)
    {
        throw new NotImplementedException();
    }

    public bool UseDateTime2 => throw new NotImplementedException();

    public bool UseReturningIdentity => throw new NotImplementedException();

    public bool UseReturningIntoVar => throw new NotImplementedException();

    public bool UseRowNum => throw new NotImplementedException();

    public bool UseScopeIdentity => throw new NotImplementedException();

    public bool UseTakeAtEnd => throw new NotImplementedException();

    public string QuoteColumnAlias(string s)
    {
        return s;
    }

    public string QuoteIdentifier(string s)
    {
        return s;
    }

    public string QuoteUnicodeString(string s)
    {
        throw new NotImplementedException();
    }
}