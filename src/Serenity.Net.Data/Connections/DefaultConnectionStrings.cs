using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Data
{
    /// <summary>
    /// Default connection string source
    /// </summary>
    public class DefaultConnectionStrings : IConnectionStrings
    {
        private readonly IOptions<ConnectionStringOptions> options;

        /// <summary>
        /// Creates a new instance of DefaultConnectionStringSource
        /// </summary>
        /// <param name="options">Connection string options</param>
        public DefaultConnectionStrings(IOptions<ConnectionStringOptions> options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        private static readonly Dictionary<string, ISqlDialect> dialectByProviderName =
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
        /// Determines dialect for a connection
        /// </summary>
        /// <param name="connectionKey">Connection key</param>
        /// <param name="entry">Connection entry</param>
        protected virtual ISqlDialect DetermineDialect(string connectionKey, ConnectionStringEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (entry.DialectInstance != null)
                return entry.DialectInstance;

            if (!string.IsNullOrEmpty(entry.Dialect))
            {
                var dialectType = Type.GetType("Serenity.Data." + entry.Dialect + "Dialect") ??
                    Type.GetType("Serenity.Data." + entry.Dialect) ??
                    Type.GetType(entry.Dialect);

                if (dialectType == null)
                    throw new ArgumentException($"Dialect type {entry.Dialect} specified for connection {connectionKey} is not found!");

                return (ISqlDialect)Activator.CreateInstance(dialectType);
            }

            return dialectByProviderName.TryGetValue(entry.ProviderName, out ISqlDialect dialect) ?
                dialect : SqlSettings.DefaultDialect;
        }

        private readonly ConcurrentDictionary<string, ConnectionStringInfo> byKey = new ConcurrentDictionary<string, ConnectionStringInfo>();

        /// <summary>
        /// Gets a connection string by its key
        /// </summary>
        /// <param name="connectionKey">Connection key</param>
        /// <returns>Connection string or null if not found</returns>
        public IConnectionString TryGetConnectionString(string connectionKey)
        {
            if (byKey.TryGetValue(connectionKey, out ConnectionStringInfo info))
                return info;

            if (!options.Value.TryGetValue(connectionKey, out ConnectionStringEntry entry))
                return null;

            info = new ConnectionStringInfo(connectionKey, entry.ConnectionString, entry.ProviderName,
                DetermineDialect(connectionKey, entry));

            byKey[connectionKey] = info;
            return info;
        }

        /// <summary>
        /// Lists all known connections strings
        /// </summary>
        /// <returns>List of all registered connections</returns>
        public IEnumerable<IConnectionString> ListConnectionStrings()
        {
            return options.Value.Keys.Select(x => TryGetConnectionString(x));
        }
    }
}