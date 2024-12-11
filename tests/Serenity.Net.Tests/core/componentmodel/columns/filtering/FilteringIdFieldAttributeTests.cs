namespace Serenity.ComponentModel;

public class FilteringIdFieldAttributeTests
{
    [Fact]
    public void Value_CanBeSet_ViaConstructor()
    {
        var attribute = new FilteringIdFieldAttribute("someField");
        Assert.Equal("someField", attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet_ToNull()
    {
        var attribute = new FilteringIdFieldAttribute(null);
        Assert.Null(attribute.Value);
    }
}
