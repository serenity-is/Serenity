#if !COREFX
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
    public interface IDependencyResolver
    {
        TType Resolve<TType>() where TType : class;
        TType Resolve<TType>(string name) where TType : class;
        object Resolve(Type type);
        object Resolve(string name, Type type);
        
        IEnumerable<TType> ResolveAll<TType>() where TType : class;
        IEnumerable<object> ResolveAll(Type type);

        Func<TType> LazyResolve<TType>() where TType : class;
        Func<TType> LazyResolve<TType>(string name) where TType : class;
        Func<object> LazyResolve(Type type);
        Func<object> LazyResolve(string name, Type type);

        TType TryResolve<TType>() where TType : class;
        TType TryResolve<TType>(string name) where TType : class;
        object TryResolve(Type type);
        object TryResolve(string name, Type type);
    }
}
#endif