namespace Serenity.ComponentModel;

public class MediumHalfWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new MediumHalfWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }
    [Fact]
    public void Value_Is_ColMd6()
    {
        var attribute = new MediumHalfWidthAttribute();
        Assert.Equal("col-md-6", attribute.Value);
    }
}

