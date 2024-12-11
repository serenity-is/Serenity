namespace Serenity.ComponentModel;

public class UrlEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_URL()
    {
        var attribute = new URLEditorAttribute();
        Assert.Equal("URL", attribute.EditorType);
    }
}

