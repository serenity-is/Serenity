using FluentMigrator;
using FluentMigrator.Runner;

namespace Serenity.Extensions;

public class MigrationUtilsTests
{
    public static bool IsOracleResult;
    public static bool IsMySqlResult;
    public static bool IsSqliteResult;
    public static bool IsSqlServerResult;
    public static bool IsPostgresResult;
    public static bool IsFirebirdResult;
    public static bool IsDatabasePrefixResult;
    public static bool IsDatabasePredicateResult;
    public static bool IfTrueResult;
    public static bool IfFalseResult;

    [MigrationKey(20240101_0001)]
    public class CreateTableA : Migration
    {
        public override void Up()
        {
            this.CreateTableWithId32("TableA", "Id",
                col => col.WithColumn("Name").AsString(50).NotNullable());
        }

        public override void Down() => Delete.Table("TableA");
    }

    [MigrationKey(20240101_0002)]
    public class CreateTableB : Migration
    {
        public override void Up()
        {
            this.CreateTableWithId64("TableB", "Id",
                col => col.WithColumn("Name").AsString(50), checkExists: true);
        }

        public override void Down() => Delete.Table("TableB");
    }

    [MigrationKey(20240101_0005)]
    public class SkipExistingTable : Migration
    {
        public override void Up()
        {
            this.CreateTableWithId32("TableB", "Id",
                col => col.WithColumn("Name").AsString(50), checkExists: true);
        }

        public override void Down() => Delete.Table("TableB");
    }

    [MigrationKey(20240101_0003)]
    public class CreateTableC : Migration
    {
        public override void Up()
        {
            this.CreateTableWithId32("TableC", "Id",
                col => col.WithColumn("Name").AsString(50), primaryKey: false);
        }

        public override void Down() => Delete.Table("TableC");
    }

    [MigrationKey(20240101_0004)]
    public class MiscChecks : Migration
    {
        public override void Up()
        {
            IsOracleResult = this.IsOracle();
            IsMySqlResult = this.IsMySql();
            IsSqliteResult = this.IsSqlite();
            IsSqlServerResult = this.IsSqlServer();
            IsPostgresResult = this.IsPostgres();
            IsFirebirdResult = this.IsFirebird();
            IsDatabasePrefixResult = this.IsDatabase("SQLite");
            IsDatabasePredicateResult = this.IsDatabase(x => x.Contains("SQLite", StringComparison.OrdinalIgnoreCase));

            this.AddOracleIdentity("SomeTable", "Id");

            var falseResult = this.Create.Table("TableD")
                .WithColumn("X").AsInt32()
                .If(false, x => x.NotNullable())
                .If(true, x => x.NotNullable());
            IfTrueResult = true;
            IfFalseResult = false;
        }

        public override void Down() => Delete.Table("TableD");
    }

    [MigrationKey(20240101_0006)]
    public class UserIdTypeHelpers(IOptions<UserEntityOptions> userEntityOptions) : Migration
    {
        public override void Up()
        {
            Create.Table(userEntityOptions.Value.TableName)
                .WithColumn(userEntityOptions.Value.IdColumnName).AsUserIdType(userEntityOptions).PrimaryKey();

            Create.Table("UserIdReferences")
                .WithColumn("Id").AsInt32().PrimaryKey()
                .WithColumn("UserId").AsUserIdType(userEntityOptions).NotNullable()
                    .UserIdForeignKey(userEntityOptions, "FK_UserIdReferences_Users");
        }

        public override void Down()
        {
            Delete.Table("UserIdReferences");
            Delete.Table(userEntityOptions.Value.TableName);
        }
    }

