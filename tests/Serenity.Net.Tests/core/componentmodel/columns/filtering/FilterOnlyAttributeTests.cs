namespace Serenity.ComponentModel;

public class FilterOnlyAttributeTests
{
    [Fact]
    public void Value_IsTrue_ByDefault()
    {
        var attribute = new FilterOnlyAttribute();
        Assert.True(attribute.Value);
    }

    [Fact]
    public void Value_CanBe_Set_ToFalse()
    {
        var attribute = new FilterOnlyAttribute(false);
        Assert.False(attribute.Value);
    }
}