using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity.Tests.Caching;

public class TwoLevelCacheTests
{
    private class UserDefinition
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    public class TwoLevelCache_GetTests
    {
        private static UserDefinition GetUserByName(ITwoLevelCache cache, string username, ref int loadCount)
        {
            var l = loadCount;
            var result = cache.Get("UserByName_" + username.ToLowerInvariant(),
                TimeSpan.Zero, TimeSpan.FromDays(1), "UserGenerationKey", () =>
                {
                    l++;
                    return new UserDefinition { UserId = 1, Username = username };
                });

            loadCount = l;
            return result;
        }

        [Fact]
        public void Get_WithEmptyCache()
        {
            var memoryCache = new MemoryCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryCacheOptions>(
                new MemoryCacheOptions()));
            var distributedCache = new MemoryDistributedCache(
                    new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(
                        new MemoryDistributedCacheOptions()));
            var cache = new TwoLevelCache(memoryCache, distributedCache);
            var loadCount = 0;
            var user = GetUserByName(cache, "admin", ref loadCount);
            Assert.Equal("admin", user.Username);
            Assert.Equal(1, loadCount);
        }

        [Fact]
        public void Get_WithLocalCachedItem_Null_Distributed_NotNull()
        {
            var memoryCache = new MemoryCache(new Microsoft.Extensions.Options.OptionsWrapper<MemoryCacheOptions>(
                new MemoryCacheOptions()));
            memoryCache.Set("UserByName_admin$Generation$", (ulong)641427502);
            memoryCache.Set("UserGenerationKey", (ulong)641427502);
            memoryCache.Set<UserDefinition>("UserByName_admin", null);
            var distributedCache = new MemoryDistributedCache(
                    new Microsoft.Extensions.Options.OptionsWrapper<MemoryDistributedCacheOptions>(
                        new MemoryDistributedCacheOptions()));
            distributedCache.Set("UserByName_admin$Generation$", BitConverter.GetBytes((ulong)641427502));
            distributedCache.SetAutoJson("UserByName_admin", new UserDefinition { Username = "admin", UserId = 1 });
            distributedCache.Set("UserGenerationKey", BitConverter.GetBytes((ulong)641427502));
            var cache = new TwoLevelCache(memoryCache, distributedCache);
            var loadCount = 0;
            var user = GetUserByName(cache, "admin", ref loadCount);
            Assert.Equal("admin", user?.Username);
            Assert.Equal(0, loadCount);
        }
    }
}