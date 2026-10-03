using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task SqlServer_Migrations_And_Service_Calls_Succeed()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
        using var serverConnection = SqlIntegrationConnections.CreateConnection("SqlServer");

        testCleanup += () =>
        {
            SqlConnection.ClearAllPools();
            using var serverConnection = SqlIntegrationConnections.CreateConnection("SqlServer");
            serverConnection.Open();
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"IF DB_ID(N'{defaultDatabase}') IS NOT NULL BEGIN ALTER DATABASE [{defaultDatabase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{defaultDatabase}]; END;");
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"IF DB_ID(N'{northwindDatabase}') IS NOT NULL BEGIN ALTER DATABASE [{northwindDatabase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{northwindDatabase}]; END;");
        };
        
        var defaultConnectionString = WithSqlServerDatabase(serverConnection.ConnectionString, defaultDatabase);
        var northwindConnectionString = WithSqlServerDatabase(serverConnection.ConnectionString, northwindDatabase);

        await RunMigrationsAndServiceCall(defaultConnectionString, northwindConnectionString,
            "Microsoft.Data.SqlClient");
    }

    private static string WithSqlServerDatabase(string connectionString, string database)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = database
        };

        return builder.ConnectionString;
    }
}
