using Serenity.Web;

namespace Serenity.Extensions;

public class BasePermissionKeyListerTests
{
    [ReadPermission("Test:TypePerm")]
    [PageAuthorize("Test:TypePage")]
    [ServiceAuthorize("Test:TypeService")]
    private class TypeWithPermissions
    {
        [ReadPermission("Role:RolePerm")]
        public int? RoleProperty { get; set; }

        [InsertPermission("Test:MethodPerm")]
        public void SomeMethod()
        {
        }

        [PageAuthorize("Test:PagePerm")]
        public void PageMethod()
        {
        }

        [ServiceAuthorize("Test:ServicePerm")]
        public void ServiceMethod()
        {
        }

        [UpdatePermission("Test:PropertyPerm")]
        public int? SomeProperty { get; set; }

        [ReadPermission("*")]
        public int? MarkerProperty { get; set; }
    }

    [NestedPermissionKeys]
    private static class TestNestedPermissions
    {
        public const string Parent = "Nested:Parent";
        public const string WithOperators = "Nested:A|Nested:B";
    }

    private sealed class ThrowingAttribute : Attribute
    {
        public ThrowingAttribute()
        {
            throw new InvalidOperationException("boom");
        }
    }

    [Throwing]
    private static class TypeWithThrowingAttribute
    {
        [Throwing]
        public static int? Value { get; set; }
    }

    private class TestPermissionKeyLister : BasePermissionKeyLister
    {
        public TestPermissionKeyLister(ITwoLevelCache cache, ITypeSource typeSource)
            : base(cache, typeSource)
        {
        }

        public Func<IEnumerable<string>> ExternalFunc { get; set; } = () => [];
        public Func<IEnumerable<string>> RoleKeysFunc { get; set; } = () => [];

        protected override string GetCacheGroupKey() => "TestGroup";
        protected override IEnumerable<string> GetExternalPermissions() => ExternalFunc();
        protected override IEnumerable<string> GetRoleKeys() => RoleKeysFunc();

