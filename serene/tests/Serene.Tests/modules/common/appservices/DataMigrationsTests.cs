using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using Serenity.Extensions;

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
            new ServiceCollection(), DefaultConnectionAttribute.Key, connectionString).BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<UserEntityOptions>>().Value;

        Assert.Same(configured.RowType, options.RowType);
        Assert.Equal(configured.TableName, options.TableName);
        Assert.Equal(configured.IdColumnName, options.IdColumnName);
        Assert.Equal(configured.IdFieldType, options.IdFieldType);
        Assert.Equal(configured.IdColumnSize, options.IdColumnSize);
    }

    [Fact]
    public void AddMigratorServices_AssignsTagsForTheDatabase()
    {
        // no IConnectionKeyFallbacks, so only the database itself is requested
        using var serviceProvider = CreateServiceProvider(new TestSqlConnections(new TestConnectionString()));
        var options = serviceProvider.GetRequiredService<IOptions<RunnerOptions>>().Value;

        Assert.Equal(new[] { TargetDBAttribute.TagPrefix + DefaultConnectionAttribute.Key }, options.Tags);
        Assert.True(options.IncludeUntaggedMigrations);
    }

    [Fact]
    public void AddMigratorServices_AssignsTagsOfConnectionKeysResolvingToTheDatabase()
    {
        // the tags are the union of all connection keys that resolve to the database being
        // migrated, e.g. the pro features that fall back to the default connection
        using var serviceProvider = CreateServiceProvider(new TestSqlConnectionsWithFallbacks(
            new TestConnectionString(), [DefaultConnectionAttribute.Key, "ProFeatures", "ProDataSources"]));
        var options = serviceProvider.GetRequiredService<IOptions<RunnerOptions>>().Value;

        Assert.Equal(new[] {
            TargetDBAttribute.TagPrefix + DefaultConnectionAttribute.Key,
            TargetDBAttribute.TagPrefix + "ProFeatures",
            TargetDBAttribute.TagPrefix + "ProDataSources"
        }, options.Tags);
    }

    [Fact]
    public void AddMigratorServices_LoadsTheMigrationsOfTheDatabase()
    {
        // verifies the whole wiring, i.e. the requested db: tags select the migrations that
        // declare a matching target database via TargetDBAttribute
        using var serviceProvider = CreateServiceProvider(new TestSqlConnections(new TestConnectionString()));
        var migrations = serviceProvider.GetRequiredService<IMigrationInformationLoader>().LoadMigrations();

        Assert.NotEmpty(migrations);
    }

    [Fact]
    public void AddMigratorServices_UsesDimensionedTagMatching()
    {
        using var serviceProvider = CreateServiceProvider(new TestSqlConnections(new TestConnectionString()));
        var conventions = serviceProvider.GetRequiredService<IMigrationRunnerConventionsAccessor>()
            .MigrationRunnerConventions;

        Assert.True(conventions.TypeHasMatchingTags(typeof(TaggedWithDefaultDB), ["db:Default"]));
        Assert.True(conventions.TypeHasMatchingTags(typeof(TaggedWithDefaultDB), ["db:Default", "env:Staging"]));

        // the legacy tag form is normalized to the db: form, so old style tags still match
        Assert.True(conventions.TypeHasMatchingTags(typeof(TaggedWithDefaultDB), ["DefaultDB"]));
        Assert.False(conventions.TypeHasMatchingTags(typeof(TaggedWithDefaultDB), ["db:Northwind"]));

        // the other conventions are still delegated to FluentMigrator
        Assert.True(conventions.TypeIsMigration(typeof(TestMigration)));
        Assert.False(conventions.TypeIsMigration(typeof(TaggedWithDefaultDB)));
    }

    private static ServiceProvider CreateServiceProvider(ISqlConnections sqlConnections)
    {
        var connectionString = sqlConnections.TryGetConnectionString(DefaultConnectionAttribute.Key);
        using var typeSource = new DefaultTypeSource([typeof(Serene.AppServices.DataMigrations).Assembly]);
        var migrations = new Serene.AppServices.DataMigrations(
            typeSource,
            sqlConnections,
            new MockWebHostEnvironment(),
            Options.Create(new UserEntityOptions()));

        return migrations.AddMigratorServices(
            new ServiceCollection(), DefaultConnectionAttribute.Key, connectionString!)
            .BuildServiceProvider();
    }

    [DefaultDB]
    private class TaggedWithDefaultDB
    {
    }

    [Migration(20990101_0000)]
    private class TestMigration : Migration
    {
        public override void Up()
        {
        }

        public override void Down()
        {
        }
    }

    private sealed class TestConnectionString : IConnectionString
    {
        public ISqlDialect Dialect => SqliteDialect.Instance;
        public string ConnectionKey => DefaultConnectionAttribute.Key;
        public string ConnectionString => "Data Source=:memory:";
        public string ProviderName => "Microsoft.Data.Sqlite";
    }

    private class TestSqlConnections(IConnectionString connectionString) : ISqlConnections
    {
        public IEnumerable<IConnectionString> ListConnectionStrings() => [connectionString];

        public IDbConnection New(string connectionString, string providerName, ISqlDialect dialect) =>
            throw new NotSupportedException();

        public IDbConnection NewByKey(string connectionKey) => throw new NotSupportedException();

        public IConnectionString? TryGetConnectionString(string connectionKey) => connectionString;
    }

    private sealed class TestSqlConnectionsWithFallbacks(IConnectionString connectionString, string[] resolvingKeys)
        : TestSqlConnections(connectionString), IConnectionKeyFallbacks
    {
        public IEnumerable<string> GetConnectionKeyFallbacks(string connectionKey) => [connectionKey];

        public string? ResolveConnectionKey(string connectionKey) => connectionKey;

        public IEnumerable<string> GetConnectionKeysResolvingTo(string connectionKey) => resolvingKeys;
    }
}
