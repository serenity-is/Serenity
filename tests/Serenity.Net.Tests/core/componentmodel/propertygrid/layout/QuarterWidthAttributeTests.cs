namespace Serenity.ComponentModel;

public class QuarterWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new QuarterWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColLg3_ColSm6()
    {
        var attribute = new QuarterWidthAttribute();
        Assert.Equal("col-lg-3 col-sm-6", attribute.Value);
    }
}

