using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace Serenity.TestUtils;

/// <summary>
/// Shared database connection and SQL command helpers for SQL integration tests.
/// Server connection strings can be supplied as SQLINTEGRATIONTEST_{SERVER}_CONNECTION.
/// Otherwise SQLINTEGRATIONTEST_{SERVER}_HOST / _PASSWORD / _USER override the shared
/// SQLINTEGRATIONTEST_HOST / _PASSWORD / _USER settings. Hosts may include a port, and SQL Server
/// hosts may include an instance (for example .\SQLExpress:1345). Defaults use common
/// local Docker ports: SQL Server 1433, PostgreSQL 5432, MySQL 3306, Oracle 1521, and
/// Firebird 3050. Defaults match the compose users/databases (SQL Server sa, PostgreSQL root,
/// MySQL root, Oracle SYSTEM, and Firebird SYSDBA), Oracle XEPDB1 service, and Firebird
/// /var/lib/firebird/data/test.fdb database; customize with a full connection string if needed.
/// </summary>
internal static class SqlIntegrationConnections
{
    public static DbConnection CreateConnection(string provider)
    {
        ArgumentException.ThrowIfNullOrEmpty(provider);

        if (provider == "Sqlite")
            return SqliteFactory.Instance.CreateConnection() is { } sqlite
                ? SetConnectionString(sqlite, GetEnvironment("SQLINTEGRATIONTEST_SQLITE_CONNECTION") ?? "Data Source=:memory:")
                : throw new InvalidOperationException("Could not create a SQLite connection.");

        var prefix = "SQLINTEGRATIONTEST_" + provider.ToUpperInvariant();
        var connectionString = GetEnvironment(prefix + "_CONNECTION");
        if (connectionString is null)
        {
            var password = GetEnvironment(prefix + "_PASSWORD") ?? GetEnvironment("SQLINTEGRATIONTEST_PASSWORD");
            Assert.SkipWhen(string.IsNullOrEmpty(password), $"Set {prefix}_CONNECTION or {prefix}_PASSWORD / SQLINTEGRATIONTEST_PASSWORD to run this integration test.");

            var host = GetEnvironment(prefix + "_HOST") ?? GetEnvironment("SQLINTEGRATIONTEST_HOST") ?? "localhost";
            var user = GetEnvironment(prefix + "_USER") ?? GetEnvironment("SQLINTEGRATIONTEST_USER");
            connectionString = BuildConnectionString(provider, host, password!, user);
        }

        var dbConnection = provider switch
        {
            "SqlServer" => SqlClientFactory.Instance.CreateConnection(),
            "Postgres" => NpgsqlFactory.Instance.CreateConnection(),
            "MySql" => MySqlConnectorFactory.Instance.CreateConnection(),
            "Oracle" => OracleClientFactory.Instance.CreateConnection(),
            "Firebird" => FirebirdClientFactory.Instance.CreateConnection(),
            _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown database provider.")
        } ?? throw new InvalidOperationException($"Could not create a {provider} connection.");

        return SetConnectionString(dbConnection, connectionString);
    }

    public static void ExecuteSql(DbConnection connection, string sql)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(sql);

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandTimeout = 15;
        command.ExecuteNonQuery();
    }

    private static string BuildConnectionString(string provider, string host, string password, string? user)
    {
        var (hostName, port) = SplitHostAndPort(host);
        var builder = new DbConnectionStringBuilder();

        switch (provider)
        {
            case "SqlServer":
                builder["Server"] = port.HasValue ? $"{hostName},{port}" : host;
                builder["Database"] = "master";
                builder["User ID"] = user ?? "sa";
                builder["Password"] = password;
                builder["Encrypt"] = false;
                builder["TrustServerCertificate"] = true;
                builder["Connect Timeout"] = 5;
                break;

            case "Postgres":
                builder["Host"] = hostName;
                builder["Port"] = port ?? 5432;
                builder["Database"] = "root";
                builder["Username"] = user ?? "root";
                builder["Password"] = password;
                builder["Timeout"] = 5;
                break;

            case "MySql":
                builder["Server"] = hostName;
                builder["Port"] = port ?? 3306;
                builder["Database"] = "mysql";
                builder["User ID"] = user ?? "root";
                builder["Password"] = password;
                builder["Connection Timeout"] = 5;
                break;

            case "Oracle":
                builder["Data Source"] = $"//{hostName}:{port ?? 1521}/XEPDB1";
                builder["User Id"] = user ?? "system";
                builder["Password"] = password;
                builder["Connection Timeout"] = 5;
                break;

            case "Firebird":
                builder["DataSource"] = hostName;
                builder["Port"] = port ?? 3050;
                builder["Database"] = "/var/lib/firebird/data/test.fdb";
                builder["User"] = user ?? "sysdba";
                builder["Password"] = password;
                builder["Connection Timeout"] = 5;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, "Unknown database provider.");
        }

        return builder.ConnectionString;
    }

    private static (string Host, int? Port) SplitHostAndPort(string host)
    {
        var separator = host.LastIndexOf(':');
        if (separator > 0 && int.TryParse(host.AsSpan(separator + 1), out var port))
            return (host[..separator], port);

        return (host, null);
    }

    private static TConnection SetConnectionString<TConnection>(TConnection connection, string connectionString)
        where TConnection : DbConnection
    {
        connection.ConnectionString = connectionString;
        return connection;
    }

    private static string? GetEnvironment(string name)
        => Environment.GetEnvironmentVariable(name)?.Trim() is { Length: > 0 } value ? value : null;
}
