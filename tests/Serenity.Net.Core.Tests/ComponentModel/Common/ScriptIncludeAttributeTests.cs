namespace Serenity.Tests.ComponentModel;

public class ScriptIncludeAttributeTests
{
    [Fact]
    public void ScriptIncludeAttribute_ShouldInheritAttribute()
    {
        var attribute = new ScriptIncludeAttribute();
        Assert.IsAssignableFrom<Attribute>(attribute);
    }
}
