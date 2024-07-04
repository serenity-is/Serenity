namespace Serenity.Tests.ComponentModel;

public class FormatterTypeAttributeTests
{
    [Fact]
    public void FormatterType_CanBePassed_ViaConstructor()
    {
        var attribute = new FormatterTypeAttribute("SomeFormatter");
        Assert.Equal("SomeFormatter", attribute.FormatterType);
    }
}