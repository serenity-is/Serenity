namespace Serenity.ComponentModel;

public class DateTimeFilteringTests
{
    [Fact]
    public void FilteringType_ShouldBe_DateTime()
    {
        var attribute = new DateTimeFilteringAttribute();
        Assert.Equal("DateTime", attribute.FilteringType);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new DateTimeFilteringAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new DateTimeFilteringAttribute()
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
  





