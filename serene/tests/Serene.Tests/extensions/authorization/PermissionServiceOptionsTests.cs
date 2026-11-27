using Microsoft.Extensions.Configuration;

namespace Serenity.Extensions;

public class PermissionServiceOptionsTests
{
    private class OptionsPermissionService(IUserAccessor userAccessor, PermissionServiceOptions? options)
        : BasePermissionService(userAccessor, new MockRolePermissions(), null,
            options == null ? null : Microsoft.Extensions.Options.Options.Create(options))
    {
        protected override IEnumerable<string> GetUserRoles(ClaimsPrincipal user) => [];
        protected override bool? UserHasPermission(ClaimsPrincipal user, string permission) => null;
    }

    [Fact]
    public void IsSuperAdmin_Is_The_Admin_User_By_Default_Ignoring_Case()
    {
        var options = new PermissionServiceOptions();
        Assert.True(options.IsSuperAdmin("admin"));
        Assert.True(options.IsSuperAdmin("ADMIN"));
        Assert.False(options.IsSuperAdmin("administrator"));
        Assert.False(options.IsSuperAdmin(""));
        Assert.False(options.IsSuperAdmin(null));
    }

    [Fact]
    public void IsSuperAdmin_Uses_The_SuperAdmins_Instead_Of_The_Default()
    {
        var options = new PermissionServiceOptions { SuperAdmins = ["root", " ops "] };
        Assert.True(options.IsSuperAdmin("root"));
        Assert.True(options.IsSuperAdmin("Ops"));
        Assert.False(options.IsSuperAdmin("admin"));

        Assert.False(new PermissionServiceOptions { SuperAdmins = [] }.IsSuperAdmin("admin"));
    }

    [Fact]
    public void SuperAdmins_From_The_Settings_Replace_The_Default()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["PermissionService:SuperAdmins:0"] = "root" })
            .Build();
        var options = new PermissionServiceOptions();
        configuration.GetSection(PermissionServiceOptions.SectionKey).Bind(options);

        Assert.Equal(["root"], options.SuperAdmins);
        Assert.False(options.IsSuperAdmin("admin"));
    }

    [Fact]
    public void A_Super_Admin_Of_The_Options_Has_Every_Permission_And_Can_Impersonate()
    {
        var admin = new OptionsPermissionService(new MockUserAccessor(() => "Admin"), new PermissionServiceOptions());
        Assert.True(admin.HasPermission("Some:Permission"));
        Assert.True(admin.HasPermission("ImpersonateAs"));

        var user = new OptionsPermissionService(new MockUserAccessor(() => "nancy"), new PermissionServiceOptions());
        Assert.False(user.HasPermission("Some:Permission"));
        Assert.False(user.HasPermission("ImpersonateAs"));
    }

    [Fact]
    public void Without_The_Options_There_Is_No_Super_Admin()
    {
        var service = new OptionsPermissionService(new MockUserAccessor(() => "admin"), options: null);
        Assert.False(service.HasPermission("Some:Permission"));
        Assert.False(service.HasPermission("ImpersonateAs"));
    }
}
