#if !COREFX
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
    /// <summary>
    /// The implementation of the IOC container.  Implements the IDependencyRegistrar and
    /// IDependencyResolver, along with the IContainerFluent and IDisposable interfaces.
    /// The container is thread safe.
    /// </summary>
    public partial class IocContainer : IContainerFluent, IDisposable
    {
        private readonly TypeRegistry typeRegistry = new TypeRegistry();
        private readonly TypeRegistry opentypeRegistry = new TypeRegistry();

        // Track whether Dispose has been called.
        private bool disposed;

        // null for the lifetime manager is the same as AlwaysNew, but slightly faster.
        public ILifetimeManager DefaultLifetimeManager { get; set; }

        public IContainerFluent UsesDefaultLifetimeManagerOf(ILifetimeManager lifetimeManager)
        {
            DefaultLifetimeManager = lifetimeManager;
            return this;
        }

        /// <summary>
        /// Initializes a new instance of the IocContainer class;
        /// The types IocContainer, IDependencyRegistrar, and IDependencyResolver are all registered
        /// to resolve to this instance of the class.
        /// </summary>
        public IocContainer()
        {
            RegisterInstance<IocContainer>(this);
            RegisterInstance<IDependecyRegistrar>(this);
            RegisterInstance<IDependencyResolver>(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// Disposes of all Container scoped (ContainerLifetime) instances cached in the type registry, and
        /// disposes of the type registry itself.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implements the Disposed(boolean disposing) method of Disposable pattern.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all ContainerLifetime instances
                if (disposing)
                {
                    typeRegistry.Dispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// The finalizer just ensures the container is disposed.
        /// </summary>
        ~IocContainer()
        {
            Dispose(false);
        }
    }
}
#endif