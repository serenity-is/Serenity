using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class ScriptBundleManagerTests
{
    public ScriptBundleManagerTests()
    {
        BundleUtils.ClearVersionCache();
    }

    private static (ScriptBundleManager manager, MockDynamicScriptManager scripts,
        MockHostEnvironment env, ScriptBundlingOptions options, MockScriptMinifier minifier) Create(
        Action<ScriptBundlingOptions>? configure = null, IHttpContextAccessor? accessor = null,
        Action<MockHostEnvironment>? configureEnv = null)
    {
        var env = new MockHostEnvironment();
        configureEnv?.Invoke(env);
        var options = new ScriptBundlingOptions();
        configure?.Invoke(options);
        var scripts = new MockDynamicScriptManager();
        var minifier = new MockScriptMinifier();
        var manager = new ScriptBundleManager(Options.Create(options),
            minifier, scripts, env, accessor);
        return (manager, scripts, env, options, minifier);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var env = new MockHostEnvironment();
        var options = Options.Create(new ScriptBundlingOptions());
        var scripts = new MockDynamicScriptManager();
        var minifier = new MockScriptMinifier();

        Assert.Throws<ArgumentNullException>(() =>
            new ScriptBundleManager(null!, minifier, scripts, env));
        Assert.Throws<ArgumentNullException>(() =>
            new ScriptBundleManager(options, null!, scripts, env));
        Assert.Throws<ArgumentNullException>(() =>
            new ScriptBundleManager(options, minifier, null!, env));
        Assert.Throws<ArgumentNullException>(() =>
            new ScriptBundleManager(options, minifier, scripts, null!));
    }

    [Fact]
    public void Reset_With_No_Bundles_Disables_Bundling()
    {
        var (manager, _, _, _, _) = Create(options => options.Enabled = true);

        Assert.False(manager.IsEnabled);
        Assert.Empty(manager.GetBundleIncludes("App"));
    }

    [Fact]
    public void Reset_When_Disabled_Disables_Bundling()
    {
        var (manager, _, _, options, _) = Create();
        options.Enabled = false;
        options.Bundles["App"] = ["~/Scripts/a.js"];

        manager.Reset();

        Assert.False(manager.IsEnabled);
    }

    [Fact]
    public void File_Bundle_Registers_Dynamic_Script_And_Resolves_Includes()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "var a = 1;");
        manager.Reset();

        Assert.True(manager.IsEnabled);
        Assert.True(scripts.IsRegistered("Bundle.App"));
        Assert.Equal(["~/Scripts/a.js"], manager.GetBundleIncludes("App"));
        Assert.Contains("var a = 1;", scripts.Registered["Bundle.App"].GetScript());
        Assert.Equal("/DynJS.axd/Bundle.App.js", manager.GetScriptBundle("~/Scripts/a.js"));
    }

    [Fact]
    public void Dynamic_Bundle_Resolves_Includes_And_Bundle_Url()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "dynamic code");
        manager.Reset();

        Assert.Contains("dynamic code", scripts.Registered["Bundle.App"].GetScript());
        Assert.Equal("/DynJS.axd/Bundle.App.js", manager.GetScriptBundle("dynamic://MyScript"));
        Assert.Equal("/DynJS.axd/Unknown.js", manager.GetScriptBundle("dynamic://Unknown"));
    }

    [Fact]
    public void Missing_File_Produces_Error_Content()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Scripts/missing.js"];
        });

        var content = scripts.Registered["Bundle.App"].GetScript();

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

        var content = scripts.Registered["Bundle.App"].GetScript();

        Assert.Contains("is not found", content);
    }

    [Fact]
    public void Minimize_Uses_Minifier_Result()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinJS = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "var a = 1;");
        minifier.MinifyCallback = (_, _) => new ScriptMinifyResult { Code = "MINIFIED" };
        manager.Reset();

        Assert.Contains("MINIFIED", scripts.Registered["Bundle.App"].GetScript());
    }

    [Fact]
    public void Minimize_Error_Keeps_Original_Content()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinJS = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "ORIGINAL");
        minifier.MinifyCallback = (_, _) => new ScriptMinifyResult { Code = "", HasErrors = true };
        manager.Reset();

        Assert.Contains("ORIGINAL", scripts.Registered["Bundle.App"].GetScript());
    }

    [Fact]
    public void Minimize_Exception_Keeps_Original_Content()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinJS = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "ORIGINAL");
        minifier.MinifyCallback = (_, _) => throw new InvalidOperationException("boom");
        manager.Reset();

        Assert.Contains("ORIGINAL", scripts.Registered["Bundle.App"].GetScript());
    }

    [Fact]
    public void UseMinJS_Prefers_Minified_File()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinJS = true;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "ORIGINAL");
        env.AddWebFile("Scripts/a.min.js", "MINFILE");
        manager.Reset();

        Assert.Contains("MINFILE", scripts.Registered["Bundle.App"].GetScript());
    }

    [Fact]
    public void NoMinimize_List_Keeps_Original_Content()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.NoMinimize = ["~/Scripts/a.js"];
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "ORIGINAL");
        bool called = false;
        minifier.MinifyCallback = (_, _) => { called = true; return new ScriptMinifyResult { Code = "X" }; };
        manager.Reset();

        Assert.False(called);
        Assert.Contains("ORIGINAL", scripts.Registered["Bundle.App"].GetScript());
    }

    [Fact]
    public void Nested_Bundle_Key_Does_Not_Register_Source_Url()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["Module/App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "var a = 1;");
        manager.Reset();

        Assert.Equal("/Scripts/a.js", manager.GetScriptBundle("~/Scripts/a.js"));
    }

    [Fact]
    public void GetScriptBundle_Uses_PathBase_From_Context_Accessor()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = "/app";
        var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
        var (manager, _, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        }, accessor);
        env.AddWebFile("Scripts/a.js", "var a = 1;");
        manager.Reset();

        Assert.Equal("/app/DynJS.axd/Bundle.App.js", manager.GetScriptBundle("~/Scripts/a.js"));
    }

    [Fact]
    public void GetScriptBundle_Throws_For_Null()
    {
        var (manager, _, _, _, _) = Create();
        Assert.Throws<ArgumentNullException>(() => manager.GetScriptBundle(null!));
    }

    [Fact]
    public void ScriptChanged_Raises_Bundle_Changed()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Bundles["App"] = ["dynamic://MyScript"];
        });
        scripts.SetScriptText("MyScript", "code");
        manager.Reset();
        scripts.ChangedNames.Clear();

        scripts.Changed("MyScript");

        Assert.Contains("Bundle.App", scripts.ChangedNames);
    }

    [Fact]
    public void ScriptsChanged_Notifies_Registered_Bundles_And_Resets()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Scripts/a.js"];
        });
        env.AddWebFile("Scripts/a.js", "var a = 1;");
        manager.Reset();

        manager.ScriptsChanged();

        Assert.Contains("Bundle.App", scripts.ChangedNames);
        Assert.True(manager.IsEnabled);
    }

    [Fact]
    public void ScriptsChanged_Does_Nothing_When_No_Bundles()
    {
        var (manager, scripts, _, _, _) = Create();

        manager.ScriptsChanged();

        Assert.Empty(scripts.ChangedNames);
    }

    [Fact]
    public void DoReplacements_Is_Applied_To_Bundle_Contents()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Replacements["env"] = "dev";
            options.Bundles["App"] = ["~/Scripts/{env}.js"];
        });
        env.AddWebFile("Scripts/dev.js", "DEV");
        manager.Reset();

        Assert.Contains("DEV", scripts.Registered["Bundle.App"].GetScript());
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
            options.Bundles["Ext"] = ["http://external/lib.js"];
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
        scripts.SetScriptText("MyScript", "code");
        manager.Reset();

        scripts.Registered["Bundle.App"].CheckRights(new MockPermissions(_ => true), NullTextLocalizer.Instance);

        Assert.Contains("MyScript", scripts.CheckedRights);
    }

    [Fact]
    public void ExpandVersionVariable_Is_Applied_To_Bundle_Contents()
    {
        var (manager, scripts, _, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Scripts/vlib-{version}.js"];
        }, configureEnv: env => env.AddWebFile("Scripts/vlib-1.2.js", "JQUERY"));

        Assert.Contains("JQUERY", scripts.Registered["Bundle.App"].GetScript());
    }
}
