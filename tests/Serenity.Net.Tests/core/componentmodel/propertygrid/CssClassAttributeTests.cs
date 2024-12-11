namespace Serenity.ComponentModel;

public class CssClassAttributeTests
{
    [Fact]
    public void CssClass_CanBePassed_AsString()
    {
        var attribute = new CssClassAttribute("cssClass");
        Assert.Equal("cssClass", attribute.CssClass);
    }

    [Fact]
    public void CssClass_CanBePassed_AsNull()
    {
        var attribute = new CssClassAttribute(null);
        Assert.Null(attribute.CssClass);
    }
}

