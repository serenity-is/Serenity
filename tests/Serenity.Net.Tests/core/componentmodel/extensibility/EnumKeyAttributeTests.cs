namespace Serenity.ComponentModel;

public class EnumKeyAttributeTests()
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new EnumKeyAttribute("value");
        Assert.Equal("value", attribute.Value);
    }
}