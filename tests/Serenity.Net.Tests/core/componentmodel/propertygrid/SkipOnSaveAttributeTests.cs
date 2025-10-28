namespace Serenity.ComponentModel;

public class SkipOnSaveAttributeTests
{
    [Fact]
    public void IsSubClassOf_Attribute()
    {
        var attribute = new SkipOnSaveAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }
}

