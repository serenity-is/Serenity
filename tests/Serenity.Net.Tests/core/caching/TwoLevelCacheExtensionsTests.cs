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

    public class TestItem
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
    public void GetLocalStoreOnly_MultipleGroups_CachesTheValueLocally()
    {
        var cache = NewCache();
        var loadCount = 0;
        TestItem load() { loadCount++; return new TestItem { Value = "v" + loadCount }; }

        Assert.Equal("v1", cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
        Assert.Equal("v1", cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
        Assert.Equal(1, loadCount);
        Assert.Null(cache.Distributed.Get("key"));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public void GetLocalStoreOnly_MultipleGroups_ExpiresWithAnyOfTheGroups(string group)
    {
        var cache = NewCache();
        var loadCount = 0;
        TestItem load() { loadCount++; return new TestItem { Value = "v" + loadCount }; }

        cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load);
        cache.ExpireGroupItems(group);

        Assert.Equal("v2", cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
    }

    [Fact]
    public void GetLocalStoreOnly_MultipleGroups_DoesNotExpireWithAnotherGroup()
    {
        var cache = NewCache();
        var loadCount = 0;
        TestItem load() { loadCount++; return new TestItem { Value = "v" + loadCount }; }

        cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load);
        cache.ExpireGroupItems("c");

        Assert.Equal("v1", cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
    }

    [Fact]
    public void GetLocalStoreOnly_MultipleGroups_ShareTheGenerationsOfTheSingleGroupOverload()
    {
        var cache = NewCache();
        var single = 0;
        var multiple = 0;

        cache.GetLocalStoreOnly("single", TimeSpan.FromMinutes(1), "a", () => { single++; return new TestItem(); });
        cache.GetLocalStoreOnly("multiple", TimeSpan.FromMinutes(1), ["a", "b"], () => { multiple++; return new TestItem(); });

        cache.GetLocalStoreOnly("single", TimeSpan.FromMinutes(1), "a", () => { single++; return new TestItem(); });
        cache.GetLocalStoreOnly("multiple", TimeSpan.FromMinutes(1), ["a", "b"], () => { multiple++; return new TestItem(); });
        Assert.Equal(1, single);
        Assert.Equal(1, multiple);

        cache.ExpireGroupItems("a");

        cache.GetLocalStoreOnly("single", TimeSpan.FromMinutes(1), "a", () => { single++; return new TestItem(); });
        cache.GetLocalStoreOnly("multiple", TimeSpan.FromMinutes(1), ["a", "b"], () => { multiple++; return new TestItem(); });
        Assert.Equal(2, single);
        Assert.Equal(2, multiple);
    }

    [Fact]
    public void GetLocalStoreOnly_MultipleGroups_CachesANullValue()
    {
        var cache = NewCache();
        var loadCount = 0;
        TestItem load() { loadCount++; return null; }

        Assert.Null(cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load));
        Assert.Null(cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], load));
        Assert.Equal(1, loadCount);
    }

    [Fact]
    public void GetLocalStoreOnly_MultipleGroups_RequiresAGroup()
    {
        var cache = NewCache();

        Assert.Throws<ArgumentException>(() => cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), [], () => new TestItem()));
    }

    [Fact]
    public void Get_MultipleGroups_RequiresAGroup()
    {
        var cache = NewCache();

        Assert.Throws<ArgumentException>(() => cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), [], () => new TestItem()));
        Assert.Throws<ArgumentNullException>(() => cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), (IReadOnlyList<string>)null, () => new TestItem()));
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    public void Get_MultipleGroups_ExpiresWithAnyOfTheGroups(string group)
    {
        var cache = NewCache();
        var loadCount = 0;
        TestItem load() { loadCount++; return new TestItem { Value = "v" + loadCount }; }

        Assert.Equal("v1", cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
        Assert.Equal("v1", cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load).Value);

        cache.ExpireGroupItems(group);

        Assert.Equal("v2", cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
    }

    [Fact]
    public void Get_MultipleGroups_ReadsTheItemOfAnotherServer_UntilAGroupExpires()
    {
        var distributed = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var server1 = new TwoLevelCache(new MemoryCache(new MemoryCacheOptions()), distributed);
        var server2 = new TwoLevelCache(new MemoryCache(new MemoryCacheOptions()), distributed);
        var loadCount = 0;
        TestItem load() { loadCount++; return new TestItem { Value = "v" + loadCount }; }

        server1.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load);

        // a fresh local cache, so only the distributed one can provide the item
        Assert.Equal("v1", server2.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
        Assert.Equal(1, loadCount);

        var server3 = new TwoLevelCache(new MemoryCache(new MemoryCacheOptions()), distributed);
        server3.ExpireGroupItems("b");

        var server4 = new TwoLevelCache(new MemoryCache(new MemoryCacheOptions()), distributed);
        Assert.Equal("v2", server4.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], load).Value);
    }

    [Fact]
    public void Get_SingleGroup_StoresTheItemWithTheGenerationOfTheGroup()
    {
        var cache = NewCache();

        cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), "group", () => new TestItem());

        // so the items in a distributed cache stay valid for the versions with and without several groups
        Assert.Equal(cache.Distributed.Get("group"), cache.Distributed.Get("key" + TwoLevelCache.GenerationSuffix));
    }

    [Fact]
    public void Get_ReloadsAnItemInDistributedCache_WhenItsGroupsChange()
    {
        var distributed = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        // a fresh local cache each time, like a restarted server, so only the distributed one can provide the item
        TwoLevelCache server() => new(new MemoryCache(new MemoryCacheOptions()), distributed);
        var expiration = TimeSpan.FromMinutes(1);

        // stored while the item depended on a single group
        server().Get("key", expiration, expiration, "a", () => new TestItem { Value = "single" });

        // now depends on several groups, which the stored generation doesn't match
        Assert.Equal("several", server().Get("key", expiration, expiration, ["a", "b"], () => new TestItem { Value = "several" }).Value);
        Assert.Equal("several", server().Get("key", expiration, expiration, ["a", "b"], () => new TestItem { Value = "unexpected" }).Value);

        // and back, e.g. a server still running the previous version
        Assert.Equal("single again", server().Get("key", expiration, expiration, "a", () => new TestItem { Value = "single again" }).Value);
        Assert.Equal("single again", server().Get("key", expiration, expiration, "a", () => new TestItem { Value = "unexpected" }).Value);
    }

    public static TheoryData<string, Func<ITwoLevelCache, Func<TestItem>, TestItem>> Readers => new()
    {
        { "get", (cache, loader) => cache.Get("key", TimeSpan.FromMinutes(1), "a", loader) },
        { "local", (cache, loader) => cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), "a", loader) },
        { "get several", (cache, loader) => cache.Get("key", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), ["a", "b"], loader) },
        { "local several", (cache, loader) => cache.GetLocalStoreOnly("key", TimeSpan.FromMinutes(1), ["a", "b"], loader) }
    };

    [Theory]
    [MemberData(nameof(Readers))]
    public void Get_DoesNotKeepAnItem_LoadedWhileItsGroupExpires(string _, Func<ITwoLevelCache, Func<TestItem>, TestItem> read)
    {
        var cache = NewCache();
        var loadCount = 0;

        var first = read(cache, () =>
        {
            loadCount++;
            // the data changes after the loader has read it
            cache.ExpireGroupItems("a");
            return new TestItem { Value = "stale" };
        });
        Assert.Equal("stale", first.Value);

        var second = read(cache, () => { loadCount++; return new TestItem { Value = "fresh" }; });

        Assert.Equal("fresh", second.Value);
        Assert.Equal(2, loadCount);

        var third = read(cache, () => { loadCount++; return new TestItem { Value = "unexpected" }; });

        Assert.Equal("fresh", third.Value);
        Assert.Equal(2, loadCount);
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
