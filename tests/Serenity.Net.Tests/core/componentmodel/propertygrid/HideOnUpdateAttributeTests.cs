namespace Serenity.ComponentModel;

public class HideOnUpdateAttributeTests
{
    [Fact]
    public void Value_ShouldBeTrue_ByDefault()
    {
        var attribute = new HideOnUpdateAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsFalse()
    {
        var attribute = new HideOnUpdateAttribute(false);
        Assert.False(attribute.Value);
    }
   
    [Fact]
    public void Value_CanBePassed_AsTrue()
    {
        var attribute = new HideOnUpdateAttribute(true);
        Assert.True(attribute.Value);
    }
}

