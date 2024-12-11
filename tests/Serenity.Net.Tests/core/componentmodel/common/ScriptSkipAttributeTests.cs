namespace Serenity.ComponentModel;

public class ScriptSkipAttributeTests
{
    [Fact]
    public void ScriptSkipAttribute_ShouldInheritFromAttribute()
    {
        {
            var attribute = new ScriptSkipAttribute();
            Assert.IsAssignableFrom<Attribute>(attribute);
        }
    }
}