namespace Serenity.ComponentModel;

public class VisibleAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new VisibleAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new VisibleAttribute(false);
        Assert.False(attribute.Value);
    }
}
