namespace Serenity.ComponentModel;

public class FormCssClassAttributeTests
{
    [Fact]
    public void Value_CanBePassed_cssClass()
    {
        var attribute = new FormCssClassAttribute("cssClass");
        Assert.Equal("cssClass", attribute.Value);
    }

    [Fact]
    public void UntilNext_CanBeSet_ToTrue()
    {
        var attribute = new FormCssClassAttribute("cssClass")
        {
            UntilNext = true
        };
        Assert.True(attribute.UntilNext);
    }

    [Fact]
    public void UntilNext_CanBeSet_ToFalse()
    {
        var attribute = new FormCssClassAttribute("cssClass")
        {
            UntilNext = false
        };
        Assert.False(attribute.UntilNext);
    }
}

