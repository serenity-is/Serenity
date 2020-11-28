using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Data;
using Serenity.Localization;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void AddAllTexts(this ILocalTextRegistry textRegistry, IEnumerable<string> jsonTextPaths,
            IEnumerable<Assembly> assemblies)
        {
            textRegistry.AddNestedTexts(assemblies);
            textRegistry.AddEnumTexts(assemblies);
            var rowInstances = DefaultRowTypeRegistry.EnumerateRowTypes(assemblies)
                .Select(x => (IRow)Activator.CreateInstance(x));
            textRegistry.AddRowTexts(rowInstances);
            foreach (var path in jsonTextPaths)
                textRegistry.AddJsonTexts(path);
        }
    }
}