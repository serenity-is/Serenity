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

    private static void RunMigrations(string dbFile)
    {
        using var services = new ServiceCollection()
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

    private static void WithMigrations(Action action)
    {
        var dbFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "fm_" + Guid.NewGuid().ToString("N") + ".db");
        try
        {
            RunMigrations(dbFile);
            action();
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
        WithMigrations(() =>
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

    private class FakeConnectionString(ISqlDialect dialect, string connectionString = "Data Source=:memory:",
        string providerName = "Microsoft.Data.Sqlite") : IConnectionString
    {
        public ISqlDialect Dialect { get; } = dialect;
        public string ConnectionKey => "Default";
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

        MigrationUtils.EnsureDatabase("Default", null!, connections);
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

            MigrationUtils.EnsureDatabase("Default", root, connections);

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
        MigrationUtils.EnsureDatabase("Default", null!, connections);
    }

    [Fact]
    public void EnsureDatabase_Returns_For_Local_Firebird()
    {
        EnsureSqliteProviderRegistered();
        var connections = new FakeSqlConnections(
            new FakeConnectionString(FirebirdDialect.Instance, "Data Source=foo.fdb"));
        MigrationUtils.EnsureDatabase("Default", null!, connections);
    }

    [Fact]
    public void EnsureDatabase_Creates_AppData_For_Sqlite()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "serenity_db_" + Guid.NewGuid().ToString("N"));
        try
        {
            var connections = new FakeSqlConnections(new FakeConnectionString(SqliteDialect.Instance));
            MigrationUtils.EnsureDatabase("Default", root, connections);
            Assert.True(System.IO.Directory.Exists(System.IO.Path.Combine(root, "App_Data")));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, true);
        }
    }
}
