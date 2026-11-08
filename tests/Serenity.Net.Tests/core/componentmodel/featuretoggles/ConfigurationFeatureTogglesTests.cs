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

    [Fact]
    public void IsEnabled_ReturnsTrue_ForUnknownFeature_WhenNoKnownFeaturesPassed()
    {
        // no known feature list, so we cannot tell if the feature is declared, and the previous
        // behavior of enabling unknown features is preserved
        var toggles = new ConfigurationFeatureToggles(NewConfig());
        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Empty(toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_ForUnknownFeature_WhenKnownFeaturesPassedAndDisableUnknown()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            disableUnknown: true);

        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.False(toggles.IsEnabled("UnknownFeature"));
        Assert.Equal(["UnknownFeature"], toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForUnknownFeature_WhenConfigured()
    {
        // an explicitly configured feature key is declared by the operator, so it is known
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:UnknownFeature", "true")),
            knownFeatures: ["Feature1"]);

        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Empty(toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForUnknownFeature_WhenGlobalDefaultEnabled()
    {
        // a global default means the operator handles the feature keys generically
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:*", "true")),
            knownFeatures: ["Feature1"]);

        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Empty(toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_ForUnknownFeature_WhenGlobalDefaultDisabled()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:*", "false")),
            knownFeatures: ["Feature1"]);

        Assert.False(toggles.IsEnabled("UnknownFeature"));
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForFeatureInDependencyMap_WhenKnownFeaturesPassed()
    {
        // features that are only in the dependency map or disabled by default are known too
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [],
            ["Feature2"] = [new RequiresFeatureAttribute("Feature1")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(),
            disableByDefault: ["Feature3"], dependencyMap: dependencyMap, knownFeatures: ["Feature1"],
            disableUnknown: true);

        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature2"));
        Assert.False(toggles.IsEnabled("Feature3")); // known, but disabled by default
        Assert.False(toggles.IsEnabled("UnknownFeature"));
    }

    [Fact]
    public void IsEnabled_MatchesKnownFeaturesCaseInsensitively()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            disableUnknown: true);

        Assert.True(toggles.IsEnabled("feature1"));
        Assert.False(toggles.IsEnabled("unknownfeature"));
    }

    [Fact]
    public void IsEnabled_MatchesDisabledByDefaultCaseInsensitively()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), disableByDefault: ["Feature1"],
            knownFeatures: ["Feature1"]);

        Assert.False(toggles.IsEnabled("feature1"));
        Assert.False(toggles.IsEnabled("FEATURE1"));
    }

    [Fact]
    public void IsEnabled_MatchesDependencyMapCaseInsensitively()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:feature2", "false")),
            dependencyMap: dependencyMap, knownFeatures: ["Feature1", "Feature2"]);

        Assert.False(toggles.IsEnabled("feature1"));
        Assert.False(toggles.IsEnabled("FEATURE1"));
    }

    [Fact]
    public void IsEnabled_ToleratesDependencyMapKeysWithDifferentCaseAndNullValues()
    {
        // such entries would otherwise throw while initializing the toggles
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("Feature2")],
            ["FEATURE1"] = null,
            ["Feature3"] = null
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(),
            dependencyMap: dependencyMap, knownFeatures: ["Feature1", "Feature2", "Feature3"]);

        Assert.True(toggles.IsEnabled("Feature1"));
        Assert.True(toggles.IsEnabled("Feature3"));
    }

    [Fact]
    public void IsEnabled_InvokesOnUnknownFeatureOncePerKey()
    {
        var unknownKeys = new List<string>();
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            disableUnknown: true, onUnknownFeature: unknownKeys.Add);

        Assert.False(toggles.IsEnabled("UnknownFeature"));
        Assert.False(toggles.IsEnabled("unknownfeature"));
        Assert.Equal(["UnknownFeature"], unknownKeys);
    }

    [Fact]
    public void IsEnabled_IgnoresUnparsableGlobalDefault_WhenCheckingUnknownFeatures()
    {
        // an unparsable global default is not an opt out, so unknown features stay disabled
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:*", "yes")),
            knownFeatures: ["Feature1"], disableUnknown: true);

        Assert.False(toggles.IsEnabled("UnknownFeature"));
    }

    [Fact]
    public void IsEnabled_TreatsConfiguredFeatureAsKnown_EvenIfValueCannotBeParsed()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(("FeatureToggles:UnknownFeature", "yes")),
            knownFeatures: ["Feature1"]);

        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Empty(toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_IgnoresSelfDependencyCaseInsensitively()
    {
        var dependencyMap = new Dictionary<string, List<RequiresFeatureAttribute>>
        {
            ["Feature1"] = [new RequiresFeatureAttribute("feature1")]
        };
        var toggles = new ConfigurationFeatureToggles(NewConfig(), dependencyMap: dependencyMap,
            knownFeatures: ["Feature1"]);

        Assert.True(toggles.IsEnabled("Feature1"));
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForUnknownFeature_ByDefault()
    {
        // unknown features are only reported by default, e.g. to log them before opting in
        // to disabling them in an existing installation
        var unknownKeys = new List<string>();
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            onUnknownFeature: unknownKeys.Add);

        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Equal(["UnknownFeature"], toggles.UnknownFeatures);
        Assert.Equal(["UnknownFeature"], unknownKeys);
    }

    [Fact]
    public void IsEnabled_ReturnsTrue_ForUnknownFeature_WhenDisableUnknownIsFalse()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            disableUnknown: false);

        Assert.True(toggles.IsEnabled("UnknownFeature"));
        Assert.Equal(["UnknownFeature"], toggles.UnknownFeatures);
    }

    [Fact]
    public void IsEnabled_IgnoresExceptionsFromOnUnknownFeature()
    {
        var toggles = new ConfigurationFeatureToggles(NewConfig(), knownFeatures: ["Feature1"],
            disableUnknown: true, onUnknownFeature: _ => throw new InvalidOperationException());

        Assert.False(toggles.IsEnabled("UnknownFeature"));
        Assert.Equal(["UnknownFeature"], toggles.UnknownFeatures);
    }
}