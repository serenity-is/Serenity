using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests : IDisposable
{
    [Fact]
    public async Task Oracle_Migrations_And_Service_Calls_Succeed()
    {
        var suffix = Guid.NewGuid().ToString("N")[..16].ToUpperInvariant();
        var defaultUser = "SDEF" + suffix;
        var northwindUser = "SNW" + suffix;
        var password = "S" + Guid.NewGuid().ToString("N")[..12] + "Aa1";

        DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", OracleClientFactory.Instance);

        using var serverConnection = SqlIntegrationConnections.CreateConnection("Oracle");
        serverConnection.Open();

        var privileges = serverConnection.Query<string>("SELECT PRIVILEGE FROM SESSION_PRIVS")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!privileges.Contains("CREATE USER") || !privileges.Contains("DROP USER") ||
            !privileges.Contains("GRANT ANY PRIVILEGE"))
            Assert.Skip("Oracle migration integration requires an account with CREATE USER, DROP USER, and GRANT ANY PRIVILEGE privileges. Set SQLINTEGRATIONTEST_ORACLE_CONNECTION to an Oracle admin connection string.");

        testCleanup += () =>
        {
            OracleConnection.ClearAllPools();
            using var cleanupConnection = SqlIntegrationConnections.CreateConnection("Oracle");
            cleanupConnection.Open();
            TryDropOracleUser(cleanupConnection, defaultUser);
            TryDropOracleUser(cleanupConnection, northwindUser);
        };

        CreateOracleUser(serverConnection, defaultUser, password);
        CreateOracleUser(serverConnection, northwindUser, password);

        var connectionString = serverConnection.ConnectionString;
        await RunMigrationsAndServiceCall(
            WithOracleUser(connectionString, defaultUser, password),
            WithOracleUser(connectionString, northwindUser, password),
            "Oracle.ManagedDataAccess.Client");
    }

    private static string WithOracleUser(string connectionString, string user, string password)
    {
        var builder = new OracleConnectionStringBuilder(connectionString)
        {
            UserID = user,
            Password = password
        };

        return builder.ConnectionString;
    }

    private static void CreateOracleUser(DbConnection connection, string user, string password)
    {
        SqlIntegrationConnections.ExecuteSql(connection, $"CREATE USER {user} IDENTIFIED BY {password}");
        SqlIntegrationConnections.ExecuteSql(connection,
            $"GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE, CREATE TRIGGER, CREATE VIEW, CREATE PROCEDURE TO {user}");
        SqlIntegrationConnections.ExecuteSql(connection, $"GRANT UNLIMITED TABLESPACE TO {user}");
    }

    private static void TryDropOracleUser(DbConnection connection, string user)
    {
        try
        {
            SqlIntegrationConnections.ExecuteSql(connection, $"DROP USER {user} CASCADE");
        }
        catch (OracleException ex) when (ex.Number is 1918 or 1031)
        {
            // The test may have failed before creating this user or before it could grant DROP USER.
        }
    }
}
