namespace Serenity.ComponentModel;

public class TwoThirdWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new TwoThirdWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_ColMd8()
    {
        var attribute = new TwoThirdWidthAttribute();
        Assert.Equal("col-md-8",attribute.Value);
    }
}

