namespace Serenity.ComponentModel;

public class MinuteFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Serenity_MinuteFormatter()
    {
        var attribute = new MinuteFormatterAttribute();
        Assert.Equal("Serenity.MinuteFormatter", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new MinuteFormatterAttribute()
        {
            DisplayFormat = "DisplayFormat"
        };
        Assert.Equal("DisplayFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new MinuteFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new MinuteFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }
}