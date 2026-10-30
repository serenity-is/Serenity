using Microsoft.Extensions.Caching.Distributed;

namespace Serenity.Testing;

public class NullTwoLevelCache : ITwoLevelCache
{
    public NullTwoLevelCache()
    {
    }

    public IMemoryCache Memory { get; } = new NullMemoryCache();
    public IDistributedCache Distributed { get; } = new NullDistributedCache();
}