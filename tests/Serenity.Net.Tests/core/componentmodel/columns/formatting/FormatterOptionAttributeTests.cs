namespace Serenity.ComponentModel;

public class FormatterOptionAttributeTests
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new FormatterOptionAttribute("someKey", "someStr");
        Assert.Equal("someKey", attribute.Key);
        Assert.Equal("someStr", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new FormatterOptionAttribute("key", null);
        Assert.Equal("key",attribute.Key);
        Assert.Null(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsBoolean()
    {
        var attribute = new FormatterOptionAttribute("key", true);
        Assert.Equal("key", attribute.Key);
        Assert.Equal(true, attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsInt()
    {
        var attribute = new FormatterOptionAttribute("key", 123);
        Assert.Equal("key", attribute.Key);
        Assert.Equal(123, attribute.Value);
    }
}