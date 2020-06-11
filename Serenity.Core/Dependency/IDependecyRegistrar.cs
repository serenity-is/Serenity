#if NET45
namespace Serenity
{
    /// <summary>
    /// Dependency registerer abstraction
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Registers an instance as singleton implementation of TType.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>Registration reference.</returns>
        object RegisterInstance<TType>(TType instance) where TType : class;

        /// <summary>
        /// Registers TImpl as implementation of TType.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <returns>Registration reference</returns>
        object Register<TType, TImpl>() where TType : class where TImpl : class, TType;

        /// <summary>
        /// Registers the instance for specified name. Avoid this as it is not supported in .NET Core.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>Registration reference</returns>
        object RegisterInstance<TType>(string name, TType instance) where TType : class;

        /// <summary>
        /// Registers TImpl as implementation of TType for specified name. Avoid this as it is not supported in .NET Core.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>Registration reference</returns>
        object Register<TType, TImpl>(string name) where TType : class where TImpl : class, TType;

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <param name="registration">The registration reference.</param>
        void Remove(object registration);
    }
}
#endif