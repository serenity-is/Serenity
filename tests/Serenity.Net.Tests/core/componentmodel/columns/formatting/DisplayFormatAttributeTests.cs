namespace Serenity.ComponentModel;

public class DisplayFormatAttributeTests
{
    [Fact]
    public void Value_CanBe_Set()
    {
        var attribute = new DisplayFormatAttribute("expectedValue");
        Assert.Equal("expectedValue", attribute.Value);
    }
}