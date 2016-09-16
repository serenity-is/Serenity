// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Munq
{
    public partial class IocContainer : IDependencyResolver
    {
        public TType Resolve<TType>() where TType : class
        {
            return Resolve(null, typeof(TType)) as TType;
        }
        
        public TType Resolve<TType>(string name) where TType : class
        {
            return Resolve(name, typeof(TType)) as TType;
        }

        public object Resolve(Type type)
        {
            return Resolve(null, type);
        }

        public object Resolve(string name, Type type)
        {
            try
            {
                return typeRegistry.Get(name, type).GetInstance();
            }
            catch (KeyNotFoundException knfe)
            {
                return HandleUnResolved(knfe, name, type);
            }

        }

        private object HandleUnResolved(Exception knfe, string name, Type type)
        {
            if (type.GetIsGenericType())
            {
                object result = ResolveUsingOpenType(knfe, name, type);
                if (result!=null)
                    return result;
            }

            if (type.GetIsClass())
            {
                try
                {
                    var func = CreateInstanceDelegateFactory.Create(type);
                    Register(name, type, func);
                    // Thanks to dfullerton for catching this.
                    return typeRegistry.Get(name, type).GetInstance();
                }
                catch
                {
                    throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
                }
            }

            if (type.GetIsInterface())
            {
                var regs = typeRegistry.GetDerived(name, type);
                var reg = regs.FirstOrDefault();
                if (reg != null)
                {
                    object instance = reg.GetInstance();
                    Register(name, type, (c) => c.Resolve(name, instance.GetType()));
                    return instance;
                }
                else
                    throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
            }
            throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
        }

        private object ResolveUsingOpenType(Exception knfe, string name, Type type)
        {
            if (type.GetContainsGenericParameters())
                throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
            else
            {
                // Look for an Open Type Definition registration
                // create a type using the registered Open Type
                // Try and resolve this type
                var definition = type.GetGenericTypeDefinition();
                var arguments  = type.GetGenericArguments();
                var reg = opentypeRegistry.TryGet(name, definition);
                if (reg != null)
                {
                    var implType = reg.ImplType;
                    var newType  = implType.MakeGenericType(arguments);
                    try
                    {
                        var result = TryResolve(name, newType);
                        if (result != null)
                            return Resolve(name, newType);

                        Register(name, type, newType).WithLifetimeManager(reg.LifetimeManager);
                        return typeRegistry.Get(name, type).GetInstance();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public TType TryResolve<TType>()
                where TType : class
        {
            return (TType)TryResolve(null, typeof(TType));
        }

        public TType TryResolve<TType>(string name)
                where TType : class
        {
            return (TType)TryResolve(name, typeof(TType));
        }

        public object TryResolve(Type type)
        {
            return TryResolve(null, type);
        }

        public object TryResolve(string name, Type type)
        {
            var registration = typeRegistry.TryGet(name, type);
            return registration != null ? registration.GetInstance() : null;
        }

        public IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return ResolveAll(typeof(TType)).Cast<TType>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var registrations = typeRegistry.GetDerived(type);
            var instances = new List<object>();
            foreach (var reg in registrations)
            {
                instances.Add(reg.GetInstance());
            }
            return instances;
        }

        public Func<TType> LazyResolve<TType>() where TType : class
        {
            return LazyResolve<TType>(null);
        }

        public Func<TType> LazyResolve<TType>(string name) where TType : class
        {
            return () => Resolve<TType>(name);
        }

        public Func<Object> LazyResolve(Type type)
        {
            return LazyResolve(null, type);
        }

        public Func<Object> LazyResolve(string name, Type type)
        {
            return () => Resolve(name, type);
        }

        private static string ResolveFailureMessage(Type type)
        {
            return String.Format("Munq IocContainer failed to resolve {0}", type);
        }
    }
}