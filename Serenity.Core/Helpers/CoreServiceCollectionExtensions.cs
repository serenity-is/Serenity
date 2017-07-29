#if ASPNETCORE

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Caching;
using Serenity.Localization;
using Serenity.Logging;

namespace Serenity.Extensions.DependencyInjection
{
    public static class DataServiceCollectionExtensions
    {
        public static void AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<ILocalCache, MemoryLocalCache>();
            services.TryAddSingleton<IDistributedCache, DistributedCacheEmulator>();
        }

        public static void AddFileLogging(this IServiceCollection services)
        {
            services.TryAddSingleton<ILogger, FileLogger>();
        }

        public static void AddTextRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<ILocalTextRegistry, LocalTextRegistry>();
        }
    }
}
#endif