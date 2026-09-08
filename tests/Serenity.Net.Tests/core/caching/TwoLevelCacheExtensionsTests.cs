using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Serenity;

public class TwoLevelCacheExtensionsTests
{
    private static TwoLevelCache NewCache()
    {
        var memory = new MemoryCache(new MemoryCacheOptions());
        var distributed = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        return new TwoLevelCache(memory, distributed);
    }

    private class TestItem
    {
        public string Value { get; set; }
    }

    [Fact]
    public void Get_LoadsAndCachesValue()
    {
        var cache = NewCache();
        var loadCount = 0;

        var result = cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), "group",
            () => { loadCount++; return new TestItem { Value = "loaded" }; });

        Assert.Equal("loaded", result.Value);
        Assert.Equal(1, loadCount);

        // Second call should hit cache
        var result2 = cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), "group",
            () => { loadCount++; return new TestItem { Value = "loaded2" }; });

        Assert.Equal("loaded", result2.Value);
        Assert.Equal(1, loadCount);
    }

    [Fact]
    public void Get_WithSingleExpiration_LoadsAndCachesValue()
    {
        var cache = NewCache();
        var result = cache.Get("key", TimeSpan.FromMinutes(1), "group",
            () => new TestItem { Value = "loaded" });

        Assert.Equal("loaded", result.Value);
    }

    [Fact]
    public void Get_ReturnsNull_WhenLoaderReturnsNull()
    {
        var cache = NewCache();
        var result = cache.Get<TestItem>("key", TimeSpan.FromMinutes(1), "group", () => null);
        Assert.Null(result);
    }

    [Fact]
    public void Get_ReturnsNull_WhenLoaderIsNull()
    {
        var cache = NewCache();
        var result = cache.Get<TestItem>("key", TimeSpan.FromMinutes(1), "group", null);
        Assert.Null(result);
    }

    [Fact]
    public void Get_Reloads_WhenGroupExpired()
    {
        var cache = NewCache();
        var loadCount = 0;

        cache.Get("key", TimeSpan.FromMinutes(1), "group",
            () => { loadCount++; return new TestItem { Value = "v1" }; });
        Assert.Equal(1, loadCount);

        cache.ExpireGroupItems("group");

        var result = cache.Get("key", TimeSpan.FromMinutes(1), "group",
            () => { loadCount++; return new TestItem { Value = "v2" }; });

        Assert.Equal("v2", result.Value);
        Assert.Equal(2, loadCount);
    }

    [Fact]
    public void GetLocalStoreOnly_OnlyCachesLocally()
    {
        var cache = NewCache();
        var result = cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), "group",
            () => new TestItem { Value = "local" });

        Assert.Equal("local", result.Value);
    }

    [Fact]
    public void Set_StoresValueInBothCaches()
    {
        var cache = NewCache();
        var result = cache.Set("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), "group",
            new TestItem { Value = "set" });

        Assert.Equal("set", result.Value);

        // Should be retrievable via Get without calling loader
        var loadCount = 0;
        var got = cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), "group",
            () => { loadCount++; return new TestItem { Value = "loaded" }; });

        Assert.Equal("set", got.Value);
        Assert.Equal(0, loadCount);
    }

    [Fact]
    public void Set_WithSingleExpiration_StoresValue()
    {
        var cache = NewCache();
        var result = cache.Set("key", TimeSpan.FromMinutes(1), "group", new TestItem { Value = "set" });
        Assert.Equal("set", result.Value);
    }

    [Fact]
    public void SetLocalStoreOnly_StoresValueLocally()
    {
        var cache = NewCache();
        var result = cache.SetLocalStoreOnly("key", TimeSpan.FromMinutes(1), "group",
            new TestItem { Value = "local" });
        Assert.Equal("local", result.Value);
    }

    [Fact]
    public void ExpireGroupItems_RemovesGroupFromBothCaches()
    {
        var cache = NewCache();
        cache.Memory.Set("group", 1);
        cache.Distributed.SetString("group", "x");

        cache.ExpireGroupItems("group");

        Assert.Null(cache.Memory.Get("group"));
        Assert.Null(cache.Distributed.GetString("group"));
    }

    [Fact]
    public void Remove_RemovesKeyAndGenerationFromBothCaches()
    {
        var cache = NewCache();
        cache.Memory.Set("key", "value");
        cache.Memory.Set("key$Generation$", 1);
        cache.Distributed.SetString("key", "value");
        cache.Distributed.SetString("key$Generation$", "x");

        cache.Remove("key");

        Assert.Null(cache.Memory.Get("key"));
        Assert.Null(cache.Memory.Get("key$Generation$"));
        Assert.Null(cache.Distributed.GetString("key"));
        Assert.Null(cache.Distributed.GetString("key$Generation$"));
    }

    [Fact]
    public void Get_WithNegativeRemoteExpiration_RemovesDistributedEntries()
    {
        var cache = NewCache();
        var result = cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(-1), "group",
            () => new TestItem { Value = "x" });

        Assert.Equal("x", result.Value);
        // Distributed entries should be removed
        Assert.Null(cache.Distributed.GetString("key"));
    }
}
