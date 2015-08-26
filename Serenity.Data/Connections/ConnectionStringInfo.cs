
using System;
using System.Data.Common;
namespace Serenity.Data
{
    public class ConnectionStringInfo
    {
        private string databaseName;

        public ConnectionStringInfo(string connectionString, string providerName, DbProviderFactory providerFactory)
        {
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
                    databaseName = csb["Initial Catalog"] as string ?? csb["Database"] as string ?? "";
                }

                return databaseName == "" ? null : databaseName;
            }
        }

        public string ConnectionString { get; private set; }
        public string ProviderName { get; private set; }
        public DbProviderFactory ProviderFactory { get; private set; }

        [Obsolete("Use ConnectionString")]
        public string Item1 { get { return ConnectionString; } }
        [Obsolete("Use ProviderName")]
        public string Item2 { get { return ProviderName; } }
    }
}