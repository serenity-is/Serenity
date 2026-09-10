namespace Serenity.ComponentModel;

public class RequiresFeatureAttributeTests
{
    private enum TestFeature
    {
        Feature1,
        Feature2
    }

    [Fact]
    public void Ctor_WithStringFeatures_SetsFeatures()
    {
        var attr = new RequiresFeatureAttribute("A", "B");
        Assert.Equal(["A", "B"], attr.Features);
    }

    [Fact]
    public void Ctor_WithNullStringFeatures_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RequiresFeatureAttribute((string[])null));
    }

    [Fact]
    public void Ctor_WithEmptyStringFeatures_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RequiresFeatureAttribute(Array.Empty<string>()));
    }

    [Fact]
    public void Ctor_WithEnumFeatures_SetsFeatureKeys()
    {
        var attr = new RequiresFeatureAttribute(TestFeature.Feature1, TestFeature.Feature2);
        Assert.Equal(["Feature1", "Feature2"], attr.Features);
    }

    [Fact]
    public void Ctor_WithNullEnumFeatures_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RequiresFeatureAttribute((object[])null));
    }

    [Fact]
    public void RequireAny_DefaultsToFalse()
    {
        var attr = new RequiresFeatureAttribute("A");
        Assert.False(attr.RequireAny);
    }

    [Fact]
    public void RequireAny_CanBeSet()
    {
        var attr = new RequiresFeatureAttribute("A") { RequireAny = true };
        Assert.True(attr.RequireAny);
    }
}