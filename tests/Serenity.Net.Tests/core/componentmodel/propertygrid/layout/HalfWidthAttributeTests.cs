namespace Serenity.ComponentModel;

public class HalfWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new HalfWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColSm6()
    {
        var attribute = new HalfWidthAttribute();
        Assert.Equal("col-sm-6", attribute.Value);
    }
}

