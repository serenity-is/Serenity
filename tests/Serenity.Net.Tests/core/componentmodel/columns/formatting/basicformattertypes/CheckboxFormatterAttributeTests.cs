namespace Serenity.ComponentModel;

public class CheckboxFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Checkbox()
    {
        var attribute = new CheckboxFormatterAttribute();
        Assert.Equal("Checkbox", attribute.FormatterType);
    }

    [Fact]
    public void FalseIcon_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            FalseIcon = "icon-false"
        };
        Assert.Equal("icon-false", attr.FalseIcon);
    }

    [Fact]
    public void FalseText_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            FalseText = "No"
        };
        Assert.Equal("No", attr.FalseText);
    }

    [Fact]
    public void NullIcon_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            NullIcon = "icon-null"
        };
        Assert.Equal("icon-null", attr.NullIcon);
    }

    [Fact]
    public void TrueIcon_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            TrueIcon = "icon-true"
        };
        Assert.Equal("icon-true", attr.TrueIcon);
    }

    [Fact]
    public void TrueText_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            TrueText = "Yes"
        };
        Assert.Equal("Yes", attr.TrueText);
    }

    [Fact]
    public void ShowText_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            ShowText = true
        };
        Assert.True(attr.ShowText);
    }

    [Fact]
    public void ShowHint_GetSet_Works()
    {
        var attr = new CheckboxFormatterAttribute
        {
            ShowHint = false
        };
        Assert.False(attr.ShowHint);
    }
}