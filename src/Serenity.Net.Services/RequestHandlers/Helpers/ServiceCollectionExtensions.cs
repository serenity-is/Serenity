using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serenity.Abstractions;
using Serenity.Data;
using Serenity.Localization;
using Serenity.Services;
using System;
using System.Collections.Generic;
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

        public static IServiceCollection AddProxyRequestHandlers(this IServiceCollection collection)
        {
            collection.TryAddTransient(typeof(ICreateHandler<,,>), typeof(CreateHandlerProxy<,,>));
            collection.TryAddTransient(typeof(ICreateHandler<,>), typeof(CreateHandlerProxy<,>));
            collection.TryAddTransient(typeof(ICreateHandler<>), typeof(CreateHandlerProxy<>));
            collection.TryAddTransient(typeof(IUpdateHandler<,,>), typeof(UpdateHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IUpdateHandler<,>), typeof(UpdateHandlerProxy<,>));
            collection.TryAddTransient(typeof(IUpdateHandler<>), typeof(UpdateHandlerProxy<>));
            collection.TryAddTransient(typeof(IDeleteHandler<,,>), typeof(DeleteHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IDeleteHandler<,>), typeof(DeleteHandlerProxy<,>));
            collection.TryAddTransient(typeof(IDeleteHandler<>), typeof(DeleteHandlerProxy<>));
            collection.TryAddTransient(typeof(IListHandler<,,>), typeof(ListHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IListHandler<,>), typeof(ListHandlerProxy<,>));
            collection.TryAddTransient(typeof(IListHandler<>), typeof(ListHandlerProxy<>));
            collection.TryAddTransient(typeof(IRetrieveHandler<,,>), typeof(RetrieveHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IRetrieveHandler<,>), typeof(RetrieveHandlerProxy<,>));
            collection.TryAddTransient(typeof(IRetrieveHandler<>), typeof(RetrieveHandlerProxy<>));

            return collection;
        }

        public static IServiceCollection AddCustomRequestHandlers(this IServiceCollection collection, 
            ITypeSource typeSource = null, Func<Type, Type, bool> predicate = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            typeSource ??= collection.FirstOrDefault(x => x.ServiceType == typeof(ITypeSource))?.ImplementationInstance as ITypeSource;
            if (typeSource == null)
                throw new ArgumentNullException(nameof(typeSource));

            var interfacesToAdd = new Dictionary<Type, List<Type>>();

            foreach (var type in typeSource.GetTypesWithInterface(typeof(IRequestHandler)))
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                    continue;

                if (predicate == null || predicate(type, type))
                    collection.TryAddTransient(type, type);

                foreach (var intf in type.GetInterfaces())
                {
                    if (!typeof(IRequestHandler).IsAssignableFrom(intf))
                        continue;

                    if (intf == typeof(IRequestHandler) ||
                        intf == typeof(ISaveRequestProcessor) ||
                        intf == typeof(ISaveRequestHandler) ||
                        intf == typeof(IListRequestProcessor) ||
                        intf == typeof(IListRequestHandler) ||
                        intf == typeof(IRetrieveRequestProcessor) ||
                        intf == typeof(IRetrieveRequestHandler) ||
                        intf == typeof(IDeleteRequestProcessor) ||
                        intf == typeof(IDeleteRequestHandler))
                        continue;

                    if (intf.IsGenericType)
                    {
                        var genericBase = intf.GetGenericTypeDefinition();
                        if (genericBase == typeof(IRequestHandler<,,>) ||
                            genericBase == typeof(IRequestHandler<>) ||
                            genericBase == typeof(IRequestType<>) ||
                            genericBase == typeof(IResponseType<>))
                            continue;
                    }

                    if (predicate != null && !predicate(intf, type))
                        continue;

                    if (interfacesToAdd.TryGetValue(intf, out var list))
                        list.Add(type);
                    else
                        interfacesToAdd[intf] = new List<Type> { type };
                }
            }

            foreach (var pair in interfacesToAdd)
            {
                // don't override users own registration if any
                var existing = collection.FirstOrDefault(x => x.ServiceType == pair.Key);
                if (existing != null)
                    continue;

                if (pair.Value.Count != 1)
                {
                    var defaults = pair.Value.Where(x => x.GetAttribute<DefaultHandlerAttribute>()?.Value == true).ToArray();
                    if (defaults.Length != 1)
                    {
                        if (defaults.Length == 0)
                            throw new InvalidProgramException($"There are multiple {pair.Key} request handler types ({string.Join(", ", pair.Value)}). " +
                                "Please add [DefaultHandler] to one of them!");

                        throw new InvalidProgramException($"There are multiple {pair.Key} request handler types with [DefaultHandler] attribute ({string.Join(", ", (IEnumerable<Type>)defaults)}). " +
                            "Please use [DefaultHandler] on only one of them!");
                    }

                    collection.AddTransient(pair.Key, defaults[0]);
                    continue;
                }

                collection.AddTransient(pair.Key, pair.Value[0]);
            }

            return collection;
        }

        public static IServiceCollection AddServiceHandlers(this IServiceCollection collection, 
            ITypeSource customHandlerTypeSource = null, Func<Type, Type, bool> customHandlerPredicate = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            collection.AddCaching();
            collection.AddEntities();
            collection.AddTextRegistry();
            collection.AddServiceBehaviors();
            collection.AddServiceHandlerFactory();
            collection.AddCustomRequestHandlers(customHandlerTypeSource, customHandlerPredicate);
            collection.AddProxyRequestHandlers();

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
            textRegistry.AddNestedPermissions(typeSource);
            var rowInstances = rowTypeRegistry.AllRowTypes.Select(x => (IRow)Activator.CreateInstance(x));
            textRegistry.AddRowTexts(rowInstances);
            foreach (var path in jsonTextPaths)
                textRegistry.AddJsonTexts(path);
        }
    }
}