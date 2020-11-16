#if TODO
using Serenity.Data;
using Serenity.Reflection;
using System;
using System.Collections.Generic;

namespace Serenity.Services
{
    /// <summary>
    /// Base class that stores a static, cached list of THandler type handlers that are 
    /// used by some behaviors like MasterDetailRelationBehavior instead of generic handlers.
    /// </summary>
    /// <typeparam name="TRow">Type of row</typeparam>
    public static class DefaultHandlerFactory
    {
        private static Dictionary<Type, Factories> factoriesByType = new Dictionary<Type, Factories>();

        private class Factories
        {
            public Func<ISaveRequestProcessor> saveHandlerFactory;
            public Func<ISaveRequest> saveRequestFactory;
            public Func<IListRequestProcessor> listHandlerFactory;
            public Func<ListRequest> listRequestFactory;
            public Func<IDeleteRequestProcessor> deleteHandlerFactory;
            public Func<DeleteRequest> deleteRequestFactory;
            public Func<IRetrieveRequestProcessor> retrieveHandlerFactory;
            public Func<RetrieveRequest> retrieveRequestFactory;
        }

        private static Type[] IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return toCheck.GetGenericArguments();
                toCheck = toCheck.BaseType;
            }
            return null;
        }

        public static IListRequestProcessor ListHandlerFor(Type rowType)
        {
            return GetFactoriesFor(rowType).listHandlerFactory();
        }

        public static ListRequest ListRequestFor(Type rowType)
        {
            return GetFactoriesFor(rowType).listRequestFactory();
        }

        public static IRetrieveRequestProcessor RetrieveHandlerFor(Type rowType)
        {
            return GetFactoriesFor(rowType).retrieveHandlerFactory();
        }

        public static RetrieveRequest RetrieveRequestFor(Type rowType)
        {
            return GetFactoriesFor(rowType).retrieveRequestFactory();
        }

        public static ISaveRequestProcessor SaveHandlerFor(Type rowType)
        {
            return GetFactoriesFor(rowType).saveHandlerFactory();
        }

        public static ISaveRequest SaveRequestFor(Type rowType)
        {
            return GetFactoriesFor(rowType).saveRequestFactory();
        }

        public static IDeleteRequestProcessor DeleteHandlerFor(Type rowType)
        {
            return GetFactoriesFor(rowType).deleteHandlerFactory();
        }

        public static DeleteRequest DeleteRequestFor(Type rowType)
        {
            return GetFactoriesFor(rowType).deleteRequestFactory();
        }

        private static Factories GetFactoriesFor(Type rowType)
        {
            Factories factories;
            if (factoriesByType.TryGetValue(rowType, out factories))
                return factories;

            factories = new Factories();

            var registry = Dependency.TryResolve<IDefaultHandlerRegistry>() ??
                DefaultHandlerRegistry.Instance;

            foreach (var handlerType in registry.GetTypes())
            {
                Type[] genericArguments;
                if (typeof(IListRequestProcessor).IsAssignableFrom(handlerType))
                {
                    genericArguments = IsSubclassOfRawGeneric(typeof(ListRequestHandler<,,>), handlerType);
                    if (genericArguments == null || genericArguments[0] != rowType)
                        continue;

                    if (factories.listRequestFactory != null)
                        throw new InvalidProgramException("There are multiple ListRequestHandler types " +
                            "with [DefaultHandler] attribute for row type " + rowType.FullName);

                    factories.listHandlerFactory = FastReflection
                        .DelegateForConstructor<IListRequestProcessor>(handlerType);
                    factories.listRequestFactory = FastReflection
                        .DelegateForConstructor<ListRequest>(genericArguments[1]);
                    continue;
                }

                if (typeof(IRetrieveRequestProcessor).IsAssignableFrom(handlerType))
                {
                    genericArguments = IsSubclassOfRawGeneric(typeof(RetrieveRequestHandler<,,>), handlerType);
                    if (genericArguments == null || genericArguments[0] != rowType)
                        continue;

                    if (factories.retrieveRequestFactory != null)
                        throw new InvalidProgramException("There are multiple RetrieveRequestHandler types " +
                            "with [DefaultHandler] attribute for row type " + rowType.FullName);

                    factories.retrieveHandlerFactory = FastReflection
                        .DelegateForConstructor<IRetrieveRequestProcessor>(handlerType);
                    factories.retrieveRequestFactory = FastReflection
                        .DelegateForConstructor<RetrieveRequest>(genericArguments[1]);
                    continue;
                }

                if (typeof(ISaveRequestProcessor).IsAssignableFrom(handlerType))
                {
                    genericArguments = IsSubclassOfRawGeneric(typeof(SaveRequestHandler<,,>), handlerType);
                    if (genericArguments == null || genericArguments[0] != rowType)
                        continue;

                    if (factories.saveRequestFactory != null)
                        throw new InvalidProgramException("There are multiple SaveRequestHandler types " +
                            "with [DefaultHandler] attribute for row type " + rowType.FullName);

                    factories.saveHandlerFactory = FastReflection
                        .DelegateForConstructor<ISaveRequestProcessor>(handlerType);
                    factories.saveRequestFactory = FastReflection
                        .DelegateForConstructor<ISaveRequest>(genericArguments[1]);
                    continue;
                }

                if (typeof(IDeleteRequestProcessor).IsAssignableFrom(handlerType))
                {
                    genericArguments = IsSubclassOfRawGeneric(typeof(DeleteRequestHandler<,,>), handlerType);
                    if (genericArguments == null || genericArguments[0] != rowType)
                        continue;

                    if (factories.deleteRequestFactory != null)
                        throw new InvalidProgramException("There are multiple DeleteRequestHandler types " +
                            "with [DefaultHandler] attribute for row type " + rowType.FullName);

                    factories.deleteHandlerFactory = FastReflection
                        .DelegateForConstructor<IDeleteRequestProcessor>(handlerType);
                    factories.deleteRequestFactory = FastReflection
                        .DelegateForConstructor<DeleteRequest>(genericArguments[1]);
                    continue;
                }

            }

            factories.listHandlerFactory = factories.listHandlerFactory ?? FastReflection
                .DelegateForConstructor<IListRequestProcessor>(typeof(ListRequestHandler<>).MakeGenericType(rowType));
            factories.listRequestFactory = factories.listRequestFactory ?? new Func<ListRequest>(() => new ListRequest());

            factories.retrieveHandlerFactory = factories.retrieveHandlerFactory ?? FastReflection
                .DelegateForConstructor<IRetrieveRequestProcessor>(typeof(RetrieveRequestHandler<>).MakeGenericType(rowType));
            factories.retrieveRequestFactory = factories.retrieveRequestFactory ?? new Func<RetrieveRequest>(() => new RetrieveRequest());

            factories.saveHandlerFactory = factories.saveHandlerFactory ?? FastReflection
                .DelegateForConstructor<ISaveRequestProcessor>(typeof(SaveRequestHandler<>).MakeGenericType(rowType));
            factories.saveRequestFactory = factories.saveRequestFactory ?? FastReflection
                .DelegateForConstructor<ISaveRequest>(typeof(SaveRequest<>).MakeGenericType(rowType));

            factories.deleteHandlerFactory = factories.deleteHandlerFactory ?? FastReflection
                .DelegateForConstructor<IDeleteRequestProcessor>(typeof(DeleteRequestHandler<>).MakeGenericType(rowType));
            factories.deleteRequestFactory = factories.deleteRequestFactory ?? new Func<DeleteRequest>(() => new DeleteRequest());

            var newDict = new Dictionary<Type, Factories>(factoriesByType);
            newDict[rowType] = factories;
            factoriesByType = newDict;

            return factories;
        }
    }
}
#endif