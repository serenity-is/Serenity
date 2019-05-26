using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Serenity.Data
{
    /// <summary>
    /// Contains a connection string, its key and provider name.
    /// </summary>
    public class ConnectionStringInfo
    {
        private static readonly string[] databaseNameKeys = new string[]
        {
            "Initial Catalog",
            "Database"
        };

        private static readonly Dictionary<string, ISqlDialect> dialectByProviderName = 
            new Dictionary<string, ISqlDialect>(StringComparer.OrdinalIgnoreCase)
            {
                { "System.Data.SqlClient", SqlServer2012Dialect.Instance },
                { "FirebirdSql.Data.FirebirdClient", FirebirdDialect.Instance },
                { "Npgsql", PostgresDialect.Instance },
                { "MySql.Data.MySqlClient", MySqlDialect.Instance },
                { "System.Data.SQLite", SqliteDialect.Instance },
                { "Microsoft.Data.SQLite", SqliteDialect.Instance },
                { "System.Data.OracleClient", OracleDialect.Instance },
                { "Oracle.ManagedDataAccess.Client", Oracle12cDialect.Instance }
            };

        private string databaseName;
        private ISqlDialect dialect;
        private DbProviderFactory providerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStringInfo"/> class.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">Name of the provider.</param>
        public ConnectionStringInfo(string connectionKey, string connectionString, string providerName)
        {
            this.ConnectionKey = connectionKey;
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
        }

        /// <summary>
        /// Tries to get the name of the database corresponding to a connection key
        /// by parsing the connection string, returns null if it can't be parsed.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        public string DatabaseName
        {
            get
            {
                if (databaseName == null)
                {
                    var csb = new DbConnectionStringBuilder();
                    csb.ConnectionString = ConnectionString;

                    foreach (var s in databaseNameKeys)
                        if (csb.ContainsKey(s))
                        {
                            databaseName = csb[s] as string ?? "";
                            return databaseName;
                        }

                    databaseName = "";
                    return databaseName;
                }

                return databaseName == "" ? null : databaseName;
            }
        }

        /// <summary>
        /// Gets or sets the dialect.
        /// </summary>
        /// <value>
        /// The dialect.
        /// </value>
        /// <exception cref="System.ArgumentException"></exception>
        public ISqlDialect Dialect
        {
            get
            {
                if (dialect != null)
                    return dialect;

                var connectionSettings = Config.TryGet<ConnectionSettings>();

                ConnectionSetting setting;
                if (connectionSettings != null && 
                    connectionSettings.TryGetValue(ConnectionKey, out setting) &&
                    !setting.Dialect.IsEmptyOrNull())
                {
                    var dialectType = Type.GetType("Serenity.Data." + setting.Dialect + "Dialect") ??
                        Type.GetType("Serenity.Data." + setting.Dialect) ??
                        Type.GetType(setting.Dialect);

                    if (dialectType == null)
                        throw new ArgumentException(String.Format("Dialect type {0} specified for connection key {1} is not found!",
                            setting.Dialect, ConnectionKey));

                    return (this.dialect = (ISqlDialect)Activator.CreateInstance(dialectType));
                }

                return (this.dialect = GetDialectByProviderName(ProviderName) ?? SqlSettings.DefaultDialect);
            }
            set
            {
                dialect = value;
            }
        }

        /// <summary>
        /// Gets the name of the dialect by provider name.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static ISqlDialect GetDialectByProviderName(string providerName)
        {
            ISqlDialect dialect;

            if (dialectByProviderName.TryGetValue(providerName, out dialect))
                return dialect;

            return null;
        }

        /// <summary>
        /// Gets the connection key.
        /// </summary>
        /// <value>
        /// The connection key.
        /// </value>
        public string ConnectionKey { get; private set; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string ProviderName { get; private set; }

        /// <summary>
        /// Gets the provider factory.
        /// </summary>
        /// <value>
        /// The provider factory.
        /// </value>
        public DbProviderFactory ProviderFactory
        {
            get
            {
                if (providerFactory == null)
                    providerFactory = SqlConnections.GetFactory(ProviderName);

                return providerFactory;
            }
        }

        /// <summary>
        /// Gets the connection string, obselete, use ConnectionString.
        /// </summary>
        /// <value>
        /// Connection string.
        /// </value>
        [Obsolete("Use ConnectionString")]
        public string Item1 { get { return ConnectionString; } }


        /// <summary>
        /// Gets the provider name, obsolete, use ProviderName.
        /// </summary>
        /// <value>
        /// The provider name.
        /// </value>
        [Obsolete("Use ProviderName")]
        public string Item2 { get { return ProviderName; } }

        [SettingScope("Application"), SettingKey("ConnectionSettings")]
        private class ConnectionSettings : Dictionary<string, ConnectionSetting>
        {

        }

        private class ConnectionSetting
        {
            public string Dialect { get; set; }
        }
    }
}