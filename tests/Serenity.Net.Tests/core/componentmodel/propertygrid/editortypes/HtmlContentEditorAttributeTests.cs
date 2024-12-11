namespace Serenity.ComponentModel;

public class HtmlContentEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_HtmlContent()
    {
        var attribute = new HtmlContentEditorAttribute();
        Assert.Equal("HtmlContent", attribute.EditorType);
    }

    [Fact]
    public void Cols_CanBeSet_ToInt()
    {
        var attribute = new HtmlContentEditorAttribute()
        {
            Cols = 1
        };
        Assert.Equal(1, attribute.Cols);
    }

    [Fact]
    public void Rows_CanBeSet_ToInt()
    {
        var attribute = new HtmlContentEditorAttribute()
        {
            Rows = 1
        };
        Assert.Equal(1, attribute.Rows);
    }
}
