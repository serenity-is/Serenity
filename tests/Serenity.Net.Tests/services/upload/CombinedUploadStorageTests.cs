using System.IO;

namespace Serenity.Web;

public class CombinedUploadStorageTests
{
    private static (CombinedUploadStorage combined, RecordingUploadStorage main, RecordingUploadStorage sub) Create()
    {
        var main = new RecordingUploadStorage();
        var sub = new RecordingUploadStorage();
        return (new CombinedUploadStorage(main, sub, "temporary"), main, sub);
    }

    [Fact]
    public void Throws_ForNullArguments()
    {
        var main = new RecordingUploadStorage();
        var sub = new RecordingUploadStorage();
        Assert.Throws<ArgumentNullException>(() => new CombinedUploadStorage(null!, sub, "temporary"));
        Assert.Throws<ArgumentNullException>(() => new CombinedUploadStorage(main, null!, "temporary"));
        Assert.Throws<ArgumentNullException>(() => new CombinedUploadStorage(main, sub, null!));
    }

    [Fact]
    public void ArchiveFile_RoutesBySubPath()
    {
        var (combined, main, sub) = Create();

        Assert.Equal("temporary/archive/x.jpg", combined.ArchiveFile("temporary/x.jpg"));
        Assert.Equal("archive/y.jpg", combined.ArchiveFile("y.jpg"));
        Assert.Contains(("ArchiveFile", "x.jpg"), sub.Calls);
        Assert.Contains(("ArchiveFile", "y.jpg"), main.Calls);
    }

    [Fact]
    public void CopyFrom_RoutesByTargetPath()
    {
        var (combined, main, sub) = Create();
        var source = new RecordingUploadStorage();

        Assert.Equal("temporary/copied/x.jpg", combined.CopyFrom(source, "s", "temporary/x.jpg", OverwriteOption.AutoRename));
        Assert.Equal("copied/y.jpg", combined.CopyFrom(source, "s", "y.jpg", OverwriteOption.AutoRename));
    }

    [Fact]
    public void DeleteFile_RoutesBySubPath()
    {
        var (combined, main, sub) = Create();

        combined.DeleteFile("temporary/x.jpg");
        combined.DeleteFile("y.jpg");

        Assert.Contains(("DeleteFile", "x.jpg"), sub.Calls);
        Assert.Contains(("DeleteFile", "y.jpg"), main.Calls);
    }

    [Fact]
    public void FileExists_RoutesBySubPath()
    {
        var (combined, main, sub) = Create();

        combined.FileExists("temporary/x.jpg");
        combined.FileExists("y.jpg");

        Assert.Contains(("FileExists", "x.jpg"), sub.Calls);
        Assert.Contains(("FileExists", "y.jpg"), main.Calls);
    }

    [Fact]
    public void GetFiles_RoutesAndPrefixes()
    {
        var (combined, _, sub) = Create();
        sub.GetFilesResult.Add("folder/x_t.jpg");

        var under = combined.GetFiles("temporary/folder", "*.jpg");
        Assert.Equal(["temporary/folder/x_t.jpg"], under);

        var subPath = combined.GetFiles("temporary", "*.jpg");
        Assert.Equal(["temporary/folder/x_t.jpg"], subPath);
    }

    [Fact]
    public void GetFileSize_GetFileUrl_OpenFile_RouteBySubPath()
    {
        var (combined, main, sub) = Create();

        Assert.Equal(42, combined.GetFileSize("temporary/x.jpg"));
        Assert.Equal("/url/x.jpg", combined.GetFileUrl("temporary/x.jpg"));
        using var stream = combined.OpenFile("temporary/x.jpg");
        Assert.NotNull(stream);

        Assert.Equal(42, combined.GetFileSize("y.jpg"));
        Assert.Equal("/url/y.jpg", combined.GetFileUrl("y.jpg"));
        using var mainStream = combined.OpenFile("y.jpg");
        Assert.NotNull(mainStream);
    }

    [Fact]
    public void GetAndSetMetadata_RouteBySubPath()
    {
        var (combined, main, sub) = Create();
        sub.Metadata["x.jpg"] = new Dictionary<string, string> { ["k"] = "v" };

        Assert.Equal("v", combined.GetFileMetadata("temporary/x.jpg")["k"]);
        combined.SetFileMetadata("temporary/z.jpg", new Dictionary<string, string> { ["a"] = "b" }, true);
        Assert.Equal("b", sub.Metadata["z.jpg"]["a"]);

        combined.SetFileMetadata("y.jpg", new Dictionary<string, string> { ["c"] = "d" }, true);
        Assert.Equal("d", main.Metadata["y.jpg"]["c"]);
    }

    [Fact]
    public void WriteFile_RoutesAndPrefixes()
    {
        var (combined, main, sub) = Create();
        using var stream = new MemoryStream();

        Assert.Equal("temporary/x.jpg", combined.WriteFile("temporary/x.jpg", stream, OverwriteOption.Overwrite));
        Assert.Equal("y.jpg", combined.WriteFile("y.jpg", stream, OverwriteOption.Overwrite));

        Assert.Contains(("WriteFile", "x.jpg"), sub.Calls);
        Assert.Contains(("WriteFile", "y.jpg"), main.Calls);
    }

    [Fact]
    public void PurgeTemporaryFiles_PurgesBoth()
    {
        var (combined, main, sub) = Create();

        combined.PurgeTemporaryFiles();

        Assert.Contains(("PurgeTemporaryFiles", ""), main.Calls);
        Assert.Contains(("PurgeTemporaryFiles", ""), sub.Calls);
    }
}
