#if NET45
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Munq
{
    public partial class IocContainer : IDependecyRegistrar
    {
        private const string STR_RegistrationNotFoundFor = "Registration not found for {0}";

        /// <summary>
        /// Registers the specified function.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public IRegistration Register<TType>(Func<IDependencyResolver, TType> func)
            where TType : class
        {
            return Register(null, typeof(TType), c => (func(c) as Object));
        }

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func)
            where TType : class
        {
            return Register(name, typeof(TType), c => (func(c) as Object));
        }

        /// <summary>
        /// Registers the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return Register(null, type, func);
        }

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">func</exception>
        public IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            var entry = new Registration(this, name, type, func);
            entry.WithLifetimeManager(DefaultLifetimeManager);
            typeRegistry.Add(entry);
            return entry;
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <returns></returns>
        public IRegistration Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return Register(typeof(TType), typeof(TImpl));
        }

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TImpl">The type of the implementation.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IRegistration Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return Register(name, typeof(TType), typeof(TImpl));
        }

        /// <summary>
        /// Registers the specified t type.
        /// </summary>
        /// <param name="tType">Type of the t.</param>
        /// <param name="tImpl">The t implementation.</param>
        /// <returns></returns>
        public IRegistration Register(Type tType, Type tImpl)
        {
            return Register(null, tType, tImpl);
        }

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="tType">Type of the t.</param>
        /// <param name="tImpl">The t implementation.</param>
        /// <returns></returns>
        public IRegistration Register(string name, Type tType, Type tImpl)
        {
            if (tType.ContainsGenericParameters)
                return RegisterOpenType(name, tType, tImpl);
                
            return Register(name, tType, CreateInstanceDelegateFactory.Create(tImpl));
        }

        private IRegistration RegisterOpenType(string name, Type tType, Type tImpl)
        {
            var entry = new Registration(this, name, tType, tImpl);
            entry.WithLifetimeManager(DefaultLifetimeManager);
            opentypeRegistry.Add(entry);
            return entry;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return Register<TType>(c => instance).WithLifetimeManager(null);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return Register<TType>(name, c => instance).WithLifetimeManager(null);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IRegistration RegisterInstance(Type type, object instance)
        {
            return Register(type, c => instance).WithLifetimeManager(null);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return Register(name, type, c => instance).WithLifetimeManager(null);
        }

        /// <summary>
        /// Removes the specified ireg.
        /// </summary>
        /// <param name="ireg">The ireg.</param>
        public void Remove(IRegistration ireg)
        {
            typeRegistry.Remove(ireg);
        }

        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public IRegistration GetRegistration<TType>() where TType : class
        {
            return GetRegistration(null, typeof(TType));
        }

        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return GetRegistration(name, typeof(TType));
        }

        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IRegistration GetRegistration(Type type)
        {
            return GetRegistration(null, type);
        }

        /// <summary>
        /// Gets the registration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public IRegistration GetRegistration(string name, Type type)
        {
            try
            {
                return typeRegistry.Get(name, type);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException(String.Format(STR_RegistrationNotFoundFor, type), ex);
            }
        }

        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return GetRegistrations(typeof(TType));
        }

        /// <summary>
        /// Gets the registrations.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return typeRegistry.All(type).Cast<IRegistration>();
        }
    }
}
#endif