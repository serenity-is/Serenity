using Microsoft.Extensions.Configuration;
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

    /// <summary>
    /// Gets the specified connection string entry from the specified configuration's Data:[name] section.
    /// Only for use in Startup where IConnectionStrings is not yet available.
    /// Throws if no such connection is found.
    /// </summary>
    /// <param name="configuration">The configuration to enumerate.</param>
    /// <param name="name">The connection string key.</param>
    /// <returns>The connection string entry from Data:[name].</returns>
    public static ConnectionStringEntry GetDataConnectionString(this IConfiguration configuration, string name)
    {
        var section = configuration.GetSection("Data:" + name);
        var connection = new ConnectionStringEntry
        {
            ConnectionString = section["ConnectionString"],
            ProviderName = section["ProviderName"],
            Dialect = section["Dialect"]
        };

        if (string.IsNullOrEmpty(connection.ConnectionString))
            throw new ArgumentException($"No connection string entry found for {name}");

        return connection;
    }
}