namespace Serenity.ComponentModel;

public class HideOnInsertAttributeTests
{
    [Fact]
    public void Value_ShouldBeTrue_ByDefault()
    {
        var attribute = new HideOnInsertAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsFalse()
    {
        var attribute = new HideOnInsertAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsTrue()
    {
        var attribute = new HideOnInsertAttribute(true);
        Assert.True(attribute.Value);
    }
}

