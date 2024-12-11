namespace Serenity.ComponentModel;

public class UpdatableAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new UpdatableAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new UpdatableAttribute(false);
        Assert.False(attribute.Value);
    }
}
