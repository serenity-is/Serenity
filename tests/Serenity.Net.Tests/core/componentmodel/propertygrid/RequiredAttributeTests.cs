namespace Serenity.ComponentModel;

public class RequiredAttributeTests
{
    [Fact]
    public void IsRequired_ShouldBeTrue_ByDefault()
    {
        var attribute = new RequiredAttribute();
        Assert.True(attribute.IsRequired);
    }

    [Fact]
    public void RequiredAttribute_CanBePassed_AsFalse()
    {
        var attribute = new RequiredAttribute(false);
        Assert.False(attribute.IsRequired);
    }

    [Fact]
    public void RequiredAttribute_CanBePassed_AsTrue()
    {
        var attribute = new RequiredAttribute(true);
        Assert.True(attribute.IsRequired);
    }
}

