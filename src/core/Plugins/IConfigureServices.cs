using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Plugins;

/// <summary>
/// Interface for plugins that require configuring their own services or options.
/// </summary>
public interface IConfigureServices
{
    /// <summary>
    /// Configures the services and options for the plugin.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}