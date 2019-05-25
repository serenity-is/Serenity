#if !COREFX
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public interface IDependecyRegistrar
    {
        /// <summary>
        /// Gets or sets the default lifetime manager.
        /// </summary>
        /// <value>
        /// The default lifetime manager.
        /// </value>
        ILifetimeManager DefaultLifetimeManager { get; set; }
        /// <summary>
        /// Registers the specified function.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class;
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class;
        /// <summary>
        /// Registers the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        IRegistration Register(Type type, Func<IDependencyResolver, object> func);
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func);
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IRegistration RegisterInstance<TType>(TType instance) where TType : class;
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class;
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IRegistration RegisterInstance(Type type, object instance);
        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        IRegistration RegisterInstance(string name, Type type, object instance);
        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <returns></returns>
        IRegistration Register<TType, TImpl>() where TType: class where TImpl : class, TType;
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IRegistration Register<TType, TImpl>(string name) where TType: class where TImpl : class, TType;
        /// <summary>
        /// Registers the specified t type.
        /// </summary>
        /// <param name="tType">Type of the t.</param>
        /// <param name="tImpl">The t implementation.</param>
        /// <returns></returns>
        IRegistration Register(Type tType, Type tImpl);
        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tType">Type of the t.</param>
        /// <param name="tImpl">The t implementation.</param>
        /// <returns></returns>
        IRegistration Register(string name, Type tType, Type tImpl);
        /// <summary>
        /// Removes the specified ireg.
        /// </summary>
        /// <param name="ireg">The ireg.</param>
        void Remove(IRegistration ireg);
        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        IRegistration GetRegistration<TType>() where TType : class;
        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IRegistration GetRegistration<TType>(string name) where TType : class;
        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IRegistration GetRegistration(Type type);
        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IRegistration GetRegistration(string name, Type type);
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class;
        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<IRegistration> GetRegistrations(Type type);
    }
}
#endif