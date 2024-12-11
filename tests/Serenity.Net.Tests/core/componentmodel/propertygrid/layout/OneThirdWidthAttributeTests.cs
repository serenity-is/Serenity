namespace Serenity.ComponentModel;

public class OneThirdWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new OneThirdWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd4()
    {
        var attribute = new OneThirdWidthAttribute();
        Assert.Equal("col-md-4", attribute.Value);
    }
}

