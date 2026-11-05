using FirebirdSql.Data.FirebirdClient;
using MySqlConnector;
using Npgsql;
using Serene.Administration;
using Serenity.Extensions;
using System.Data.Common;

namespace Serene.Migrations;

public sealed partial class MigrationIntegrationTests
{
    /// <summary>
    /// Runs the expression of the migration on the database, as UPPER and REPLACE can depend on
    /// its collation, and checks that the result is the same with what the code calculates.
    /// </summary>
    private static void VerifyBackfillMatchesCode(IDbConnection connection)
    {
        var dialect = connection.GetDialect();
        var table = dialect.QuoteIdentifier("Users");
        var normalizedColumn = dialect.QuoteIdentifier("NormalizedUsername");
        var usernameColumn = dialect.QuoteIdentifier("Username");
        var idColumn = dialect.QuoteIdentifier("UserId");

        var expression = usernameColumn;
        foreach (var letter in new[] { "ı", "İ", "i" })
            expression = $"REPLACE({expression}, {dialect.QuoteUnicodeString(letter)}, 'I')";
        expression = $"UPPER({expression})";

        var usernames = new[] { "isik1", "ISIK2", "Isik3", "ısık4", "İSİK5", "MixedCase6", "user.name7@example.com" };
        var ids = new List<int>();
        try
        {
            foreach (var username in usernames)
            {
                ids.Add(Convert.ToInt32(connection.InsertAndGetID(new UserRow
                {
                    Username = username,
                    NormalizedUsername = "TMP" + ids.Count,
                    DisplayName = username,
                    Source = "site",
                    PasswordHash = "x",
                    PasswordSalt = "x",
                    InsertDate = DateTime.Now,
                    InsertUserId = 1,
                    IsActive = 1
                })));

                connection.Execute($"UPDATE {table} SET {normalizedColumn} = {expression} WHERE {idColumn} = @id",
                    new { id = ids[^1] });

                var actual = connection.Query<string>($"SELECT {normalizedColumn} FROM {table} WHERE {idColumn} = @id",
                    new { id = ids[^1] }).Single();

                Assert.Equal(BaseUserRetrieveService.NormalizeUsername(username), actual);
            }
        }
        finally
        {
            foreach (var id in ids)
                connection.Execute($"DELETE FROM {table} WHERE {idColumn} = @id", new { id });
        }
    }

