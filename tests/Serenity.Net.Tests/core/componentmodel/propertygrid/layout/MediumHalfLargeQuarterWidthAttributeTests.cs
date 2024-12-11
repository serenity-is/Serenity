namespace Serenity.ComponentModel;

public class MediumHalfLargeQuarterWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new MediumHalfLargeQuarterWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd6_ColLg3()
    {
        var attribute = new MediumHalfLargeQuarterWidthAttribute();
        Assert.Equal("col-md-6 col-lg-3", attribute.Value);
    }
}
