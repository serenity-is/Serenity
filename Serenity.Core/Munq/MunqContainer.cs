#if !COREFX
using System;

namespace Serenity
{
    public class MunqContainer : 
        Serenity.IDependencyResolver, 
        Serenity.IDependencyRegistrar, 
        IDisposable
    {
        private Munq.IocContainer container;

        public MunqContainer()
        {
            container = new Munq.IocContainer();
            container.RegisterInstance<Serenity.IDependencyRegistrar>(this);
        }

        public void Dispose()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        public TService Resolve<TService>() where TService : class
        {
            return container.Resolve<TService>();
        }

        public TService Resolve<TService>(string name) where TService : class
        {
            return container.Resolve<TService>(name);
        }

        public TService TryResolve<TService>() where TService : class
        {
            return container.TryResolve<TService>();
        }

        public TService TryResolve<TService>(string name) where TService : class
        {
            return container.TryResolve<TService>(name);
        }

        public object RegisterInstance<TType>(TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(instance);
        }

        public object RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(name, instance);
        }

        public object Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>();
        }

        public object Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>(name);
        }

        public void Remove(object registration)
        {
            container.Remove((Munq.IRegistration)registration);
        }
    }
}
#endif