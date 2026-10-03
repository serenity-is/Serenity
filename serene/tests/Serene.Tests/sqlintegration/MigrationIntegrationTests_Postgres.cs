using Npgsql;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task Postgres_Migrations_And_Service_Calls_Succeed()
    {
        DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);

        using var serverConnection = SqlIntegrationConnections.CreateConnection("Postgres");
        
        testCleanup += () =>
        {
            using var serverConnection = SqlIntegrationConnections.CreateConnection("Postgres");
            serverConnection.Open();
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"DROP DATABASE IF EXISTS \"{defaultDatabase}\" WITH (FORCE)");
            SqlIntegrationConnections.ExecuteSql(serverConnection,
                $"DROP DATABASE IF EXISTS \"{northwindDatabase}\" WITH (FORCE)");
        };
        
        var defaultConnectionString = WithPostgresDatabase(serverConnection.ConnectionString, defaultDatabase);
        var northwindConnectionString = WithPostgresDatabase(serverConnection.ConnectionString, northwindDatabase);

        await RunMigrationsAndServiceCall(defaultConnectionString, northwindConnectionString, "Npgsql");
    }

    private static string WithPostgresDatabase(string connectionString, string database)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = database
        };

        return builder.ConnectionString;
    }
}
