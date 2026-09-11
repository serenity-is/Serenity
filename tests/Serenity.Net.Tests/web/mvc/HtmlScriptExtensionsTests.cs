using System.Globalization;
using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class HtmlScriptExtensionsTests
{
    private static ServiceProvider CreateServices(
        MockCssBundleManager? css = null,
        MockScriptBundleManager? script = null,
        MockContentHashCache? hash = null,
        MockDynamicScriptManager? dynamicScripts = null,
        MockHostEnvironment? env = null,
        LocalTextPackages? packages = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICssBundleManager>(css ?? new MockCssBundleManager());
        services.AddSingleton<IScriptBundleManager>(script ?? new MockScriptBundleManager());
        services.AddSingleton<IContentHashCache>(hash ?? new MockContentHashCache());
        services.AddSingleton<IDynamicScriptManager>(dynamicScripts ?? new MockDynamicScriptManager());
        services.AddSingleton<IWebHostEnvironment>(env ?? new MockHostEnvironment());
        services.AddSingleton<ILocalTextRegistry>(new MockLocalTextRegistry());
        services.AddSingleton(Options.Create(packages ?? []));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Stylesheet_Throws_For_Null_Helper_And_Url()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HtmlScriptExtensions.Stylesheet(null!, "a.css"));
        var html = MockHtmlHelper.Create(CreateServices());
        Assert.Throws<ArgumentNullException>(() => html.Stylesheet(null!));
    }

    [Fact]
    public void Stylesheet_Converts_Js_To_Css_And_Renders_Link()
    {
        var css = new MockCssBundleManager();
        var html = MockHtmlHelper.Create(CreateServices(css: css));

        var result = html.Stylesheet("~/Scripts/site.js").Value;

        Assert.Contains("href=\"~/Scripts/site.css\"", result);
        Assert.Contains("rel=\"stylesheet\"", result);
    }

    [Fact]
    public void Stylesheet_Returns_Empty_When_Already_Included()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        html.Stylesheet("~/site.css");
        var second = html.Stylesheet("~/site.css");

        Assert.Equal("", second.Value);
    }

    [Fact]
    public void AutoIncludeModuleCss_Returns_Empty_For_Empty_Or_Non_Module()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        Assert.Equal("", html.AutoIncludeModuleCss("").Value);
        Assert.Equal("", html.AutoIncludeModuleCss("~/Scripts/mod.txt").Value);
        Assert.Equal("", html.AutoIncludeModuleCss("Scripts/mod.js").Value);
    }

    [Fact]
    public void AutoIncludeModuleCss_Returns_Empty_When_Css_Missing()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        Assert.Equal("", html.AutoIncludeModuleCss("~/Scripts/mod.js").Value);
    }

    [Fact]
    public void AutoIncludeModuleCss_Includes_Css_When_Exists()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("Scripts/mod.css", "body{}");
        var html = MockHtmlHelper.Create(CreateServices(env: env));

        var result = html.AutoIncludeModuleCss("~/Scripts/mod.js").Value;

        Assert.Contains("mod.css", result);
    }

    [Fact]
    public void ModulePageInit_Sets_ViewData_And_Renders_Script()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        var result = html.ModulePageInit("~/Scripts/mod.js", new { a = 1 }).Value;

        Assert.Equal("~/Scripts/mod.js", html.ViewData["ModulePageScript"]);
        Assert.Contains("import pageInit", result);
        Assert.Contains("pageInit({", result);
        Assert.Contains("nonce=", result);
    }

    [Fact]
    public void ModulePageInit_Without_Css_And_Options()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        var result = html.ModulePageInit("~/Scripts/mod.js", css: false).Value;

        Assert.DoesNotContain("<link", result);
        Assert.Contains("pageInit();", result);
    }

    [Fact]
    public void StyleBundle_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlScriptExtensions.StyleBundle(null!, "bundle"));
        var html = MockHtmlHelper.Create(CreateServices());
        Assert.Throws<ArgumentNullException>(() => html.StyleBundle(null!));
        Assert.Throws<ArgumentNullException>(() => html.StyleBundle(""));
    }

    [Fact]
    public void StyleBundle_When_Enabled_Uses_Stylesheet()
    {
        var css = new MockCssBundleManager
        {
            IsEnabled = true,
            GetCssBundleCallback = _ => "/bundle.css"
        };
        var html = MockHtmlHelper.Create(CreateServices(css: css));

        var result = html.StyleBundle("mybundle").Value;

        Assert.Contains("href=\"/bundle.css\"", result);
    }

    [Fact]
    public void StyleBundle_When_Disabled_Renders_Includes()
    {
        var css = new MockCssBundleManager();
        css.Includes["mybundle"] = ["~/a.css", "dynamic://SomeScript", ""];
        var html = MockHtmlHelper.Create(CreateServices(css: css));

        var result = html.StyleBundle("mybundle").Value;

        Assert.Contains("href=\"/a.css\"", result);
        Assert.Contains("DynJS.axd/SomeScript.css", result);
    }

    [Fact]
    public void ResolveWithHash_Throws_For_Null_Helper_And_Empty_Url()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlScriptExtensions.ResolveWithHash(null!, "a.css"));
        var html = MockHtmlHelper.Create(CreateServices());
        Assert.Throws<ArgumentNullException>(() => html.ResolveWithHash(""));
    }

    [Fact]
    public void ResolveWithHash_Encodes_Result()
    {
        var hash = new MockContentHashCache
        {
            ResolveWithHashCallback = (_, url) => "/x?v=1&y=2"
        };
        var html = MockHtmlHelper.Create(CreateServices(hash: hash));

        var result = html.ResolveWithHash("~/x").Value;

        Assert.Equal("/x?v=1&amp;y=2", result);
    }

    [Fact]
    public void Script_Throws_For_Null_Helper_And_Empty_Url()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlScriptExtensions.Script(null!, "a.js"));
        var html = MockHtmlHelper.Create(CreateServices());
        Assert.Throws<ArgumentNullException>(() => html.Script(""));
    }

    [Fact]
    public void Script_Renders_And_Skips_Already_Included()
    {
        var html = MockHtmlHelper.Create(CreateServices());

        var first = html.Script("~/Scripts/site.js").Value;
        var second = html.Script("~/Scripts/site.js").Value;

        Assert.Contains("<script src=", first);
        Assert.Equal("", second);
    }

    [Fact]
    public void ScriptBundle_Throws_For_Null_Arguments()
    {
        Assert.Throws<ArgumentNullException>(() => HtmlScriptExtensions.ScriptBundle(null!, "bundle"));
        var html = MockHtmlHelper.Create(CreateServices());
        Assert.Throws<ArgumentNullException>(() => html.ScriptBundle(null!));
        Assert.Throws<ArgumentNullException>(() => html.ScriptBundle(""));
    }

    [Fact]
    public void ScriptBundle_When_Enabled_Uses_Script()
    {
        var script = new MockScriptBundleManager
        {
            IsEnabled = true,
            GetScriptBundleCallback = _ => "/bundle.js"
        };
        var html = MockHtmlHelper.Create(CreateServices(script: script));

        var result = html.ScriptBundle("mybundle").Value;

        Assert.Contains("src=\"/bundle.js\"", result);
    }

    [Fact]
    public void ScriptBundle_When_Disabled_Renders_Includes()
    {
        var script = new MockScriptBundleManager();
        script.Includes["mybundle"] = ["~/a.js", "dynamic://SomeScript", ""];
        var html = MockHtmlHelper.Create(CreateServices(script: script));

        var result = html.ScriptBundle("mybundle").Value;

        Assert.Contains("src=\"/a.js\"", result);
        Assert.Contains("DynJS.axd/SomeScript", result);
    }

    [Fact]
    public void GetLocalTextContent_Registers_Script_And_Returns_Text()
    {
        var packages = new LocalTextPackages { ["Site"] = "" };
        var dynamicScripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices(dynamicScripts: dynamicScripts, packages: packages));

        var content = html.GetLocalTextContent("Site");

        Assert.Equal("", content);
        Assert.Contains("LocalText.Site", dynamicScripts.Registered.Keys.Single());
    }

    [Fact]
    public void GetLocalTextInclude_Returns_Include()
    {
        var packages = new LocalTextPackages { ["Site"] = "" };
        var dynamicScripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices(dynamicScripts: dynamicScripts, packages: packages));

        var include = html.GetLocalTextInclude("Site");

        var languageId = CultureInfo.CurrentUICulture.Name.TrimToNull() ?? "invariant";
        Assert.Equal($"LocalText.Site.{languageId}.Public.js", include);
    }

    [Fact]
    public void LocalTextScript_Renders_Script()
    {
        var packages = new LocalTextPackages { ["Site"] = "" };
        var dynamicScripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices(dynamicScripts: dynamicScripts, packages: packages));

        var result = html.LocalTextScript("Site").Value;

        Assert.Contains("<script src=", result);
    }
}
