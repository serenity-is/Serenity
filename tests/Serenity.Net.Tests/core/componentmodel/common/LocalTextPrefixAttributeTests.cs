namespace Serenity.ComponentModel;

public class LocalTextPrefixAttributeTests
{
    [Fact]
    public void Value_IsNull_ByDefault()
    {
        var attribute = new LocalTextPrefixAttribute(null);
        Assert.Null(attribute.Value);
    }

    [Fact]
    public void Value_CanBeSet()
    {
        var attribute = new LocalTextPrefixAttribute("Test.Prefix");
        Assert.Equal("Test.Prefix", attribute.Value);
    }
}