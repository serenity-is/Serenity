namespace Serenity.ComponentModel;

public class NotFilterableAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new NotFilterableAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new NotFilterableAttribute(false);
        Assert.False(attribute.Value);
    }
}