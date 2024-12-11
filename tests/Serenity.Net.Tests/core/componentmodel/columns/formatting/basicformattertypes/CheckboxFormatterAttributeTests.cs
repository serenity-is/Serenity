namespace Serenity.ComponentModel;

public class CheckboxFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Checkbox()
    {
        var attribute = new CheckboxFormatterAttribute();
        Assert.Equal("Checkbox", attribute.FormatterType);
    }
}