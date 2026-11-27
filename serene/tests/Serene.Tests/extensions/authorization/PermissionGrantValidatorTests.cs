namespace Serenity.Extensions;

public class PermissionGrantValidatorTests
{
    [NestedPermissionKeys]
    private static class TestKeys
    {
        [ImplicitPermission(Hidden)]
        public const string Visible = "Test:Visible";

        public const string Hidden = "Administration:Hidden";
    }

    private sealed class GrantedPermissions(Dictionary<string, string[]> byUserId) : IUserGrantedPermissionsProvider
    {
        public IEnumerable<string> GetGrantedPermissions(string userId) => byUserId.TryGetValue(userId, out var keys) ? keys : [];
    }

    private sealed class ListedKeys(params string[] keys) : IPermissionKeyLister
    {
        public IEnumerable<string> ListPermissionKeys(bool includeRoles) => keys;
    }

    private static PermissionGrantValidator Validator(string username, Func<string, bool> has, IUserGrantedPermissionsProvider? granted = null,
        IPermissionKeyLister? keyLister = null)
    {
        var grants = new PermissionGrantOptions
        {
            HolderOnly = "Administration:*",
            SuperAdminOnly = "Administration:Security"
        };

        return new PermissionGrantValidator(new MockPermissions(has), new MockUserAccessor(() => username),
            Options.Create(grants), Options.Create(new PermissionServiceOptions()),
            new MemoryCache(new MemoryCacheOptions()), new MockTypeSource([typeof(TestKeys)]), NullTextLocalizer.Instance, granted,
            keyLister);
    }

    private static IUserDefinition User(string id, string username) =>
        new MockUserDefinition(id, username);

    [Fact]
    public void An_Admin_Grants_The_Grantable_Ones_And_The_HolderOnly_Ones_That_The_Admin_Has()
    {
        var validator = Validator("sec", x => x == "Administration:Security" || x == "Administration:Translation");

        Assert.Null(validator.GetDenial("Northwind:General"));
        Assert.Null(validator.GetDenial("Administration:Translation"));
        Assert.Equal(PermissionGrantDenial.NotHeld, validator.GetDenial("Administration:DataExplorer"));
        // having it is not enough for a super admin only one
        Assert.Equal(PermissionGrantDenial.SuperAdminOnly, validator.GetDenial("Administration:Security"));

        var error = Assert.Throws<ValidationError>(() => validator.ValidateGrants(["Northwind:General", "Administration:DataExplorer"]));
        Assert.Equal("Administration:DataExplorer", error.Arguments);
        validator.ValidateGrants(["Northwind:General", "Administration:Translation"]);
    }

    [Fact]
    public void A_Permission_Is_Checked_With_The_Ones_It_Implies()
    {
        var validator = Validator("sec", _ => false);
        Assert.Equal(PermissionGrantDenial.NotHeld, validator.GetDenial(TestKeys.Visible));
        Assert.Null(Validator("sec", x => x == TestKeys.Hidden).GetDenial(TestKeys.Visible));
    }

    [Fact]
    public void A_Super_Admin_Grants_Anything()
    {
        var validator = Validator("admin", _ => false);
        Assert.True(validator.IsSuperAdmin);
        Assert.Null(validator.GetDenial("Administration:Security"));
        validator.ValidateGrants(["Administration:Security", "Administration:DataExplorer"]);
    }

    [Fact]
    public void Only_Listed_Permissions_Can_Be_Added_Also_By_A_Super_Admin()
    {
        var keys = new ListedKeys("Northwind:General", "Administration:Security");
        foreach (var validator in new[] { Validator("sec", _ => true, keyLister: keys), Validator("admin", _ => false, keyLister: keys) })
        {
            validator.ValidateListed(["northwind:general", "Administration:Security"]);
            foreach (var key in new[] { "ImpersonateAs", "*", "Role:Administrators", "Northwind:Genral" })
            {
                var error = Assert.Throws<ValidationError>(() => validator.ValidateListed(["Northwind:General", key]));
                Assert.Equal("PermissionNotListed", error.ErrorCode);
                Assert.Equal(key, error.Arguments);
            }
        }

        // without a lister, any key
        Validator("sec", _ => false).ValidateListed(["Anything"]);
    }

    [Fact]
    public void An_Admin_Changes_Only_The_Accounts_Whose_Permissions_The_Admin_Could_Grant()
    {
        var granted = new GrantedPermissions(new()
        {
            ["2"] = ["Northwind:General"],
            ["3"] = ["Northwind:General", "Administration:Security"],
            ["4"] = ["Administration:DataExplorer"]
        });
        var validator = Validator("sec", x => x == "Administration:Security", granted);

        Assert.Null(validator.GetUserDenial(User("2", "nancy")));
        Assert.Equal(PermissionGrantDenial.SuperAdminOnly, validator.GetUserDenial(User("3", "andrew")));
        Assert.Equal(PermissionGrantDenial.NotHeld, validator.GetUserDenial(User("4", "janet")));
        Assert.Equal(PermissionGrantDenial.SuperAdminOnly, validator.GetUserDenial(User("1", "admin")));
        Assert.Throws<ValidationError>(() => validator.ValidateUser(User("1", "admin")));

        // the own account, and any account for a super admin
        Assert.Null(validator.GetUserDenial(User("9", "sec")));
        Assert.Null(Validator("admin", _ => false, granted).GetUserDenial(User("3", "andrew")));
    }
}
