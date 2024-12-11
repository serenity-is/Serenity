namespace Serenity.ComponentModel;

public class DateTimeFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_DateTime()
    {
        var attribute = new DateTimeFormatterAttribute();
        Assert.Equal("DateTime", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new DateTimeFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new DateTimeFormatterAttribute()
        {
            DisplayFormat = "displayFormat"
        };
        Assert.Equal("displayFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new DateTimeFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }
}