using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class LayoutCssFilesTests
{
    private sealed class RecordingLayoutCssFileWatcher : ILayoutCssFileWatcher
    {
        public List<(string ScriptName, IReadOnlyList<string> Files)> Calls { get; } = [];

        public void Watch(string scriptName, IReadOnlyList<string> appRelativeUrls)
        {
            Calls.Add((scriptName, appRelativeUrls));
        }
    }

    private static ServiceProvider CreateServices(Attribute[] attributes,
        MockDynamicScriptManager scripts, MockHostEnvironment env,
        LayoutCssFileOptions? options = null, bool bundlingEnabled = true,
        IFeatureToggles? featureToggles = null, ILayoutCssFileWatcher? layoutWatcher = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDynamicScriptManager>(scripts);
        services.AddSingleton<ICssBundleManager>(new MockCssBundleManager());
        services.AddSingleton<IContentHashCache>(new MockContentHashCache());
        services.AddSingleton<IWebHostEnvironment>(env);
        if (featureToggles != null)
            services.AddSingleton(featureToggles);
        if (layoutWatcher != null)
            services.AddSingleton(layoutWatcher);
        services.AddSingleton(Options.Create(new CssBundlingOptions
        {
            Enabled = bundlingEnabled,
            Minimize = false
        }));
        services.AddSingleton<ICssBundleBuilder>(new CssBundleBuilder(
            Options.Create(new CssBundlingOptions { Minimize = false }),
            scripts, new MockCssMinifier(), env));
        services.AddSingleton<ILayoutCssFileProvider>(new LayoutCssFileProvider(
            new MockTypeSource(attributes), services.BuildServiceProvider(),
            Options.Create(options ?? new LayoutCssFileOptions())));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void LayoutCssFiles_Throws_For_Null_Helper()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HtmlLayoutCssFilesExtensions.LayoutCssFiles(null!));
    }

    [Fact]
    public void LayoutCssFiles_Returns_Empty_When_No_Files()
    {
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([], scripts, new MockHostEnvironment()));

        Assert.Equal("", html.LayoutCssFiles().Value);
        Assert.Empty(scripts.Registered);
    }

    [Fact]
    public void LayoutCssFiles_Renders_Single_Concatenated_Bundle_When_Enabled()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        env.AddWebFile("b.css", "B_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css") { Order = 1 },
            new LayoutCssFileAttribute("~/b.css") { Order = 2 },
        ], scripts, env));

        var result = html.LayoutCssFiles().Value;

        Assert.Contains("DynJS.axd/LayoutCssFiles.", result);
        var content = scripts.Registered.Values.Single().GetScript();
        Assert.Contains("A_MARK", content);
        Assert.Contains("B_MARK", content);
        Assert.True(content.IndexOf("A_MARK", StringComparison.Ordinal) <
            content.IndexOf("B_MARK", StringComparison.Ordinal));
    }

    [Fact]
    public void LayoutCssFiles_Renders_Individual_Links_When_Disabled()
    {
        var env = new MockHostEnvironment();
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css") { Order = 1 },
            new LayoutCssFileAttribute("~/b.css") { Order = 2 },
        ], scripts, env, bundlingEnabled: false));

        var result = html.LayoutCssFiles().Value;

        Assert.Contains("href=\"~/a.css\"", result);
        Assert.Contains("href=\"~/b.css\"", result);
        Assert.Empty(scripts.Registered);
    }

    [Fact]
    public void LayoutCssFiles_Returns_Empty_When_Already_Included()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
        ], scripts, env));

        Assert.Contains("<link", html.LayoutCssFiles().Value);
        Assert.Equal("", html.LayoutCssFiles().Value);
    }

    [Fact]
    public void LayoutCssFiles_Considers_Feature_Filtered_Files()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        env.AddWebFile("b.css", "B_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
            new LayoutCssFileAttribute("~/b.css"),
        ], scripts, env, new LayoutCssFileOptions { Filter = attr => attr.Path != "~/a.css" }));

        html.LayoutCssFiles();

        var content = scripts.Registered.Values.Single().GetScript();
        Assert.DoesNotContain("A_MARK", content);
        Assert.Contains("B_MARK", content);
    }

    [Fact]
    public void LayoutCssFiles_Registers_Distinct_Bundle_Per_Feature_Set()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        env.AddWebFile("b.css", "B_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var toggles = new MockFeatureToggles { IsEnabledCallback = feature => feature == "A" };
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css") { RequireFeatures = ["A"] },
            new LayoutCssFileAttribute("~/b.css") { RequireFeatures = ["B"] },
        ], scripts, env, featureToggles: toggles));

        var first = html.LayoutCssFiles().Value;

        toggles.IsEnabledCallback = feature => feature == "B";
        var second = html.LayoutCssFiles().Value;

        Assert.NotEqual(first, second);
        Assert.Equal(2, scripts.Registered.Count);
        Assert.Contains(scripts.Registered.Values,
            s => s.GetScript().Contains("A_MARK", StringComparison.Ordinal));
        Assert.Contains(scripts.Registered.Values,
            s => s.GetScript().Contains("B_MARK", StringComparison.Ordinal));
    }

    [Fact]
    public void LayoutCssFiles_Registers_Watcher_Only_When_Bundling_Enabled()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var layoutWatcher = new RecordingLayoutCssFileWatcher();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
        ], scripts, env, layoutWatcher: layoutWatcher));

        html.LayoutCssFiles();

        var call = Assert.Single(layoutWatcher.Calls);
        Assert.Equal(["~/a.css"], call.Files);

        var disabledWatcher = new RecordingLayoutCssFileWatcher();
        var disabledScripts = new MockDynamicScriptManager();
        var disabledHtml = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
        ], disabledScripts, env, bundlingEnabled: false, layoutWatcher: disabledWatcher));

        disabledHtml.LayoutCssFiles();

        Assert.Empty(disabledWatcher.Calls);
    }

    [Fact]
    public void LayoutCssFiles_Filter_Excludes_Files()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        env.AddWebFile("b.css", "B_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
            new LayoutCssFileAttribute("~/b.css"),
        ], scripts, env));

        var result = html.LayoutCssFiles(path => path != "~/a.css").Value;

        Assert.Contains("<link", result);
        var content = scripts.Registered.Values.Single().GetScript();
        Assert.DoesNotContain("A_MARK", content);
        Assert.Contains("B_MARK", content);
    }

    [Fact]
    public void LayoutCssFiles_Filter_Excludes_Files_When_Bundling_Disabled()
    {
        var env = new MockHostEnvironment();
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
            new LayoutCssFileAttribute("~/b.css"),
        ], scripts, env, bundlingEnabled: false));

        var result = html.LayoutCssFiles(path => path != "~/a.css").Value;

        Assert.DoesNotContain("href=\"~/a.css\"", result);
        Assert.Contains("href=\"~/b.css\"", result);
    }

    [Fact]
    public void LayoutCssFiles_Filter_Excluding_All_Returns_Empty()
    {
        var env = new MockHostEnvironment();
        env.AddWebFile("a.css", "A_MARK{}");
        var scripts = new MockDynamicScriptManager();
        var html = MockHtmlHelper.Create(CreateServices([
            new LayoutCssFileAttribute("~/a.css"),
        ], scripts, env));

        Assert.Equal("", html.LayoutCssFiles(_ => false).Value);
        Assert.Empty(scripts.Registered);
    }
}
