using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Data;
using Serenity.Localization;
using Serenity.Services;
using System;
using System.Linq;

namespace Serenity.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRequestHandlers(this IServiceCollection services)
        {
            services.TryAddSingleton<IBehaviorFactory, DefaultBehaviorFactory>();
            services.TryAddSingleton<IImplicitBehaviorRegistry, DefaultImplicitBehaviorRegistry>();
            services.TryAddSingleton<IBehaviorProvider, DefaultBehaviorProvider>();
            services.TryAddSingleton<IRequestContext, DefaultRequestContext>();
        }

        public static void AddAllTexts(this IServiceProvider provider,
            params string[] jsonTextPaths)
        {
            var typeSource = provider.GetRequiredService<ITypeSource>();
            var textRegistry = provider.GetRequiredService<ILocalTextRegistry>();
            var rowTypeRegistry = provider.GetRequiredService<IRowTypeRegistry>();
            textRegistry.AddNestedTexts(typeSource);
            textRegistry.AddEnumTexts(typeSource);
            var rowInstances = rowTypeRegistry.AllRowTypes.Select(x => (IRow)Activator.CreateInstance(x));
            textRegistry.AddRowTexts(rowInstances);
            foreach (var path in jsonTextPaths)
                textRegistry.AddJsonTexts(path);
        }
    }
}