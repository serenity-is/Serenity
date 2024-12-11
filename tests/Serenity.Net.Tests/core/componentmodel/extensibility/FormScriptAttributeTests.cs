namespace Serenity.ComponentModel;

public class FormScriptAttributeTests()
{
    [Fact]
    public void Constructor_CanBeCalled_WithoutAnyKey()
    {
        var attribute = new FormScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new FormScriptAttribute("key");
        Assert.Equal("key", attribute.Key);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FormScriptAttribute(null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new FormScriptAttribute(""));
    }
}