namespace Serenity.ComponentModel;

public class ThreeQuarterWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new ThreeQuarterWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColLg9()
    {
        var attribute = new ThreeQuarterWidthAttribute();
        Assert.Equal("col-lg-9", attribute.Value);
    }
}

