namespace Serenity.ComponentModel;

public class SortableAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new SortableAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new SortableAttribute(false);
        Assert.False(attribute.Value);
    }
}