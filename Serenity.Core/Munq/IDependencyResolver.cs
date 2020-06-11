#if NET45
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
    /// <summary>
    /// Dependency resolver
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        TType Resolve<TType>() where TType : class;
        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        TType Resolve<TType>(string name) where TType : class;
        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object Resolve(Type type);
        /// <summary>
        /// Resolves the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object Resolve(string name, Type type);

        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        IEnumerable<TType> ResolveAll<TType>() where TType : class;
        /// <summary>
        /// Resolves all.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IEnumerable<object> ResolveAll(Type type);

        /// <summary>
        /// Lazies the resolve.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        Func<TType> LazyResolve<TType>() where TType : class;
        /// <summary>
        /// Lazies the resolve.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Func<TType> LazyResolve<TType>(string name) where TType : class;
        /// <summary>
        /// Lazies the resolve.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Func<object> LazyResolve(Type type);
        /// <summary>
        /// Lazies the resolve.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        Func<object> LazyResolve(string name, Type type);

        /// <summary>
        /// Tries the resolve.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        TType TryResolve<TType>() where TType : class;
        /// <summary>
        /// Tries the resolve.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        TType TryResolve<TType>(string name) where TType : class;
        /// <summary>
        /// Tries the resolve.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object TryResolve(Type type);
        /// <summary>
        /// Tries the resolve.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        object TryResolve(string name, Type type);
    }
}
#endif