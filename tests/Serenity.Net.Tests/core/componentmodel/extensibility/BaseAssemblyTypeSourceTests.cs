using Microsoft.Extensions.Primitives;

namespace Serenity.Abstractions;

public class BaseAssemblyTypeSourceTests
{
    private class TestTypeSource : BaseAssemblyTypeSource
    {
        private readonly Assembly[] assemblies;

        public TestTypeSource(IFeatureToggles? featureToggles = null, params Assembly[] assemblies)
            : base(featureToggles)
        {
            this.assemblies = assemblies;
        }

        public override IEnumerable<Assembly> GetAssemblies() => assemblies;
    }

    private class MockFeatureToggles : IFeatureToggles
    {
        private readonly HashSet<string> enabled;

        public MockFeatureToggles(params string[] enabled)
        {
            this.enabled = new HashSet<string>(enabled, StringComparer.Ordinal);
        }

        public bool IsEnabled(string feature) => enabled.Contains(feature);
    }

    [Fact]
    public void GetChangeToken_ReturnsToken()
    {
        var source = new TestTypeSource(assemblies: []);
        var token = source.GetChangeToken();
        Assert.NotNull(token);
        Assert.False(token.HasChanged);
    }

    [Fact]
    public void NotifyChanged_TriggersChangeToken()
    {
        var source = new TestTypeSource(assemblies: []);
        var token = source.GetChangeToken();
        Assert.False(token.HasChanged);

        source.NotifyChanged();
        Assert.True(token.HasChanged);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var source = new TestTypeSource(assemblies: []);
        source.Dispose();
    }

    [Fact]
    public void GetAssemblyAttributes_ReturnsAttributes()
    {
        var source = new TestTypeSource(assemblies: [typeof(BaseAssemblyTypeSourceTests).Assembly]);
        var attributes = source.GetAssemblyAttributes(typeof(AssemblyTitleAttribute));
        Assert.All(attributes, attr => Assert.IsType<AssemblyTitleAttribute>(attr));
    }

    [Fact]
    public void GetTypes_ReturnsTypes()
    {
        var source = new TestTypeSource(assemblies: [typeof(BaseAssemblyTypeSourceTests).Assembly]);
        Assert.Contains(typeof(BaseAssemblyTypeSourceTests), source.GetTypes());
    }

    [Fact]
    public void GetTypesWithInterface_ReturnsMatchingTypes()
    {
        var source = new TestTypeSource(assemblies: [typeof(BaseAssemblyTypeSourceTests).Assembly]);
        Assert.Contains(typeof(TestTypeSource), source.GetTypesWithInterface(typeof(ITypeSource)));
    }

    [Fact]
    public void GetTypesWithAttribute_ReturnsMatchingTypes()
    {
        var source = new TestTypeSource(assemblies: [typeof(BaseAssemblyTypeSourceTests).Assembly]);
        Assert.Contains(typeof(FeatureEnabledType), source.GetTypesWithAttribute(typeof(RequiresFeatureAttribute)));
    }

    [RequiresFeature("EnabledFeature")]
    private class FeatureEnabledType
    {
    }

    [RequiresFeature("DisabledFeature")]
    private class FeatureDisabledType
    {
    }

    [Fact]
    public void GetTypes_FiltersByFeatureToggles()
    {
        var source = new TestTypeSource(new MockFeatureToggles("EnabledFeature"),
            assemblies: [typeof(BaseAssemblyTypeSourceTests).Assembly]);

        var types = source.GetTypes().ToList();
        Assert.Contains(typeof(FeatureEnabledType), types);
        Assert.DoesNotContain(typeof(FeatureDisabledType), types);
    }
}