namespace Serenity.TestUtils;

/// <summary>
/// Names of xUnit test collections used by SQL integration tests.
/// </summary>
public static class SqlIntegrationCollections
{
    /// <summary>
    /// Collection that serializes Firebird integration tests, since concurrent DDL
    /// on the shared Firebird database can deadlock.
    /// </summary>
    public const string Firebird = "FirebirdIntegration";
}

/// <summary>
/// Serializes Firebird integration tests so that concurrent DDL against the shared
/// Firebird database does not deadlock.
/// </summary>
[CollectionDefinition(SqlIntegrationCollections.Firebird, DisableParallelization = true)]
public class FirebirdIntegrationCollection
{
}
