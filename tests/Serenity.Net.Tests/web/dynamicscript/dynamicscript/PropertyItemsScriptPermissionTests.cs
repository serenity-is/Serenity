using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class PropertyItemsScriptPermissionTests
{
    [ReadPermission("RowRead")]
    private class PermissionRow
    {
    }

    [FormScript("Perm.BasedOnRow")]
    [BasedOnRow(typeof(PermissionRow))]
    private class BasedOnRowForm
    {
    }

    [FormScript("Perm.Own")]
    [ReadPermission("FormRead")]
    [BasedOnRow(typeof(PermissionRow))]
    private class ReadPermissionOnFormType
    {
    }

    [FormScript("Perm.Explicit", Permission = "*")]
    [BasedOnRow(typeof(PermissionRow))]
    private class ExplicitForm
    {
    }

    [FormScript("Perm.None")]
    private class NoPermissionForm
    {
    }

    [ColumnsScript("Perm.Columns")]
    [BasedOnRow(typeof(PermissionRow))]
    private class BasedOnRowColumns
    {
    }

    private class NoReadPermissionRow
    {
    }

    [FormScript("Perm.RowWithoutPermission")]
    [BasedOnRow(typeof(NoReadPermissionRow))]
    private class RowWithoutPermissionForm
    {
    }

    [Fact]
    public void Row_Without_ReadPermission_Is_Denied_Regardless_Of_FallbackPermission()
    {
        Assert.Equal(SpecialPermissionKeys.Deny, PropertyItemsScript.GetDefaultPermission(typeof(RowWithoutPermissionForm)));

        var options = new PropertyItemsScriptOptions
        {
            PublicByDefault = false,
            FallbackPermission = SpecialPermissionKeys.LoggedIn
        };
        Assert.Equal(SpecialPermissionKeys.Deny, options.GetPermissionFor(typeof(RowWithoutPermissionForm), null));
        Assert.Equal(SpecialPermissionKeys.LoggedIn, options.GetPermissionFor(typeof(NoPermissionForm), null));
    }

    [Fact]
    public void GetDefaultPermission_Uses_Row_ReadPermission_And_Ignores_One_On_Form_Type()
    {
        Assert.Equal("RowRead", PropertyItemsScript.GetDefaultPermission(typeof(ReadPermissionOnFormType)));
        Assert.Equal("RowRead", PropertyItemsScript.GetDefaultPermission(typeof(BasedOnRowForm)));
        Assert.Null(PropertyItemsScript.GetDefaultPermission(typeof(NoPermissionForm)));
    }

    [Fact]
    public void GetPermissionFor_Returns_Null_When_Public_By_Default()
    {
        var options = new PropertyItemsScriptOptions();
        Assert.Null(options.GetPermissionFor(typeof(BasedOnRowForm), null));
        Assert.Null(options.GetPermissionFor(typeof(NoPermissionForm), null));
    }

    [Fact]
    public void GetPermissionFor_Always_Uses_Explicit_Permission()
    {
        Assert.Equal("X", new PropertyItemsScriptOptions().GetPermissionFor(typeof(BasedOnRowForm), "X"));
        Assert.Equal("X", new PropertyItemsScriptOptions { PublicByDefault = false }
            .GetPermissionFor(typeof(BasedOnRowForm), "X"));
    }

    [Fact]
    public void GetPermissionFor_Uses_Row_Permission_Then_Fallback_When_Not_Public_By_Default()
    {
        var options = new PropertyItemsScriptOptions { PublicByDefault = false };
        Assert.Equal("RowRead", options.GetPermissionFor(typeof(BasedOnRowForm), null));
        Assert.Equal(SpecialPermissionKeys.Deny, options.GetPermissionFor(typeof(NoPermissionForm), null));

        options.FallbackPermission = SpecialPermissionKeys.LoggedIn;
        Assert.Equal(SpecialPermissionKeys.LoggedIn, options.GetPermissionFor(typeof(NoPermissionForm), null));

        options.FallbackPermission = null;
        Assert.Equal(SpecialPermissionKeys.Deny, options.GetPermissionFor(typeof(NoPermissionForm), null));
    }

    private static IServiceProvider CreateServices(bool publicByDefault, Func<string, bool>? hasPermission = null)
    {
        var services = new ServiceCollection();
        services.Configure<PropertyItemsScriptOptions>(x => x.PublicByDefault = publicByDefault);
        services.AddSingleton<IPermissionService>(new MockPermissions(hasPermission ?? (_ => false)));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void RegisterFormScripts_Sets_Permissions_From_Options()
    {
        var manager = new MockDynamicScriptManager();
        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(BasedOnRowForm), typeof(ReadPermissionOnFormType), typeof(ExplicitForm), typeof(NoPermissionForm)),
            new MockPropertyItemProvider(), CreateServices(publicByDefault: false)).ToDictionary(x => x.ScriptName);

        Assert.Equal("RowRead", scripts["Form.Perm.BasedOnRow"].Permission);
        Assert.Equal("RowRead", scripts["Form.Perm.Own"].Permission);
        Assert.Equal("*", scripts["Form.Perm.Explicit"].Permission);
        Assert.Equal(SpecialPermissionKeys.Deny, scripts["Form.Perm.None"].Permission);
    }

    [Fact]
    public void RegisterFormScripts_Keeps_Scripts_Public_When_Public_By_Default()
    {
        var manager = new MockDynamicScriptManager();
        var scripts = FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(BasedOnRowForm), typeof(ExplicitForm)),
            new MockPropertyItemProvider(), CreateServices(publicByDefault: true)).ToDictionary(x => x.ScriptName);

        Assert.Null(scripts["Form.Perm.BasedOnRow"].Permission);
        Assert.Equal("*", scripts["Form.Perm.Explicit"].Permission);
    }

    [Fact]
    public void RegisterColumnsScripts_Sets_Permissions_From_Options()
    {
        var manager = new MockDynamicScriptManager();
        var script = Assert.Single(ColumnsScriptRegistration.RegisterColumnsScripts(manager,
            new MockTypeSource(typeof(BasedOnRowColumns)), new MockPropertyItemProvider(),
            CreateServices(publicByDefault: false)));

        Assert.Equal("RowRead", script.Permission);
    }

    [Fact]
    public void FormBundle_Only_Contains_Permitted_Scripts()
    {
        var granted = new HashSet<string> { "*" };
        var manager = new MockDynamicScriptManager();
        FormScriptRegistration.RegisterFormScripts(manager,
            new MockTypeSource(typeof(BasedOnRowForm), typeof(ExplicitForm), typeof(NoPermissionForm)),
            new MockPropertyItemProvider(), CreateServices(publicByDefault: false, granted.Contains));

        var bundle = manager.Registered["FormBundle"];
        Assert.True(bundle.HasPermission(new MockPermissions(x => x == "*")));

        var anonymous = bundle.GetScript();
        var anonymousSuffix = ((ICacheSuffix)bundle).CacheSuffix;
        Assert.Contains("'Explicit'", anonymous, StringComparison.Ordinal);
        Assert.DoesNotContain("'BasedOnRow'", anonymous, StringComparison.Ordinal);
        Assert.DoesNotContain("'None'", anonymous, StringComparison.Ordinal);

        granted.Add("RowRead");
        var reader = bundle.GetScript();
        Assert.Contains("'Explicit'", reader, StringComparison.Ordinal);
        Assert.Contains("'BasedOnRow'", reader, StringComparison.Ordinal);
        Assert.DoesNotContain("'None'", reader, StringComparison.Ordinal);
        Assert.NotEqual(anonymousSuffix, ((ICacheSuffix)bundle).CacheSuffix);
    }

    [Fact]
    public void Bundle_Without_Permission_Service_Excludes_Permission_Checked_Scripts()
    {
        var provider = new MockPropertyItemProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        var publicScript = new FormScript("Public", typeof(object), provider, services);
        var checkedScript = new FormScript("Checked", typeof(object), provider, services) { Permission = "*" };

        var script = new PropertyItemsBundleScript([publicScript, checkedScript], permissions: null).GetScript();

        Assert.Contains("'Public'", script, StringComparison.Ordinal);
        Assert.DoesNotContain("'Checked'", script, StringComparison.Ordinal);
    }

    [Fact]
    public void Bundle_Without_Accessible_Scripts_Is_Valid_Script()
    {
        var provider = new MockPropertyItemProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        var checkedScript = new FormScript("Checked", typeof(object), provider, services) { Permission = "X" };

        var script = new PropertyItemsBundleScript([checkedScript], new MockPermissions(_ => false)).GetScript();

        Assert.DoesNotContain("var ;", script, StringComparison.Ordinal);
        Assert.StartsWith("(function(){[]", script, StringComparison.Ordinal);
    }

    [Fact]
    public void Bundle_CacheSuffix_Is_Empty_When_No_Script_Requires_Permission()
    {
        var provider = new MockPropertyItemProvider();
        var services = new ServiceCollection().BuildServiceProvider();
        var bundle = new PropertyItemsBundleScript(
            [new FormScript("A", typeof(object), provider, services)], new MockPermissions(_ => false));

        Assert.Equal("", bundle.CacheSuffix);
    }

    [Fact]
    public void Options_Have_Default_Section_Key()
    {
        Assert.Equal("PropertyItemsScripts", typeof(PropertyItemsScriptOptions)
            .GetCustomAttribute<DefaultSectionKeyAttribute>()?.SectionKey);
        Assert.True(Options.Create(new PropertyItemsScriptOptions()).Value.PublicByDefault);
    }
}
