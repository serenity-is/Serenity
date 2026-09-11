using System.IO;

namespace Serenity.Web;

public class DiskUploadStorageMoreTests
{
    private const string Root = @"C:\uploads\";

    private static MemoryStream Stream(string content) => new(Encoding.UTF8.GetBytes(content));

    private static MockUploadStorage CreateStorage() =>
        new(new DiskUploadStorageOptions { RootPath = Root, RootUrl = "/upload/" });

    [Fact]
    public void Relative_RootPath_Is_Combined_With_BaseDirectory()
    {
        var storage = new DiskUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "relative-uploads",
            RootUrl = "/upload/"
        }, new MockDiskUploadFileSystem());

        Assert.Contains("relative-uploads", storage.RootPath);
    }

    [Fact]
    public void GetFileMetadata_Returns_Empty_For_Invalid_Json()
    {
        var storage = CreateStorage();
        storage.MockFileSystem.AddFile(Root + "a.jpg.meta", "not-json");

        Assert.Empty(storage.GetFileMetadata("a.jpg"));
    }

    [Fact]
    public void SetFileMetadata_Does_Nothing_For_Empty_NonOverwrite()
    {
        var storage = CreateStorage();
        using (var s = Stream("a"))
            storage.WriteFile("a.jpg", s, OverwriteOption.Overwrite);

        storage.SetFileMetadata("a.jpg", new Dictionary<string, string>(), false);
        Assert.Empty(storage.GetFileMetadata("a.jpg"));
    }

    [Fact]
    public void CopyFrom_Copies_Thumbnails_And_Metadata()
    {
        var storage = CreateStorage();
        using (var s = Stream("a"))
            storage.WriteFile("a.jpg", s, OverwriteOption.Overwrite);
        using (var s = Stream("th"))
            storage.WriteFile("a_t100x100.jpg", s, OverwriteOption.Overwrite);
        storage.SetFileMetadata("a_t100x100.jpg", new Dictionary<string, string>
        {
            [FileMetadataKeys.IsThumbnail] = "true",
            ["k"] = "v"
        }, true);
        storage.SetFileMetadata("a.jpg", new Dictionary<string, string> { ["m"] = "n" }, true);

        var target = storage.CopyFrom(storage, "a.jpg", "b.jpg", OverwriteOption.Overwrite);

        Assert.Equal("b.jpg", target);
        Assert.True(storage.FileExists("b.jpg"));
        Assert.Equal("v", storage.GetFileMetadata("b_t100x100.jpg")["k"]);
        Assert.Equal("n", storage.GetFileMetadata("b.jpg")["m"]);
    }

    [Fact]
    public void DeleteFile_Deletes_Thumbnails()
    {
        var storage = CreateStorage();
        using (var s = Stream("a"))
            storage.WriteFile("a.jpg", s, OverwriteOption.Overwrite);
        using (var s = Stream("th"))
            storage.WriteFile("a_t100x100.jpg", s, OverwriteOption.Overwrite);

        storage.DeleteFile("a.jpg");

        Assert.False(storage.FileExists("a.jpg"));
        Assert.False(storage.FileExists("a_t100x100.jpg"));
    }

    [Fact]
    public void GetFiles_Excludes_Internal_Files()
    {
        var storage = CreateStorage();
        using (var s = Stream("a"))
            storage.WriteFile("a.jpg", s, OverwriteOption.Overwrite);
        storage.SetFileMetadata("a.jpg", new Dictionary<string, string> { ["k"] = "v" }, true);

        var files = storage.GetFiles("", "*.jpg");

        Assert.Contains("a.jpg", files);
        Assert.DoesNotContain(files, f => f.EndsWith(".meta"));
    }
}
