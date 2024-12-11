namespace Serenity.ComponentModel;

public class MediumQuarterWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new MediumQuarterWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd3()
    {
        var attribute = new MediumQuarterWidthAttribute();
        Assert.Equal("col-md-3", attribute.Value);
    }
}

