using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Services;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRequestHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            services.TryAddSingleton<IBehaviorFactory, DefaultBehaviorFactory>();
            services.TryAddSingleton<IImplicitBehaviorRegistry>(s => new DefaultImplicitBehaviorRegistry(
                DefaultImplicitBehaviorRegistry.FindImplicitBehaviorTypes(assemblies)));
            services.TryAddSingleton<IBehaviorProvider, DefaultBehaviorProvider>();
            services.TryAddSingleton<IRequestContext, DefaultRequestContext>();
        }
    }
}