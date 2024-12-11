namespace Serenity.ComponentModel;

public class LabelWidthAttributeTests
{
    [Fact]
    public void Value_ShouldIncludePxSuffix()
    {
        var attribute = new LabelWidthAttribute(2);
        Assert.Equal(2 + "px", attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsNull()
    {
        var attribute = new LabelWidthAttribute(null);
        Assert.Null(attribute.Value);
    }

    [Fact]
    public void Value_CanBePassed_AsString()
    {
        var attribute = new LabelWidthAttribute("value");
        Assert.Equal("value", attribute.Value);
    }

    [Fact]
    public void UntilNext_CanBeSet_ToTrue()
    {
        var attribute = new LabelWidthAttribute(1)
        {
            UntilNext = true
        };
        Assert.True(attribute.UntilNext);
    }

    [Fact]
    public void UntilNext_CanBeSet_ToFalse()
    {
        var attribute = new LabelWidthAttribute(1)
        {
            UntilNext = false
        };
        Assert.False(attribute.UntilNext);
    }

    [Fact]
    public void JustThis_CanBeSet_ToTrue()
    {
        var attribute = new LabelWidthAttribute(2)
        {
            JustThis = true
        };
        Assert.True(attribute.JustThis);
    }

    [Fact]
    public void JustThis_CanBeSet_ToFalse()
    {
        var attribute = new LabelWidthAttribute(2)
        {
            JustThis = false
        };
        Assert.False(attribute.JustThis);
    }
}
