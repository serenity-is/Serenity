namespace Serenity.ComponentModel;

public class ResizableAttributeTests
{
    [Fact]
    public void Value_ShouldBeTrue_ByDefault()
    {
        var attribute = new ResizableAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsFalse()
    {
        var attribute = new ResizableAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsTrue()
    {
        var attribute = new ResizableAttribute(true);
        Assert.True(attribute.Value);
    }
}

