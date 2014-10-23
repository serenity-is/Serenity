namespace Serenity
{
    using System;
    
    /// <summary>
    /// Service locator for Serenity. It requires setting an IoC container 
    /// that implements IDependencyResolver interface through SetResolver 
    /// method to operate normally.
    /// </summary>
    public static class Dependency
    {
        private static IDependencyResolver resolver;

        /// <summary>
        /// Maps TService service to its registered provider.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <exception cref="System.KeyNotFoundException">
        /// No provider is registered for TService</exception>
        /// <exception cref="System.InvalidProgramException">
        /// No dependency resolver is configured using SetResolver</exception>
        public static TService Resolve<TService>() where TService : class
        {
            return Resolver.Resolve<TService>();
        }

        /// <summary>
        /// Maps TService service to its registered provider for specified scope.
        /// Use this method when an interface can be handled by different providers
        /// in different scopes (e.g. Application / Server configuration scope)
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Scope name</param>
        /// <exception cref="System.KeyNotFoundException">
        /// No provider is registered for TService</exception>
        /// <exception cref="System.InvalidProgramException">
        /// No dependency resolver is configured using SetResolver</exception>
        public static TService Resolve<TService>(string name) where TService : class
        {
            return Resolver.Resolve<TService>(name);
        }

        /// <summary>
        /// Maps TService service to its registered provider. 
        /// Returns null if registration for TService doesn't exist or 
        /// no dependency resolver is configured using SetResolver.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        public static TService TryResolve<TService>() where TService : class
        {
            return resolver == null ? null : Resolver.TryResolve<TService>();
        }

        /// <summary>
        /// Maps TService service to its registered provider for specified scope.
        /// Returns null if registration for TService doesn't exist or 
        /// no dependency resolver is configured using SetResolver.
        /// Use this method when an interface can be handled by different providers
        /// in different scopes (e.g. Application / Server configuration scope)
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Scope name</param>
        public static TService TryResolve<TService>(string name) where TService : class
        {
            return resolver == null ? null : Resolver.TryResolve<TService>(name);
        }

        /// <summary>
        /// Sets current dependency resolver and returns previous one if exists.
        /// </summary>
        /// <param name="value">Dependency resolver</param>
        public static IDependencyResolver SetResolver(IDependencyResolver value)
        {
            var old = resolver;
            resolver = value;
            return old;
        }

        /// <summary>
        /// Returns true if a dependency resolver is set through SetResolver.
        /// Use this property to check if there is a current resolver as Resolver 
        /// property raises an exception if not.
        /// </summary>
        public static bool HasResolver
        {
            get { return resolver != null; }
        }

        /// <summary>
        /// Returns currently registered IDependencyResolver implementation.
        /// </summary>
        /// <exception cref="System.InvalidProgramException">
        /// No dependency resolver is configured using SetResolver</exception>
        public static IDependencyResolver Resolver
        {
            get
            {
                var current = resolver;

                if (current == null)
                    throw new InvalidProgramException(
                        "Default IDependencyResolver implementation is not set. Use Serenity.Dependency.SetResolver " +
                        "to set your dependency resolver implementation.");

                return current;
            }
        }
    }
}