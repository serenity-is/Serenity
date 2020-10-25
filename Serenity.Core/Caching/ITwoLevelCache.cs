#if !NET45
    using ILocalCache = Microsoft.Extensions.Caching.Memory.IMemoryCache;
#endif

namespace Serenity.Abstractions
{
   /// <summary>
    /// An abstraction for a combination of in memory and distributed cache.
    /// </summary>
    public interface ITwoLevelCache
    {
        /// <summary>
        /// Gets the memory cache
        /// </summary>
        ILocalCache Memory { get; }

        /// <summary>
        /// Gets the distributed cache
        /// </summary>
        IDistributedCache Distributed { get; }
    }
}