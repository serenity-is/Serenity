namespace Serenity.ComponentModel;

public class EditorTypeAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_AsString()
    {
        var attribute = new EditorTypeAttribute("text");      
        Assert.Equal("text", attribute.EditorType);
    }

    [Fact]
    public void SetParams_DoesNotModifyEditorParams()
    {
        var attribute = new EditorTypeAttribute("text");
        var editorParams = new Dictionary<string, object>();
        attribute.SetParams(editorParams);
        Assert.Empty(editorParams);
    }
}
