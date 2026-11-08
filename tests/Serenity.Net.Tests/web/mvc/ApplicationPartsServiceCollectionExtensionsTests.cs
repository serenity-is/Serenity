using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

[assembly: TypeSourceAssembly]

namespace Serenity.Extensions.DependencyInjection;

public class ApplicationPartsServiceCollectionExtensionsTests
{
    private static IFeatureToggles CreateFeatureToggles(Dictionary<string, string> config = null,
        bool? disableUnknown = true, IEnumerable<string> knownFeatures = null,
        Action<string> onUnknownFeature = null)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection((config ?? []).ToDictionary(x => "FeatureToggles:" + x.Key,
                x => x.Value))
            .Build();
        var partManager = new ApplicationPartManager();
        partManager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationPartsServiceCollectionExtensionsTests).Assembly));
        if (disableUnknown is bool disable)
            services.AddApplicationPartsFeatureToggles(configuration, partManager,
                disableUnknown: disable, knownFeatures: knownFeatures, onUnknownFeature: onUnknownFeature);
        else // null uses the default of the method
            services.AddApplicationPartsFeatureToggles(configuration, partManager,
                knownFeatures: knownFeatures, onUnknownFeature: onUnknownFeature);
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IFeatureToggles>();
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_ScansFeatureKeysForDefaultValueFalse()
    {
        var featureToggles = CreateFeatureToggles();
        Assert.False(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.DisabledByDefaultFeatureKey1.ToString()));
        Assert.True(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.NotDisabledByDefaultFeatureKey2.ToString()));
        Assert.True(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.EnabledByDefaultFeatureKey3));
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_ScansCommonDependencyOfFeatureKeySets()
    {
        var featureToggles = CreateFeatureToggles(new()
        {
            [nameof(WithCommonDependencyFeatureKeySet.CommonFeature)] = "false"
        });
        Assert.False(featureToggles.IsEnabled(WithCommonDependencyFeatureKeySet.CommonFeature));
        Assert.False(featureToggles.IsEnabled(WithCommonDependencyFeatureKeySet.WithCommonOneDependent.ToString()));
        Assert.False(featureToggles.IsEnabled(WithCommonDependencyFeatureKeySet.WithCommonAnotherOneDependent));
        Assert.True(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.EnabledByDefaultFeatureKey3));
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_ScansIndividualDependencyOfFeatureKeySets()
    {
        var featureToggles = CreateFeatureToggles(new()
        {
            ["IndividualDisabled1"] = "false"
        }, knownFeatures: ["IndividualEnabled1"]);
        Assert.False(featureToggles.IsEnabled(WithIndividualDependencies.DependesOnIndividualDisabled1));
        Assert.True(featureToggles.IsEnabled(WithIndividualDependencies.DependsOnIndividualEnabled1));
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_DisablesFeature_WhenDependencyTargetIsNotDeclared()
    {
        // a dependency reference is not a declaration, so an undeclared target is unknown, and
        // the features depending on it are disabled. Pass it as a known feature to avoid that.
        var unknownKeys = new List<string>();
        var featureToggles = CreateFeatureToggles(onUnknownFeature: unknownKeys.Add);

        Assert.False(featureToggles.IsEnabled(WithUndeclaredDependency.DependsOnUndeclared));
        Assert.Contains("UndeclaredTarget", unknownKeys);
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_ReportsUnknownFeatureOnlyOnce()
    {
        var unknownKeys = new List<string>();
        var featureToggles = CreateFeatureToggles(onUnknownFeature: unknownKeys.Add);

        Assert.False(featureToggles.IsEnabled("UnknownFeatureKey"));
        Assert.False(featureToggles.IsEnabled("unknownfeaturekey"));
        Assert.Equal(["UnknownFeatureKey"], unknownKeys);
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_MergesAdditionalKnownFeatures()
    {
        // a feature key declared neither by a FeatureKeySet enum nor in the configuration can be
        // passed as known, so it is enabled by default while the other unknown keys are disabled
        var featureToggles = CreateFeatureToggles(knownFeatures: ["AdditionalFeatureKey"]);

        Assert.True(featureToggles.IsEnabled("AdditionalFeatureKey"));
        Assert.True(featureToggles.IsEnabled("additionalfeaturekey")); // case insensitive
        Assert.True(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.EnabledByDefaultFeatureKey3));
        Assert.False(featureToggles.IsEnabled("UnknownFeatureKey"));

        var unknown = ((ConfigurationFeatureToggles)featureToggles).UnknownFeatures;
        Assert.Contains("UnknownFeatureKey", unknown);
        Assert.DoesNotContain("AdditionalFeatureKey", unknown);
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_DisablesUnknownFeatureKeys()
    {
        var featureToggles = CreateFeatureToggles();

        // declared by a scanned FeatureKeySet, so it is enabled by default
        Assert.True(featureToggles.IsEnabled(OneDisabledByDefaultFeatureKeySet.EnabledByDefaultFeatureKey3));

        // not declared anywhere, so it is disabled by default
        Assert.False(featureToggles.IsEnabled("UnknownFeatureKey"));
        Assert.Contains("UnknownFeatureKey", ((ConfigurationFeatureToggles)featureToggles).UnknownFeatures);
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_KeepsUnknownFeatureKeysEnabled_ByDefault()
    {
        var unknownKeys = new List<string>();
        var featureToggles = CreateFeatureToggles(disableUnknown: null, onUnknownFeature: unknownKeys.Add);

        Assert.True(featureToggles.IsEnabled("UnknownFeatureKey"));
        Assert.Equal(["UnknownFeatureKey"], unknownKeys);
    }

    [Fact]
    public void AddApplicationPartsFeatureToggles_KeepsUnknownFeatureKeysEnabled_WhenDisableUnknownIsFalse()
    {
        var featureToggles = CreateFeatureToggles(disableUnknown: false);

        Assert.True(featureToggles.IsEnabled("UnknownFeatureKey"));
        Assert.Contains("UnknownFeatureKey", ((ConfigurationFeatureToggles)featureToggles).UnknownFeatures);
    }

    [Fact]
    public void ScanFeatureKeySets_AcceptsNullAsDisableByDefaultAndDependencyMap()
    {
        object[] disableByDefault = null;
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = null;
        ApplicationPartsServiceCollectionExtensions.ScanFeatureKeySets([],
            ref disableByDefault, ref dependencyMap);
        Assert.Empty(disableByDefault);
        Assert.Empty(dependencyMap);
    }

    [Fact]
    public void ScanFeatureKeySets_AcceptsNullAsDisableByDefault()
    {
        object[] disableByDefault = null;
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = [];
        ApplicationPartsServiceCollectionExtensions.ScanFeatureKeySets([],
            ref disableByDefault, ref dependencyMap);
        Assert.Empty(disableByDefault);
        Assert.Empty(dependencyMap);
    }

    [Fact]
    public void ScanFeatureKeySets_AcceptsNullAsDependencyMap()
    {
        object[] disableByDefault = null;
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = null;
        ApplicationPartsServiceCollectionExtensions.ScanFeatureKeySets([],
            ref disableByDefault, ref dependencyMap);
        Assert.Empty(disableByDefault);
        Assert.Empty(dependencyMap);
    }

    [Fact]
    public void ScanFeatureKeySets_AppendsToDisableByDefault()
    {
        object[] disableByDefault = ["SomethingDisabledByDefault1", "SomethingDisabledByDefault2"];
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = null;
        ApplicationPartsServiceCollectionExtensions.ScanFeatureKeySets([GetType().Assembly],
            ref disableByDefault, ref dependencyMap);
        Assert.Contains("SomethingDisabledByDefault1", disableByDefault);
        Assert.Contains("SomethingDisabledByDefault2", disableByDefault);
        Assert.Contains(nameof(OneDisabledByDefaultFeatureKeySet.DisabledByDefaultFeatureKey1), disableByDefault);
    }

    [Fact]
    public void ScanFeatureKeySets_AppendsToDependencyMap()
    {
        object[] disableByDefault = null;
        var deps1 = new RequiresFeatureAttribute("SomeDep1", "SomeDep2");
        var deps2 = new RequiresFeatureAttribute("SomeDep3");
        var deps3a = new RequiresFeatureAttribute("SomeDep4", "SomeDep5") { RequireAny = true };
        var deps3b = new RequiresFeatureAttribute("SomeDep6");
        Dictionary<string, List<RequiresFeatureAttribute>> dependencyMap = new()
        {
            [nameof(WithIndividualDependencies.DependsOnIndividualEnabled1)] = [deps1],
            ["XFeature2"] = [deps2],
            [nameof(WithIndividualDependencies.DependesOnIndividualDisabled1)] = [deps3a, deps3b]
        };
        ApplicationPartsServiceCollectionExtensions.ScanFeatureKeySets([GetType().Assembly],
            ref disableByDefault, ref dependencyMap);

        var newDeps1 = Assert.Contains(nameof(WithIndividualDependencies.DependsOnIndividualEnabled1), dependencyMap);
        Assert.Contains(deps1, newDeps1);
        Assert.Collection(deps1.Features, 
            x => Assert.Equal("SomeDep1", x), 
            x => Assert.Equal("SomeDep2", x));
        Assert.Contains(newDeps1, x => x.Features.Contains("IndividualEnabled1"));

        var newDeps2 = Assert.Contains("XFeature2", dependencyMap);
        Assert.Contains(deps2, newDeps2);
        Assert.Equal("SomeDep3", Assert.Single(deps2.Features));

        var newDeps3 = Assert.Contains(nameof(WithIndividualDependencies.DependesOnIndividualDisabled1), dependencyMap);
        Assert.Contains(deps3a, newDeps3);
        Assert.Contains(deps3b, newDeps3);
        Assert.Collection(deps3a.Features,
            x => Assert.Equal("SomeDep4", x),
            x => Assert.Equal("SomeDep5", x));
        Assert.Equal("SomeDep6", Assert.Single(deps3b.Features));
        Assert.Contains(newDeps3, x => x.Features.Contains("IndividualDisabled1"));
    }
}

[FeatureKeySet]
public enum OneDisabledByDefaultFeatureKeySet
{
    [DefaultValue(false)]
    DisabledByDefaultFeatureKey1,
    NotDisabledByDefaultFeatureKey2,
    EnabledByDefaultFeatureKey3
}

[FeatureKeySet]
[RequiresFeature(CommonFeature)]
public enum WithCommonDependencyFeatureKeySet
{
    CommonFeature,
    WithCommonOneDependent,
    WithCommonAnotherOneDependent
}

[FeatureKeySet]
public enum WithIndividualDependencies
{
    [RequiresFeature("IndividualDisabled1")]
    DependesOnIndividualDisabled1,
    [RequiresFeature("IndividualEnabled1")]
    DependsOnIndividualEnabled1
}

[FeatureKeySet]
public enum WithUndeclaredDependency
{
    [RequiresFeature("UndeclaredTarget")]
    DependsOnUndeclared
}
