
using System.Data.Common;
namespace Serenity.Data
{
    public class ConnectionStringInfo
    {
        public ConnectionStringInfo(string connectionString, string providerName, DbProviderFactory providerFactory)
        {
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
            this.ProviderFactory = providerFactory;
        }

        public string ConnectionString { get; private set; }
        public string ProviderName { get; private set; }
        public DbProviderFactory ProviderFactory { get; private set; }
    }
}