namespace Serenity.Extensions;

public class BasePermissionServiceTests
{
    private static ClaimsPrincipal LoggedIn(string name = "user")
    {
        return new ClaimsPrincipal(new GenericIdentity(name, "Test"));
    }

    private class TestPermissionService(IUserAccessor userAccessor, IRolePermissionService rolePermissions,
        IHttpContextItemsAccessor? itemsAccessor = null) : BasePermissionService(userAccessor, rolePermissions, itemsAccessor)
    {
        public Func<ClaimsPrincipal, IEnumerable<string>> RolesFunc { get; set; } = _ => [];
        public Func<ClaimsPrincipal, string, bool?>? DirectPermissionFunc { get; set; }
        public Func<ClaimsPrincipal, bool> SuperAdminFunc { get; set; } = _ => false;
        public Func<ClaimsPrincipal, string, bool> SuperAdminPermissionFunc { get; set; } = (_, _) => true;
        public Func<string, bool> AnonymousPermissionFunc { get; set; } = _ => false;
        public Func<ClaimsPrincipal, string, bool>? ImpersonationFunc { get; set; }

        protected override IEnumerable<string> GetUserRoles(ClaimsPrincipal user) => RolesFunc(user);
        protected override bool? UserHasPermission(ClaimsPrincipal user, string permission) =>
            DirectPermissionFunc?.Invoke(user, permission);
        protected override bool IsSuperAdmin(ClaimsPrincipal user) => SuperAdminFunc(user);
        protected override bool SuperAdminHasPermission(ClaimsPrincipal user, string permission) =>
            SuperAdminPermissionFunc(user, permission);
        protected override bool AnonymousUsersHavePermission(string permission) =>
            AnonymousPermissionFunc(permission);
        protected override bool HasImpersonationPermission(ClaimsPrincipal user, string permission) =>
            ImpersonationFunc?.Invoke(user, permission) ??
                base.HasImpersonationPermission(user, permission);
    }

    private class DefaultPermissionService(IUserAccessor userAccessor, IRolePermissionService rolePermissions)
        : BasePermissionService(userAccessor, rolePermissions)
    {
        protected override IEnumerable<string> GetUserRoles(ClaimsPrincipal user) => [];
        protected override bool? UserHasPermission(ClaimsPrincipal user, string permission) => null;
        protected override bool IsSuperAdmin(ClaimsPrincipal user) => true;
    }

    [Fact]
    public void AnonymousUsersHavePermission_Defaults_To_False()
    {
        var service = new DefaultPermissionService(new MockUserAccessor(() => (string)null),
            new MockRolePermissions());
        Assert.False(service.HasPermission("someperm"));
    }

    [Fact]
    public void SuperAdminHasPermission_Defaults_To_True()
    {
        var service = new DefaultPermissionService(new MockUserAccessor(() => "user"),
            new MockRolePermissions());
        Assert.True(service.HasPermission("someperm"));
    }

    [Fact]
    public void Empty_Identity_Name_Uses_Default_AnonymousPermission()
    {
        var service = new DefaultPermissionService(
            new MockUserAccessor(() => new ClaimsPrincipal(new GenericIdentity("", "Test"))),
            new MockRolePermissions());
        Assert.False(service.HasPermission("someperm"));

        var service2 = new DefaultPermissionService(
            new MockUserAccessor(() => "", () => "1"),
            new MockRolePermissions());
        Assert.False(service2.HasPermission("someperm"));
    }

    private static TestPermissionService CreateLoggedInService(
        Func<ClaimsPrincipal, IEnumerable<string>>? roles = null,
        Func<string, bool>? rolePermission = null)
    {
        var service = new TestPermissionService(
            new MockUserAccessor(() => "user"),
            new MockRolePermissions(perm => rolePermission?.Invoke(perm) ?? false));
        if (roles != null)
            service.RolesFunc = roles;
        return service;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var rolePermissions = new MockRolePermissions();
        Assert.Throws<ArgumentNullException>(() =>
            new TestPermissionService(null!, rolePermissions));
        Assert.Throws<ArgumentNullException>(() =>
            new TestPermissionService(new MockUserAccessor(() => "user"), null!));
    }

