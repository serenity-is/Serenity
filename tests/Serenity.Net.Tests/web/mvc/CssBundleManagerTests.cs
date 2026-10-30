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
            scripts, new CssBundleBuilder(Microsoft.Extensions.Options.Options.Create(options),
                scripts, minifier, env), env, accessor);
        return (manager, scripts, env, options, minifier);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var env = new MockHostEnvironment();
        var options = Microsoft.Extensions.Options.Options.Create(new CssBundlingOptions());
        var scripts = new MockDynamicScriptManager();
        var minifier = new MockCssMinifier();
        var builder = new CssBundleBuilder(options, scripts, minifier, env);

        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(null!, scripts, builder, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, null!, builder, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, scripts, null!, env));
        Assert.Throws<ArgumentNullException>(() =>
            new CssBundleManager(options, scripts, builder, null!));
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

    private static int CountOccurrences(string content, string value)
    {
        var count = 0;
        var index = 0;
        while ((index = content.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }
        return count;
    }

    [Fact]
    public void Plain_Import_Is_Inlined()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"b.css\";\nA_MARK{}");
        env.AddWebFile("Styles/b.css", "B_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("B_MARK", content);
        Assert.Contains("A_MARK", content);
        Assert.DoesNotContain("@import", content);
    }

    [Fact]
    public void Nested_Imports_Are_Inlined()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"b.css\";");
        env.AddWebFile("Styles/b.css", "@import \"c.css\";");
        env.AddWebFile("Styles/c.css", "C_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("C_MARK", content);
        Assert.DoesNotContain("@import", content);
    }

    [Fact]
    public void Media_External_And_Absolute_Imports_Are_Left_As_Is()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css",
            "@import \"http://cdn/x.css\";\n" +
            "@import \"/abs.css\";\n" +
            "@import \"cond.css\" screen;\n" +
            "@import \"b.css\";\n" +
            "A_MARK{}");
        env.AddWebFile("Styles/b.css", "B_MARK{}");
        env.AddWebFile("Styles/cond.css", "C_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Equal(3, CountOccurrences(content, "@import"));
        Assert.Contains("B_MARK", content);
        Assert.DoesNotContain("C_MARK", content);
    }

    [Fact]
    public void Imported_File_Urls_Are_Rewritten()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/a.css"];
        });
        env.AddWebFile("a.css", "@import \"b.css\";");
        env.AddWebFile("b.css", ".b { background: url(rel.png); }");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("../rel.png", content);
        Assert.DoesNotContain("@import", content);
    }

    [Fact]
    public void ProcessImports_False_Leaves_Import_Statements()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.ProcessImports = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"b.css\";\nA_MARK{}");
        env.AddWebFile("Styles/b.css", "B_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("@import", content);
        Assert.DoesNotContain("B_MARK", content);
        Assert.Contains("A_MARK", content);
    }

    [Fact]
    public void DedupImports_True_Embeds_Shared_Import_Once()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.DedupImports = true;
            options.Bundles["App"] = ["~/Styles/a.css", "~/Styles/b.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"shared.css\";\nA_MARK{}");
        env.AddWebFile("Styles/b.css", "@import \"shared.css\";\nB_MARK{}");
        env.AddWebFile("Styles/shared.css", "SHARED_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Equal(1, CountOccurrences(content, "SHARED_MARK"));
        Assert.Contains("A_MARK", content);
        Assert.Contains("B_MARK", content);
    }

    [Fact]
    public void DedupImports_False_Embeds_Shared_Import_At_Each_Position()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.DedupImports = false;
            options.Bundles["App"] = ["~/Styles/a.css", "~/Styles/b.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"shared.css\";\nA_MARK{}");
        env.AddWebFile("Styles/b.css", "@import \"shared.css\";\nB_MARK{}");
        env.AddWebFile("Styles/shared.css", "SHARED_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Equal(2, CountOccurrences(content, "SHARED_MARK"));
    }

    [Fact]
    public void Circular_Imports_Do_Not_Loop()
    {
        var (manager, scripts, env, _, _) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"b.css\";\nA_MARK{}");
        env.AddWebFile("Styles/b.css", "@import \"a.css\";\nB_MARK{}");
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("A_MARK", content);
        Assert.Contains("B_MARK", content);
        Assert.DoesNotContain("@import", content);
    }

    [Fact]
    public void Imported_Files_Are_Minified_Even_When_Entry_Is_Min_Css()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinCSS = true;
            options.Bundles["App"] = ["~/Styles/a.min.css"];
        });
        env.AddWebFile("Styles/a.min.css", "@import \"b.css\";\nA{}");
        env.AddWebFile("Styles/b.css", "B_ORIGINAL{}");
        minifier.MinifyCallback = (content, _) =>
            new CssMinifyResult { Code = content.Replace("B_ORIGINAL", "B_MINIFIED") };
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("B_MINIFIED", content);
        Assert.DoesNotContain("B_ORIGINAL", content);
    }

    [Fact]
    public void Imported_Files_Are_Minified_With_InMemory_Minifier()
    {
        var (manager, scripts, env, _, minifier) = Create(options =>
        {
            options.Enabled = true;
            options.Minimize = true;
            options.UseMinCSS = false;
            options.Bundles["App"] = ["~/Styles/a.css"];
        });
        env.AddWebFile("Styles/a.css", "@import \"b.css\";\nA{}");
        env.AddWebFile("Styles/b.css", "B_ORIGINAL{}");
        minifier.MinifyCallback = (content, _) =>
            new CssMinifyResult { Code = content.Replace("B_ORIGINAL", "B_MINIFIED") };
        manager.Reset();

        var content = scripts.Registered["CssBundle.App"].GetScript();

        Assert.Contains("B_MINIFIED", content);
        Assert.DoesNotContain("B_ORIGINAL", content);
    }
}
