using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Serenity.Tests;

public class NullMemoryCache : IMemoryCache
{
    public ICacheEntry CreateEntry(object key)
    {
        return new NullCacheEntry { Key = key };
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
    }

    public void Remove(object key)
    {
    }

    public bool TryGetValue(object key, out object value)
    {
        value = null;
        return false;
    }

    private class NullCacheEntry : ICacheEntry
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();
        public object Key { get; set; }
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = new List<PostEvictionCallbackRegistration>();
        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public object Value { get; set; }

        public void Dispose()
        {
        }
    }
}