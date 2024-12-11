namespace Serenity.ComponentModel;

public class GenerateFieldsAttributeTests()
{
    [Fact]
    public void IsAssignableFrom_Attribute()
    {
        var attribute = new GenerateFieldsAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }
}