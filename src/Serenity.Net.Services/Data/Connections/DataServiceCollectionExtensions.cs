using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Serenity.Extensions.DependencyInjection;

/// <summary>
/// Contains extensions to register data services
/// </summary>
public static class DataServiceCollectionExtensions
{
    /// <summary>
    /// Adds default IConnectionStrings and ISqlConnections interfaces
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddSqlConnections(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services.AddOptions();
        services.TryAddSingleton<ISqlDialectMapper, DefaultSqlDialectMapper>();
        services.TryAddSingleton<IConnectionStrings, DefaultConnectionStrings>();
        services.TryAddSingleton<ISqlConnections, DefaultSqlConnections>();
    }

    /// <summary>
    /// Adds default IConnectionStrings and ISqlConnections interfaces
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="setupAction">Setup action to manually modify connection strings</param>
    public static void AddSqlConnections(this IServiceCollection services, 
        Action<ConnectionStringOptions> setupAction)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction));

        services.AddSqlConnections();
        services.Configure(setupAction);
    }
}