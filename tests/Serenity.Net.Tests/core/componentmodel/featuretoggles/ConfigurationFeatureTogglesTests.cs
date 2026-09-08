using Microsoft.Extensions.Configuration;

namespace Serenity;

public class ConfigurationFeatureTogglesTests
{
    private static IConfiguration NewConfig(params (string key, string value)[] values)
    {
        var builder = new ConfigurationBuilder();
        var dict = new Dictionary<string, string?>();
        foreach (var (key, value) in values)
            dict[key] = value;
        builder.AddInMemoryCollection(dict);
        return builder.Build();
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenNotConfigured()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig());
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenConfiguredFalse()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:Feature1", "false")));
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenConfiguredTrue()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:Feature1", "true")));
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenDisabledByDefault()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), disableByDefault: ["Feature1"]);
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenAllDisabledByDefault()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), disableByDefault: ["*"]);
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenWildcardDisabled()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:*", "false")));
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenDependencyNotEnabled()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:Feature2", "false")),
            dependencyMap: dependencyMap);
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenDependencyEnabled()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:Feature2", "true")),
            dependencyMap: dependencyMap);
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_WhenRequireAnyDependencyNotEnabled()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2", "Feature3") { RequireAny = true }]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(
            ("FeatureToggles:Feature2", "false"),
            ("FeatureToggles:Feature3", "false")), dependencyMap: dependencyMap);
        Assert.False(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_WhenRequireAnyDependencyEnabled()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2", "Feature3") { RequireAny = true }]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(
            ("FeatureToggles:Feature2", "false"),
            ("FeatureToggles:Feature3", "true")), dependencyMap: dependencyMap);
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_IgnoresSelfDependency()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature1")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(), dependencyMap: dependencyMap);
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ThrowsInvalidOperationException_ForCircularDependencies()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2")],
            ["Feature2"] = [new RequiresFeatureAttribute("Feature1")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(), dependencyMap: dependencyMap);
        Assert.Throws<InvalidOperationException>(() => toggles.IsEnabled("Feature1"));
    }
}