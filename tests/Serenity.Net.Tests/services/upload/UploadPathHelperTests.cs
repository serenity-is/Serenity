namespace Serenity.Web;

public class UploadPathHelperTests
{
    [Fact]
    public void GetThumbnailName_ReturnsInput_ForNullOrEmpty()
    {
        Assert.Null(UploadPathHelper.GetThumbnailName(null));
        Assert.Equal("", UploadPathHelper.GetThumbnailName(""));
    }

    [Fact]
    public void GetThumbnailName_RemovesExtension_AndAddsSuffix()
    {
        Assert.Equal("file_t.jpg", UploadPathHelper.GetThumbnailName("file.jpg"));
        Assert.Equal("file_t.jpg", UploadPathHelper.GetThumbnailName("file.png"));
    }

    [Fact]
    public void GetThumbnailName_WithDimensions_AddsSizedSuffix()
    {
        Assert.Equal("file_t100x200.jpg", UploadPathHelper.GetThumbnailName("file.jpg", 100, 200));
    }

    [Fact]
    public void TryParseThumbSuffix_ReturnsFalse_ForInvalidInputs()
    {
        Assert.False(UploadPathHelper.TryParseThumbSuffix(null!, out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file.png", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file.jpg", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file_tXxY.jpg", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file_t100xabc.jpg", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file_tabcx200.jpg", out _, out _, out _, out _));
        Assert.False(UploadPathHelper.TryParseThumbSuffix("file_t1x.jpg", out _, out _, out _, out _));
    }

    [Fact]
    public void TryParseThumbSuffix_ParsesBaseThumb()
    {
        Assert.True(UploadPathHelper.TryParseThumbSuffix("folder/file_t.jpg", out var baseName, out var suffix, out var width, out var height));
        Assert.Equal("folder/file", baseName);
        Assert.Equal("_t.jpg", suffix);
        Assert.Equal(-1, width);
        Assert.Equal(-1, height);
    }

    [Fact]
    public void TryParseThumbSuffix_ParsesSizedThumb()
    {
        Assert.True(UploadPathHelper.TryParseThumbSuffix("file_t100x200.jpg", out var baseName, out var suffix, out var width, out var height));
        Assert.Equal("file", baseName);
        Assert.Equal("_t100x200.jpg", suffix);
        Assert.Equal(100, width);
        Assert.Equal(200, height);
    }

    [Fact]
    public void CheckFileNameSecurity_AcceptsRelativeFile()
    {
        UploadPathHelper.CheckFileNameSecurity("folder/file.jpg");
    }

    [Theory]
    [InlineData("../file.jpg")]
    [InlineData("folder/../file.jpg")]
    public void CheckFileNameSecurity_Throws_ForUnsafePath(string path)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UploadPathHelper.CheckFileNameSecurity(path));
    }

    [Fact]
    public void FindAvailableName_Throws_ForNulls()
    {
        Assert.Throws<ArgumentNullException>(() => UploadPathHelper.FindAvailableName(null!, _ => false));
        Assert.Throws<ArgumentNullException>(() => UploadPathHelper.FindAvailableName("a.jpg", null!));
    }

    [Fact]
    public void FindAvailableName_ReturnsSameName_WhenNotExists()
    {
        Assert.Equal("a.jpg", UploadPathHelper.FindAvailableName("a.jpg", _ => false));
    }

    [Fact]
    public void FindAvailableName_AppendsCounter_WhenExists()
    {
        var tries = 0;
        var result = UploadPathHelper.FindAvailableName("a.jpg", _ => tries++ < 2);
        Assert.Equal("a (2).jpg", result);
    }

    [Fact]
    public void IsTemporaryFile_ChecksPrefix()
    {
        Assert.True(UploadPathHelper.IsTemporaryFile("temporary/file.jpg"));
        Assert.False(UploadPathHelper.IsTemporaryFile("Temporary/file.jpg"));
        Assert.False(UploadPathHelper.IsTemporaryFile(null!));
        Assert.False(UploadPathHelper.IsTemporaryFile(""));
    }
}
