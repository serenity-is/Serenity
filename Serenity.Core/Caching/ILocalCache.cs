
namespace Serenity.Abstractions
{
    using System;

    /// <summary>
    /// An abstraction for local cache access (e.g. System.Web.Cache). The 'local' means 
    /// objects in local memory, so no serialization should be in effect.
    /// </summary>
    public interface ILocalCache
    {
        /// <summary>
        /// Adds a value to cache with a given key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expiration">Expire time (Use TimeSpan.Zero to hold value with no expiration)</param>
        void Add(string key, object value, TimeSpan expiration);
        
        /// <summary>
        /// Reads the value with specified key from the local cache.</summary>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="key">Key</param>
        TItem Get<TItem>(string key);
        
        /// <summary>
        /// Removes the value with specified key from the local cache. If the value doesn't exist, no error is raised.
        /// </summary>
        /// <param name="key">Key</param>
        object Remove(string key);

        /// <summary>
        /// Removes all items from the cache (avoid expect unit tests).
        /// </summary>
        void RemoveAll();
    }
}