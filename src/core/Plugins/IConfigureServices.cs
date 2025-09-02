using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Plugins;

/// <summary>
/// Interface for plugins that requires to configure their own services / options
/// </summary>
public interface IConfigureServices
{
    /// <summary>
    /// Configures the services / options for the plugin
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="configuration">Configuration</param>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}