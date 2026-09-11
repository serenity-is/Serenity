using System.IO;

namespace Serenity.Web;

public class DiskUploadStorageTests
{
    private static MemoryStream Stream(string content) => new(Encoding.UTF8.GetBytes(content));

    [Fact]
    public void Throws_ForInvalidOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new DiskUploadStorage(null!, new MockDiskUploadFileSystem()));
        Assert.Throws<ArgumentException>(() => new DiskUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "", RootUrl = "/"
        }, new MockDiskUploadFileSystem()));
        Assert.Throws<ArgumentException>(() => new DiskUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "/", RootUrl = ""
        }, new MockDiskUploadFileSystem()));
    }

    [Fact]
    public void WriteFile_WritesAndReads()
    {
        var storage = new MockUploadStorage();
        using var stream = Stream("hello");

        var path = storage.WriteFile("a/b.txt", stream, OverwriteOption.Overwrite);

        Assert.Equal("a/b.txt", path);
        Assert.True(storage.FileExists("a/b.txt"));
        Assert.Equal(5, storage.GetFileSize("a/b.txt"));
        Assert.Equal("hello", Encoding.UTF8.GetString(storage.ReadAllFileBytes("a/b.txt")));
    }

    [Fact]
    public void WriteFile_Disallowed_Throws_WhenExists()
    {
        var storage = new MockUploadStorage();
        using (var s1 = Stream("a"))
            storage.WriteFile("a.txt", s1, OverwriteOption.Overwrite);

        using var s2 = Stream("b");
        Assert.Throws<IOException>(() => storage.WriteFile("a.txt", s2, OverwriteOption.Disallowed));
    }

    [Fact]
    public void WriteFile_AutoRename_FindsAvailableName()
    {
        var storage = new MockUploadStorage();
        using (var s1 = Stream("a"))
            storage.WriteFile("a.txt", s1, OverwriteOption.Overwrite);

        using var s2 = Stream("b");
        var path = storage.WriteFile("a.txt", s2, OverwriteOption.AutoRename);

        Assert.Equal("a (1).txt", path);
    }

    [Fact]
    public void WriteFile_Throws_ForInternalOrEmptyPaths()
    {
        var storage = new MockUploadStorage();
        using var s = Stream("a");
        Assert.Throws<ArgumentNullException>(() => storage.WriteFile("", s, OverwriteOption.Overwrite));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.WriteFile("a/x.meta", s, OverwriteOption.Overwrite));
    }

    [Fact]
    public void GetFileSize_And_GetFileUrl_Throw_ForInvalidPaths()
    {
        var storage = new MockUploadStorage();
        Assert.Throws<ArgumentNullException>(() => storage.GetFileSize(""));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.GetFileSize("x.meta"));
        Assert.Throws<ArgumentNullException>(() => storage.GetFileUrl(""));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.GetFileUrl("x.meta"));
    }

    [Fact]
    public void GetFileUrl_CombinesRootUrl()
    {
        var storage = new MockUploadStorage();
        Assert.Equal("/upload/a/b.txt", storage.GetFileUrl("a/b.txt"));
    }

    [Fact]
    public void Metadata_RoundTrips()
    {
        var storage = new MockUploadStorage();
        using (var s = Stream("a"))
            storage.WriteFile("a/b.txt", s, OverwriteOption.Overwrite);

        Assert.Empty(storage.GetFileMetadata("a/b.txt"));

        storage.SetFileMetadata("a/b.txt", new Dictionary<string, string> { ["k"] = "v" }, true);
        Assert.Equal("v", storage.GetFileMetadata("a/b.txt")["k"]);

        storage.SetFileMetadata("a/b.txt", new Dictionary<string, string> { ["k2"] = "v2" }, false);
        var meta = storage.GetFileMetadata("a/b.txt");
        Assert.Equal("v", meta["k"]);
        Assert.Equal("v2", meta["k2"]);

        storage.SetFileMetadata("a/b.txt", new Dictionary<string, string>(), true);
        Assert.Empty(storage.GetFileMetadata("a/b.txt"));
    }

    [Fact]
    public void Metadata_Throws_ForInvalidPaths()
    {
        var storage = new MockUploadStorage();
        Assert.Throws<ArgumentNullException>(() => storage.GetFileMetadata(""));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.GetFileMetadata("x.meta"));
        Assert.Throws<ArgumentException>(() => storage.SetFileMetadata("", new Dictionary<string, string>(), true));
        Assert.Throws<ArgumentNullException>(() => storage.SetFileMetadata("a.txt", null!, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.SetFileMetadata("x.meta", new Dictionary<string, string>(), true));
    }

    [Fact]
    public void ArchiveFile_Throws_ForInvalidPaths()
    {
        var storage = new MockUploadStorage();
        Assert.Throws<ArgumentNullException>(() => storage.ArchiveFile(""));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.ArchiveFile("x.meta"));
    }

    [Fact]
    public void CopyFrom_Throws_ForInvalidPaths()
    {
        var storage = new MockUploadStorage();
        Assert.Throws<ArgumentNullException>(() => storage.CopyFrom(storage, "", "t", OverwriteOption.Overwrite));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.CopyFrom(storage, "x.meta", "t", OverwriteOption.Overwrite));
        Assert.Throws<ArgumentNullException>(() => storage.CopyFrom(storage, "s", "", OverwriteOption.Overwrite));
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.CopyFrom(storage, "s", "x.meta", OverwriteOption.Overwrite));
    }

    [Fact]
    public void DeleteFile_Throws_ForInternalPath_And_IgnoresEmpty()
    {
        var storage = new MockUploadStorage();
        Assert.Throws<ArgumentOutOfRangeException>(() => storage.DeleteFile("x.meta"));
        storage.DeleteFile("");
    }

    [Fact]
    public void PurgeTemporaryFiles_DoesNothing()
    {
        var storage = new MockUploadStorage();
        storage.PurgeTemporaryFiles();
    }
}