        public IEnumerable<string> BaseExternalPermissions() => base.GetExternalPermissions();
        public IEnumerable<string> BaseRoleKeys() => base.GetRoleKeys();
        public IEnumerable<string> Nested() => GetNestedPermissions(typeSource);
        public IEnumerable<string> Assembly() => GetAssemblyPermissions(typeSource);
        public IEnumerable<string> FromType(Type t) => GetPermissionsFromType(t);
        public IEnumerable<string> FromTypeAttributes(Type t) => GetPermissionsFromTypeAttributes(t);
        public IEnumerable<string> FromMethods(Type t) => GetPermissionsFromMethods(t);
        public IEnumerable<string> FromMethod(MethodInfo m) => GetPermissionsFromMethod(m);
        public IEnumerable<string> FromProperties(Type t) => GetPermissionsFromProperties(t);
        public IEnumerable<string> FromProperty(PropertyInfo p) => GetPermissionsFromProperty(p);
        public IEnumerable<string> Markers() => GetMarkerPermissions();
        public IEnumerable<string> Privates() => GetPrivatePermissions();
        public IEnumerable<string> Roles() => GetRoleKeys();
        public bool IsRole(string k) => IsRolePermission(k);
        public IEnumerable<string> Split(string p) => SplitPermissions(p);
        public string CacheKey(bool includeRoles) => GetCacheKey(includeRoles);
        public TimeSpan CacheDuration() => GetCacheDuration();
        public IEnumerable<string> Cached(bool includeRoles) => GetCachedPermissionKeys(includeRoles);
        public IEnumerable<string> MemberAttrs<TAttr>(MemberInfo m, Func<TAttr, string?> f) where TAttr : Attribute =>
            GetAttributePermissions(m, f);
        public IEnumerable<string> TypeAttrs<TAttr>(Type t, Func<TAttr, string?> f) where TAttr : Attribute =>
            GetAttributePermissions(t, f);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TestPermissionKeyLister(null!, new MockTypeSource()));
        Assert.Throws<ArgumentNullException>(() =>
            new TestPermissionKeyLister(new TestTwoLevelCache(), null!));
    }

    [Fact]
    public void CacheKey_Depends_On_IncludeRoles()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Equal("Administration:PermissionKeys:XR", lister.CacheKey(false));
        Assert.Equal("Administration:PermissionKeys:IR", lister.CacheKey(true));
    }

    [Fact]
    public void CacheDuration_Defaults_To_Zero()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Equal(TimeSpan.Zero, lister.CacheDuration());
    }

    [Fact]
    public void Default_Collections_Are_Empty()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Empty(lister.BaseExternalPermissions());
        Assert.Empty(lister.BaseRoleKeys());
    }

    [Fact]
    public void Marker_And_Private_Permissions_Are_Expected()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Equal(["*", "?", "DENY"], lister.Markers());
        Assert.Equal(["ImpersonateAs"], lister.Privates());
    }

    [Fact]
    public void IsRolePermission_Checks_Prefix()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.True(lister.IsRole("Role:admin"));
        Assert.True(lister.IsRole("role:admin"));
        Assert.False(lister.IsRole("admin"));
        Assert.False(lister.IsRole(""));
    }

    [Fact]
    public void SplitPermissions_Splits_On_Separators()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Equal(["A", "B", "C"], lister.Split("A|B&C"));
        Assert.Empty(lister.Split(""));
        Assert.Empty(lister.Split(null!));
    }

    [Fact]
    public void GetAssemblyPermissions_Reads_Assembly_Attributes()
    {
        var typeSource = new MockTypeSource([], [new ReadPermissionAttribute("Test:AssemblyPerm")]);
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource);
        Assert.Contains("Test:AssemblyPerm", lister.Assembly());
    }

    [Fact]
    public void GetNestedPermissions_Reads_Nested_Keys()
    {
        var typeSource = new MockTypeSource(typeof(TestNestedPermissions));
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource);
        var nested = lister.Nested();
        Assert.Contains("Nested:Parent", nested);
        Assert.DoesNotContain("Nested:A", nested);
    }

    [Fact]
    public void GetPermissionsFromType_Reads_Type_Method_And_Property_Attributes()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        var permissions = lister.FromType(typeof(TypeWithPermissions)).ToList();

        Assert.Contains("Test:TypePerm", permissions);
        Assert.Contains("Test:MethodPerm", permissions);
        Assert.Contains("Test:PagePerm", permissions);
        Assert.Contains("Test:ServicePerm", permissions);
        Assert.Contains("Test:PropertyPerm", permissions);
    }

    [Fact]
    public void GetPermissionsFromTypeAttributes_Reads_Type_Attributes()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        Assert.Contains("Test:TypePerm", lister.FromTypeAttributes(typeof(TypeWithPermissions)));
    }

    [Fact]
    public void GetPermissionsFromMethod_And_Property_Read_Attributes()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        var method = typeof(TypeWithPermissions).GetMethod(nameof(TypeWithPermissions.SomeMethod))!;
        Assert.Contains("Test:MethodPerm", lister.FromMethod(method));

        var property = typeof(TypeWithPermissions).GetProperty(nameof(TypeWithPermissions.SomeProperty))!;
        Assert.Contains("Test:PropertyPerm", lister.FromProperty(property));
    }

    [Fact]
    public void AttributePermissions_Return_Empty_When_Attributes_Throw()
    {
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), new MockTypeSource());
        var member = typeof(TypeWithThrowingAttribute).GetProperty(nameof(TypeWithThrowingAttribute.Value))!;
        Assert.Empty(lister.MemberAttrs<ThrowingAttribute>(member, _ => "x"));
        Assert.Empty(lister.TypeAttrs<ThrowingAttribute>(typeof(TypeWithThrowingAttribute), _ => "x"));
    }

    [Fact]
    public void ListPermissionKeys_Includes_And_Excludes_Expected_Keys()
    {
        var typeSource = new MockTypeSource(typeof(TypeWithPermissions));
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource);

        var keys = lister.ListPermissionKeys(false).ToList();
        Assert.Contains("Test:TypePerm", keys);
        Assert.Contains("Test:MethodPerm", keys);
        Assert.Contains("Test:PropertyPerm", keys);
        Assert.DoesNotContain("*", keys);
        Assert.DoesNotContain("ImpersonateAs", keys);
    }

    [Fact]
    public void ListPermissionKeys_With_Roles_Adds_Role_Prefixed_Keys()
    {
        var typeSource = new MockTypeSource(typeof(TypeWithPermissions));
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource)
        {
            RoleKeysFunc = () => ["admin", "", " "]
        };

        Assert.Contains("Role:admin", lister.ListPermissionKeys(true));
        Assert.DoesNotContain("Role:admin", lister.ListPermissionKeys(false));
    }

    [Fact]
    public void ListPermissionKeys_Filters_Role_Keys_When_Not_Included()
    {
        var typeSource = new MockTypeSource(typeof(TypeWithPermissions));
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource)
        {
            RoleKeysFunc = () => ["admin"]
        };

        Assert.DoesNotContain("Role:admin", lister.ListPermissionKeys(false));
    }

    [Fact]
    public void CachedPermissionKeys_Are_Cached()
    {
        var cache = new TestTwoLevelCache();
        var typeSource = new MockTypeSource(typeof(TypeWithPermissions));
        var lister = new TestPermissionKeyLister(cache, typeSource);

        var first = lister.Cached(false).ToList();
        var second = lister.Cached(false).ToList();
        Assert.Equal(first, second);
    }

    [Fact]
    public void ExternalPermissions_Are_Included()
    {
        var typeSource = new MockTypeSource();
        var lister = new TestPermissionKeyLister(new TestTwoLevelCache(), typeSource)
        {
            ExternalFunc = () => ["External:Perm"]
        };

        Assert.Contains("External:Perm", lister.ListPermissionKeys(false));
    }
}
