using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Serenity.Data
{
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
                { "System.Data.OracleClient", OracleDialect.Instance },
                { "Oracle.ManagedDataAccess.Client", OracleDialect.Instance }
            };

        private string databaseName;
        private ISqlDialect dialect;

        public ConnectionStringInfo(string connectionKey, string connectionString, string providerName, DbProviderFactory providerFactory)
        {
            this.ConnectionKey = connectionKey;
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
            this.ProviderFactory = providerFactory;
        }

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

        public static ISqlDialect GetDialectByProviderName(string providerName)
        {
            ISqlDialect dialect;

            if (dialectByProviderName.TryGetValue(providerName, out dialect))
                return dialect;

            return null;
        }

        public string ConnectionKey { get; private set; }
        public string ConnectionString { get; private set; }
        public string ProviderName { get; private set; }
        public DbProviderFactory ProviderFactory { get; private set; }

        [Obsolete("Use ConnectionString")]
        public string Item1 { get { return ConnectionString; } }
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