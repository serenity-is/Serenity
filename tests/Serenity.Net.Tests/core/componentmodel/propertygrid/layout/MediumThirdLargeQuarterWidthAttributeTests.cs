namespace Serenity.ComponentModel;

public class MediumThirdLargeQuarterWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new MediumThirdLargeQuarterWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd4_ColLg3()
    {
        var attribute = new MediumThirdLargeQuarterWidthAttribute();
        Assert.Equal("col-md-4 col-lg-3", attribute.Value);
    }
}

