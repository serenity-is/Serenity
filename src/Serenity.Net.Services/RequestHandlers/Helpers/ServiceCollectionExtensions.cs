using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Linq;
using Serenity.Localization;
using System.IO;

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
            collection.TryAddTransient(typeof(ICreateHandler<>), typeof(CreateHandlerProxy<>));
            collection.TryAddTransient(typeof(IUpdateHandler<,,>), typeof(UpdateHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IUpdateHandler<>), typeof(UpdateHandlerProxy<>));
            collection.TryAddTransient(typeof(IDeleteHandler<,,>), typeof(DeleteHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IDeleteHandler<>), typeof(DeleteHandlerProxy<>));
            collection.TryAddTransient(typeof(IListHandler<,,>), typeof(ListHandlerProxy<,,>));
            collection.TryAddTransient(typeof(IListHandler<,>), typeof(ListHandlerProxy<,>));
            collection.TryAddTransient(typeof(IListHandler<>), typeof(ListHandlerProxy<>));
            collection.TryAddTransient(typeof(IRetrieveHandler<,,>), typeof(RetrieveHandlerProxy<,,>));
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
            collection.AddServiceResolver();
            collection.AddCustomRequestHandlers(customHandlerTypeSource, customHandlerPredicate);
            collection.AddProxyRequestHandlers();

            collection.TryAddSingleton<IRequestContext, DefaultRequestContext>();
            return collection;
        }

        /// <summary>
        /// Adds nested texts, enum texts, permission texts, row texts and json local text assets
        /// </summary>
        /// <param name="provider">The service provider that will be used to locate text registry and other types</param>
        /// <param name="webFileProvider">If passed, json texts from static web assets will be added from Serenity.Scripts,
        /// and any assembly with a JsonLocalTextAssetsAttribute attribute</param>
        /// <returns>Local text registry</returns>
        /// <exception cref="ArgumentNullException">Provider is null</exception>
        public static ILocalTextRegistry AddBaseTexts(this IServiceProvider provider, IFileProvider webFileProvider)
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

            if (webFileProvider is not null)
            {
                textRegistry.AddJsonTexts(webFileProvider, "Serenity.Scripts/texts");
                foreach (var attr in typeSource.GetAssemblyAttributes<JsonLocalTextAssetsAttribute>())
                    if (!string.IsNullOrEmpty(attr.Path))
                        textRegistry.AddJsonTexts(webFileProvider, attr.Path);
            }

            return textRegistry;
        }

        [Obsolete("Use AddBaseTexts().AddJsonTexts().AddJsonTexts()...")]
        /// <summary>
        /// Adds type texts and JSON texts from passed folders
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="jsonTextPaths"></param>
        public static void AddAllTexts(this IServiceProvider provider, params string[] jsonTextPaths)
        {
            var textRegistry = AddBaseTexts(provider, webFileProvider: null);
            foreach (var path in jsonTextPaths)
                textRegistry.AddJsonTexts(path);
        }

        /// <summary>
        /// Adds json texts from file provider and sub path
        /// </summary>
        /// <param name="registry">The text registry</param>
        /// <param name="provider">File provider</param>
        /// <param name="subpath">Sub path</param>
        /// <param name="recursive">True to recursively scan (default true)</param>
        /// <returns>The text registry</returns>
        /// <exception cref="ArgumentNullException">registry, provider or sub path is null</exception>
        public static ILocalTextRegistry AddJsonTexts(this ILocalTextRegistry registry, IFileProvider provider, string subpath, bool recursive = true)
        {
            if (registry is null)
                throw new ArgumentNullException(nameof(registry));

            if (provider is null)
                throw new ArgumentNullException(nameof(provider));
            
            if (subpath is null)
                throw new ArgumentNullException(nameof(subpath));

            var contents = provider.GetDirectoryContents(subpath);
            if (contents is null || !contents.Exists)
                return registry;

            foreach (var entry in contents.OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                if (entry.IsDirectory)
                {
                    if (recursive)
                        AddJsonTexts(registry, provider, Path.Combine(subpath, entry.Name), recursive);
                    continue;
                }

                if (!entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                using var stream = entry.CreateReadStream();
                using var sr = new StreamReader(stream);
                string json = sr.ReadToEnd().TrimToNull();
                if (json is null)
                    continue;
                var texts = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json);
                var langID = Path.GetFileNameWithoutExtension(entry.Name);
                var idx = langID.LastIndexOf(".");
                if (idx >= 0)
                    langID = langID[(idx + 1)..];

                if (langID.ToLowerInvariant() == "invariant")
                    langID = "";

                JsonLocalTextRegistration.AddFromNestedDictionary(texts, "", langID, registry);
            }

            return registry;
        }
    }
}