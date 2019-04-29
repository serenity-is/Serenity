namespace Serenity
{
    /// <summary>
    /// Dependency resolver abstraction
    /// </summary>
    public interface IDependencyResolver
    {

        /// <summary>
        /// Resolves the service type from registered dependencies.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>Resolved service of type TService</returns>
        TService Resolve<TService>() where TService : class;

        /// <summary>
        /// Tries to resolve the service type from registered dependencies.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>Resolved service of type TService, or null if none.</returns>
        TService TryResolve<TService>() where TService : class;

#if !COREFX
        /// <summary>
        /// Resolves the service type from registered dependencies with given name.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Resolved service of type TService with given name.
        /// </returns>
        TService Resolve<TService>(string name) where TService : class;

        /// <summary>
        /// Tries to resolve the service type from registered dependencies with given name.
        /// </summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>
        /// Resolved service of type TService with given name, or null if none.
        /// </returns>
        TService TryResolve<TService>(string name) where TService : class;
#endif
    }
}
