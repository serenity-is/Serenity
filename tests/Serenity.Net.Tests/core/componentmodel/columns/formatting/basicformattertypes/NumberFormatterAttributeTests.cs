namespace Serenity.ComponentModel;

public class NumberFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_Number()
    {
        var attribute = new NumberFormatterAttribute();
        Assert.Equal("Number", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new NumberFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new NumberFormatterAttribute()
        {
            DisplayFormat = "someFormat"
        };
        Assert.Equal("someFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new NumberFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }
}