    /// <summary>
    /// Creates a real column with each mapped collation, so that a name the engine doesn't
    /// accept fails, and checks an exact mapping compares the way it is supposed to.
    /// </summary>
    private static void VerifyColumnCollations(IDbConnection connection)
    {
        var dialect = connection.GetDialect();
        var serverType = dialect.ServerType;

        string columnType;
        if (serverType.StartsWith("SqlServer", StringComparison.OrdinalIgnoreCase) ||
            serverType.StartsWith("MySql", StringComparison.OrdinalIgnoreCase))
            columnType = "NVARCHAR(50)";
        else if (serverType.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase))
            columnType = "TEXT";
        else if (serverType.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase))
            return;
        else
            columnType = "VARCHAR(50)";

        // stored, compared to, whether it should match
        var checks = new (string Comparer, string Stored, string Lookup, bool Match)[]
        {
            ("Ordinal", "value", "value", true),
            ("Ordinal", "value", "VALUE", false),
            ("OrdinalIgnoreCase", "value", "VALUE", true),
            ("OrdinalIgnoreCase", "value", "other", false),
            ("tr-TR", "isik", "isik", true),
            ("tr-TR", "isik", "ISIK", false),
            ("tr-TR:IgnoreCase", "ısık", "ISIK", true),
            ("tr-TR:IgnoreCase", "isik", "İSİK", true),
            ("tr-TR:IgnoreCase", "isik", "ISIK", false)
        };

        foreach (var (comparer, stored, lookup, match) in checks)
        {
            Assert.True(SqlCollationSpec.TryParse(comparer, out var spec));
            var name = SqlCollationMapper.TryMap(serverType, spec, SqlCollationTarget.Column, tolerant: true);
            if (name is null)
                continue;

            var exact = SqlCollationMapper.TryMap(serverType, spec, SqlCollationTarget.Column) is not null;
            var collation = serverType.StartsWith("Postgres", StringComparison.OrdinalIgnoreCase)
                ? dialect.QuoteIdentifier(name) : name;
            var table = dialect.QuoteIdentifier("CollationCheck");
            var column = dialect.QuoteIdentifier("V");

            connection.Execute($"CREATE TABLE {table} ({column} {columnType} COLLATE {collation})");
            try
            {
                connection.Execute($"INSERT INTO {table} ({column}) VALUES (@value)", new { value = stored });
                var count = connection.Query<int>($"SELECT COUNT(*) FROM {table} WHERE {column} = @value",
                    new { value = lookup }).Single();

                if (exact)
                    Assert.True(match == (count == 1),
                        $"{name} for {comparer}: '{stored}' vs '{lookup}' should {(match ? "" : "not ")}match");
            }
            finally
            {
                connection.Execute($"DROP TABLE {table}");
            }
        }
    }

    [Fact]
    public async Task Postgres_AutoMapTolerant_Creates_Icu_Database()
    {
        if (SqlIntegrationConnections.ShouldSkip("Postgres"))
            return;

        DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
        using var serverConnection = SqlIntegrationConnections.CreateConnection("Postgres");

        testCleanup += () =>
        {
            using var cleanup = SqlIntegrationConnections.CreateConnection("Postgres");
            cleanup.Open();
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS \"{defaultDatabase}\" WITH (FORCE)");
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS \"{northwindDatabase}\" WITH (FORCE)");
        };

        // Turkish sorts dotless i before i, while code point order and the root collation don't
        await RunMigrationsAndServiceCall(WithPostgresDatabase(serverConnection.ConnectionString, defaultDatabase),
            WithPostgresDatabase(serverConnection.ConnectionString, northwindDatabase), "Npgsql",
            "automaptolerant", "tr-TR:IgnoreCase", verifyDatabase: connection =>
                Assert.True(connection.Query<bool>("SELECT 'ı' < 'i'").Single()));
    }

    [Fact]
    public async Task Postgres_Literal_C_Locale_Creates_Database()
    {
        if (SqlIntegrationConnections.ShouldSkip("Postgres"))
            return;

        DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
        using var serverConnection = SqlIntegrationConnections.CreateConnection("Postgres");

        testCleanup += () =>
        {
            using var cleanup = SqlIntegrationConnections.CreateConnection("Postgres");
            cleanup.Open();
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS \"{defaultDatabase}\" WITH (FORCE)");
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS \"{northwindDatabase}\" WITH (FORCE)");
        };

        await RunMigrationsAndServiceCall(WithPostgresDatabase(serverConnection.ConnectionString, defaultDatabase),
            WithPostgresDatabase(serverConnection.ConnectionString, northwindDatabase), "Npgsql",
            "C", verifyDatabase: connection =>
                Assert.Equal("C", connection.Query<string>("SHOW lc_collate").Single()));
    }

    [Theory]
    [InlineData("automap", "tr-TR:IgnoreCase:IgnoreAccents", "utf8mb4_tr_0900_ai_ci")]
    [InlineData("automaptolerant", "tr-TR:IgnoreCase", "utf8mb4_tr_0900_ai_ci")]
    [InlineData("automap", "OrdinalIgnoreCase", "utf8mb4_0900_as_ci")]
    public async Task MySql_AutoMap_Creates_Database_With_Collation_Of_Comparer(string keyword, string comparer, string expected)
    {
        if (SqlIntegrationConnections.ShouldSkip("MySql"))
            return;

        DbProviderFactories.RegisterFactory("MySqlConnector", MySqlConnectorFactory.Instance);
        using var serverConnection = SqlIntegrationConnections.CreateConnection("MySql");

        testCleanup += () =>
        {
            MySqlConnection.ClearAllPools();
            using var cleanup = SqlIntegrationConnections.CreateConnection("MySql");
            cleanup.Open();
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS `{defaultDatabase}`");
            SqlIntegrationConnections.ExecuteSql(cleanup, $"DROP DATABASE IF EXISTS `{northwindDatabase}`");
        };

        await RunMigrationsAndServiceCall(WithMySqlDatabase(serverConnection.ConnectionString, defaultDatabase),
            WithMySqlDatabase(serverConnection.ConnectionString, northwindDatabase), "MySqlConnector",
            keyword, comparer, verifyDatabase: connection => Assert.Equal(expected, connection.Query<string>(
                "SELECT DEFAULT_COLLATION_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = DATABASE()").Single()));
    }

    /// <summary>
    /// EnsureDatabase only creates local Firebird databases, so this runs what it
    /// does after creating one on a database of the test server.
    /// </summary>
    [Fact]
    public void Firebird_Alter_Character_Set_Changes_Default_Collation()
    {
        if (SqlIntegrationConnections.ShouldSkip("Firebird"))
            return;

        var serverConnection = SqlIntegrationConnections.CreateConnection("Firebird");
        var connectionString = WithFirebirdDatabase(serverConnection.ConnectionString, defaultDatabase);
        serverConnection.Dispose();

        testCleanup += () =>
        {
            FbConnection.ClearAllPools();
            TryDropDatabase(connectionString);
        };

        FbConnection.CreateDatabase(connectionString, 8192, true, false);

        using var connection = new FbConnection(connectionString);
        connection.Open();
        var charset = new FbConnectionStringBuilder(connectionString).Charset;

        using (var alter = connection.CreateCommand())
        {
            alter.CommandText = $"ALTER CHARACTER SET {charset} SET DEFAULT COLLATION UNICODE_CI";
            alter.ExecuteNonQuery();
        }

        using (var create = connection.CreateCommand())
        {
            create.CommandText = "CREATE TABLE CollationCheck (V VARCHAR(50))";
            create.ExecuteNonQuery();
        }

        using (var insert = connection.CreateCommand())
        {
            insert.CommandText = "INSERT INTO CollationCheck (V) VALUES ('value')";
            insert.ExecuteNonQuery();
        }

        using var select = connection.CreateCommand();
        select.CommandText = "SELECT COUNT(*) FROM CollationCheck WHERE V = 'VALUE'";
        Assert.Equal(1, Convert.ToInt32(select.ExecuteScalar()));
    }
}
