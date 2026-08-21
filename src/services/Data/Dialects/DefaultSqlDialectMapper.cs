namespace Serenity.Data;

/// <summary>
/// Default implementation of <see cref="ISqlDialectMapper"/> that maps well-known
/// provider names and dialect type names to their corresponding <see cref="ISqlDialect"/>.
/// </summary>
public class DefaultSqlDialectMapper : ISqlDialectMapper
{
    private static Dictionary<string, ISqlDialect> DialectByProviderName =>
       new(StringComparer.OrdinalIgnoreCase)
       {
            { "System.Data.SqlClient", SqlServer2012Dialect.Instance },
            { "Microsoft.Data.SqlClient", SqlServer2012Dialect.Instance },
            { "FirebirdSql.Data.FirebirdClient", FirebirdDialect.Instance },
            { "Npgsql", PostgresDialect.Instance },
            { "MySql.Data.MySqlClient", MySqlDialect.Instance },
            { "MySqlConnector", MySqlDialect.Instance },
            { "System.Data.SQLite", SqliteDialect.Instance },
            { "Microsoft.Data.SQLite", SqliteDialect.Instance },
            { "System.Data.OracleClient", OracleDialect.Instance },
            { "Oracle.ManagedDataAccess.Client", OracleDialect.Instance }
       };

    /// <summary>
    /// Returns the dialect for a dialect or provider name, or <c>null</c> if none is found.
    /// </summary>
    /// <param name="dialectOrProviderName">The dialect name or provider name.</param>
    /// <returns>The matching <see cref="ISqlDialect"/>, or <c>null</c> if no match is found.</returns>
    public ISqlDialect TryGet(string dialectOrProviderName)
    {
        if (string.IsNullOrEmpty(dialectOrProviderName))
            return null;

        if (DialectByProviderName.TryGetValue(dialectOrProviderName, out ISqlDialect dialect))
            return dialect;

        var dialectType = Type.GetType("Serenity.Data." + dialectOrProviderName + "Dialect") ??
            Type.GetType("Serenity.Data." + dialectOrProviderName) ??
            Type.GetType(dialectOrProviderName);

        if (dialectType != null)
            return Activator.CreateInstance(dialectType) as ISqlDialect;

        return null;
    }
}
