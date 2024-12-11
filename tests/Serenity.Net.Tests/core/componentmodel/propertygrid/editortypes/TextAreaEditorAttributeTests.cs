namespace Serenity.ComponentModel;

public class TextAreaEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_TextArea()
    {
        var attribute = new TextAreaEditorAttribute();
        Assert.Equal("TextArea", attribute.EditorType);
    }

    [Fact]
    public void Cols_CanBeSet_ToInt()
    {
        var attribute = new TextAreaEditorAttribute()
        {
            Cols = 1
        };
        Assert.Equal(1, attribute.Cols);
    }

    [Fact]
    public void Rows_CanBeSet_ToInt()
    {
        var attribute = new TextAreaEditorAttribute()
        {
            Rows = 1
        };
        Assert.Equal(1, attribute.Rows);
    }
}