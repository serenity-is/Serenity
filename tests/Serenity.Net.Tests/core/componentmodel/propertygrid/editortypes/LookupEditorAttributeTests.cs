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
        Assert.Throws<ArgumentNullException>(() => new LookupFilteringAttribute((Type)null));
    }
}
