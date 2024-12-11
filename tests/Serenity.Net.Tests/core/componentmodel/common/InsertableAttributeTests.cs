namespace Serenity.ComponentModel;

public class InsertableAttributeTests
{
    [Fact]
    public void Value_CanBeSet_ToTrue()
    {
        var attribute = new InsertableAttribute(true);
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToFalse()
    {
        var attribute = new InsertableAttribute(false);
        Assert.False(attribute.Value);
    }

    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new InsertableAttribute();
        Assert.True(attribute.Value);
    }
}