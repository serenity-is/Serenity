using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity.TestUtils;

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
