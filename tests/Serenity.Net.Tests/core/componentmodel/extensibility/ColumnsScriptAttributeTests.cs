namespace Serenity.ComponentModel;

public class ColumnsScriptAttributeTests
{
    [Fact]
    public void ColumnsScriptAttribute_CanBeCreated_WithoutAnyArgs()
    {
        _ = new ColumnsScriptAttribute();
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenKeyArgument_IsNullOrEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => new ColumnsScriptAttribute(null));
        Assert.Throws<ArgumentNullException>(() => new ColumnsScriptAttribute(""));
    }

    [Fact]
    public void Key_IsNull_ByDefault()
    {
        var attribute = new ColumnsScriptAttribute();
        Assert.Null(attribute.Key);
    }

    [Fact]
    public void Key_CanBePassed_AsString()
    {
        var attribute = new ColumnsScriptAttribute("someKey");
        Assert.Equal("someKey", attribute.Key);
    }

    [Fact]
    public void LocalTextPrefix_IsNull_ByDefault()
    {
        var attribute = new ColumnsScriptAttribute();
        Assert.Null(attribute.LocalTextPrefix);
    }

    [Fact]
    public void LocalTextPrefix_CanBeSet()
    {
        var attribute = new ColumnsScriptAttribute
        {
            LocalTextPrefix = "TestPrefix"
        };
        Assert.Equal("TestPrefix", attribute.LocalTextPrefix);
    }
}