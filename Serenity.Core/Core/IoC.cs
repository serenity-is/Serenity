using System;
using System.Collections.Generic;
using Munq;

namespace Serenity
{
    public static class IoC
    {
        private static readonly IocContainer Container;

        static IoC()
        {
            Container = new IocContainer();
        }

        public static TType Resolve<TType>() where TType : class
        {
            return Container.Resolve<TType>();
        }

        public static TType Resolve<TType>(string name) where TType : class
        {
            return Container.Resolve<TType>(name);
        }

        public static object Resolve(Type type)
        {
            return Container.Resolve(type);
        }

        public static object Resolve(string name, Type type)
        {
            return Container.Resolve(name, type);
        }

        public static IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return Container.ResolveAll<TType>();
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return Container.ResolveAll(type);
        }

        public static Func<TType> LazyResolve<TType>() where TType : class
        {
            return Container.LazyResolve<TType>();
        }

        public static Func<TType> LazyResolve<TType>(string name) where TType : class
        {
            return Container.LazyResolve<TType>(name);
        }

        public static Func<object> LazyResolve(Type type)
        {
            return Container.LazyResolve(type);
        }

        public static Func<object> LazyResolve(string name, Type type)
        {
            return Container.LazyResolve(name, type);
        }

        public static bool CanResolve<TType>() where TType : class
        {
            return Container.CanResolve<TType>();
        }

        public static bool CanResolve<TType>(string name) where TType : class
        {
            return Container.CanResolve<TType>(name);
        }

        public static bool CanResolve(Type type)
        {
            return Container.CanResolve(type);
        }

        public static bool CanResolve(string name, Type type)
        {
            return Container.CanResolve(name, type);
        }

        public static ILifetimeManager DefaultLifetimeManager 
        {
            get { return Container.DefaultLifetimeManager; }
            set { Container.DefaultLifetimeManager = value; }
        }

        public static IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class
        {
            return Container.Register<TType>(func);
        }

        public static IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class
        {
            return Container.Register<TType>(name, func);
        }

        public static IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return Container.Register(type, func);
        }

        public static IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            return Container.Register(name, type, func);
        }

        public static IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return Container.RegisterInstance(instance);
        }

        public static IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return Container.RegisterInstance(name, instance);
        }

        public static IRegistration RegisterInstance(Type type, object instance)
        {
            return Container.RegisterInstance(type, instance);
        }

        public static IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return Container.RegisterInstance(name, type, instance);
        }

        public static IRegistration Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return Container.Register<TType, TImpl>();
        }

        public static IRegistration Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return Container.Register<TType, TImpl>(name);
        }

        public static IRegistration Register(Type tType, Type tImpl)
        {
            return Container.Register(tType, tImpl);
        }

        public static IRegistration Register(string name, Type tType, Type tImpl)
        {
            return Container.Register(name, tType, tImpl);
        }

        public static void Remove(IRegistration ireg)
        {
            Container.Remove(ireg);
        }

        public static IRegistration GetRegistration<TType>() where TType : class
        {
            return Container.GetRegistration<TType>();
        }

        public static IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return Container.GetRegistration<TType>(name);
        }

        public static IRegistration GetRegistration(Type type)
        {
            return Container.GetRegistration(type);
        }

        public static IRegistration GetRegistration(string name, Type type)
        {
            return Container.GetRegistration(name, type);
        }

        public static IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return Container.GetRegistrations<TType>();
        }

        public static IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return Container.GetRegistrations(type);
        }
    }
}
