// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
    public interface IDependecyRegistrar
    {
        ILifetimeManager DefaultLifetimeManager { get; set; }
        IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class;
        IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class;
        IRegistration Register(Type type, Func<IDependencyResolver, object> func);
        IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func);
        IRegistration RegisterInstance<TType>(TType instance) where TType : class;
        IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class;
        IRegistration RegisterInstance(Type type, object instance);
        IRegistration RegisterInstance(string name, Type type, object instance);
        IRegistration Register<TType, TImpl>() where TType: class where TImpl : class, TType;
        IRegistration Register<TType, TImpl>(string name) where TType: class where TImpl : class, TType;
        IRegistration Register(Type tType, Type tImpl);
        IRegistration Register(string name, Type tType, Type tImpl);
        void Remove(IRegistration ireg);
        IRegistration GetRegistration<TType>() where TType : class;
        IRegistration GetRegistration<TType>(string name) where TType : class;
        IRegistration GetRegistration(Type type);
        IRegistration GetRegistration(string name, Type type);
        IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class;
        IEnumerable<IRegistration> GetRegistrations(Type type);
    }
}