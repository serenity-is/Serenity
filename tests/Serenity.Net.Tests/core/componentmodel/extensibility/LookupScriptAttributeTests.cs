namespace Serenity.ComponentModel;

public class LookupScriptAttributeTests()
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullType()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute((Type)null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullString()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute((string)null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForEmptyString()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupScriptAttribute(""));
    }

    [Fact]
    public void Constructor_CanBeCall_WithoutKey()
    {
        var attribute = new LookupScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new LookupScriptAttribute("key");
        Assert.Equal("key", attribute.Key);
    }
}