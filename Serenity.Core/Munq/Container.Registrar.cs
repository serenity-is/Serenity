#if !COREFX
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

        public IRegistration Register<TType>(Func<IDependencyResolver, TType> func)
            where TType : class
        {
            return Register(null, typeof(TType), c => (func(c) as Object));
        }

        public IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func)
            where TType : class
        {
            return Register(name, typeof(TType), c => (func(c) as Object));
        }

        public IRegistration Register(Type type, Func<IDependencyResolver, object> func)
        {
            return Register(null, type, func);
        }

        public IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            var entry = new Registration(this, name, type, func);
            entry.WithLifetimeManager(DefaultLifetimeManager);
            typeRegistry.Add(entry);
            return entry;
        }

        public IRegistration Register<TType, TImpl>()
            where TType : class
            where TImpl : class, TType
        {
            return Register(typeof(TType), typeof(TImpl));
        }

        public IRegistration Register<TType, TImpl>(string name)
            where TType : class
            where TImpl : class, TType
        {
            return Register(name, typeof(TType), typeof(TImpl));
        }

        public IRegistration Register(Type tType, Type tImpl)
        {
            return Register(null, tType, tImpl);
        }
        
        public IRegistration Register(string name, Type tType, Type tImpl)
        {
            if (tType.GetContainsGenericParameters())
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

        public IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return Register<TType>(c => instance).WithLifetimeManager(null);
        }

        public IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return Register<TType>(name, c => instance).WithLifetimeManager(null);
        }

        public IRegistration RegisterInstance(Type type, object instance)
        {
            return Register(type, c => instance).WithLifetimeManager(null);
        }

        public IRegistration RegisterInstance(string name, Type type, object instance)
        {
            return Register(name, type, c => instance).WithLifetimeManager(null);
        }

        public void Remove(IRegistration ireg)
        {
            typeRegistry.Remove(ireg);
        }

        public IRegistration GetRegistration<TType>() where TType : class
        {
            return GetRegistration(null, typeof(TType));
        }

        public IRegistration GetRegistration<TType>(string name) where TType : class
        {
            return GetRegistration(name, typeof(TType));
        }

        public IRegistration GetRegistration(Type type)
        {
            return GetRegistration(null, type);
        }

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

        public IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class
        {
            return GetRegistrations(typeof(TType));
        }

        public IEnumerable<IRegistration> GetRegistrations(Type type)
        {
            return typeRegistry.All(type).Cast<IRegistration>();
        }
    }
}
#endif