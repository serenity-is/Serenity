using System.Collections;
using Serenity.ComponentModel;

namespace Serenity.Extensions;

public class BaseRolePermissionServiceTTests
{
    [ConnectionKey("Default")]
    private class MockRolePermissionRow : Row<MockRolePermissionRow.RowFields>, IRolePermissionRow
    {
        public string? RoleKeyOrName { get => fields.RoleKeyOrName[this]; set => fields.RoleKeyOrName[this] = value; }
        public string? PermissionKey { get => fields.PermissionKey[this]; set => fields.PermissionKey[this] = value; }

        StringField IRolePermissionRow.RoleKeyOrNameField => fields.RoleKeyOrName;
        StringField IRolePermissionRow.PermissionKeyField => fields.PermissionKey;

        public class RowFields : RowFieldsBase
        {
            public StringField RoleKeyOrName = null!;
            public StringField PermissionKey = null!;
        }
    }

    private class TestRolePermissionService(
        ITwoLevelCache cache,
        ISqlConnections sqlConnections,
        ITypeSource typeSource)
        : BaseRolePermissionService<MockRolePermissionRow>(cache, sqlConnections, typeSource)
    {
        public bool IsValidRoleKeyOrNameExposed(string role) => IsValidRoleKeyOrName(role);
        public string GetCacheKeyExposed(string role) => GetCacheKey(role);
        public string GetCacheGroupKeyExposed() => GetCacheGroupKey();
        public TimeSpan GetCacheDurationExposed() => GetCacheDuration();
        public ISet<string> GetCachedRolePermissionsExposed(string role) => GetCachedRolePermissions(role);
        public ISet<string> LoadRolePermissionsExposed(string role) => LoadRolePermissions(role);
    }

    [NestedPermissionKeys]
    private static class ImplicitPermissions
    {
        [ImplicitPermission(Expanded)]
        public const string Base = "perm1";

        public const string Expanded = "implied1";
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var cache = new TestTwoLevelCache();
        var sql = new NullSqlConnections();
        var typeSource = new MockTypeSource();
        Assert.Throws<ArgumentNullException>(() =>
            new TestRolePermissionService(null!, sql, typeSource));
        Assert.Throws<ArgumentNullException>(() =>
            new TestRolePermissionService(cache, null!, typeSource));
        Assert.Throws<ArgumentNullException>(() =>
            new TestRolePermissionService(cache, sql, null!));
    }

    [Fact]
    public void Defaults_And_Keys_Are_Expected()
    {
        var service = new TestRolePermissionService(new TestTwoLevelCache(),
            new NullSqlConnections(), new MockTypeSource());

        Assert.True(service.IsValidRoleKeyOrNameExposed("admin"));
        Assert.False(service.IsValidRoleKeyOrNameExposed(""));
        Assert.Equal("RolePermissions:admin", service.GetCacheKeyExposed("admin"));
        Assert.Equal(MockRolePermissionRow.Fields.GenerationKey, service.GetCacheGroupKeyExposed());
        Assert.Equal(TimeSpan.Zero, service.GetCacheDurationExposed());
    }

    [Fact]
    public void HasPermission_Returns_False_For_Invalid_Role()
    {
        var service = new TestRolePermissionService(new TestTwoLevelCache(),
            new NullSqlConnections(), new MockTypeSource());

        Assert.False(service.HasPermission("", "perm1"));
        Assert.False(service.HasPermission(null!, "perm1"));
    }

    [Fact]
    public void LoadRolePermissions_Maps_Keys_And_Implicit_Permissions()
    {
        using var connection = new MockDbConnection()
            .InterceptListRows(args => new OptionalValue<System.Collections.IList>(new List<MockRolePermissionRow>
            {
                new() { RoleKeyOrName = "admin", PermissionKey = "perm1" },
                new() { RoleKeyOrName = "admin", PermissionKey = "perm2" }
            }));

        var service = new TestRolePermissionService(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            new MockTypeSource(typeof(ImplicitPermissions)));

        var permissions = service.LoadRolePermissionsExposed("admin");
        Assert.Contains("perm1", permissions);
        Assert.Contains("perm2", permissions);
        Assert.Contains("Role:admin", permissions);
        Assert.Contains("implied1", permissions);
    }

    [Fact]
    public void HasPermission_Uses_Cached_Role_Permissions()
    {
        var loadCount = 0;
        using var connection = new MockDbConnection()
            .InterceptListRows(args =>
            {
                loadCount++;
                return new OptionalValue<System.Collections.IList>(new List<MockRolePermissionRow>
                {
                    new() { RoleKeyOrName = "admin", PermissionKey = "perm1" }
                });
            });

        var service = new TestRolePermissionService(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            new MockTypeSource());

        Assert.True(service.HasPermission("admin", "perm1"));
        Assert.False(service.HasPermission("admin", "perm2"));
        Assert.True(service.HasPermission("admin", "perm1"));
        Assert.Equal(1, loadCount);
    }
}


