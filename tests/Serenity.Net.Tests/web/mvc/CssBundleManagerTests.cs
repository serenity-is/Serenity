using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class CssBundleManagerTests
{
    public CssBundleManagerTests()
    {
        BundleUtils.ClearVersionCache();
    }

    private static (CssBundleManager manager, MockDynamicScriptManager scripts,
        MockHostEnvironment env, CssBundlingOptions options, MockCssMinifier minifier) Create(
        Action<CssBundlingOptions>? configure = null, IHttpContextAccessor? accessor = null,
        Action<MockHostEnvironment>? configureEnv = null)
    {
        var env = new MockHostEnvironment();
        configureEnv?.Invoke(env);
        var options = new CssBundlingOptions();
        configure?.Invoke(options);
        var scripts = new MockDynamicScriptManager();
        var minifier = new MockCssMinifier();
        var manager = new CssBundleManager(Microsoft.Extensions.Options.Options.Create(options),
            scripts, minifier, env, accessor);
        return (manager, scripts, env, options, minifier);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var env = new MockHostEnvironment();
        var options = Microsoft.Extensions.Options.Options.Create(new CssBundlingOptions());
        var scripts = new MockDynamicScriptManager();
        var minifier = new MockCssMinifier();

        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(null!, scripts, minifier, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, null!, minifier, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, scripts, null!, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, scripts, minifier, null!));
    }

    [Fact]
    public void Reset_With_No_Bundles_Disables_Bundling()
    {
        var (manager, _, _, _, _) = Create(options => options.Enabled = true);

        Assert.False(manager.IsEnabled);
        Assert.Empty(manager.GetBundleIncludes("App"));
    }

    [Fact]
    public void File_Bundle_Registers_And_Resolves_Css_Bundle()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "body { color: red; }");
        manager.Reset();

        Assert.True(manager.IsEnabled);
        Assert.Equal(["~/Styles/a.css"], manager.GetBundleIncludes("App"));
        Assert.Contains("body", scripts.Registered["CssBundle.App"].GetScript());
        Assert.Equal("/DynJS.axd/CssBundle.App.css", manager.GetCssBundle("~/Styles/a.css"));
    }

    [Fact]
    public void Dynamic_Bundle_Resolves_Css_Bundle()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "dynamic css");
        manager.Reset();

        Assert.Contains("dynamic css", scripts.Registered["CssBundle.App"].GetScript());
        Assert.Equal("/DynJS.axd/CssBundle.App.css", manager.GetCssBundle("dynamic://MyScript"));
        Assert.Equal("/DynJS.axd/Unknown.css", manager.GetCssBundle("dynamic://Unknown"));
    }

    [Fact]
    public void Rtl_Bundle_Is_Generated_Automatically()
    {
        var (manager, _, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a{.rtl}.css"];
        });

        Assert.Equal(["~/Styles/a.rtl.css"], manager.GetBundleIncludes("App.rtl"));
        Assert.Equal(["~/Styles/a.css"], manager.GetBundleIncludes("App"));
    }

    [Fact]
    public void GetCssBundle_Throws_For_Null()
    {
        var (manager, _, _, _, _) = Create();
        Assert.Throws<ArgumentNullException>(() => manager.GetCssBundle(null!));
    }

    [Fact]
    public void GetCssBundle_Returns_Source_Url_When_Not_In_Bundle()
    {
        var (manager, _, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        manager.Reset();

        Assert.Equal("~/Styles/other.css", manager.GetCssBundle("~/Styles/other.css"));
    }

    [Fact]
    public void GetCssBundle_Converts_Absolute_Url_To_App_Relative()
    {
        var (manager, _, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        manager.Reset();

        Assert.Equal("/DynJS.axd/CssBundle.App.css", manager.GetCssBundle("/Styles/a.css"));
    }

    [Fact]
    public void Missing_File_Produces_Error_Content()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/missing.css"];
        });

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("!ERROR:", content);
        Assert.Contains("is not found", content);
    }

    [Fact]
    public void Missing_Dynamic_Script_Produces_Error_Content()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://Missing"];
        });

        Assert.Contains("is not found", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void Minimize_Uses_Minifier_Result()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinCSS = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "ORIGINAL");
        minifier.MinifyCallback = (_, _) => new CssMinifyResult { Code = "MINIFIED" };
        manager.Reset();

        Assert.Contains("MINIFIED", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void Minimize_Exception_Keeps_Original_Content()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinCSS = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "ORIGINAL");
        minifier.MinifyCallback = (_, _) => throw new InvalidOperationException("boom");
        manager.Reset();

        Assert.Contains("ORIGINAL", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void UseMinCSS_Prefers_Minified_File()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinCSS = true;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "ORIGINAL");
        env.AddWebFile("Styles/a.min.css", "MINFILE");
        manager.Reset();

        Assert.Contains("MINFILE", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void NoMinimize_List_Keeps_Original_Content()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.NoMinimize = ["~/Styles/a.css"];
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "ORIGINAL");
        bool called = false;
        minifier.MinifyCallback = (_, _) => { called = true; return new CssMinifyResult { Code = "X" }; };
        manager.Reset();

        Assert.False(called);
        Assert.Contains("ORIGINAL", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void SourceMapping_Comments_Are_Removed()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "body{}\n/*# sourceMappingURL=a.css.map */\n");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();
        Assert.DoesNotContain("sourceMappingURL", content);
        Assert.Contains("body", content);
    }

    [Fact]
    public void CssChanged_Notifies_Registered_Bundles_And_Resets()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "body{}");
        manager.Reset();

        manager.CssChanged();

        Assert.Contains("CssBundle.App", scripts.ChangedNames);
        Assert.True(manager.IsEnabled);
    }

    [Fact]
    public void CssChanged_Does_Nothing_When_No_Bundles()
    {
        var (manager, scripts, _, _, _) = Create();

        manager.CssChanged();

        Assert.Empty(scripts.ChangedNames);
    }

    [Fact]
    public void ScriptChanged_Raises_Bundle_Changed()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "css");
        manager.Reset();
        scripts.ChangedNames.Clear();

        scripts.Changed("MyScript");

        Assert.Contains("CssBundle.App", scripts.ChangedNames);
    }

    [Fact]
    public void Reset_Skips_Empty_Blank_And_External_Entries()
    {
        var (manager, _, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["Empty"] = [];
            options.Bundles["Blank"] = ["", " "];
            options.Bundles["Ext"] = ["http://external/lib.css"];
        });

        Assert.True(manager.IsEnabled);
    }

    [Fact]
    public void Dynamic_Bundle_CheckRights_Checks_Contained_Scripts()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "css");
        manager.Reset();

        scripts.Registered["CssBundle.App"].CheckRights(new MockPermissions(_ => true), NullTextLocalizer.Instance);

        Assert.Contains("MyScript", scripts.CheckedRights);
    }

    [Fact]
    public void Dynamic_Minify_Exception_Keeps_Original_Code()
    {
        var (manager, scripts, _, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "ORIGINAL");
        minifier.MinifyCallback = (_, _) => throw new InvalidOperationException("boom");
        manager.Reset();

        Assert.Contains("ORIGINAL", scripts.Registered["CssBundle.App"].GetScript());
    }

    [Fact]
    public void RewriteUrls_Handles_Absolute_Rooted_And_Empty_Content()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/a.css", "~/Styles/abs.css", "~/Styles/empty.css"];
        });
        env.AddWebFile("a.css", "url(rel.png)");
        env.AddWebFile("Styles/abs.css", "url(/absolute.png)");
        env.AddWebFile("Styles/empty.css", "");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("url(/absolute.png)", content);
        Assert.Contains("../rel.png", content);
    }

    [Fact]
    public void GetCssBundle_Returns_Absolute_Url_When_Not_Under_PathBase()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = "/app";
        var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
        var (manager, _, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["~/Styles/a.css"];
        }, accessor);
        manager.Reset();

        Assert.Equal("/other.css", manager.GetCssBundle("/other.css"));
    }

    [Fact]
    public void Replacement_And_Version_Are_Applied()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Replacements["env"] = "dev";
            options.Bundles["App"] = ["~/Styles/{env}.css"];
        });
        env.AddWebFile("Styles/dev.css", "DEV");
        manager.Reset();

        Assert.Contains("DEV", scripts.Registered["CssBundle.App"].GetScript());
    }
}
