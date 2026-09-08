namespace Serenity;

public class FeatureTogglesExtensionsTests
{
    private enum TestFeature
    {
        Feature1,
        Feature2
    }

    private class MockFeatureToggles : IFeatureToggles
    {
        private readonly HashSet<string> enabled;

        public MockFeatureToggles(params string[] enabled)
        {
            this.enabled = new HashSet<string>(enabled, StringComparer.Ordinal);
        }

        public bool IsEnabled(string feature)
        {
            return enabled.Contains(feature);
        }
    }

    [Fact]
    public void IsEnabled_WithEnum_DelegatesByName()
    {
        var toggles = new MockFeatureToggles("Feature1");
        Assert.True(toggles.IsEnabled(TestFeature.Feature1));
        Assert.False(toggles.IsEnabled(TestFeature.Feature2));
    }

    [Fact]
    public void IsEnabled_WithEnumerable_RequiresAll()
    {
        var toggles = new MockFeatureToggles("A", "B");
        Assert.True(toggles.IsEnabled(new[] { "A", "B" }));
        Assert.False(toggles.IsEnabled(new[] { "A", "C" }));
    }

    [Fact]
    public void IsEnabled_WithEnumerable_ReturnsFalse_WhenEmpty()
    {
        var toggles = new MockFeatureToggles("A");
        Assert.False(toggles.IsEnabled(Array.Empty<string>()));
        Assert.False(toggles.IsEnabled((IEnumerable<string>)null));
    }

    [Fact]
    public void IsEnabled_WithRequireAny_RequiresOne()
    {
        var toggles = new MockFeatureToggles("A");
        Assert.True(toggles.IsEnabled(new[] { "A", "B" }, requireAny: true));
        Assert.False(toggles.IsEnabled(new[] { "B", "C" }, requireAny: true));
        Assert.False(toggles.IsEnabled(new[] { "A", "B" }, requireAny: false));
    }

    [Fact]
    public void ToFeatureKey_ReturnsString_ForString()
    {
        Assert.Equal("A", FeatureTogglesExtensions.ToFeatureKey("A"));
    }

    [Fact]
    public void ToFeatureKey_ThrowsArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => FeatureTogglesExtensions.ToFeatureKey(null));
    }

    [Fact]
    public void ToFeatureKey_ThrowsArgumentException_ForEmptyString()
    {
        Assert.Throws<ArgumentException>(() => FeatureTogglesExtensions.ToFeatureKey(""));
    }

    [Fact]
    public void ToFeatureKey_ReturnsEnumName_ForEnum()
    {
        Assert.Equal("Feature1", FeatureTogglesExtensions.ToFeatureKey(TestFeature.Feature1));
    }

    [Fact]
    public void ToFeatureKey_ThrowsArgumentException_ForNonEnumNonString()
    {
        Assert.Throws<ArgumentException>(() => FeatureTogglesExtensions.ToFeatureKey(42));
    }
}