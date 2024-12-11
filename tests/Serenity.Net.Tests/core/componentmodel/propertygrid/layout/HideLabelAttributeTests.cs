namespace Serenity.ComponentModel;

public class HideLabelAttributeTests
{
    [Fact]
    public void IsSubClassOf_LabelWidthAttribute()
    {
        var attribute = new HideLabelAttribute();
        Assert.IsAssignableFrom<LabelWidthAttribute>(attribute);
    }

    [Fact]
    public void JustThis_IsTrue()
    {
        var attribute = new HideLabelAttribute()
        {
            JustThis = true
        };
        Assert.True(attribute.JustThis);
    }
}

