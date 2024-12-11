namespace Serenity.ComponentModel;

public class FullWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new FullWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute); 
    }

    [Fact]
    public void Value_Is_Null()
    {
        var attribute = new FullWidthAttribute();
        Assert.Null(attribute.Value);
    }
}
