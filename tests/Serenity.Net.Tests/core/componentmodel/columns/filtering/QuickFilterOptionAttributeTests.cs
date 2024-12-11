namespace Serenity.ComponentModel;

public class QuickFilterOptionAttributeTests
{

    [Fact]
    public void Value_And_Key_CanBeSet_Via_Constructor_ToString()
    {
        var attribute = new QuickFilterOptionAttribute("key", "value");
        Assert.Equal("key", attribute.Key);
        Assert.Equal("value", attribute.Value);
    }

    [Fact]
    public void Value_CanBeNull()
    {
        var attribute = new QuickFilterOptionAttribute("some", null);
        Assert.Equal("some", attribute.Key);
        Assert.Null(attribute.Value);
    }

    [Fact]
    public void Value_CanBe_Boolean()
    {
        var attribute = new QuickFilterOptionAttribute("key", true);
        Assert.Equal("key", attribute.Key);
        Assert.Equal(true, attribute.Value);
    }

    [Fact]
    public void Value_CanBe_Int32()
    {
        var attribute = new QuickFilterOptionAttribute("key", 123);
        Assert.Equal("key", attribute.Key);
        Assert.Equal(123, attribute.Value);
    }
}