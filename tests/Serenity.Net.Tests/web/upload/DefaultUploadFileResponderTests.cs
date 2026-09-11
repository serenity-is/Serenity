namespace Serenity.Web;

public class DefaultUploadFileResponderTests
{
    private static MockUploadStorage CreateStorage()
    {
        return new MockUploadStorage(fs: new MockDiskUploadFileSystem());
    }

    private static void WriteFile(IUploadStorage storage, string path, string content)
    {
        using var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content));
        storage.WriteFile(path, ms, OverwriteOption.Overwrite);
    }

    [Fact]
    public void Constructor_Throws_When_UploadStorage_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultUploadFileResponder(null!));
    }

    [Fact]
    public void Read_Throws_For_Unsafe_Path()
    {
        var responder = new DefaultUploadFileResponder(CreateStorage());
        var headers = new HeaderDictionary();

        Assert.Throws<ArgumentOutOfRangeException>(() => responder.Read("../test.png", headers));
    }

    [Fact]
    public void Read_Returns_NotFound_When_File_Does_Not_Exist()
    {
        var responder = new DefaultUploadFileResponder(CreateStorage());
        var result = responder.Read("missing.png", new HeaderDictionary());

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData("test.png", "image/png")]
    [InlineData("test.pdf", "application/pdf")]
    [InlineData("test.txt", "text/plain")]
    public void Read_Returns_FileStreamResult_For_Inline_Mime_Types(string path, string expectedMime)
    {
        var storage = CreateStorage();
        WriteFile(storage, path, "content");
        var responder = new DefaultUploadFileResponder(storage);
        var headers = new HeaderDictionary();

        var result = Assert.IsType<FileStreamResult>(responder.Read(path, headers));

        Assert.Equal(expectedMime, result.ContentType);
        Assert.Equal("nosniff", headers["X-Content-Type-Options"].ToString());
    }

    [Fact]
    public void Read_Returns_Octet_Stream_And_ContentDisposition_For_Other_Types()
    {
        var storage = CreateStorage();
        WriteFile(storage, "test.zip", "content");
        var responder = new DefaultUploadFileResponder(storage);
        var headers = new HeaderDictionary();

        var result = Assert.IsType<FileStreamResult>(responder.Read("test.zip", headers));

        Assert.Equal("application/octet-stream", result.ContentType);
        Assert.Contains("attachment", headers["Content-Disposition"].ToString());
    }
}
