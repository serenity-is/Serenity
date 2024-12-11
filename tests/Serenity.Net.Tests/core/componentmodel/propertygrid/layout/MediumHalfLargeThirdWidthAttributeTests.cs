namespace Serenity.ComponentModel;

public class MediumHalfLargeThirdWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new MediumHalfLargeThirdWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd6_ColLg4()
    {
        var attribute = new MediumHalfLargeThirdWidthAttribute();
        Assert.Equal("col-md-6 col-lg-4", attribute.Value);
    }
}

