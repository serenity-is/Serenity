using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Contains helper functions to use local and distributed cache in sync with optional cache invalidation.
/// </summary>
/// <remarks>
/// Creates a new TwoLevelCache instance
/// </remarks>
/// <param name="memoryCache">Memory cache</param>
/// <param name="distributedCache">Distributed cache</param>
public class TwoLevelCache(IMemoryCache memoryCache, IDistributedCache distributedCache) : ITwoLevelCache
{

    /// <summary>
    /// Gets memory cache
    /// </summary>
    public IMemoryCache Memory { get; private set; } = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

    /// <summary>
    /// Gets distributed cache
    /// </summary>
    public IDistributedCache Distributed { get; private set; } = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    /// <summary>
    /// Expiration timeout for cache generation keys
    /// </summary>
    public static readonly TimeSpan GenerationCacheExpiration = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Suffix for cache generation keys
    /// </summary>
    public const string GenerationSuffix = "$Generation$";
}