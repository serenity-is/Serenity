using Microsoft.Extensions.Options;
using Serenity.Localization;

namespace Serenity.Web;

public class LocalTextScriptTests
{
    private static LocalTextRegistry CreateRegistry()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Site.Title", "Title");
        registry.Add("en", "Site.Sub.Child", "Child");
        registry.Add("en", "Site.Sub", "Sub");
        registry.Add("en", "Site.", "Trailing");
        registry.Add("en", "", "Empty");
        registry.Add("en", "Other.Key", "Other");
        registry.AddPending("en", "Site.Pending", "Pending");
        return registry;
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var registry = CreateRegistry();
        Assert.Throws<ArgumentNullException>(() => new LocalTextScript(null!, "Site", "", "en", false));
        Assert.Throws<ArgumentNullException>(() => new LocalTextScript(registry, null!, "", "en", false));
    }

    [Fact]
    public void GetScriptName_Formats_Name()
    {
        Assert.Equal("LocalText.Site.en.Public", LocalTextScript.GetScriptName("Site", "en", false));
        Assert.Equal("LocalText.Site.en.Pending", LocalTextScript.GetScriptName("Site", "en", true));
    }

    [Fact]
    public void GetLocalTextPackageScript_Throws_For_Null_Package_Or_Packages()
    {
        var registry = CreateRegistry();
        Assert.Throws<ArgumentNullException>(() =>
            LocalTextScript.GetLocalTextPackageScript(registry, new LocalTextPackages(), null!, "en", false));
        Assert.Throws<ArgumentNullException>(() =>
            LocalTextScript.GetLocalTextPackageScript(registry, null!, "Site", "en", false));
    }

    [Fact]
    public void GetLocalTextPackageScript_Generates_Script()
    {
        var registry = CreateRegistry();

        var script = LocalTextScript.GetLocalTextPackageScript(registry, ".*", "en", false, "Site");

        Assert.Contains("addLocalText", script);
        Assert.Contains("Title", script);
    }

    [Fact]
    public void GetLocalTextPackageScript_Skips_Trailing_Dot_And_Empty_Keys()
    {
        var registry = CreateRegistry();

        var script = LocalTextScript.GetLocalTextPackageScript(registry, ".*", "en", false, "Site");

        Assert.DoesNotContain("Trailing", script);
        Assert.DoesNotContain("Empty", script);
    }

    [Fact]
    public void GetLocalTextPackageScript_Includes_Pending_When_Requested()
    {
        var registry = CreateRegistry();

        var script = LocalTextScript.GetLocalTextPackageScript(registry, "^Site\\.", "en", true, "Site");

        Assert.Contains("Pending", script);
    }

    [Fact]
    public void GetScript_Returns_Package_Script()
    {
        var registry = CreateRegistry();
        var script = new LocalTextScript(registry, "Site", "^Site\\.", "en", false);

        var result = script.GetScript();

        Assert.Contains("addLocalText", result);
        Assert.Equal("LocalText.Site.en.Public", script.ScriptName);
    }

    [Fact]
    public void GetLocalTextPackageScript_With_Packages_Uses_Package_Includes()
    {
        var registry = CreateRegistry();
        var packages = new LocalTextPackages { ["Site"] = "^Site\\." };

        var script = LocalTextScript.GetLocalTextPackageScript(registry, packages, "Site", "en", false);

        Assert.Contains("addLocalText", script);
    }
}
