using Serenity.ComponentModel;

namespace Serenity.Extensions;

public class BasePermissionServiceTMoreTests
{
    private static ClaimsPrincipal CreateUser(string identifier = "12345")
    {
        var identity = new GenericIdentity("user", "Test");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier));
        return new ClaimsPrincipal(identity);
    }

    private class TestService(
        ITwoLevelCache cache,
        ISqlConnections sqlConnections,
        ITypeSource typeSource,
        IUserAccessor userAccessor,
        IRolePermissionService rolePermissions)
        : BasePermissionService<MockUserPermissionRow, MockUserRoleRow>(
            cache, sqlConnections, typeSource, userAccessor, rolePermissions)
    {
        public IDictionary<string, bool>? GetUserPermissionsExposed(ClaimsPrincipal user) =>
            GetUserPermissions(user);
        public IEnumerable<string> GetUserRolesExposed(ClaimsPrincipal user) => GetUserRoles(user);
        public IDictionary<string, bool> LoadUserPermissionsExposed(ClaimsPrincipal user) =>
            LoadUserPermissions(user);
        public IEnumerable<string> LoadUserRolesExposed(ClaimsPrincipal user) =>
            LoadUserRoles(user);
        public TimeSpan UserPermissionsCacheDuration() => GetUserPermissionsCacheDuration();
        public TimeSpan UserRolesCacheDuration() => GetUserRolesCacheDuration();
        public string UserPermissionsCacheGroupKey() => GetUserPermissionsCacheGroupKey();
        public string UserRolesCacheGroupKey() => GetUserRolesCacheGroupKey();
    }

    [NestedPermissionKeys]
    private static class ImplicitPermissions
    {
        [ImplicitPermission(Expanded)]
        public const string Base = "perm1";

        public const string Expanded = "implied1";
    }

    [Fact]
    public void LoadUserPermissions_Maps_Grants_And_Implicit_Permissions()
    {
        var user = CreateUser();
        using var connection = new MockDbConnection()
            .InterceptListRows(args =>
            {
                args.EditQuery(new SqlQuery());
                if (args.Type == typeof(MockUserPermissionRow))
                {
                    return new OptionalValue<System.Collections.IList>(new List<MockUserPermissionRow>
                    {
                        new() { UserId = 12345, PermissionKey = "perm1", Granted = true },
                        new() { UserId = 12345, PermissionKey = "perm2", Granted = false }
                    });
                }

                return new OptionalValue<System.Collections.IList>(new List<MockUserRoleRow>());
            });

        var sqlConnections = new MockSqlConnections { OnNewByKey = _ => connection };
        var service = new TestService(new TestTwoLevelCache(), sqlConnections,
            new MockTypeSource(typeof(ImplicitPermissions)), new MockUserAccessor(() => "user"),
            new MockRolePermissions());

        var permissions = service.LoadUserPermissionsExposed(user);
        Assert.True(permissions["perm1"]);
        Assert.False(permissions["perm2"]);
        Assert.True(permissions["implied1"]);
        Assert.Equal(TimeSpan.Zero, service.UserPermissionsCacheDuration());
        Assert.Equal(MockUserPermissionRow.Fields.GenerationKey, service.UserPermissionsCacheGroupKey());
    }

    [Fact]
    public void LoadUserRoles_Maps_Roles()
    {
        var user = CreateUser();
        using var connection = new MockDbConnection()
            .InterceptListRows(args =>
            {
                args.EditQuery(new SqlQuery());
                if (args.Type == typeof(MockUserRoleRow))
                {
                    return new OptionalValue<System.Collections.IList>(new List<MockUserRoleRow>
                    {
                        new() { UserId = 12345, RoleName = "admin" },
                        new() { UserId = 12345, RoleName = "guest" }
                    });
                }

                return new OptionalValue<System.Collections.IList>(new List<MockUserPermissionRow>());
            });

        var sqlConnections = new MockSqlConnections { OnNewByKey = _ => connection };
        var service = new TestService(new TestTwoLevelCache(), sqlConnections,
            new MockTypeSource(), new MockUserAccessor(() => "user"),
            new MockRolePermissions());

        var roles = service.LoadUserRolesExposed(user).ToList();
        Assert.Equal(["admin", "guest"], roles);
        Assert.Equal(TimeSpan.Zero, service.UserRolesCacheDuration());
        Assert.Equal(MockUserRoleRow.Fields.GenerationKey, service.UserRolesCacheGroupKey());
    }

    [Fact]
    public void GetUserPermissions_And_Roles_Return_Null_For_Null_User()
    {
        var service = new TestService(new TestTwoLevelCache(), new NullSqlConnections(),
            new MockTypeSource(), new MockUserAccessor(() => "user"), new MockRolePermissions());

        Assert.Null(service.GetUserPermissionsExposed(null!));
        Assert.Empty(service.GetUserRolesExposed(null!));
    }

    [Fact]
    public void GetUserPermissions_Uses_Built_In_Cache()
    {
        var user = CreateUser();
        var loadCount = 0;
        using var connection = new MockDbConnection()
            .InterceptListRows(args =>
            {
                args.EditQuery(new SqlQuery());
                if (args.Type == typeof(MockUserPermissionRow))
                {
                    loadCount++;
                    return new OptionalValue<System.Collections.IList>(new List<MockUserPermissionRow>
                    {
                        new() { UserId = 12345, PermissionKey = "perm1", Granted = true }
                    });
                }

                return new OptionalValue<System.Collections.IList>(new List<MockUserRoleRow>());
            });

        var service = new TestService(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            new MockTypeSource(), new MockUserAccessor(() => "user"), new MockRolePermissions());

        var first = service.GetUserPermissionsExposed(user);
        var second = service.GetUserPermissionsExposed(user);
        Assert.Same(first, second);
        Assert.Equal(1, loadCount);
    }

    [Fact]
    public void UserHasPermission_Uses_Permissions_And_Roles()
    {
        var user = CreateUser();
        using var connection = new MockDbConnection()
            .InterceptListRows(args =>
            {
                args.EditQuery(new SqlQuery());
                if (args.Type == typeof(MockUserPermissionRow))
                {
                    return new OptionalValue<System.Collections.IList>(new List<MockUserPermissionRow>
                    {
                        new() { UserId = 12345, PermissionKey = "granted", Granted = true },
                        new() { UserId = 12345, PermissionKey = "denied", Granted = false }
                    });
                }

                return new OptionalValue<System.Collections.IList>(new List<MockUserRoleRow>
                {
                    new() { UserId = 12345, RoleName = "role1" }
                });
            });

        var service = new TestService(new TestTwoLevelCache(),
            new MockSqlConnections { OnNewByKey = _ => connection },
            new MockTypeSource(),
            new MockUserAccessor(() => user),
            new MockRolePermissions(perm => perm == "roleperm"));

        Assert.True(service.HasPermission("granted"));
        Assert.False(service.HasPermission("denied"));
        Assert.True(service.HasPermission("roleperm"));
        Assert.False(service.HasPermission("unknown"));
    }
}


