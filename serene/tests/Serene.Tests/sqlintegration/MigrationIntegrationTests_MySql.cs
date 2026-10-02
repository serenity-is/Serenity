using MySqlConnector;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task MySql_Migrations_And_Service_Calls_Succeed()
    {
        testCleanup += () =>
        {
            MySqlConnection.ClearAllPools();
            using var serverConnection = SqlIntegrationConnections.CreateConnection("MySql");
            serverConnection.Open();
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"DROP DATABASE IF EXISTS `{defaultDatabase}`");
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"DROP DATABASE IF EXISTS `{northwindDatabase}`");
        };

        DbProviderFactories.RegisterFactory("MySqlConnector", MySqlConnectorFactory.Instance);

        using var serverConnection = SqlIntegrationConnections.CreateConnection("MySql");
        var defaultConnectionString = WithMySqlDatabase(serverConnection.ConnectionString, defaultDatabase);
        var northwindConnectionString = WithMySqlDatabase(serverConnection.ConnectionString, northwindDatabase);

        await RunMigrationsAndServiceCall(defaultConnectionString, northwindConnectionString,
            "MySqlConnector");
    }

    private static string WithMySqlDatabase(string connectionString, string database)
    {
        var builder = new MySqlConnectionStringBuilder(connectionString)
        {
            Database = database
        };

        return builder.ConnectionString;
    }
}
