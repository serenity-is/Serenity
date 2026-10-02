using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task Firebird_Migrations_And_Service_Calls_Succeed()
    {
        var serverConnection = SqlIntegrationConnections.CreateConnection("Firebird");
        var defaultConnectionString = WithFirebirdDatabase(serverConnection.ConnectionString, defaultDatabase);
        var northwindConnectionString = WithFirebirdDatabase(serverConnection.ConnectionString, northwindDatabase);
        serverConnection.Dispose();

        testCleanup += () =>
        {
            FbConnection.ClearAllPools();
            TryDropDatabase(defaultConnectionString);
            TryDropDatabase(northwindConnectionString);
        };

        DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdClientFactory.Instance);
        FbConnection.CreateDatabase(defaultConnectionString, 8192, true, false);
        FbConnection.CreateDatabase(northwindConnectionString, 8192, true, false);

        await RunMigrationsAndServiceCall(defaultConnectionString, northwindConnectionString,
            "FirebirdSql.Data.FirebirdClient");
    }

    private static string WithFirebirdDatabase(string connectionString, string database)
    {
        var builder = new FbConnectionStringBuilder(connectionString);
        var databasePath = builder.Database;
        var separator = Math.Max(databasePath.LastIndexOf('/'), databasePath.LastIndexOf('\\'));
        if (separator < 0)
            throw new InvalidOperationException("Firebird database path must include a directory.");

        builder.Database = databasePath[..(separator + 1)] + database + ".fdb";
        return builder.ConnectionString;
    }

    private static void TryDropDatabase(string connectionString)
    {
        try
        {
            FbConnection.DropDatabase(connectionString);
        }
        catch
        {
            // The migration may have failed before the database was created.
        }
    }
}
