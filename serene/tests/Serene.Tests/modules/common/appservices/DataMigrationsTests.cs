using System.Data;

namespace Serene.Tests;

public class DataMigrationsTests
{
    [Fact]
    public void AddMigratorServices_AssignsUserEntityOptions()
    {
        var configured = new UserEntityOptions
        {
            RowType = typeof(Serene.Administration.UserRow),
            TableName = "MigratorUsers",
            IdColumnName = "MigratorUserId",
            IdFieldType = typeof(StringField),
            IdColumnSize = 37
        };
        var connectionString = new TestConnectionString();
        using var typeSource = new DefaultTypeSource([typeof(Serene.AppServices.DataMigrations).Assembly]);
        var migrations = new Serene.AppServices.DataMigrations(
            typeSource,
            new TestSqlConnections(connectionString),
            new MockWebHostEnvironment(),
            Options.Create(configured));

        using var serviceProvider = migrations.AddMigratorServices(
            new ServiceCollection(), "Default", connectionString).BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<UserEntityOptions>>().Value;

        Assert.Same(configured.RowType, options.RowType);
        Assert.Equal(configured.TableName, options.TableName);
        Assert.Equal(configured.IdColumnName, options.IdColumnName);
        Assert.Equal(configured.IdFieldType, options.IdFieldType);
        Assert.Equal(configured.IdColumnSize, options.IdColumnSize);
    }

    private sealed class TestConnectionString : IConnectionString
    {
        public ISqlDialect Dialect => SqliteDialect.Instance;
        public string ConnectionKey => "Default";
        public string ConnectionString => "Data Source=:memory:";
        public string ProviderName => "Microsoft.Data.Sqlite";
    }

    private sealed class TestSqlConnections(IConnectionString connectionString) : ISqlConnections
    {
        public IEnumerable<IConnectionString> ListConnectionStrings() => [connectionString];

        public IDbConnection New(string connectionString, string providerName, ISqlDialect dialect) =>
            throw new NotSupportedException();

        public IDbConnection NewByKey(string connectionKey) => throw new NotSupportedException();

        public IConnectionString? TryGetConnectionString(string connectionKey) => connectionString;
    }
}