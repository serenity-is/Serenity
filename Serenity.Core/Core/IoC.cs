using System;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using Munq;

namespace Serenity
{
    /// <summary>
    /// Provides an entry point to Munq IoC Container.
    /// This class only uses one IoCContainer, we don't need more than one
    /// in any project so far, and for unit tests, we use a basic workaround.
    /// </summary>
    public static class IoC
    {
        private static IocContainer container;

        static IoC()
        {
            container = new IocContainer();
        }

        public static TType Resolve<TType>() where TType : class
        {
            return container.Resolve<TType>();
        }

        public static TType Resolve<TType>(string name) where TType : class
        {
            return container.Resolve<TType>(name);
        }

        public static object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public static object Resolve(string name, Type type)
        {
            return container.Resolve(name, type);
        }

        public static IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return container.ResolveAll<TType>();
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return container.ResolveAll(type);
        }

        public static Func<TType> LazyResolve<TType>() where TType : class
        {
            return container.LazyResolve<TType>();
        }

        public static Func<TType> LazyResolve<TType>(string name) where TType : class
        {
            return container.LazyResolve<TType>(name);
        }

        public static Func<object> LazyResolve(Type type)
        {
            return container.LazyResolve(type);
        }

        public static Func<object> LazyResolve(string name, Type type)
        {
            return container.LazyResolve(name, type);
        }

        public static bool CanResolve<TType>() where TType : class
        {
            return container.CanResolve<TType>();
        }

        public static bool CanResolve<TType>(string name) where TType : class
        {
            return container.CanResolve<TType>(name);
        }

        public static bool CanResolve(Type type)
        {
            return container.CanResolve(type);
        }

        public static bool CanResolve(string name, Type type)
        {
            return container.CanResolve(name, type);
        }

        public static ILifetimeManager DefaultLifetimeManager 
        {
            get { return container.DefaultLifetimeManager; }
            set { container.DefaultLifetimeManager = value; }
        }

        public static IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class
        {
            return container.Register<TType>(func);
        }

        public static IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class
        {
            return container.Register<TType>(name, func);
        }

        public static IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return container.Register(type, func);
        }

        public static IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            return container.Register(name, type, func);
        }

        public static IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return container.RegisterInstance(instance);
        }

        public static IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return container.RegisterInstance(name, instance);
        }

        public static IRegistration RegisterInstance(Type type, object instance)
        {
            return container.RegisterInstance(type, instance);
        }

        public static IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return container.RegisterInstance(name, type, instance);
        }

        public static IRegistration Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>();
        }

        public static IRegistration Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>(name);
        }

        public static IRegistration Register(Type tType, Type tImpl)
        {
            return container.Register(tType, tImpl);
        }

        public static IRegistration Register(string name, Type tType, Type tImpl)
        {
            return container.Register(name, tType, tImpl);
        }

        public static void Remove(IRegistration ireg)
        {
            container.Remove(ireg);
        }

        public static IRegistration GetRegistration<TType>() where TType : class
        {
            return container.GetRegistration<TType>();
        }

        public static IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return container.GetRegistration<TType>(name);
        }

        public static IRegistration GetRegistration(Type type)
        {
            return container.GetRegistration(type);
        }

        public static IRegistration GetRegistration(string name, Type type)
        {
            return container.GetRegistration(name, type);
        }

        public static IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return container.GetRegistrations<TType>();
        }

        public static IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return container.GetRegistrations(type);
        }

        /// <summary>
        /// If a type is registered for TType, resolves it, otherwise returns a new instance of TType.
        /// Use this function in place of "new TType()" to allow unit testing.
        /// </summary>
        /// <typeparam name="TType">Type (usually with virtual methods that will be faked in unit tests)</typeparam>
        /// <returns>An instance of TType</returns>
        /// <remarks>This might not seem like a correct way to use IoC, 
        /// but as it solves 99% of our unit test needs, it just works.</remarks>
        public static TType New<TType>() 
            where TType : class, new()
        {
            if (CanResolve<TType>())
                return Resolve<TType>();

            return new TType();
        }

        /// <summary>
        /// If a type is registered for TInterface, resolves it, otherwise 
        /// returns a new instance of TType (default implementation). Use this
        /// function in place of "new TType()" to allow overriding interface
        /// implementation in unit tests.
        /// </summary>
        /// <typeparam name="TInterface">Interface</typeparam>
        /// <typeparam name="TType">Default implementation</typeparam>
        /// <returns>A new object</returns>
        /// <remarks>This might not seem like a correct way to use IoC, 
        /// but as it solves 99% of our unit test needs, it just works.</remarks>
        public static TInterface New<TInterface, TType>()
            where TInterface : class
            where TType: TInterface, new()
        {
            if (CanResolve<TInterface>())
                return Resolve<TInterface>();

            return new TType();
        }

        /// <summary>
        /// Use this class only for unit tests, it is not thread-safe!
        /// </summary>
        /// <returns>
        /// A disposable object that will revert changes made in block when disposed.
        /// </returns>
        public static IDisposable StartContext()
        {
            return new Context();
        }

        private class Context : IDisposable
        {
            private IocContainer old;
            private IocContainer mine;

            public Context()
            {
                old = container;
                container = new IocContainer();
            }

            public void Dispose()
            {
                if (old == null)
                    return;

                if (mine == null)
                    return;
                    
                if (mine != container)
                    throw new InvalidOperationException("Container changed! Possible multi-thread error, or disposing order mistake!");

                mine = null;
                container.Dispose();
                container = old;
                old = null;
            }
        }
    }
}