    [Fact]
    public void HasPermission_Returns_False_For_Empty_Or_Null_Key()
    {
        var service = CreateLoggedInService();
        Assert.False(service.HasPermission(""));
        Assert.False(service.HasPermission(null!));
    }

    [Fact]
    public void HasPermission_Returns_False_For_Deny()
    {
        var service = CreateLoggedInService();
        Assert.False(service.HasPermission(SpecialPermissionKeys.Deny));
    }

    [Fact]
    public void HasPermission_Returns_True_For_Asterisk()
    {
        var service = CreateLoggedInService();
        Assert.True(service.HasPermission(SpecialPermissionKeys.Public));
    }

    [Fact]
    public void HasPermission_Returns_True_For_Transiently_Granted()
    {
        var service = CreateLoggedInService();
        Assert.False(service.HasPermission("someperm"));

        service.Grant("someperm");
        Assert.True(service.HasPermission("someperm"));
        Assert.Contains("someperm", service.GetGranted());
        Assert.False(service.IsAllGranted());

        service.UndoGrant();
        Assert.False(service.HasPermission("someperm"));

        service.GrantAll();
        Assert.True(service.IsAllGranted());
        Assert.True(service.HasPermission("anything"));
        Assert.Empty(service.GetGranted());
        service.UndoGrant();
        Assert.False(service.IsAllGranted());
    }

    [Fact]
    public void HasPermission_QuestionMark_Checks_LoggedIn()
    {
        var loggedIn = CreateLoggedInService();
        Assert.True(loggedIn.HasPermission(SpecialPermissionKeys.LoggedIn));

        var anonymous = new TestPermissionService(
            new MockUserAccessor(() => (string)null),
            new MockRolePermissions());
        Assert.False(anonymous.HasPermission(SpecialPermissionKeys.LoggedIn));
    }

    [Fact]
    public void HasPermission_NotLoggedIn_Uses_AnonymousPermission()
    {
        var service = new TestPermissionService(
            new MockUserAccessor(() => (string)null),
            new MockRolePermissions())
        {
            AnonymousPermissionFunc = perm => perm == "anon"
        };

        Assert.True(service.HasPermission("anon"));
        Assert.False(service.HasPermission("other"));
    }

    [Fact]
    public void HasPermission_Authenticated_Without_Name_Uses_AnonymousPermission()
    {
        var service = new TestPermissionService(
            new MockUserAccessor(() => new ClaimsPrincipal(new GenericIdentity("", "Test"))),
            new MockRolePermissions())
        {
            AnonymousPermissionFunc = perm => perm == "anon"
        };

        Assert.True(service.HasPermission("anon"));
        Assert.False(service.HasPermission("other"));
    }

    [Fact]
    public void HasPermission_Impersonation_Uses_ImpersonationPermission()
    {
        var service = CreateLoggedInService();
        Assert.False(service.HasPermission("ImpersonateAsOther"));

        service.SuperAdminFunc = _ => true;
        Assert.True(service.HasPermission("ImpersonateAsOther"));

        service.SuperAdminFunc = _ => false;
        service.ImpersonationFunc = (_, perm) => perm == "ImpersonateAsOther";
        Assert.True(service.HasPermission("ImpersonateAsOther"));
    }

    [Fact]
    public void HasPermission_SuperAdmin_Uses_SuperAdminPermission()
    {
        var service = CreateLoggedInService();
        service.SuperAdminFunc = _ => true;
        Assert.True(service.HasPermission("anyperm"));

        service.SuperAdminFunc = _ => false;
        service.DirectPermissionFunc = (_, perm) => perm == "anyperm";
        Assert.True(service.HasPermission("anyperm"));
    }

    [Fact]
    public void HasPermission_DirectPermission_Takes_Precedence_Over_Roles()
    {
        var service = CreateLoggedInService(
            roles: _ => ["role1"],
            rolePermission: perm => perm == "deniedperm");
        service.DirectPermissionFunc = (_, perm) => perm == "deniedperm" ? false : null;

        Assert.False(service.HasPermission("deniedperm"));
    }

