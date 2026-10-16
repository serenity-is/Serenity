using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class LayoutScriptFileProviderTests
{
    private static LayoutScriptFileProvider Create(Attribute[] attributes,
        Func<string, bool>? isEnabled = null, Action<LayoutScriptFileOptions>? configure = null)
    {
        var services = new ServiceCollection();
        if (isEnabled != null)
            services.AddSingleton<IFeatureToggles>(new MockFeatureToggles { IsEnabledCallback = isEnabled });

        var options = new LayoutScriptFileOptions();
        configure?.Invoke(options);

        return new LayoutScriptFileProvider(new MockTypeSource(attributes),
            services.BuildServiceProvider(), Options.Create(options));
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var options = Options.Create(new LayoutScriptFileOptions());

        Assert.Throws<ArgumentNullException>(() =>
            new LayoutScriptFileProvider(null!, services, options));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutScriptFileProvider(new MockTypeSource(Array.Empty<Attribute>()), null!, options));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutScriptFileProvider(new MockTypeSource(Array.Empty<Attribute>()), services, null!));
    }

    [Fact]
    public void GetFiles_Returns_Ordered_Deduplicated_AppRelative_Paths()
    {
        var provider = Create([
            new LayoutScriptFileAttribute("~/b.js") { Order = 2 },
            new LayoutScriptFileAttribute("~/a.js") { Order = 1 },
            new LayoutScriptFileAttribute("~/a.js") { Order = 3 },
            new LayoutScriptFileAttribute("/rooted.js"),
            new LayoutScriptFileAttribute("relative.js"),
            new LayoutScriptFileAttribute("  "),
        ]);

        Assert.Equal(["~/a.js", "~/b.js"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Filters_By_Required_Features()
    {
        var provider = Create([
            new LayoutScriptFileAttribute("~/a.js") { RequireFeatures = ["A"] },
            new LayoutScriptFileAttribute("~/b.js") { RequireFeatures = ["A", "B"] },
            new LayoutScriptFileAttribute("~/c.js") { RequireFeatures = ["A", "B"], RequireAnyFeature = true },
        ], isEnabled: feature => feature == "A");

        Assert.Equal(["~/a.js", "~/c.js"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Includes_Files_With_Required_Features_When_No_Toggles_Registered()
    {
        var provider = Create([
            new LayoutScriptFileAttribute("~/a.js") { RequireFeatures = ["A"] }
        ]);

        Assert.Equal(["~/a.js"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Applies_Options_Filter()
    {
        var provider = Create([
            new LayoutScriptFileAttribute("~/a.js"),
            new LayoutScriptFileAttribute("~/b.js"),
        ], configure: options => options.Filter = attr => attr.Path != "~/a.js");

        Assert.Equal(["~/b.js"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Returns_Empty_When_No_Attributes()
    {
        var provider = Create([]);

        Assert.Empty(provider.GetFiles());
    }
}
