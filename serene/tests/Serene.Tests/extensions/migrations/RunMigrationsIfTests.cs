using Microsoft.Extensions.Hosting;
using System.Threading;
using Serenity.Extensions;

namespace Serenity.Extensions;

/// <summary>
/// Covers <see cref="HostMigrationExtensions"/>. The helper returns the exit code instead of
/// terminating the process, so every branch is assertable here.
/// </summary>
public class RunMigrationsIfTests
{
    [Fact]
    public void Returns_Null_For_Other_Arguments()
    {
        var host = CreateHost();

        // null means "not my command": the application must start normally
        Assert.Null(host.RunMigrationsIf(["other"]));
    }

    [Fact]
    public void Returns_Null_Without_Arguments()
    {
        var host = CreateHost();

        Assert.Null(host.RunMigrationsIf([]));
    }

    [Fact]
    public void Returns_Null_When_Migrate_Is_Not_The_First_Argument()
    {
        var host = CreateHost();

        Assert.Null(host.RunMigrationsIf(["--migrate"]));
        Assert.Null(host.RunMigrationsIf(["server", "migrate"]));
    }

    [Fact]
    public void Returns_Zero_And_Runs_For_Migrate()
    {
        var migrations = new FakeMigrations();
        var host = CreateHost(migrations);

        Assert.Equal(0, host.RunMigrationsIf(["migrate"]));
        Assert.Equal(1, migrations.InitializeCalls);
    }

    [Fact]
    public void Returns_Two_For_Extra_Arguments_Without_Running()
    {
        var migrations = new FakeMigrations();
        var host = CreateHost(migrations);

        // starting the web host here would look like a hang to a script
        Assert.Equal(2, host.RunMigrationsIf(["migrate", "--connection-key", "Default"]));
        Assert.Equal(0, migrations.InitializeCalls);
    }

    [Fact]
    public void Returns_One_When_The_Migrations_Throw()
    {
        var host = CreateHost(new FakeMigrations { ThrowOnInitialize = true });

        Assert.Equal(1, host.RunMigrationsIf(["migrate"]));
    }

    [Fact]
    public void Generic_Overload_Runs_The_Callback_With_The_Resolved_Service()
    {
        var host = CreateHost();
        IDataMigrations resolved = null;

        Assert.Equal(0, host.RunMigrationsIf<IDataMigrations>(["migrate"], m => resolved = m));
        Assert.IsType<FakeMigrations>(resolved);
    }

    [Fact]
    public void Throws_When_Host_Or_Runner_Is_Missing()
    {
        var host = CreateHost();

        Assert.Throws<ArgumentNullException>(() =>
            HostMigrationExtensions.RunMigrationsIf<IDataMigrations>(null!, [], _ => { }));

        Assert.Throws<ArgumentNullException>(() =>
            host.RunMigrationsIf<IDataMigrations>([], null!));
    }

    private static IHost CreateHost(IDataMigrations migrations = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(migrations ?? new FakeMigrations());
        return new FakeHost(services.BuildServiceProvider());
    }

    private sealed class FakeMigrations : IDataMigrations
    {
        public int InitializeCalls { get; private set; }
        public bool ThrowOnInitialize { get; init; }

        public void Initialize()
        {
            InitializeCalls++;
            if (ThrowOnInitialize)
                throw new InvalidOperationException("Error executing migration!");
        }
    }

    private sealed class FakeHost(IServiceProvider services) : IHost
    {
        public IServiceProvider Services { get; } = services;
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Dispose() { }
    }
}
