namespace Serenity.ComponentModel;

public class ResetFormWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_FormWidthAttribute()
    {
        var attribute = new ResetFormWidthAttribute();
        Assert.IsAssignableFrom<FormWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_Null()
    {
        var attribute = new ResetFormWidthAttribute();
        Assert.Null(attribute.Value);
    }
}

