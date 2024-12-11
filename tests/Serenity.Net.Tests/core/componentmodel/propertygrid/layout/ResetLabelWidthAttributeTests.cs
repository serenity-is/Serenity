namespace Serenity.ComponentModel;

public class ResetLabelWidthAttributeTests
{
    [Fact]
    public void IsSubClassOf_LabelWidthAttribute()
    {
        var attribute = new ResetLabelWidthAttribute();
        Assert.IsAssignableFrom<LabelWidthAttribute>(attribute);
    }

    [Fact]
    public void Value_Is_Null()
    {
        var attribute = new ResetLabelWidthAttribute();
        Assert.Null(attribute.Value);
    }
}
