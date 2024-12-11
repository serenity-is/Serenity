namespace Serenity.ComponentModel;

public class PlaceholderAttributeTests
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new PlaceholderAttribute("value");
        Assert.Equal("value", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new PlaceholderAttribute(null);
        Assert.Null(attribute.Value);
    }
}
