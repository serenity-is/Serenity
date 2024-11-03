using Serenity.Plugins;

namespace Serenity.Tests.Plugins;

public class PrecompiledViewsAttributeTests
{
    [Fact]
    public void IsSubClassOf_Attribute()
    {
        var attribute = new PrecompiledViewsAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }
}
