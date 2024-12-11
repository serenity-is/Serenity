namespace Serenity.ComponentModel;

public class EditorCssClassAttributeTests
{
    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new EditorCssClassAttribute("sometext");
        Assert.Equal("sometext", attribute.Value);
    }

    [Fact]
    public void Value_Property_NullCssClass_ThrowsArgumentNullException()
    {
        string cssClass = null;
        Assert.Throws<ArgumentNullException>(() => new EditorCssClassAttribute(cssClass));
    }
}
