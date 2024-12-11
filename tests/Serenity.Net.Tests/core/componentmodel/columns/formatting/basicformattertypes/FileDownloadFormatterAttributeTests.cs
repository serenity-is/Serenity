namespace Serenity.ComponentModel;

public class FileDownloadFormatterAttributeTests
{
    [Fact]
    public void FormatterType_ShouldBe_FileDownload()
    {
        var attribute = new FileDownloadFormatterAttribute();
        Assert.Equal("FileDownload", attribute.FormatterType);
    }

    [Fact]
    public void DisplayFormat_CanBeSet()
    {
        var attribute = new FileDownloadFormatterAttribute()
        {
            DisplayFormat = "DisplayFormat"
        };
        Assert.Equal("DisplayFormat", attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_CanBeSet_ToNull()
    {
        var attribute = new FileDownloadFormatterAttribute()
        {
            DisplayFormat = null
        };
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void DisplayFormat_IsNull_ByDefault()
    {
        var attribute = new FileDownloadFormatterAttribute();
        Assert.Null(attribute.DisplayFormat);
    }

    [Fact]
    public void IconClass_CanBeSet()
    {
        var attribute = new FileDownloadFormatterAttribute()
        {
            IconClass = "IconClass"
        };
        Assert.Equal("IconClass", attribute.IconClass);
    }

    [Fact]
    public void IconClass_CanBeSet_ToNull()
    {
        var attribute = new FileDownloadFormatterAttribute()
        {
            IconClass = null
        };
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void IconClass_IsNullByDefault()
    {
        var attribute = new FileDownloadFormatterAttribute();
        Assert.Null(attribute.IconClass);
    }

    [Fact]
    public void OriginalNameProperty_CanBeSet_ToNull()
    {
        var attribute = new FileDownloadFormatterAttribute
        {
            OriginalNameProperty = null
        };
        Assert.Null(attribute.OriginalNameProperty);

    }

    [Fact]
    public void OriginalNameProperty_CanBeSet()
    {
        var attribute = new FileDownloadFormatterAttribute
        {
            OriginalNameProperty = "someName"

        };
        Assert.Equal("someName", attribute.OriginalNameProperty);
    }

    [Fact]
    public void OriginalNameProperty_IsNull_ByDefault()
    {
        var attribute = new FileDownloadFormatterAttribute();
        Assert.Null(attribute.OriginalNameProperty);
    }
}