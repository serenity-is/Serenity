using System;

namespace Serenity
{
    public interface ICache
    {
        void Add(string key, object value, TimeSpan expiration);
        TItem Get<TItem>(string key);
        object Remove(string key);
        void RemoveAll();
    }
}