namespace Serenity.ComponentModel;

public class StringEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_String()
    {
        var attribute = new StringEditorAttribute();
        Assert.Equal("String", attribute.EditorType);
    }
}
