using System.Globalization;
using Microsoft.Extensions.Options;
using Serenity.Localization;

namespace Serenity.Web;

public class LocalTextDataScriptTests
{
    private static (LocalTextDataScript script, LocalTextRegistry registry) Create(string? query = null)
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Site.Title", "Title");
        registry.Add("en", "Other.Key", "Other");
        registry.Add("invariant", "Site.Fallback", "Fallback");

        var accessor = new MockHttpContextAccessor { HttpContext = new DefaultHttpContext() };
        if (query != null)
            accessor.HttpContext!.Request.QueryString = new QueryString(query);

        var packages = new LocalTextPackages
        {
            ["Site"] = "^Site\\.",
            ["Other"] = "^Other\\."
        };

        var script = new LocalTextDataScript(registry,
            Microsoft.Extensions.Options.Options.Create(packages), accessor);
        return (script, registry);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var registry = new LocalTextRegistry();
        var accessor = new MockHttpContextAccessor();
        var packages = Microsoft.Extensions.Options.Options.Create(new LocalTextPackages());

        Assert.Throws<ArgumentNullException>(() => new LocalTextDataScript(null!, packages, accessor));
        Assert.Throws<ArgumentNullException>(() => new LocalTextDataScript(registry, null!, accessor));
        Assert.Throws<ArgumentNullException>(() => new LocalTextDataScript(registry, packages, null!));
    }

    [Fact]
    public void CacheSuffix_Uses_Language_And_Package()
    {
        var (script, _) = Create("?lang=en&pack=Site");
        Assert.Equal("en:Site", script.CacheSuffix);
    }

    [Fact]
    public void CacheSuffix_Falls_Back_For_Invalid_Language()
    {
        var (script, _) = Create("?lang=invalid-lang&pack=Site");
        var languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
        Assert.Equal(languageId + ":Site", script.CacheSuffix);
    }

    [Fact]
    public void CacheSuffix_Falls_Back_For_Unknown_Package()
    {
        var (script, _) = Create("?lang=en&pack=Unknown");
        Assert.Equal("en:Site", script.CacheSuffix);
    }

    [Fact]
    public void GetScriptData_Returns_Package_Texts()
    {
        var (script, _) = Create("?lang=en&pack=Site");

        var data = Assert.IsAssignableFrom<IDictionary<string, string>>(script.GetScriptData());

        Assert.Equal("Title", data["Site.Title"]);
        Assert.DoesNotContain("Other.Key", data.Keys);
    }

    [Fact]
    public void GetPackageData_Throws_For_Null_Registry()
    {
        Assert.Throws<ArgumentNullException>(() =>
            LocalTextDataScript.GetPackageData(null!, ".*", "en", false));
    }

    [Fact]
    public void GetPackageData_With_Empty_Includes_Returns_Empty()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Site.Title", "Title");

        var data = LocalTextDataScript.GetPackageData(registry, null, "en", false);

        Assert.Empty(data);
    }

    [Fact]
    public void GetPackageData_Uses_Default_Site_Includes()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Validation.Required", "Required");
        registry.Add("en", "Other.Key", "Other");

        var data = LocalTextDataScript.GetPackageData(registry, "Site", "en", false, "Site");

        Assert.Equal("Required", data["Validation.Required"]);
        Assert.DoesNotContain("Other.Key", data.Keys);
    }
}
