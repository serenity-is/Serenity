using System;

namespace Serenity.Abstractions
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan expiration);
        TItem Get<TItem>(string key);
        object Remove(string key);
        void RemoveAll();
    }
}