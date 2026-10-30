namespace Serenity.Testing;

public class MockSqlDialect(bool? autoQuotedIdentifiers = null) : SqlServer2012Dialect, ISqlDialect
{
    public bool? AutoQuotedIdentifiers { get; set; } = autoQuotedIdentifiers;
}
