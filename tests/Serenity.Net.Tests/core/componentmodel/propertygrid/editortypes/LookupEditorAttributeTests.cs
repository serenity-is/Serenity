namespace Serenity.ComponentModel;

public class LookupEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Lookup()
    {
        var attribute = new LookupEditorAttribute("Lookup");
        Assert.Equal("Lookup", attribute.EditorType);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullType()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupEditorAttribute((Type)null));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_ForNullLookupKey()
    {
        Assert.Throws<ArgumentNullException>(() => new LookupEditorAttribute((string)null));
    }

    [Fact]
    public void Constructor_WithLookupKey_SetsLookupKey()
    {
        var attr = new LookupEditorAttribute("MyLookup");
        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);
        Assert.Equal("MyLookup", dict["lookupKey"]);
    }

    [LookupScript("MyLookup")]
    private class LookupType
    {
    }

    private class PlainType
    {
    }

    [Fact]
    public void Constructor_WithLookupType_SetsLookupKey()
    {
        var attr = new LookupEditorAttribute(typeof(LookupType));
        var dict = new Dictionary<string, object?>();
        attr.SetParams(dict);
        Assert.Equal("MyLookup", dict["lookupKey"]);
    }

    [Fact]
    public void Constructor_WithPlainType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new LookupEditorAttribute(typeof(PlainType)));
    }
}
