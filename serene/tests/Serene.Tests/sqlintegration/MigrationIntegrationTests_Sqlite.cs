using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task Sqlite_Migrations_And_Service_Calls_Succeed()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

        var defaultConnectionString = $"Data Source={defaultDatabase};Mode=Memory;Cache=Shared";
        var northwindConnectionString = $"Data Source={northwindDatabase};Mode=Memory;Cache=Shared";

        // Keep the named shared-memory databases alive while migrations and services
        // create and dispose their own connections.
        using var defaultAnchor = new SqliteConnection(defaultConnectionString);
        using var northwindAnchor = new SqliteConnection(northwindConnectionString);
        defaultAnchor.Open();
        northwindAnchor.Open();

        await RunMigrationsAndServiceCall(defaultConnectionString, northwindConnectionString,
            "Microsoft.Data.Sqlite");
    }
}
