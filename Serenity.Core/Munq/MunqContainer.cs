#if !COREFX
using System;

namespace Serenity
{
    /// <summary>
    /// IDependencyResolver and IDependencyRegistrar implementation using Munq.
    /// </summary>
    public class MunqContainer : 
        Serenity.IDependencyResolver, 
        Serenity.IDependencyRegistrar, 
        IDisposable
    {
        private Munq.IocContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MunqContainer"/> class.
        /// </summary>
        public MunqContainer()
        {
            container = new Munq.IocContainer();
            container.RegisterInstance<Serenity.IDependencyRegistrar>(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        /// <summary>
        /// Resolves the service type from registered dependencies.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>Resolved service of type TService</returns>
        public TService Resolve<TService>() where TService : class
        {
            return container.Resolve<TService>();
        }

        /// <summary>
        /// Resolves the service type from registered dependencies with given name.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Resolved service of type TService with given name.
        /// </returns>
        public TService Resolve<TService>(string name) where TService : class
        {
            return container.Resolve<TService>(name);
        }

        /// <summary>
        /// Tries to resolve the service type from registered dependencies.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>Resolved service of type TService, or null if none.</returns>
        public TService TryResolve<TService>() where TService : class
        {
            return container.TryResolve<TService>();
        }

        /// <summary>
        /// Tries to resolve the service type from registered dependencies with given name.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Resolved service of type TService with given name, or null if none.
        /// </returns>
        public TService TryResolve<TService>(string name) where TService : class
        {
            return container.TryResolve<TService>(name);
        }

        /// <summary>
        /// Registers an instance as singleton implementation of TType.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// Registration reference.
        /// </returns>
        public object RegisterInstance<TType>(TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(instance);
        }

        /// <summary>
        /// Registers the instance for specified name. Avoid this as it is not supported in .NET Core.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// Registration reference
        /// </returns>
        public object RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(name, instance);
        }

        /// <summary>
        /// Registers TImpl as implementation of TType.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <returns>
        /// Registration reference
        /// </returns>
        public object Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>();
        }

        /// <summary>
        /// Registers TImpl as implementation of TType for specified name. Avoid this as it is not supported in .NET Core.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Registration reference
        /// </returns>
        public object Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return container.Register<TType, TImpl>(name);
        }

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <param name="registration">The registration reference.</param>
        public void Remove(object registration)
        {
            container.Remove((Munq.IRegistration)registration);
        }
    }
}
#endif