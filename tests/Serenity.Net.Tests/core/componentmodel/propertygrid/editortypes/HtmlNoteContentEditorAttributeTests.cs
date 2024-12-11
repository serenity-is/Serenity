namespace Serenity.ComponentModel;

public class HtmlNoteContentEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_HtmlNoteContent()
    {
        var attribute = new HtmlNoteContentEditorAttribute();
        Assert.Equal("HtmlNoteContent", attribute.EditorType);
    }

    [Fact]
    public void Cols_CanBeSet_ToInt()
    {
        var attribute = new HtmlNoteContentEditorAttribute()
        {
            Cols = 1
        };
        Assert.Equal(1, attribute.Cols);
    }

    [Fact]
    public void Rows_CanBeSet_ToInt()
    {
        var attribute = new HtmlNoteContentEditorAttribute()
        {
            Rows = 1
        };
        Assert.Equal(1, attribute.Rows);
    }
}