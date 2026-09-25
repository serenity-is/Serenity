using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Serene.AppServices;

public class DataMigrations(ITypeSource typeSource,
    ISqlConnections sqlConnections,
    IWebHostEnvironment hostEnvironment,
    IOptions<UserEntityOptions> userEntityOptions) : IDataMigrations
{
    private static readonly string[] databaseKeys = [
        DefaultConnectionAttribute.Key,
#if (Northwind)
        Serenity.Demo.Northwind.NorthwindConnectionAttribute.Key,
#endif
    ];

    private readonly ITypeSource typeSource = typeSource ?? throw new ArgumentNullException(nameof(typeSource));
    private readonly ISqlConnections sqlConnections = sqlConnections ?? throw new ArgumentNullException(nameof(sqlConnections));
    private readonly IWebHostEnvironment hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

    public void Initialize()
    {
        foreach (var databaseKey in databaseKeys)
        {
            EnsureDatabase(databaseKey);
            RunMigrations(databaseKey);
        }
    }

    /// <summary>
    /// Automatically creates a database for the template if it doesn't already exists.
    /// You might delete this method to disable auto create functionality.
    /// </summary>
    private void EnsureDatabase(string databaseKey)
    {
        MigrationUtils.EnsureDatabase(databaseKey,
            hostEnvironment.ContentRootPath, sqlConnections);
        Microsoft.Data.SqlClient.SqlConnection.ClearAllPools();
    }

    /// <summary>
    /// Adds FluentMigrator services to the service collection for the specified database key and connection string.
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="databaseKey">Database key</param>
    /// <param name="cs">Connection string</param>
    public IServiceCollection AddMigratorServices(IServiceCollection collection, string databaseKey, IConnectionString cs)
    {
        string serverType = cs.Dialect.ServerType;
        bool isOracle = serverType.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase);

        var conventionSet = new DefaultConventionSet(defaultSchemaName: null,
            Path.GetDirectoryName(typeof(DataMigrations).Assembly.Location));

        return collection.AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .AddSingleton<IConventionSet>(conventionSet)
            .Configure<ProcessorOptions>(options =>
            {
                options.Timeout = TimeSpan.FromSeconds(90);
            })
            .Configure<RunnerOptions>(options =>
            {
                options.Tags = (sqlConnections as IConnectionKeyFallbacks)?
                    .GetConnectionKeysResolvingTo(databaseKey)?
                    .Select(x => x + "DB").ToArray() ?? [databaseKey + "DB"];
                options.IncludeUntaggedMigrations = databaseKey == DefaultConnectionAttribute.Key;
            })
            .ConfigureRunner(builder =>
            {
                if (serverType == OracleDialect.Instance.ServerType)
                    builder.AddOracleManaged();
                else if (serverType == SqliteDialect.Instance.ServerType)
                    builder.AddSQLite();
                else if (serverType == FirebirdDialect.Instance.ServerType)
                    builder.AddFirebird();
                else if (serverType == MySqlDialect.Instance.ServerType)
                    builder.AddMySql5();
                else if (serverType == PostgresDialect.Instance.ServerType)
                    builder.AddPostgres();
                else
                    builder.AddSqlServer();

                builder.WithGlobalConnectionString(cs.ConnectionString);
                builder.ScanIn([.. ((IGetAssemblies)typeSource).GetAssemblies()]).For.Migrations();
            })
            .Configure<UserEntityOptions>(options => options.AssignFrom(userEntityOptions));
    }

    private void RunMigrations(string databaseKey)
    {
        var cs = sqlConnections.TryGetConnectionString(databaseKey) ??
            throw new ArgumentOutOfRangeException(nameof(databaseKey));

        var serviceProvider = AddMigratorServices(new ServiceCollection(), databaseKey, cs)
            .BuildServiceProvider();

        bool isFirebird = cs.Dialect.ServerType.StartsWith("Firebird", StringComparison.OrdinalIgnoreCase);
        var culture = CultureInfo.CurrentCulture;
        try
        {
            if (isFirebird)
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            using var scope = serviceProvider.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error executing migration!", ex);
        }
        finally
        {
            if (isFirebird)
                Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}