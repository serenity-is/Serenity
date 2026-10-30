using Microsoft.Extensions.Caching.Distributed;

namespace Serenity.Testing;

public class TestTwoLevelCache : ITwoLevelCache
{
    public TestTwoLevelCache()
    {
        Memory = new MemoryCache(new MemoryCacheOptions());
        Distributed = new NullDistributedCache();
    }

    public IMemoryCache Memory { get; }
    public IDistributedCache Distributed { get; }
}
