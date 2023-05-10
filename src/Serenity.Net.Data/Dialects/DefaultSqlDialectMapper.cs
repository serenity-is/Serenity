namespace Serenity.Data;

/// <summary>
/// Sql Dialect mapper
/// </summary>
public class DefaultSqlDialectMapper : ISqlDialectMapper
{
    private static Dictionary<string, ISqlDialect> DialectByProviderName =>
       new Dictionary<string, ISqlDialect>(StringComparer.OrdinalIgnoreCase)
       {
            { "System.Data.SqlClient", SqlServer2012Dialect.Instance },
            { "Microsoft.Data.SqlClient", SqlServer2012Dialect.Instance },
            { "FirebirdSql.Data.FirebirdClient", FirebirdDialect.Instance },
            { "Npgsql", PostgresDialect.Instance },
            { "MySql.Data.MySqlClient", MySqlDialect.Instance },
            { "System.Data.SQLite", SqliteDialect.Instance },
            { "Microsoft.Data.SQLite", SqliteDialect.Instance },
            { "System.Data.OracleClient", OracleDialect.Instance },
            { "Oracle.ManagedDataAccess.Client", OracleDialect.Instance }
       };

    /// <summary>
    /// Returns dialect for a dialect or provider name
    /// </summary>
    /// <param name="dialectOrProviderName">The dialect name or provider name</param>
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
