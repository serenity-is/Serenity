namespace Serenity.Web;

public class DefaultUploadStorageTests
{
    private static DefaultUploadStorage Create(string path = "App_Data/upload/", string url = "~/upload/")
    {
        return new DefaultUploadStorage(
            Microsoft.Extensions.Options.Options.Create(new UploadSettings { Path = path, Url = url }),
            null,
            new MockDiskUploadFileSystem());
    }

    private static void WriteFile(IUploadStorage storage, string path, string content)
    {
        using var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content));
        storage.WriteFile(path, ms, OverwriteOption.Overwrite);
    }

    private static string ReadFile(IUploadStorage storage, string path)
    {
        using var stream = storage.OpenFile(path);
        using var reader = new System.IO.StreamReader(stream);
        return reader.ReadToEnd();
    }

    [Fact]
    public void Constructor_Throws_When_Options_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultUploadStorage(null!, null, new MockDiskUploadFileSystem()));
    }

    [Theory]
    [InlineData("", "~/upload/")]
    [InlineData("App_Data/upload/", "")]
    [InlineData(null, "~/upload/")]
    [InlineData("App_Data/upload/", null)]
    public void Constructor_Throws_When_Path_Or_Url_Is_Empty(string? path, string? url)
    {
        var options = Microsoft.Extensions.Options.Options.Create(
            new UploadSettings { Path = path, Url = url });

        Assert.ThrowsAny<ArgumentException>(() =>
            new DefaultUploadStorage(options, null, new MockDiskUploadFileSystem()));
    }

    [Fact]
    public void Constructor_Strips_Compatibility_Home_Prefix()
    {
        var storage = Create(path: "~/App_Data/upload/");

        WriteFile(storage, "test.txt", "hello");
        Assert.True(storage.FileExists("test.txt"));
    }

    [Fact]
    public void File_Operations_Are_Delegated_To_Combined_Storage()
    {
        var storage = Create();
        WriteFile(storage, "test.txt", "hello");

        Assert.True(storage.FileExists("test.txt"));
        Assert.Equal(5, storage.GetFileSize("test.txt"));
        Assert.Equal("hello", ReadFile(storage, "test.txt"));
        Assert.Equal("~/upload/test.txt", storage.GetFileUrl("test.txt"));
        Assert.Contains("test.txt", storage.GetFiles("", "*.*"));
    }

    [Fact]
    public void Metadata_Operations_Are_Delegated()
    {
        var storage = Create();
        WriteFile(storage, "test.txt", "hello");

        storage.SetFileMetadata("test.txt", new Dictionary<string, string> { ["key"] = "value" }, true);

        var metadata = storage.GetFileMetadata("test.txt");
        Assert.Equal("value", metadata["key"]);
    }

    [Fact]
    public void DeleteFile_And_PurgeTemporaryFiles_Are_Delegated()
    {
        var storage = Create();
        WriteFile(storage, "test.txt", "hello");

        storage.DeleteFile("test.txt");
        storage.PurgeTemporaryFiles();

        Assert.False(storage.FileExists("test.txt"));
    }

    [Fact]
    public void ArchiveFile_Is_Delegated()
    {
        var storage = Create();
        WriteFile(storage, "test.txt", "hello");

        var archived = storage.ArchiveFile("test.txt");

        Assert.StartsWith("history/", archived);
        Assert.True(storage.FileExists(archived));
    }

    [Fact]
    public void CopyFrom_Is_Delegated()
    {
        var source = new MockUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = @"C:\uploads\",
            RootUrl = "/upload/"
        }, new MockDiskUploadFileSystem());
        WriteFile(source, "source.txt", "content");

        var storage = Create();
        var target = storage.CopyFrom(source, "source.txt", "copied.txt", OverwriteOption.Overwrite);

        Assert.Equal("copied.txt", target);
        Assert.Equal("content", ReadFile(storage, "copied.txt"));
    }

    [Fact]
    public void Temporary_Files_Are_Routed_To_Temporary_Storage()
    {
        var storage = Create();
        WriteFile(storage, "temporary/temp.txt", "temp");

        Assert.True(storage.FileExists("temporary/temp.txt"));
        Assert.Equal("~/upload/temporary/temp.txt", storage.GetFileUrl("temporary/temp.txt"));
    }
}
