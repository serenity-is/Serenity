using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Web;

/// <summary>
/// DI extension methods related to configuration and options
/// </summary>
public static class ServiceCollectionConfigureExtensions
{
    /// <summary>
    /// Registers a configuration instance which TOptions will bind against from a root
    /// configuration with the default section key for TOptions
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config">The configuration being bound.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureSection<TOptions>(this IServiceCollection services,
        IConfiguration config) where TOptions : class
        => services.Configure<TOptions>((config ?? throw new ArgumentNullException(nameof(config)))
            .GetSection(typeof(TOptions).GetAttribute<DefaultSectionKeyAttribute>()?.SectionKey ??
                throw new ArgumentOutOfRangeException(nameof(TOptions))));
}