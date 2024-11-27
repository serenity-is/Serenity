using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity.Tests;

public class NullTwoLevelCache : ITwoLevelCache
{
    public NullTwoLevelCache()
    {
    }

    public IMemoryCache Memory { get; } = new NullMemoryCache();
    public IDistributedCache Distributed { get; } = new NullDistributedCache();
}