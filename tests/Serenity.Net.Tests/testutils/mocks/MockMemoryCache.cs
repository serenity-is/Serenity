using Microsoft.Extensions.Caching.Memory;

namespace Serenity.TestUtils;

public sealed class MockMemoryCache : IMemoryCache
{
    public Dictionary<object, CacheEntry> Items { get; } = [];

    public class CacheEntry : ICacheEntry
    {
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        public IList<IChangeToken> ExpirationTokens { get; } = [];

        public object Key { get; set; }

        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = [];

        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public object Value { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public ICacheEntry CreateEntry(object key)
    {
        var entry = new CacheEntry { Key = key };
        Items[key] = entry;
        return entry;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Remove(object key)
    {
        Items.Remove(key);
    }

    public bool TryGetValue(object key, out object value)
    {
        if (Items.TryGetValue(key, out var t))
        {
            value = t;
            return true;
        }

        value = null;
        return false;
    }
}