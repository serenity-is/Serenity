using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Serenity;

public class DistributedCacheExtensionsTests
{
    private static MemoryDistributedCache NewCache()
    {
        return new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
    }

    [Fact]
    public void SetAutoJson_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IDistributedCache)null).SetAutoJson("key", "value"));
    }

    [Fact]
    public void SetAutoJson_RemovesKey_WhenValueIsNull()
    {
        var cache = NewCache();
        cache.SetString("key", "value");
        cache.SetAutoJson<string>("key", null);
        Assert.Null(cache.GetString("key"));
    }

    [Fact]
    public void SetAutoJson_StoresString_AsUtf8Bytes()
    {
        var cache = NewCache();
        cache.SetAutoJson("key", "hello");
        Assert.Equal("hello", cache.GetString("key"));
    }

    [Fact]
    public void SetAutoJson_StoresByteArray_Directly()
    {
        var cache = NewCache();
        var bytes = new byte[] { 1, 2, 3 };
        cache.SetAutoJson("key", bytes);
        Assert.Equal(bytes, cache.Get("key"));
    }

    [Fact]
    public void SetAutoJson_StoresObject_AsJson()
    {
        var cache = NewCache();
        cache.SetAutoJson("key", new TestDto { Name = "test", Value = 42 });
        var json = cache.GetString("key");
        Assert.Contains("test", json);
        Assert.Contains("42", json);
    }

    [Fact]
    public void SetAutoJson_WithExpiration_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IDistributedCache)null).SetAutoJson("key", "value", TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void SetAutoJson_WithExpiration_RemovesKey_WhenValueIsNull()
    {
        var cache = NewCache();
        cache.SetString("key", "value");
        cache.SetAutoJson<string>("key", null, TimeSpan.FromMinutes(1));
        Assert.Null(cache.GetString("key"));
    }

    [Fact]
    public void SetAutoJson_WithNegativeExpiration_RemovesKey()
    {
        var cache = NewCache();
        cache.SetString("key", "value");
        cache.SetAutoJson("key", "newvalue", TimeSpan.FromMinutes(-1));
        Assert.Null(cache.GetString("key"));
    }

    [Fact]
    public void SetAutoJson_WithZeroExpiration_StoresWithoutExpiration()
    {
        var cache = NewCache();
        cache.SetAutoJson("key", "value", TimeSpan.Zero);
        Assert.Equal("value", cache.GetString("key"));
    }

    [Fact]
    public void SetAutoJson_WithPositiveExpiration_StoresValue()
    {
        var cache = NewCache();
        cache.SetAutoJson("key", "value", TimeSpan.FromMinutes(1));
        Assert.Equal("value", cache.GetString("key"));
    }

    [Fact]
    public void GetAutoJson_ThrowsArgumentNullException_WhenCacheIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IDistributedCache)null).GetAutoJson<string>("key"));
    }

    [Fact]
    public void GetAutoJson_ReturnsNull_WhenKeyMissing()
    {
        var cache = NewCache();
        Assert.Null(cache.GetAutoJson<string>("key"));
    }

    [Fact]
    public void GetAutoJson_ReturnsByteArray_WhenTypeIsByteArray()
    {
        var cache = NewCache();
        var bytes = new byte[] { 1, 2, 3 };
        cache.Set("key", bytes);
        Assert.Equal(bytes, cache.GetAutoJson<byte[]>("key"));
    }

    [Fact]
    public void GetAutoJson_ReturnsString_WhenTypeIsString()
    {
        var cache = NewCache();
        cache.SetString("key", "hello");
        Assert.Equal("hello", cache.GetAutoJson<string>("key"));
    }

    [Fact]
    public void GetAutoJson_DeserializesObject()
    {
        var cache = NewCache();
        cache.SetAutoJson("key", new TestDto { Name = "test", Value = 42 });
        var dto = cache.GetAutoJson<TestDto>("key");
        Assert.Equal("test", dto.Name);
        Assert.Equal(42, dto.Value);
    }

    private class TestDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
