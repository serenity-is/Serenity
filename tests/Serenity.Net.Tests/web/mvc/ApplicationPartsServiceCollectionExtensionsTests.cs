using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

[assembly: TypeSourceAssembly]

namespace Serenity.Extensions.DependencyInjection;

public class ApplicationPartsServiceCollectionExtensionsTests
{
    private static IFeatureToggles CreateFeatureToggles(Dictionary<string,  string> config = null)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection((config ?? []).ToDictionary(x => "FeatureToggles:" + x.Key,
                x => x.Value))
            .Build();
        var partManager = new ApplicationPartManager();
        partManager.ApplicationParts.Add(new AssemblyPart(typeof(ApplicationPartsServiceCollectionExtensionsTests).Assembly));
        services.AddApplicationPartsFeatureToggles(configuration, partManager);
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
        });
        Assert.False(featureToggles.IsEnabled(WithIndividualDependencies.DependesOnIndividualDisabled1));
        Assert.True(featureToggles.IsEnabled(WithIndividualDependencies.DependsOnIndividualEnabled1));
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
