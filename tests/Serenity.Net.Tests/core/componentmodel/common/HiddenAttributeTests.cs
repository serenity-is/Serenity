namespace Serenity.ComponentModel;

public class HiddenAttributeTests
{
    [Fact]
    public void HiddenAttribute_ShouldInheritFromVisibleAttribute()
    {
        var attribute = new HiddenAttribute();
        Assert.IsAssignableFrom<VisibleAttribute>(attribute);
    }

    [Fact]
    public void Value_IsFalse_ByDefault()
    {
        var attribute = new HiddenAttribute();
        Assert.False(attribute.Value);
    }
}
