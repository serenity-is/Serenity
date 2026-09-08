namespace Serenity;

public class UriHelperTests
{
    [Fact]
    public void Combine_ThrowsArgumentNullException_WhenFileNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => UriHelper.Combine("http://x", null));
    }

    [Fact]
    public void Combine_ThrowsArgumentNullException_WhenUrlIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => UriHelper.Combine(null, "file"));
    }

    [Fact]
    public void Combine_ThrowsArgumentOutOfRangeException_WhenFileNameContainsDotDot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UriHelper.Combine("http://x", "../file"));
    }

    [Fact]
    public void Combine_ThrowsArgumentOutOfRangeException_WhenUrlContainsDotDot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UriHelper.Combine("http://x/../", "file"));
    }

    [Fact]
    public void Combine_ReturnsFileName_WhenUrlIsEmpty()
    {
        Assert.Equal("file.txt", UriHelper.Combine("", "file.txt"));
    }

    [Fact]
    public void Combine_AppendsFileName_WhenUrlEndsWithSlash()
    {
        Assert.Equal("http://x/", UriHelper.Combine("http://x/", ""));
        Assert.Equal("http://x/file", UriHelper.Combine("http://x/", "file"));
    }

    [Fact]
    public void Combine_RemovesLeadingSlash_WhenUrlEndsWithSlashAndFileNameStartsWithSlash()
    {
        Assert.Equal("http://x/file", UriHelper.Combine("http://x/", "/file"));
    }

    [Fact]
    public void Combine_AppendsFileName_WhenFileNameStartsWithSlash()
    {
        Assert.Equal("http://x/file", UriHelper.Combine("http://x", "/file"));
    }

    [Fact]
    public void Combine_InsertsSlash_WhenNeitherHasSlash()
    {
        Assert.Equal("http://x/file", UriHelper.Combine("http://x", "file"));
    }
}
