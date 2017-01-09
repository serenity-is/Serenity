#if COREFX
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Configuration;

namespace Serenity.Extensions.DependencyInjection
{
    public static class CoreServiceCollectionExtensions
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IConfiguration>(configuration);
            services.TryAddSingleton<IConfigurationRepository, AppSettingsJsonConfigRepository>();
        }
    }
}
#endif