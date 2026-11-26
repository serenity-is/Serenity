using Microsoft.Extensions.Options;
using Serenity.Localization;
using Serenity.PropertyGrid;

namespace Serenity.Web;

public class LocalTextPermissionFilterTests
{
    [ReadPermission("SecretRead")]
    private class SecretRow : Row<SecretRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "Test.Secret";
            }
        }
    }

    private class NoPermissionRow : Row<NoPermissionRow.RowFields>
    {
        public class RowFields : RowFieldsBase
        {
            public RowFields()
            {
                LocalTextPrefix = "Test.NoPermission";
            }
        }
    }

    // a public form based on the secret row, like a login form based on the user row
    [FormScript("Test.PublicForm", Permission = "*")]
    [BasedOnRow(typeof(SecretRow))]
    private class PublicForm
    {
    }

    // a form only users with the row permission can access
    [FormScript("Test.SecretForm")]
    [BasedOnRow(typeof(SecretRow))]
    private class SecretForm
    {
    }

    [NestedLocalTexts(Prefix = "Db.Test.Secret.", Permission = "?")]
    private static class ProfileTexts
    {
        public static readonly LocalText Email = "Email";
    }

    [NestedLocalTexts(Prefix = "Site.Module.", Permission = "ModuleAccess")]
    private static class ModuleTexts
    {
        public static readonly LocalText Title = "Module Title";
    }

    // used by the public form, but has an explicit permission
    [EnumKey("Test.PublicStatus"), RequiredPermission("EnumAccess")]
    private enum ExplicitPublicStatus
    {
        Active
    }

    [EnumKey("Test.ReadEnum"), ReadPermission("EnumRead")]
    private enum ReadPermissionEnum
    {
        Value
    }

    // restricts a key the public form uses, which the form still gets
    [NestedLocalTexts(Prefix = "Db.Test.Secret.", Permission = "Restricted")]
    private static class RestrictsFormKeyTexts
    {
        public static readonly LocalText Name = "Name";
    }

    private static readonly Type[] AllTypes = [typeof(SecretRow), typeof(NoPermissionRow), typeof(PublicForm),
        typeof(SecretForm), typeof(ProfileTexts), typeof(ModuleTexts)];

    private static LocalTextPermissionFilter CreateFilter(HashSet<string> granted, Action<LocalTextPermissionOptions>? configure = null,
        params Type[] types)
    {
        var options = new LocalTextPermissionOptions { Enabled = true };
        configure?.Invoke(options);

        var propertyProvider = new MockPropertyItemProvider
        {
            Factory = (type, _) => type == typeof(PublicForm) ?
                [new PropertyItem { Name = "Name", Title = "Db.Test.Secret.Name" },
                 new PropertyItem { Name = "Status", Title = "Db.Test.Secret.Status", EditorParams = new() { ["enumKey"] = "Test.PublicStatus" } }] :
                type == typeof(SecretForm) ?
                [new PropertyItem { Name = "Level", Title = "Db.Test.Secret.Level", EditorParams = new() { ["enumKey"] = "Test.SecretStatus" } }] :
                []
        };

        var services = new ServiceCollection();
        services.AddSingleton<IPropertyItemProvider>(propertyProvider);
        services.Configure<PropertyItemsScriptOptions>(x => x.PublicByDefault = false);
        var serviceProvider = services.BuildServiceProvider();

        return new LocalTextPermissionFilter(Options.Create(options),
            new MockTypeSource(types.Length > 0 ? types : AllTypes),
            new MockPermissions(x => x == "*" || granted.Contains(x)), serviceProvider);
    }

    [Fact]
    public void Does_Not_Filter_When_Not_Enabled()
    {
        var filter = CreateFilter([], x => x.Enabled = false);
        Assert.Null(filter.GetKeyFilter());
        Assert.Equal("", filter.GetCacheSuffix());
    }

    [Fact]
    public void Row_Texts_Require_Row_ReadPermission()
    {
        Assert.False(CreateFilter([]).GetKeyFilter()!("Db.Test.Secret.Other"));
        Assert.True(CreateFilter(["SecretRead"]).GetKeyFilter()!("Db.Test.Secret.Other"));
    }

    [Fact]
    public void Row_Without_ReadPermission_Is_Denied()
    {
        Assert.False(CreateFilter(["SecretRead"]).GetKeyFilter()!("Db.Test.NoPermission.Name"));
    }

    [Fact]
    public void Texts_That_Belong_To_Nothing_Protected_Are_Allowed()
    {
        var isAllowed = CreateFilter([]).GetKeyFilter()!;
        Assert.True(isAllowed("Site.Layout.Title"));
        Assert.True(isAllowed("Controls.Something"));
        Assert.True(isAllowed("Validation.Required"));
        Assert.True(isAllowed("Db.Shared.RecordId"));
        Assert.True(isAllowed("Forms.SiteTitle"));
        Assert.True(isAllowed("Enums.Unreferenced.Value"));
    }

    [Fact]
    public void Texts_Used_By_Accessible_Forms_Are_Allowed_Without_Row_Permission()
    {
        var isAllowed = CreateFilter([]).GetKeyFilter()!;
        Assert.True(isAllowed("Db.Test.Secret.Name"));
        Assert.True(isAllowed("Db.Test.Secret.Status"));
        Assert.False(isAllowed("Db.Test.Secret.Level"));
    }

    [Fact]
    public void Form_Texts_Require_Form_Permission()
    {
        Assert.True(CreateFilter([]).GetKeyFilter()!("Forms.Test.PublicForm.Title"));
        Assert.False(CreateFilter([]).GetKeyFilter()!("Forms.Test.SecretForm.Title"));
        Assert.True(CreateFilter(["SecretRead"]).GetKeyFilter()!("Forms.Test.SecretForm.Title"));
    }

    [Fact]
    public void Enum_Texts_Require_Access_To_A_Script_Using_The_Enum()
    {
        var isAllowed = CreateFilter([]).GetKeyFilter()!;
        Assert.True(isAllowed("Enums.Test.PublicStatus.Active"));
        Assert.False(isAllowed("Enums.Test.SecretStatus.High"));
        Assert.True(CreateFilter(["SecretRead"]).GetKeyFilter()!("Enums.Test.SecretStatus.High"));
    }

    [Fact]
    public void Permission_Texts_Require_Configured_Permission()
    {
        Assert.False(CreateFilter([], x => x.PermissionTextsPermission = "Admin").GetKeyFilter()!("Permission.Some:Key"));
        Assert.True(CreateFilter(["Admin"], x => x.PermissionTextsPermission = "Admin").GetKeyFilter()!("Permission.Some:Key"));
    }

    [Fact]
    public void Permission_Texts_Require_LoggedIn_By_Default()
    {
        Assert.False(CreateFilter([]).GetKeyFilter()!("Permission.Some"));
        Assert.True(CreateFilter([SpecialPermissionKeys.LoggedIn]).GetKeyFilter()!("Permission.Some"));
    }

    [Fact]
    public void NestedLocalTexts_With_Permission_Grants_Its_Keys()
    {
        Assert.False(CreateFilter([]).GetKeyFilter()!("Db.Test.Secret.Email"));
        Assert.True(CreateFilter([SpecialPermissionKeys.LoggedIn]).GetKeyFilter()!("Db.Test.Secret.Email"));
        Assert.False(CreateFilter([SpecialPermissionKeys.LoggedIn]).GetKeyFilter()!("Db.Test.Secret.Other"));
    }

    [Fact]
    public void NestedLocalTexts_With_Permission_Restricts_Texts_That_Would_Be_Allowed_Otherwise()
    {
        Assert.False(CreateFilter([]).GetKeyFilter()!("Site.Module.Title"));
        Assert.True(CreateFilter(["ModuleAccess"]).GetKeyFilter()!("Site.Module.Title"));
        Assert.True(CreateFilter([]).GetKeyFilter()!("Site.Module.NotDeclared"));
    }

    [Fact]
    public void Texts_Used_By_Accessible_Forms_Win_Over_NestedLocalTexts_Restriction()
    {
        var types = AllTypes.Append(typeof(RestrictsFormKeyTexts)).ToArray();
        Assert.True(CreateFilter([], types: types).GetKeyFilter()!("Db.Test.Secret.Name"));
    }

    [Fact]
    public void Enum_With_RequiredPermission_Requires_It_Even_When_Used_By_Accessible_Form()
    {
        var types = AllTypes.Append(typeof(ExplicitPublicStatus)).ToArray();
        Assert.False(CreateFilter([], types: types).GetKeyFilter()!("Enums.Test.PublicStatus.Active"));
        Assert.True(CreateFilter(["EnumAccess"], types: types).GetKeyFilter()!("Enums.Test.PublicStatus.Active"));
    }

    [Fact]
    public void Enum_With_ReadPermission_Requires_It()
    {
        var types = AllTypes.Append(typeof(ReadPermissionEnum)).ToArray();
        Assert.False(CreateFilter([], types: types).GetKeyFilter()!("Enums.Test.ReadEnum.Value"));
        Assert.True(CreateFilter(["EnumRead"], types: types).GetKeyFilter()!("Enums.Test.ReadEnum.Value"));
    }

    [Fact]
    public void AlwaysInclude_Prefixes_Are_Not_Filtered()
    {
        var isAllowed = CreateFilter([], x => x.AlwaysInclude = ["Db.Test."]).GetKeyFilter()!;
        Assert.True(isAllowed("Db.Test.Secret.Other"));
        Assert.True(isAllowed("Db.Test.NoPermission.Name"));
    }

    [Fact]
    public void Restrict_Prefixes_Require_Their_Permission()
    {
        void restrict(LocalTextPermissionOptions x) => x.Restrict = new() { ["Site.JsonOnly"] = "JsonAccess" };
        Assert.False(CreateFilter([], restrict).GetKeyFilter()!("Site.JsonOnly.Title"));
        Assert.True(CreateFilter(["JsonAccess"], restrict).GetKeyFilter()!("Site.JsonOnly.Title"));
        Assert.True(CreateFilter([], restrict).GetKeyFilter()!("Site.Other.Title"));
    }

    [Fact]
    public void Restrict_Does_Not_Remove_Texts_Used_By_Accessible_Forms()
    {
        var isAllowed = CreateFilter([], x => x.Restrict = new() { ["Db.Test.Secret."] = "Nobody" }).GetKeyFilter()!;
        Assert.True(isAllowed("Db.Test.Secret.Name"));
        Assert.False(isAllowed("Db.Test.Secret.Other"));
    }

    [Fact]
    public void Users_With_An_Unfiltered_Permission_Get_All_Texts()
    {
        var admin = CreateFilter(["Admin"], x => x.UnfilteredPermission = "Admin");
        Assert.Null(admin.GetKeyFilter());
        Assert.Equal("", admin.GetCacheSuffix());

        var other = CreateFilter([SpecialPermissionKeys.LoggedIn], x => x.UnfilteredPermission = "Admin");
        Assert.False(other.GetKeyFilter()!("Db.Test.Secret.Other"));
    }

    [Fact]
    public void Logged_In_Users_Get_All_Texts_With_LoggedIn_Unfiltered()
    {
        void onlyAnonymous(LocalTextPermissionOptions x) => x.UnfilteredPermission = SpecialPermissionKeys.LoggedIn;

        var loggedIn = CreateFilter([SpecialPermissionKeys.LoggedIn], onlyAnonymous);
        Assert.Null(loggedIn.GetKeyFilter());
        Assert.Equal("", loggedIn.GetCacheSuffix());

        var anonymous = CreateFilter([], onlyAnonymous);
        Assert.False(anonymous.GetKeyFilter()!("Db.Test.Secret.Other"));
        Assert.NotEqual("", anonymous.GetCacheSuffix());
    }

    [Fact]
    public void Cache_Suffix_Varies_By_Permissions()
    {
        var anonymous = CreateFilter([]).GetCacheSuffix();
        Assert.NotEqual("", anonymous);
        Assert.Equal(anonymous, CreateFilter([]).GetCacheSuffix());
        Assert.NotEqual(anonymous, CreateFilter(["SecretRead"]).GetCacheSuffix());
    }

    [Fact]
    public void LocalTextScript_Only_Contains_Allowed_Texts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Db.Test.Secret.Name", "Name Caption");
        registry.Add("en", "Db.Test.Secret.Other", "Other Caption");
        registry.Add("en", "Site.Layout.Title", "Layout Title");

        var script = new LocalTextScript(registry, "Site", "", "en", false, CreateFilter([]));
        var code = script.GetScript();

        Assert.Contains("Name Caption", code, StringComparison.Ordinal);
        Assert.Contains("Layout Title", code, StringComparison.Ordinal);
        Assert.DoesNotContain("Other Caption", code, StringComparison.Ordinal);
        Assert.NotEqual("", script.CacheSuffix);
    }

    [Fact]
    public void LocalTextScript_Without_Filter_Has_Empty_Suffix_And_All_Texts()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Db.Test.Secret.Other", "Other Caption");

        var script = new LocalTextScript(registry, "Site", "", "en", false);
        Assert.Equal("", script.CacheSuffix);
        Assert.Contains("Other Caption", script.GetScript(), StringComparison.Ordinal);
    }

    [Fact]
    public void Options_Have_Default_Section_Key_And_Are_Disabled_By_Default()
    {
        Assert.Equal("LocalTextPermissions", typeof(LocalTextPermissionOptions)
            .GetCustomAttribute<DefaultSectionKeyAttribute>()?.SectionKey);
        Assert.False(new LocalTextPermissionOptions().Enabled);
    }
}
