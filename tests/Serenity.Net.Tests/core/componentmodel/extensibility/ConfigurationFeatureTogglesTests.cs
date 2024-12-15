using Microsoft.Extensions.Configuration;

namespace Serenity.ComponentModel;

public class ConfigurationFeatureTogglesTests
{
    [Fact]
    public void AllFeaturesAreEnabledByDefault()
    {
        var configuration = new ConfigurationBuilder().Build();
        var toggles = new ConfigurationFeatureToggles(configuration);
        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature2"));
        Assert.True(toggles.IsEnabled("Feature3"));
    }

    [Fact]
    public void FeatureIsDisabled_BySettingToFalse()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["FeatureToggles:Feature1"] = "false"
            })
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration);
        Assert.False(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature2"));
        Assert.True(toggles.IsEnabled("Feature3"));
    }

    [Fact]
    public void FeatureStaysEnabled_BySettingToTrue()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["FeatureToggles:Feature1"] = "true"
            })
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration);
        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature2"));
        Assert.True(toggles.IsEnabled("Feature3"));
    }

    [Fact]
    public void ConfigurationOverrides_DisabledByDefault()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["FeatureToggles:Feature1"] = "true"
            })
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration,
            disableByDefault: ["Feature1"]);
        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void DisableByDefault_MakesFeatureDisabledUnlessConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["FeatureToggles:Feature1"] = "true"
            })
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration,
            disableByDefault: ["Feature1", "Feature2"]);
        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.False(toggles.IsEnabled("Feature2"));
    }

    [Fact]
    public void ConfigurationValues_OtherThan_TrueFalse_AreIgnored()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["FeatureToggles:Feature1"] = "no",
                ["FeatureToggles:Feature2"] = null,
                ["FeatureToggles:Feature3"] = "yes",
                ["FeatureToggles:Feature4"] = "disabled",
            })
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration,
            disableByDefault: ["Feature3"]);
        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature2"));
        Assert.False(toggles.IsEnabled("Feature3"));
        Assert.True(toggles.IsEnabled("Feature4"));
    }

    [Fact]
    public void CircularDependencies_ThrowException()
    {
        var configuration = new ConfigurationBuilder()
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration,
            dependencyMap: new Dictionary<string, List<RequiresFeatureAttribute>>
            {
                ["Feature1"] = [new("Feature2")],
                ["Feature2"] = [new("Feature3")],
                ["Feature3"] = [new("Feature1")]
            });
        Assert.Throws<InvalidOperationException>(() => toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void AllDependencies_MustBeEnabled_ForAFeatureToBeEnabled()
    {
        var configuration = new ConfigurationBuilder()
            .Build();
        var toggles = new ConfigurationFeatureToggles(configuration,
            disableByDefault: ["DisabledFeature"],
            dependencyMap: new()
            {
                ["Feature1"] = [new("DisabledFeature")],
                ["Feature2"] = [new("EnabledFeature", "DisabledFeature")],
                ["Feature3"] = [new("EnabledFeature"), new("DisabledFeature")],
                ["Feature4"] = [new("EnabledFeature"), new("EnabledFeature")],
                ["Feature5"] = [new("EnabledFeature", "DisabledFeature") { RequireAny = true }],
            });

        Assert.True(toggles.IsEnabled("EnabledFeature"));
        Assert.False(toggles.IsEnabled("DisabledFeature"));

        Assert.False(toggles.IsEnabled("Feature1"));
        Assert.False(toggles.IsEnabled("Feature2"));
        Assert.False(toggles.IsEnabled("Feature3"));
        Assert.True(toggles.IsEnabled("Feature4"));
        Assert.True(toggles.IsEnabled("Feature5"));
    }
}
