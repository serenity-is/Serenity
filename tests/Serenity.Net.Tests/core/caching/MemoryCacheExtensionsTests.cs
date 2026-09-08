using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

public class MemoryCacheExtensionsTests
{
    private static MemoryCache NewCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public void Add_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IMemoryCache)null).Add("key", "value", TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void Add_WithZeroExpiration_SetsValueWithoutExpiration()
    {
        var cache = NewCache();
        var result = cache.Add("key", "value", TimeSpan.Zero);
        Assert.Equal("value", result);
        Assert.Equal("value", cache.Get("key"));
    }

    [Fact]
    public void Add_WithPositiveExpiration_SetsValue()
    {
        var cache = NewCache();
        var result = cache.Add("key", "value", TimeSpan.FromMinutes(1));
        Assert.Equal("value", result);
        Assert.Equal("value", cache.Get("key"));
    }

    [Fact]
    public void Add_WithNegativeExpiration_RemovesExistingValue()
    {
        var cache = NewCache();
        cache.Set("key", "value");
        var result = cache.Add("key", "newvalue", TimeSpan.FromMinutes(-1));
        Assert.Equal("newvalue", result);
        Assert.Null(cache.Get("key"));
    }

    [Fact]
    public void Get_WithLoader_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IMemoryCache)null).Get("key", TimeSpan.FromMinutes(1), () => "value"));
    }

    [Fact]
    public void Get_WithLoader_ReturnsCachedValue()
    {
        var cache = NewCache();
        cache.Set("key", "cached");
        var result = cache.Get("key", TimeSpan.FromMinutes(1), () => "loaded");
        Assert.Equal("cached", result);
    }

    [Fact]
    public void Get_WithLoader_LoadsAndCachesWhenMissing()
    {
        var cache = NewCache();
        var loadCount = 0;
        var result = cache.Get("key", TimeSpan.FromMinutes(1), () =>
        {
            loadCount++;
            return "loaded";
        });

        Assert.Equal("loaded", result);
        Assert.Equal(1, loadCount);

        // Second call should use cache
        var result2 = cache.Get("key", TimeSpan.FromMinutes(1), () =>
        {
            loadCount++;
            return "loaded";
        });
        Assert.Equal("loaded", result2);
        Assert.Equal(1, loadCount);
    }

    [Fact]
    public void Get_WithLoader_ReturnsNull_WhenLoaderReturnsNull()
    {
        var cache = NewCache();
        var result = cache.Get<string>("key", TimeSpan.FromMinutes(1), () => null);
        Assert.Null(result);
    }

    [Fact]
    public void Get_WithLoader_ReturnsNull_WhenLoaderIsNull()
    {
        var cache = NewCache();
        var result = cache.Get<string>("key", TimeSpan.FromMinutes(1), null);
        Assert.Null(result);
    }

    [Fact]
    public void Get_WithLoader_ReturnsNull_WhenCachedDBNull()
    {
        var cache = NewCache();
        cache.Set("key", DBNull.Value);
        var result = cache.Get<string>("key", TimeSpan.FromMinutes(1), () => "loaded");
        Assert.Null(result);
    }

    [Fact]
    public void TryGet_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IMemoryCache)null).TryGet<string>("key"));
    }

    [Fact]
    public void TryGet_ReturnsValue_WhenPresent()
    {
        var cache = NewCache();
        cache.Set("key", "value");
        Assert.Equal("value", cache.TryGet<string>("key"));
    }

    [Fact]
    public void TryGet_ReturnsNull_WhenMissing()
    {
        var cache = NewCache();
        Assert.Null(cache.TryGet<string>("key"));
    }

    [Fact]
    public void TryGet_ReturnsNull_WhenTypeMismatch()
    {
        var cache = NewCache();
        cache.Set("key", 42);
        Assert.Null(cache.TryGet<string>("key"));
    }

    [Fact]
    public void RemoveAll_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => ((IMemoryCache)null).RemoveAll());
    }

    [Fact]
    public void RemoveAll_RemovesAllItems_FromMemoryCache()
    {
        var cache = NewCache();
        cache.Set("a", 1);
        cache.Set("b", 2);
        cache.RemoveAll();
        Assert.Null(cache.Get("a"));
        Assert.Null(cache.Get("b"));
    }
}