    private static void RunMigrations(string dbFile, Action<UserEntityOptions>? configureOptions = null)
    {
        using var services = new ServiceCollection()
            .Configure<UserEntityOptions>(options => configureOptions?.Invoke(options))
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString("Data Source=" + dbFile + ";Pooling=False")
                .ScanIn(typeof(MigrationUtilsTests).Assembly).For.Migrations())
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    private static void WithMigrations(Action<string> action, Action<UserEntityOptions>? configureOptions = null)
    {
        var dbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "fm_" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            RunMigrations(dbFile, configureOptions);
            action(dbFile);
        }
        finally
        {
            foreach (var file in new[] { dbFile, dbFile + "-journal", dbFile + "-wal", dbFile + "-shm" })
                try
                {
                    if (System.IO.File.Exists(file))
                        System.IO.File.Delete(file);
                }
                catch
                {
                }
        }
    }

    [Fact]
    public void Sqlite_Migrations_Exercise_Helpers()
    {
        WithMigrations(_ =>
        {
            Assert.True(IsSqliteResult);
            Assert.False(IsOracleResult);
            Assert.False(IsMySqlResult);
            Assert.False(IsSqlServerResult);
            Assert.False(IsPostgresResult);
            Assert.False(IsFirebirdResult);
            Assert.True(IsDatabasePrefixResult);
            Assert.True(IsDatabasePredicateResult);
            Assert.True(IfTrueResult);
            Assert.False(IfFalseResult);
        });
    }

    [Fact]
    public void UserId_Migration_Helpers_UseConfiguredSchema()
    {
        WithMigrations(dbFile =>
        {
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(
                "Data Source=" + dbFile + ";Pooling=False");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA table_info('UserIdReferences')";
            string? userIdType = null;
            using (var columns = command.ExecuteReader())
            {
                while (columns.Read())
                {
                    if (columns["name"] as string == "UserId")
                        userIdType = columns["type"] as string;
                }
            }

            Assert.Equal("TEXT", userIdType?.ToUpperInvariant());

            command.CommandText = "PRAGMA foreign_key_list('UserIdReferences')";
            using var foreignKeys = command.ExecuteReader();
            Assert.True(foreignKeys.Read());
            Assert.Equal("ConfiguredUsers", foreignKeys["table"] as string);
            Assert.Equal("UserKey", foreignKeys["to"] as string);
        }, options =>
        {
            options.TableName = "ConfiguredUsers";
            options.IdColumnName = "UserKey";
            options.IdFieldType = typeof(StringField);
            options.IdColumnSize = 37;
        });
    }

    private class FakeConnectionString(ISqlDialect dialect, string connectionString = "Data Source=:memory:",
        string providerName = "Microsoft.Data.Sqlite", string? collation = null) : IConnectionString
    {
        public string? Collation { get; } = collation;
        public ISqlDialect Dialect { get; } = dialect;
        public string ConnectionKey => DefaultConnectionAttribute.Key;
        public string ConnectionString { get; } = connectionString;
        public string ProviderName { get; } = providerName;
    }

    private class FakeSqlConnections(IConnectionString connectionString, IDbConnection? connection = null)
        : ISqlConnections
    {
        public IEnumerable<IConnectionString> ListConnectionStrings() => [connectionString];
        public IDbConnection New(string connectionString, string providerName, ISqlDialect dialect)
        {
            if (connection == null)
                throw new NotImplementedException();

            connection.ConnectionString = connectionString;
            return connection;
        }
        public IDbConnection NewByKey(string connectionKey) => connection ?? throw new NotImplementedException();
        public IConnectionString? TryGetConnectionString(string connectionKey) => connectionString;
    }

    private static void EnsureSqliteProviderRegistered()
    {
        try
        {
            System.Data.Common.DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite",
                Microsoft.Data.Sqlite.SqliteFactory.Instance);
        }
        catch (ArgumentException)
        {
        }
    }

    private static void EnsureSqlServerProviderRegistered()
    {
        try
        {
            System.Data.Common.DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient",
                Microsoft.Data.SqlClient.SqlClientFactory.Instance);
        }
        catch (ArgumentException)
        {
        }
    }

    private class CountingConnection : MockDbConnection
    {
        public static int ClearCount;

        public static void ClearAllPools() => ClearCount++;
    }

    private class CountingFactory : System.Data.Common.DbProviderFactory
    {
        public static readonly CountingFactory Instance = new();

        public override System.Data.Common.DbConnection CreateConnection() => new CountingConnection();
    }

    private class NoClearFactory : System.Data.Common.DbProviderFactory
    {
        public static readonly NoClearFactory Instance = new();

        public override System.Data.Common.DbConnection CreateConnection() => new MockDbConnection();
    }

    private static void EnsureProviderRegistered(string providerName, System.Data.Common.DbProviderFactory factory)
    {
        System.Data.Common.DbProviderFactories.RegisterFactory(providerName, factory);
    }

    private static string CreateDatabaseCommand(ISqlDialect dialect, string connectionString, string providerName,
        string? collation)
    {
        string? command = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader())
            .OnDbCommandExecuteNonQuery(cmd =>
            {
                command = cmd.CommandText;
                return 1;
            });

        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, new FakeSqlConnections(
            new FakeConnectionString(dialect, connectionString, providerName), connection), collation);

        return command.AssertNotNull();
    }

    private static string CreateSqlServerDatabaseCommand(string? collation)
    {
        EnsureSqlServerProviderRegistered();
        return CreateDatabaseCommand(SqlServer2012Dialect.Instance,
            "Server=.;Database=mydb;Trusted_Connection=True;", "Microsoft.Data.SqlClient", collation);
    }

    private static string CreatePostgresDatabaseCommand(string? collation)
    {
        EnsureProviderRegistered("Npgsql", Npgsql.NpgsqlFactory.Instance);
        return CreateDatabaseCommand(PostgresDialect.Instance,
            "Host=localhost;Database=mydb;Username=x", "Npgsql", collation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void EnsureDatabase_Uses_No_Collation_When_Empty(string? collation)
    {
        Assert.Equal("CREATE DATABASE [mydb]", CreateSqlServerDatabaseCommand(collation));
    }

    [Fact]
    public void EnsureDatabase_Creates_SqlServer_Database_With_Collation()
    {
        Assert.Equal("CREATE DATABASE [mydb] COLLATE Turkish_100_CI_AS",
            CreateSqlServerDatabaseCommand("Turkish_100_CI_AS"));
    }

    private static string CreateDatabaseCommandWithConnectionCollation(string? collationArgument)
    {
        EnsureSqlServerProviderRegistered();
        string? command = null;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader())
            .OnDbCommandExecuteNonQuery(cmd =>
            {
                command = cmd.CommandText;
                return 1;
            });

        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance, "Server=.;Database=mydb;Trusted_Connection=True;",
                "Microsoft.Data.SqlClient", collation: "Turkish_100_CI_AS"), connection), collationArgument);

        return command.AssertNotNull();
    }

    [Fact]
    public void EnsureDatabase_Uses_Collation_Of_Connection_By_Default()
    {
        Assert.Equal("CREATE DATABASE [mydb] COLLATE Turkish_100_CI_AS",
            CreateDatabaseCommandWithConnectionCollation(null));
    }

    [Fact]
    public void EnsureDatabase_Collation_Argument_Overrides_Connection()
    {
        Assert.Equal("CREATE DATABASE [mydb] COLLATE Latin1_General_100_CI_AS",
            CreateDatabaseCommandWithConnectionCollation("Latin1_General_100_CI_AS"));
    }

    [Fact]
    public void EnsureDatabase_Empty_Collation_Argument_Ignores_Connection()
    {
        Assert.Equal("CREATE DATABASE [mydb]", CreateDatabaseCommandWithConnectionCollation(""));
    }

    [Fact]
    public void EnsureDatabase_Throws_For_Connection_Collation_On_Sqlite()
    {
        var connections = new FakeSqlConnections(new FakeConnectionString(SqliteDialect.Instance, collation: "NOCASE"));

        Assert.Throws<NotSupportedException>(() =>
            MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, connections));
    }

    [Fact]
    public void EnsureDatabase_Creates_MySql_Database_With_Collation()
    {
        EnsureProviderRegistered("MySqlConnector", MySqlConnector.MySqlConnectorFactory.Instance);
        Assert.Equal("CREATE DATABASE `mydb` COLLATE utf8mb4_turkish_ci",
            CreateDatabaseCommand(MySqlDialect.Instance, "Server=localhost;Database=mydb", "MySqlConnector", "utf8mb4_turkish_ci"));
    }

    [Fact]
    public void EnsureDatabase_Creates_Postgres_Database_With_Locale()
    {
        Assert.Equal("CREATE DATABASE \"mydb\" TEMPLATE template0 LOCALE 'tr_TR.UTF-8'",
            CreatePostgresDatabaseCommand("tr_TR.UTF-8"));
    }

    [Fact]
    public void EnsureDatabase_Creates_Postgres_Database_With_Icu_Collation()
    {
        Assert.Equal("CREATE DATABASE \"mydb\" TEMPLATE template0 LOCALE_PROVIDER icu ICU_LOCALE 'tr-TR'",
            CreatePostgresDatabaseCommand("tr-TR-x-icu"));
    }

    [Fact]
    public void EnsureDatabase_Escapes_Postgres_Locale()
    {
        Assert.Equal("CREATE DATABASE \"mydb\" TEMPLATE template0 LOCALE 'a''b'",
            CreatePostgresDatabaseCommand("a'b"));
    }

    [Theory]
    [InlineData("Turkish_100_CI_AS; DROP DATABASE x")]
    [InlineData("a b")]
    [InlineData("a-b")]
    public void EnsureDatabase_Throws_For_Invalid_Collation_Name(string collation)
    {
        Assert.Throws<ArgumentException>(() => CreateSqlServerDatabaseCommand(collation));
    }

    [Fact]
    public void EnsureDatabase_Throws_For_Collation_On_Sqlite_Without_Side_Effects()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "serenity_db_" + Guid.NewGuid().ToString("N"));
        var connections = new FakeSqlConnections(new FakeConnectionString(SqliteDialect.Instance));

        Assert.Throws<NotSupportedException>(() =>
            MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, root, connections, "NOCASE"));

        Assert.False(System.IO.Directory.Exists(root));
    }

    [Fact]
    public void EnsureDatabase_Throws_For_Collation_On_Oracle()
    {
        var connections = new FakeSqlConnections(new FakeConnectionString(OracleDialect.Instance));

        Assert.Throws<NotSupportedException>(() =>
            MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, connections, "BINARY_CI"));
    }

    [Fact]
    public void EnsureDatabase_Ignores_Collation_When_Database_Exists()
    {
        EnsureSqlServerProviderRegistered();
        var executed = false;
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { name = "mydb" }))
            .OnDbCommandExecuteNonQuery(_ =>
            {
                executed = true;
                return 1;
            });

        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance,
                "Server=.;Database=mydb;Trusted_Connection=True;", "Microsoft.Data.SqlClient"),
            connection), "Turkish_100_CI_AS");

        Assert.False(executed);
    }

    [Fact]
    public void ClearAllPools_Calls_Static_Method_Of_Provider_Connection()
    {
        EnsureProviderRegistered("CountingProvider", CountingFactory.Instance);
        var before = CountingConnection.ClearCount;

        MigrationUtils.ClearAllPools(DefaultConnectionAttribute.Key, new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance, "x", "CountingProvider")));

        Assert.Equal(before + 1, CountingConnection.ClearCount);
    }

    [Fact]
    public void ClearAllPools_Does_Nothing_When_Provider_Has_No_Method()
    {
        EnsureProviderRegistered("NoClearProvider", NoClearFactory.Instance);

        MigrationUtils.ClearAllPools(DefaultConnectionAttribute.Key, new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance, "x", "NoClearProvider")));
    }

    [Fact]
    public void ClearAllPools_Does_Nothing_When_Provider_Is_Not_Registered()
    {
        MigrationUtils.ClearAllPools(DefaultConnectionAttribute.Key, new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance, "x", "NotRegisteredProvider")));
    }

    [Theory]
    [InlineData(typeof(Microsoft.Data.SqlClient.SqlConnection))]
    [InlineData(typeof(Microsoft.Data.Sqlite.SqliteConnection))]
    [InlineData(typeof(MySqlConnector.MySqlConnection))]
    [InlineData(typeof(Npgsql.NpgsqlConnection))]
    [InlineData(typeof(Oracle.ManagedDataAccess.Client.OracleConnection))]
    [InlineData(typeof(FirebirdSql.Data.FirebirdClient.FbConnection))]
    public void ClearAllPools_Exists_On_All_Supported_Providers(Type connectionType)
    {
        Assert.NotNull(connectionType.GetMethod("ClearAllPools",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, Type.EmptyTypes));
    }

    [Fact]
    public void EnsureDatabase_Returns_When_SqlServer_Database_Exists()
    {
        EnsureSqlServerProviderRegistered();
        using var connection = new MockDbConnection()
            .OnDbCommandExecuteReader(_ => new MockDbDataReader(new { name = "mydb" }));
        var connections = new FakeSqlConnections(
            new FakeConnectionString(SqlServer2012Dialect.Instance,
                "Server=.;Database=mydb;Trusted_Connection=True;", "Microsoft.Data.SqlClient"),
            connection);

        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, connections);
    }

    [Fact]
    public void EnsureDatabase_Creates_LocalDb_Database()
    {
        EnsureSqlServerProviderRegistered();
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "serenity_localdb_" + Guid.NewGuid().ToString("N"));
        try
        {
            using var connection = new MockDbConnection()
                .OnDbCommandExecuteReader(_ => new MockDbDataReader())
                .OnDbCommandExecuteNonQuery(_ => 1);
            var connections = new FakeSqlConnections(
                new FakeConnectionString(SqlServer2012Dialect.Instance,
                    @"Server=(localdb)\MSSQLLocalDB;Database=mydb;Trusted_Connection=True;",
                    "Microsoft.Data.SqlClient"),
                connection);

            MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, root, connections);

            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(root, "App_Data")));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, true);
        }
    }

    [Fact]
    public void EnsureDatabase_Returns_For_Unsupported_Server_Type()
    {
        EnsureSqliteProviderRegistered();
        var connections = new FakeSqlConnections(new FakeConnectionString(OracleDialect.Instance));
        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, connections);
    }

    [Fact]
    public void EnsureDatabase_Returns_For_Local_Firebird()
    {
        EnsureSqliteProviderRegistered();
        var connections = new FakeSqlConnections(
            new FakeConnectionString(FirebirdDialect.Instance, "Data Source=foo.fdb"));
        MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, null!, connections);
    }

    [Fact]
    public void EnsureDatabase_Creates_AppData_For_Sqlite()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "serenity_db_" + Guid.NewGuid().ToString("N"));
        try
        {
            var connections = new FakeSqlConnections(new FakeConnectionString(SqliteDialect.Instance));
            MigrationUtils.EnsureDatabase(DefaultConnectionAttribute.Key, root, connections);
            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(root, "App_Data")));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, true);
        }
    }
}
