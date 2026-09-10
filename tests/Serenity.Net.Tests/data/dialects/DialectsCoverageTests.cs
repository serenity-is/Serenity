namespace Serenity.Data;

public class DialectsCoverageTests
{
    public static IEnumerable<object[]> DialectInstances()
    {
        yield return [SqlServer2000Dialect.Instance];
        yield return [SqlServer2005Dialect.Instance];
        yield return [SqlServer2008Dialect.Instance];
        yield return [SqlServer2012Dialect.Instance];
        yield return [MySqlDialect.Instance];
        yield return [PostgresDialect.Instance];
        yield return [SqliteDialect.Instance];
        yield return [OracleDialect.Instance];
        yield return [Oracle12cDialect.Instance];
        yield return [FirebirdDialect.Instance];
    }

    private static T Probe<T>(Func<T> func)
    {
        try
        {
            return func();
        }
        catch (NotImplementedException)
        {
            return default!;
        }
    }

    [Theory]
    [MemberData(nameof(DialectInstances))]
    public void Dialect_Members_Are_Accessible(ISqlDialect dialect)
    {
        Assert.NotNull(dialect);

        _ = dialect.AutoQuotedIdentifiers;
        _ = dialect.CanUseConcat;
        _ = dialect.CanUseOffsetFetch;
        _ = dialect.CanUseRowNumber;
        _ = dialect.CanUseSkipKeyword;
        _ = dialect.CloseQuote;
        _ = Probe(() => dialect.ConcatOperator);
        _ = Probe(() => dialect.DateFormat);
        _ = Probe(() => dialect.DateTimeFormat);
        _ = dialect.IsLikeCaseSensitive;
        _ = dialect.MultipleResultsets;
        _ = dialect.NeedsExecuteBlockStatement;
        _ = dialect.NeedsBoolWorkaround;
        _ = Probe(() => dialect.OffsetFormat);
        _ = Probe(() => dialect.OffsetFetchFormat);
        _ = dialect.OpenQuote;
        _ = Probe(() => dialect.QuoteColumnAlias("a"));
        _ = dialect.QuoteIdentifier("");
        _ = dialect.QuoteIdentifier("a");
        _ = dialect.QuoteIdentifier("" + dialect.OpenQuote + "x" + dialect.CloseQuote);
        _ = Probe(() => dialect.QuoteIdentifier("a b"));
        _ = Probe(() => dialect.QuoteIdentifier("_x"));
        _ = Probe(() => dialect.QuoteColumnAlias(""));
        _ = Probe(() => dialect.QuoteColumnAlias("\"X\""));
        _ = Probe(() => dialect.QuoteUnicodeString("ab"));
        _ = Probe(() => dialect.QuoteUnicodeString("a'b"));
        _ = Probe(() => dialect.ScopeIdentityExpression);
        _ = dialect.ServerType;
        _ = Probe(() => dialect.SkipKeyword);
        _ = Probe(() => dialect.TakeKeyword);
        _ = Probe(() => dialect.TimeFormat);
        _ = dialect.UnionKeyword(SqlUnionType.Union);
        _ = dialect.UnionKeyword(SqlUnionType.UnionAll);
        _ = Probe(() => dialect.UnionKeyword(SqlUnionType.Intersect));
        _ = Probe(() => dialect.UnionKeyword(SqlUnionType.IntersectAll));
        _ = Probe(() => dialect.UnionKeyword(SqlUnionType.Except));
        _ = Probe(() => dialect.UnionKeyword(SqlUnionType.ExceptAll));
        _ = Probe(() => dialect.UnionKeyword((SqlUnionType)999));
        _ = dialect.UseDateTime2;
        _ = dialect.UseReturningIdentity;
        _ = dialect.UseReturningIntoVar;
        _ = dialect.UseScopeIdentity;
        _ = dialect.UseTakeAtEnd;
        _ = dialect.UseRowNum;
        _ = dialect.ParameterPrefix;
        _ = dialect.IsReservedKeyword("SELECT");
        _ = dialect.IsReservedKeyword("definitely_not_a_keyword_xyz");
    }

    [Fact]
    public void Default_IsReservedKeyword_Uses_Any_Dialect_Keywords()
    {
        ISqlDialect dialect = new DefaultIsReservedKeywordDialect();

        Assert.True(dialect.IsReservedKeyword("SELECT"));
    }

    private sealed class DefaultIsReservedKeywordDialect : ISqlDialect
    {
        public bool CanUseOffsetFetch => false;
        public bool CanUseRowNumber => false;
        public bool CanUseSkipKeyword => false;
        public char CloseQuote => ']';
        public string ConcatOperator => "+";
        public string DateFormat => "";
        public string DateTimeFormat => "";
        public bool IsLikeCaseSensitive => false;
        public bool MultipleResultsets => true;
        public bool NeedsExecuteBlockStatement => false;
        public bool NeedsBoolWorkaround => false;
        public string OffsetFormat => "";
        public string OffsetFetchFormat => "";
        public char OpenQuote => '[';
        public string QuoteColumnAlias(string s) => s;
        public string QuoteIdentifier(string s) => s;
        public string QuoteUnicodeString(string s) => s;
        public string ScopeIdentityExpression => "";
        public string ServerType => "Default";
        public string SkipKeyword => "";
        public string TakeKeyword => "TOP";
        public string TimeFormat => "";
        public string UnionKeyword(SqlUnionType unionType) => "UNION";
        public bool UseDateTime2 => false;
        public bool UseReturningIdentity => false;
        public bool UseReturningIntoVar => false;
        public bool UseScopeIdentity => false;
        public bool UseTakeAtEnd => false;
        public bool UseRowNum => false;
        public char ParameterPrefix => '@';
    }
}
