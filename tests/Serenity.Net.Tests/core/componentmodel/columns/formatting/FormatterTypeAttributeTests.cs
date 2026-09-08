namespace Serenity.ComponentModel;

public class FormatterTypeAttributeTests
{
    [Fact]
    public void FormatterType_CanBePassed_ViaConstructor()
    {
        var attribute = new FormatterTypeAttribute("SomeFormatter");
        Assert.Equal("SomeFormatter", attribute.FormatterType);
    }

    [Fact]
    public void SetParams_DoesNothing()
    {
        var attribute = new FormatterTypeAttribute("SomeFormatter");
        var dict = new Dictionary<string, object?>();
        attribute.SetParams(dict);
        Assert.Empty(dict);
    }
}