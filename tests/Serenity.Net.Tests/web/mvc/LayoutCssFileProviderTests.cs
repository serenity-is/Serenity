using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class LayoutCssFileProviderTests
{
    private static LayoutCssFileProvider Create(Attribute[] attributes,
        Func<string, bool>? isEnabled = null, Action<LayoutCssFileOptions>? configure = null)
    {
        var services = new ServiceCollection();
        if (isEnabled != null)
            services.AddSingleton<IFeatureToggles>(new MockFeatureToggles { IsEnabledCallback = isEnabled });

        var options = new LayoutCssFileOptions();
        configure?.Invoke(options);

        return new LayoutCssFileProvider(new MockTypeSource(attributes),
            services.BuildServiceProvider(), Options.Create(options));
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var options = Options.Create(new LayoutCssFileOptions());

        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssFileProvider(null!, services, options));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssFileProvider(new MockTypeSource(Array.Empty<Attribute>()), null!, options));
        Assert.Throws<ArgumentNullException>(() =>
            new LayoutCssFileProvider(new MockTypeSource(Array.Empty<Attribute>()), services, null!));
    }

    [Fact]
    public void GetFiles_Returns_Ordered_Deduplicated_AppRelative_Paths()
    {
        var provider = Create([
            new LayoutCssFileAttribute("~/b.css") { Order = 2 },
            new LayoutCssFileAttribute("~/a.css") { Order = 1 },
            new LayoutCssFileAttribute("~/a.css") { Order = 3 },
            new LayoutCssFileAttribute("/rooted.css"),
            new LayoutCssFileAttribute("relative.css"),
            new LayoutCssFileAttribute("  "),
        ]);

        Assert.Equal(["~/a.css", "~/b.css"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Filters_By_Required_Features()
    {
        var provider = Create([
            new LayoutCssFileAttribute("~/a.css") { RequireFeatures = ["A"] },
            new LayoutCssFileAttribute("~/b.css") { RequireFeatures = ["A", "B"] },
            new LayoutCssFileAttribute("~/c.css") { RequireFeatures = ["A", "B"], RequireAnyFeature = true },
        ], isEnabled: feature => feature == "A");

        Assert.Equal(["~/a.css", "~/c.css"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Includes_Files_With_Required_Features_When_No_Toggles_Registered()
    {
        var provider = Create([
            new LayoutCssFileAttribute("~/a.css") { RequireFeatures = ["A"] }
        ]);

        Assert.Equal(["~/a.css"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Applies_Options_Filter()
    {
        var provider = Create([
            new LayoutCssFileAttribute("~/a.css"),
            new LayoutCssFileAttribute("~/b.css"),
        ], configure: options => options.Filter = attr => attr.Path != "~/a.css");

        Assert.Equal(["~/b.css"], provider.GetFiles());
    }

    [Fact]
    public void GetFiles_Returns_Empty_When_No_Attributes()
    {
        var provider = Create([]);

        Assert.Empty(provider.GetFiles());
    }
}
