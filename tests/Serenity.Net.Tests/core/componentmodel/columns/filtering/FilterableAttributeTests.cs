namespace Serenity.ComponentModel;

public class FilterableAttributeTests
{
    [Fact]
    public void Value_IsTrue_WhenSetToTrue()
    {
        Assert.True(new FilterableAttribute(true).Value);
    }

    [Fact]
    public void Value_IsFalse_WhenSetToFalse()
    {
        Assert.False(new FilterableAttribute(false).Value);
    }
}
