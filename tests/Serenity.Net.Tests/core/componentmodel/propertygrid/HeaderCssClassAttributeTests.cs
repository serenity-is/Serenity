namespace Serenity.ComponentModel;

public class HeaderCssClassAttributeTests
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new HeaderCssClassAttribute("value");
        Assert.Equal("value", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new HeaderCssClassAttribute(null);
        Assert.Null(attribute.Value);
    }
}

