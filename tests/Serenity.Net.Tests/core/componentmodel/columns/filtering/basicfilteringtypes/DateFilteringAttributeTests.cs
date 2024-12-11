namespace Serenity.ComponentModel;

public class DateFilteringAttributeTests
{
    [Fact]
    public void FilteringType_ShouldBe_Date()
    {
        var attribute = new DateFilteringAttribute();
        Assert.Equal("Date", attribute.FilteringType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new DateFilteringAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new DateFilteringAttribute()
        {
            DisplayFormat = "DisplayFormat"
        };
        Assert.Equal("DisplayFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new DateTimeFilteringAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }
}













