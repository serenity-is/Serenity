namespace Serenity.ComponentModel;

public class TabAttributeTests
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new TabAttribute("value");
        Assert.Equal("value", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new TabAttribute(null);
        Assert.Null(attribute.Value);
    }
}

