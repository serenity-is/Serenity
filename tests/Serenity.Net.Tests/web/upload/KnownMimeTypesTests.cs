namespace Serenity.Web;

public class KnownMimeTypesTests
{
    [Theory]
    [InlineData("file.png", "image/png")]
    [InlineData("file.jpg", "image/jpeg")]
    [InlineData("file.jpeg", "image/jpeg")]
    [InlineData("file.pdf", "application/pdf")]
    [InlineData("file.txt", "text/plain")]
    [InlineData("file.html", "text/html")]
    [InlineData("file.apng", "image/apng")]
    [InlineData("file.avif", "image/avif")]
    public void Get_Returns_Known_Mime_Type(string path, string expected)
    {
        Assert.Equal(expected, KnownMimeTypes.Get(path));
    }

    [Fact]
    public void Get_Is_Case_Insensitive_For_Custom_Types()
    {
        Assert.Equal("image/apng", KnownMimeTypes.Get("FILE.APNG"));
    }

    [Fact]
    public void Get_Returns_Octet_Stream_For_Unknown_Extension()
    {
        Assert.Equal("application/octet-stream", KnownMimeTypes.Get("file.unknownext"));
    }

    [Fact]
    public void TryGet_Returns_Null_For_Unknown_Extension()
    {
        Assert.Null(KnownMimeTypes.TryGet("file.unknownext"));
    }

    [Fact]
    public void TryGet_Returns_Mime_Type_For_Known_Extension()
    {
        Assert.Equal("image/png", KnownMimeTypes.TryGet("file.png"));
    }

    [Fact]
    public void TryGet_Throws_When_Path_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => KnownMimeTypes.TryGet(null!));
    }
}
