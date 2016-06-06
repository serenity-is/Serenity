namespace Serenity.Data
{
    public interface ISqlDialect
    {
        bool CanUseOffsetFetch { get; }
        bool CanUseRowNumber { get; }
        bool CanUseSkipKeyword { get; }
        char CloseQuote { get; }
        string ConcatOperator { get; }
        string DateFormat { get; }
        string DateTimeFormat { get; }
        bool IsLikeCaseSensitive { get; }
        bool MultipleResultsets { get; }
        bool NeedsExecuteBlockStatement { get; }
        string OffsetFormat { get; }
        string OffsetFetchFormat { get; }
        char OpenQuote { get; }
        string QuoteColumnAlias(string s);
        string QuoteIdentifier(string s);
        string QuoteUnicodeString(string s);
        string ScopeIdentityExpression { get; }
        string SkipKeyword { get; }
        string TakeKeyword { get; }
        string TimeFormat { get; }
        bool UseDateTime2 { get; }
        bool UseReturningIdentity { get; }
        bool UseReturningIntoVar { get; }
        bool UseScopeIdentity { get; }
        bool UseTakeAtEnd { get; }
        bool UseRowNum { get; }
        char ParameterPrefix { get; }
    }
}