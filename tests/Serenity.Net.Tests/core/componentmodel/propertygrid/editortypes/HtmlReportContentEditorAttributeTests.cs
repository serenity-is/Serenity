namespace Serenity.ComponentModel;

public class HtmlReportContentEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_HtmlReportContent()
    {
        var attribute = new HtmlReportContentEditorAttribute();
        Assert.Equal("HtmlReportContent", attribute.EditorType);
    }

    [Fact]
    public void Cols_CanBeSet_ToInt()
    {
        var attribute = new HtmlReportContentEditorAttribute()
        {
            Cols = 1
        };
        Assert.Equal(1, attribute.Cols);
    }

    [Fact]
    public void Rows_CanBeSet_ToInt()
    {
        var attribute = new HtmlReportContentEditorAttribute()
        {
            Rows = 1
        };
        Assert.Equal(1, attribute.Rows);
    }
}

