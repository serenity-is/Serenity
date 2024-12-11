namespace Serenity.ComponentModel;

public class ModuleAttributeTests
{
    [Fact]
    public void Value_CanBeSet()
    {
        var attribute = new ModuleAttribute("SomeModule");
        Assert.Equal("SomeModule", attribute.Value);
    }
}