    [Fact]
    public void HasPermission_Grants_From_Roles()
    {
        var service = CreateLoggedInService(
            roles: _ => ["role1", "role2"],
            rolePermission: perm => perm == "roleperm");
        service.DirectPermissionFunc = (_, _) => null;

        Assert.True(service.HasPermission("roleperm"));
        Assert.False(service.HasPermission("otherperm"));
    }

    [NestedPermissionKeys]
    private static class TestImplicitPermissions
    {
        [ImplicitPermission(Child)]
        [ImplicitPermission(Nested.Deep)]
        public const string Parent = "Test:Parent";

        public const string Child = "Test:Child";

        public static int Number = 0;

        public static string? NullField = null;

        public static class Nested
        {
            public const string Deep = "Test:Deep";
        }
    }

    [Fact]
    public void GetImplicitPermissions_Throws_For_Null_Arguments()
    {
        var cache = new TestTwoLevelCache();
        Assert.Throws<ArgumentNullException>(() =>
            BasePermissionService.GetImplicitPermissions(null!, new MockTypeSource()));
        Assert.Throws<ArgumentNullException>(() =>
            BasePermissionService.GetImplicitPermissions(cache.Memory, null!));
    }

    [Fact]
    public void GetImplicitPermissions_Collects_Implicit_Permissions_And_Caches()
    {
        var cache = new TestTwoLevelCache();
        var typeSource = new MockTypeSource(typeof(TestImplicitPermissions));

        var permissions = BasePermissionService.GetImplicitPermissions(cache.Memory, typeSource);
        Assert.True(permissions.ContainsKey("Test:Parent"));
        Assert.Contains("Test:Child", permissions["Test:Parent"]);

        var cached = BasePermissionService.GetImplicitPermissions(cache.Memory, typeSource);
        Assert.Same(permissions, cached);
    }

    private TestPermissionService CreateDirect(params string[] granted)
    {
        return new TestPermissionService(new MockUserAccessor(() => "user"), new MockRolePermissions())
        {
            DirectPermissionFunc = (_, permission) => granted.Contains(permission) ? true : null
        };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void HasPermission_Returns_False_For_Null_Or_Empty_Permission(string permission)
    {
        var service = CreateDirect("A");
        service.SuperAdminFunc = _ => true;
        Assert.False(service.HasPermission(permission));
    }

    [Fact]
    public void HasPermission_Evaluates_Logical_Expressions_Per_Key()
    {
        var service = CreateDirect("A");
        Assert.True(service.HasPermission("A|B"));
        Assert.False(service.HasPermission("A&B"));
        Assert.True(service.HasPermission("A&!B"));
        Assert.False(service.HasPermission("!A"));
        Assert.True(service.HasPermission("(B|A)&!C"));
    }

    [Fact]
    public void HasPermission_Applies_Deny_And_Special_Keys_Inside_Expressions()
    {
        var service = CreateDirect("A");
        Assert.False(service.HasPermission("A&DENY"));
        Assert.True(service.HasPermission("B|*"));
        Assert.True(service.HasPermission("B|?"));
    }

    private class KeyRecordingPermissionService(IUserAccessor userAccessor, IRolePermissionService rolePermissions)
        : BasePermissionService(userAccessor, rolePermissions)
    {
        public List<string> Checked { get; } = [];
        protected override IEnumerable<string> GetUserRoles(ClaimsPrincipal user) => [];
        protected override bool? UserHasPermission(ClaimsPrincipal user, string permission) => permission == "A";

        protected override bool HasPermissionKey(string permission)
        {
            Checked.Add(permission);
            return base.HasPermissionKey(permission);
        }
    }

    [Fact]
    public void HasPermissionKey_Override_Only_Sees_Single_Keys()
    {
        var service = new KeyRecordingPermissionService(new MockUserAccessor(() => "user"), new MockRolePermissions());
        Assert.True(service.HasPermission("B|A"));
        Assert.Equal(["B", "A"], service.Checked);
    }

    [Fact]
    public void Transient_Grant_Of_A_Key_Satisfies_An_Expression_With_It()
    {
        var service = CreateDirect();
        Assert.False(service.HasPermission("A|B"));
        service.Grant("B");
        try
        {
            Assert.True(service.HasPermission("A|B"));
            Assert.False(service.HasPermission("A&B"));
        }
        finally
        {
            service.UndoGrant();
        }
    }
}
