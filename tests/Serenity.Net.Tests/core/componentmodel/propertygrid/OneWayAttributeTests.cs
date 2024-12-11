namespace Serenity.ComponentModel;

public class OneWayAttributeTests
{
    [Fact]
    public void IsSubClassOf_Attribute()
    {
        var attribute = new OneWayAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }
}

