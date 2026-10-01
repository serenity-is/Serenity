using System.Reflection;

namespace Serenity.Data;

/// <summary>
/// Default implementation of <see cref="ISqlDialectMapper"/> that maps well-known
/// provider names and dialect type names to their corresponding <see cref="ISqlDialect"/>.
/// </summary>
public class DefaultSqlDialectMapper : ISqlDialectMapper
{
    private static readonly Dictionary<string, ISqlDialect> DialectByName =
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
            { "Oracle.ManagedDataAccess.Client", OracleDialect.Instance },
            // Short names with no matching type ("SqlServer" has no SqlServerDialect type)
            { "SqlServer", SqlServer2012Dialect.Instance },
            { "PostgreSQL", PostgresDialect.Instance }
       };

    // Cache for Type.GetType resolutions. Nulls included (misses shouldn't
    // pay reflection on every call); Dictionary allows null values.
    private static readonly Dictionary<string, ISqlDialect?> DialectByTypeName =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the dialect for a dialect or provider name, or <c>null</c> if none is found.
    /// </summary>
    /// <param name="dialectOrProviderName">The dialect name or provider name.</param>
    /// <returns>The matching <see cref="ISqlDialect"/>, or <c>null</c> if no match is found.</returns>
    public ISqlDialect? TryGet(string? dialectOrProviderName)
    {
        if (string.IsNullOrEmpty(dialectOrProviderName))
            return null;

        if (DialectByName.TryGetValue(dialectOrProviderName, out ISqlDialect? dialect))
            return dialect;

        lock (DialectByTypeName)
        {
            if (DialectByTypeName.TryGetValue(dialectOrProviderName, out dialect))
                return dialect;

            var dialectType = Type.GetType(dialectOrProviderName, false, true) ??
                Type.GetType("Serenity.Data." + dialectOrProviderName + "Dialect", false, true) ??
                Type.GetType("Serenity.Data." + dialectOrProviderName, false, true) ??
                null;

            dialect = dialectType is null ? null : CreateDialect(dialectType);
            DialectByTypeName[dialectOrProviderName] = dialect;
            return dialect;
        }
    }

    private static ISqlDialect? CreateDialect(Type dialectType)
    {
        // Prefer the shared singleton when the type itself declares one (all
        // built-in dialects do, as a static Instance field or property)
        // instead of allocating per call.
        var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        if (dialectType.GetField("Instance", flags)?.GetValue(null) is ISqlDialect instance)
            return instance;

        if (dialectType.GetProperty("Instance", flags)?.GetValue(null) is ISqlDialect propertyInstance)
            return propertyInstance;

        if (dialectType.GetConstructor(Type.EmptyTypes) is null)
            return null;

        try
        {
            return Activator.CreateInstance(dialectType) as ISqlDialect;
        }
        catch (MissingMethodException)
        {
            return null;
        }
    }
}
