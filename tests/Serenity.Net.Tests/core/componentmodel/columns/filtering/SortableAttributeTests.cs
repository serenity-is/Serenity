namespace Serenity.Tests.ComponentModel;

public class SortableAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new QuickFilterAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new QuickFilterAttribute(false);
        Assert.False(attribute.Value);
    }
}