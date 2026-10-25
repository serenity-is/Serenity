using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Serenity.Extensions;

/// <summary>
/// Helper methods for app host migration commands, e.g. "migrate" to run database migrations before starting the app.
/// </summary>
public static class HostMigrationExtensions
{
    /// <summary>
    /// Runs <see cref="IDataMigrations.Initialize"/> when the first argument is "migrate".
    /// </summary>
    /// <param name="host">Host</param>
    /// <param name="args">Arguments, e.g. the ones passed to <c>Main</c>.</param>
    /// <returns><c>null</c> when <paramref name="args"/> is not the migrate command, so the
    /// application should start normally; otherwise the exit code the caller should return:
    /// 0 on success, 1 when the migrations throw, 2 for extra arguments.</returns>
    /// <remarks>See <see cref="RunMigrationsIf{TService}(IHost, string[], Action{TService})"/>.</remarks>
    public static int? RunMigrationsIf(this IHost host, string[] args) =>
        RunMigrationsIf<IDataMigrations>(host, args, migrations => migrations.Initialize());

    /// <summary>
    /// Runs the migrations via the provided runner when the first argument is "migrate".
    /// </summary>
    /// <typeparam name="TService">Service to resolve from the host, normally <c>IDataMigrations</c>.</typeparam>
    /// <param name="host">Host</param>
    /// <param name="args">Arguments, e.g. the ones passed to <c>Main</c>. Anything other than
    /// a lone "migrate" is left to the caller, so the application starts normally.</param>
    /// <param name="runner">Runner callback that receives the resolved service.</param>
    /// <returns><c>null</c> when <paramref name="args"/> is not the migrate command, so the
    /// application should start normally; otherwise the exit code the caller should return:
    /// 0 on success, 1 when <paramref name="runner"/> throws, 2 for extra arguments.</returns>
    /// <remarks>
    /// Terminating the process is the caller's job, so this can be used from tests and from
    /// applications that want their own exit codes. Extra arguments are rejected instead of
    /// starting the web host, which would otherwise look like a hang to a script.
    /// </remarks>
    public static int? RunMigrationsIf<TService>(this IHost host, string[] args, Action<TService> runner)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(runner);

        if (args is not ["migrate", ..])
            return null;

        if (args.Length > 1)
        {
            Console.Error.WriteLine("usage: <app> migrate");
            return 2;
        }

        try
        {
            runner(host.Services.GetRequiredService<TService>());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Migration failed: " + ex.Message);
            return 1;
        }

        Console.WriteLine("Migration completed successfully.");
        return 0;
    }
}
