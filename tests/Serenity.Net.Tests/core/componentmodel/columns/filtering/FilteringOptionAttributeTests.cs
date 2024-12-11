namespace Serenity.ComponentModel;

public class FilteringOptionAttributeTests
{
    [Fact]
    public void Key_And_Value_CanBePassed_AsString()
    {
        var attribute = new FilteringOptionAttribute("testKey", "TestValue");

        Assert.Equal("testKey", attribute.Key);
        Assert.Equal("TestValue", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new FilteringOptionAttribute("some", null);

        Assert.Equal("some", attribute.Key);
        Assert.Null(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsBoolean()
    {
        var attribute = new FilteringOptionAttribute("testKey", true);

        Assert.Equal("testKey", attribute.Key);
        Assert.Equal(true, attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsInt32()
    {
        var attribute = new FilteringOptionAttribute("testKey", 123);

        Assert.Equal("testKey", attribute.Key);
        Assert.Equal(123, attribute.Value);
    }
}