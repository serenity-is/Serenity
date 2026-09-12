using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Serenity.Extensions;

public class BasePermissionServiceTTests
{
    const string testRole1 = "testrole1";

    private class TestPermissionService(
        ITwoLevelCache cache,
        IUserAccessor userAccessor,
        IRolePermissionService rolePermissions)
        : BasePermissionService<MockUserPermissionRow, MockUserRoleRow>(
            cache, new NullSqlConnections(), new DefaultTypeSource([]), userAccessor, rolePermissions)
    {
        protected override IDictionary<string, bool> LoadUserPermissions(ClaimsPrincipal user)
        {
            return new Dictionary<string, bool>()
            {
                ["testpermission"] = true
            };
        }

        protected override IEnumerable<string> LoadUserRoles(ClaimsPrincipal user)
        {
            return [testRole1];
        }
    }
    static bool IsSpecialCacheKey(string key)
    {
        return key != null &&
            (key.EndsWith(TwoLevelCache.GenerationSuffix) ||
             key == MockUserPermissionRow.Fields.GenerationKey ||
             key == MockUserRoleRow.Fields.GenerationKey);
    }

    [Fact]
    public void Caches_UserRoles_With_UserId_AsPartOfTheKey()
    {
        var memCache = new MockMemoryCache();
        var distCache = new MemoryDistributedCache(Options.Create<MemoryDistributedCacheOptions>(new()));
        var cache = new TwoLevelCache(memCache, distCache);
        string username = "test1";
        var userAccessor = new MockUserAccessor(() => username);
        var rolePermissions = new MockRolePermissions();
        var permissionService = new TestPermissionService(cache, userAccessor, rolePermissions);
        permissionService.HasPermission("TestPermission");
        var cacheItems = memCache.Items
            .Where(x => !IsSpecialCacheKey(x.Key as string))
            .OrderBy(x => (x.Key as string), StringComparer.Ordinal);

        Assert.Collection(cacheItems,
            item =>
            {
                Assert.Equal("UserPermissions:" + MockUserAccessor.MockUserId(username), item.Key);
                var perm = Assert.Single(item.Value.Value as IDictionary<string, bool>);
                Assert.Equal("testpermission", perm.Key);
                Assert.True(perm.Value);
            },
            item =>
            {
                Assert.Equal("UserRoles:" + MockUserAccessor.MockUserId(username), item.Key);
                Assert.Equal([testRole1], item.Value.Value as IEnumerable<string>);
            });

        var cachedRoles = memCache.Get("UserRoles:" + MockUserAccessor.MockUserId(username));

    }
}