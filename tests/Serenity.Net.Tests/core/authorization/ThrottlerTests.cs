using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Serenity;

public class ThrottlerTests
{
    private static MemoryCache NewMemoryCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    private static MemoryDistributedCache NewDistributedCache()
    {
        return new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
    }

    [Fact]
    public void Ctor_InitializesProperties()
    {
        var duration = TimeSpan.FromMinutes(2);
        var throttler = new Throttler(NewMemoryCache(), "user1", duration, 5);

        Assert.Equal("user1", throttler.Key);
        Assert.Equal(duration, throttler.Duration);
        Assert.Equal(5, throttler.Limit);
        Assert.Equal("Throttling:user1:" + duration.Ticks.ToInvariant(), throttler.CacheKey);
    }

    [Fact]
    public void Check_ReturnsTrueUntilLimitIsExceeded_WithMemoryCache()
    {
        var throttler = new Throttler(NewMemoryCache(), "user1", TimeSpan.FromMinutes(1), 3);

        Assert.True(throttler.Check());
        Assert.True(throttler.Check());
        Assert.True(throttler.Check());
        Assert.False(throttler.Check());
        Assert.False(throttler.Check());
    }

    [Fact]
    public void Check_IsolatedBetweenKeys()
    {
        var cache = NewMemoryCache();
        var throttler1 = new Throttler(cache, "user1", TimeSpan.FromMinutes(1), 2);
        var throttler2 = new Throttler(cache, "user2", TimeSpan.FromMinutes(1), 2);

        Assert.True(throttler1.Check());
        Assert.True(throttler2.Check());
        Assert.True(throttler1.Check());
        Assert.True(throttler2.Check());
        Assert.False(throttler1.Check());
        Assert.False(throttler2.Check());
    }

    [Fact]
    public void Reset_ClearsCount_WithMemoryCache()
    {
        var throttler = new Throttler(NewMemoryCache(), "user1", TimeSpan.FromMinutes(1), 1);

        Assert.True(throttler.Check());
        Assert.False(throttler.Check());

        throttler.Reset();
        Assert.True(throttler.Check());
    }

    [Fact]
    public void Check_ReturnsTrueUntilLimitIsExceeded_WithDistributedCache()
    {
        var throttler = new Throttler(NewDistributedCache(), "user1", TimeSpan.FromMinutes(1), 3);

        Assert.True(throttler.Check());
        Assert.True(throttler.Check());
        Assert.True(throttler.Check());
        Assert.False(throttler.Check());
        Assert.False(throttler.Check());
    }

    [Fact]
    public void Reset_ClearsCount_WithDistributedCache()
    {
        var throttler = new Throttler(NewDistributedCache(), "user1", TimeSpan.FromMinutes(1), 1);

        Assert.True(throttler.Check());
        Assert.False(throttler.Check());

        throttler.Reset();
        Assert.True(throttler.Check());
    }
}
