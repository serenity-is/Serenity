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
        public static IServiceCollection AddServiceBehaviors(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IBehaviorFactory, DefaultBehaviorFactory>();
            collection.TryAddSingleton<IImplicitBehaviorRegistry, DefaultImplicitBehaviorRegistry>();
            collection.TryAddSingleton<IBehaviorProvider, DefaultBehaviorProvider>();
            return collection;
        }

        public static IServiceCollection AddServiceHandlerFactory(this IServiceCollection collection)
        {
            collection.TryAddSingleton<IHandlerActivator, DefaultHandlerActivator>();
            collection.TryAddSingleton<IDefaultHandlerRegistry, DefaultHandlerRegistry>();
            collection.TryAddSingleton<IDefaultHandlerFactory, DefaultHandlerFactory>();
            return collection;
        }

        public static IServiceCollection AddServiceHandlers(this IServiceCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddCaching();
            collection.AddEntities();
            collection.AddTextRegistry();
            collection.AddServiceBehaviors();
            collection.AddServiceHandlerFactory();
            collection.TryAddSingleton<IRequestContext, DefaultRequestContext>();
            return collection;
        }

        public static void AddAllTexts(this IServiceProvider provider,
            params string[] jsonTextPaths)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

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