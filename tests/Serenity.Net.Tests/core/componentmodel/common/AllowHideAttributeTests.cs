namespace Serenity.ComponentModel;

public class AllowHideAttributeTests
{
    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new AllowHideAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToTrue()
    {
        var attribute = new AllowHideAttribute(true);
        Assert.True(attribute.Value);
    }
}