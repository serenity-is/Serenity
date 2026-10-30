using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class LayoutScriptFilesTests
{
    private sealed class RecordingWebRootFileWatcher : IWebRootFileWatcher
    {
        public List<(string Key, IReadOnlyList<string> Files)> Calls { get; } = [];

        public IChangeToken? GetChangeToken(string webRelativePath) => null;

        public void RegisterCallback(string key, IEnumerable<string> appRelativePaths, Action callback)
        {
            Calls.Add((key, appRelativePaths.ToList()));
        }
    }

    private static ServiceProvider CreateServices(Attribute[] attributes,
        MockDynamicScriptManager scripts, MockHostEnvironment env,
        LayoutScriptFileOptions? options = null, bool bundlingEnabled = true,
        IFeatureToggles? featureToggles = null, IWebRootFileWatcher? layoutWatcher = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDynamicScriptManager>(scripts);
        services.AddSingleton<IScriptBundleManager>(new MockScriptBundleManager());
        services.AddSingleton<IContentHashCache>(new MockContentHashCache());
        services.AddSingleton<IWebHostEnvironment>(env);
        if (featureToggles != null)
            services.AddSingleton(featureToggles);
        if (layoutWatcher != null)
            services.AddSingleton(layoutWatcher);
        services.AddSingleton(Options.Create(new ScriptBundlingOptions
        {
            Enabled = bundlingEnabled,
            Minimize = false
        }));
        services.AddSingleton<IScriptBundleBuilder>(new ScriptBundleBuilder(
            Options.Create(new ScriptBundlingOptions { Minimize = false }),
            new MockScriptMinifier(), scripts, env));
        services.AddSingleton<ILayoutScriptFileProvider>(new LayoutScriptFileProvider(
            new MockTypeSource(attributes), services.BuildServiceProvider(),
            Options.Create(options ?? new LayoutScriptFileOptions())));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void LayoutScriptFiles_Throws_For_Null_Helper()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HtmlLayoutFilesExtensions.LayoutScriptFiles(null!));
    }

    [Fact]
    public void LayoutScriptFiles_Returns_Empty_When_No_Files()
    {
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([], scripts, new MockHostEnvironment()));

        Assert.Equal("", html.LayoutScriptFiles().Value);
        Assert.Empty(scripts.Registered);
    }

    [Fact]
    public void LayoutScriptFiles_Renders_Single_Concatenated_Bundle_When_Enabled()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "var a = 'A_MARK';");
        env.AddWebFile("b.js", "var b = 'B_MARK';");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js") { Order = 1 },
            new LayoutScriptFileAttribute("~/b.js") { Order = 2 },
        ], scripts, env));

        var result = html.LayoutScriptFiles().Value;

        Assert.Contains("DynJS.axd/LayoutScriptFiles.", result);
        var content = scripts.Registered.Values.Single().GetScript();
        Assert.Contains("A_MARK", content);
        Assert.Contains("B_MARK", content);
        Assert.True(content.IndexOf("A_MARK", StringComparison.Ordinal) <
            content.IndexOf("B_MARK", StringComparison.Ordinal));
    }

    [Fact]
    public void LayoutScriptFiles_Renders_Individual_Tags_When_Disabled()
    {
        var env = new MockHostEnvironment();
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js") { Order = 1 },
            new LayoutScriptFileAttribute("~/b.js") { Order = 2 },
        ], scripts, env, bundlingEnabled: false));

        var result = html.LayoutScriptFiles().Value;

        Assert.Contains("src=\"~/a.js\"", result);
        Assert.Contains("src=\"~/b.js\"", result);
        Assert.Empty(scripts.Registered);
    }

    [Fact]
    public void LayoutScriptFiles_Returns_Empty_When_Already_Included()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "A_MARK");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js"),
        ], scripts, env));

        Assert.Contains("<script", html.LayoutScriptFiles().Value);
        Assert.Equal("", html.LayoutScriptFiles().Value);
    }

    [Fact]
    public void LayoutScriptFiles_Registers_Distinct_Bundle_Per_Feature_Set()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "A_MARK");
        env.AddWebFile("b.js", "B_MARK");
        var scripts = new MockDynamicScriptManager();
        var toggles = new MockFeatureToggles { IsEnabledCallback = feature => feature == "A" };
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js") { RequireFeatures = ["A"] },
            new LayoutScriptFileAttribute("~/b.js") { RequireFeatures = ["B"] },
        ], scripts, env, featureToggles: toggles));

        var first = html.LayoutScriptFiles().Value;

        toggles.IsEnabledCallback = feature => feature == "B";
        var second = html.LayoutScriptFiles().Value;

        Assert.NotEqual(first, second);
        Assert.Equal(2, scripts.Registered.Count);
    }

    [Fact]
    public void LayoutScriptFiles_Registers_Watcher_Only_When_Bundling_Enabled()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "A_MARK");
        var scripts = new MockDynamicScriptManager();
        var layoutWatcher = new RecordingWebRootFileWatcher();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js"),
        ], scripts, env, layoutWatcher: layoutWatcher));

        html.LayoutScriptFiles();

        var call = Assert.Single(layoutWatcher.Calls);
        Assert.Equal(["~/a.js"], call.Files);

        var disabledWatcher = new RecordingWebRootFileWatcher();
        var disabledHtml = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js"),
        ], new MockDynamicScriptManager(), env, bundlingEnabled: false, layoutWatcher: disabledWatcher));

        disabledHtml.LayoutScriptFiles();

        Assert.Empty(disabledWatcher.Calls);
    }

    [Fact]
    public void LayoutScriptFiles_Filter_Excludes_Files()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "A_MARK");
        env.AddWebFile("b.js", "B_MARK");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js"),
            new LayoutScriptFileAttribute("~/b.js"),
        ], scripts, env));

        var result = html.LayoutScriptFiles(path => path != "~/a.js").Value;

        Assert.Contains("<script", result);
        var content = scripts.Registered.Values.Single().GetScript();
        Assert.DoesNotContain("A_MARK", content);
        Assert.Contains("B_MARK", content);
    }

    [Fact]
    public void LayoutScriptFiles_Filter_Excluding_All_Returns_Empty()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.js", "A_MARK");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutScriptFileAttribute("~/a.js"),
        ], scripts, env));

        Assert.Equal("", html.LayoutScriptFiles(_ => false).Value);
        Assert.Empty(scripts.Registered);
    }
}
