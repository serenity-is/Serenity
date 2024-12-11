namespace Serenity.ComponentModel;

public class DateFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Date()
    {
        var attribute = new DateFormatterAttribute();
        Assert.Equal("Date", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new DateFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new DateFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new DateFormatterAttribute()
        {
            DisplayFormat = "Date"
        };
        Assert.Equal("Date", attribute.DisplayFormat);
    }
}