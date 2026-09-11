using System.IO;

namespace Serenity.Web;

public class UploadStorageExtensionsTests
{
    private class RecordingFilesToDelete : IFilesToDelete
    {
        public List<string> NewFiles { get; } = [];
        public List<string> OldFiles { get; } = [];
        public void RegisterNewFile(string file) => NewFiles.Add(file);
        public void RegisterOldFile(string file) => OldFiles.Add(file);
        public void KeepNewFiles() { }
    }

    [Fact]
    public void GetThumbnailUrl_ReturnsNull_ForEmptyPath()
    {
        var storage = new RecordingUploadStorage();
        Assert.Null(storage.GetThumbnailUrl(null!));
        Assert.Null(storage.GetThumbnailUrl(""));
    }

    [Fact]
    public void GetThumbnailUrl_UsesThumbnailName()
    {
        var storage = new RecordingUploadStorage();
        Assert.Equal("/url/file_t.jpg", storage.GetThumbnailUrl("file.jpg"));
    }

    [Fact]
    public void GetPrimaryFileFromThumb_ReturnsNull_ForInvalidThumb()
    {
        var storage = new RecordingUploadStorage();
        Assert.Null(storage.GetPrimaryFileFromThumb("file.jpg"));
    }

    [Fact]
    public void GetPrimaryFileFromThumb_UsesMetadataExtension()
    {
        var storage = new RecordingUploadStorage();
        storage.Metadata["a/file_t.jpg"] = new Dictionary<string, string>
        {
            [FileMetadataKeys.PrimaryFileExtension] = ".png"
        };

        Assert.Equal("a/file.png", storage.GetPrimaryFileFromThumb("a/file_t.jpg"));
    }

    [Fact]
    public void GetPrimaryFileFromThumb_FallsBackToLegacyExtensions()
    {
        var storage = new RecordingUploadStorage();
        storage.Files.Add("a/file.png");

        Assert.Equal("a/file.png", storage.GetPrimaryFileFromThumb("a/file_t.jpg"));
    }

    [Fact]
    public void GetPrimaryFileFromThumb_ReturnsNull_WhenNoFileFound()
    {
        var storage = new RecordingUploadStorage();
        Assert.Null(storage.GetPrimaryFileFromThumb("a/file_t.jpg"));
    }

    [Fact]
    public void GetThumbnailFiles_ReturnsMatchingThumbs()
    {
        var storage = new RecordingUploadStorage();
        storage.GetFilesResult.Add("a/file_t.jpg");
        storage.GetFilesResult.Add("a/file_t100x200.jpg");
        storage.GetFilesResult.Add("a/other_t.jpg");
        storage.Metadata["a/file_t.jpg"] = new Dictionary<string, string>
        {
            [FileMetadataKeys.IsThumbnail] = "true",
            [FileMetadataKeys.PrimaryFileExtension] = ".jpg"
        };
        storage.Metadata["a/file_t100x200.jpg"] = new Dictionary<string, string>
        {
            [FileMetadataKeys.IsThumbnail] = "true",
            [FileMetadataKeys.PrimaryFileExtension] = ".jpg"
        };

        var thumbs = storage.GetThumbnailFiles("a/file.jpg").ToList();

        Assert.Equal(2, thumbs.Count);
        Assert.Contains("a/file_t.jpg", thumbs);
        Assert.Contains("a/file_t100x200.jpg", thumbs);
    }

    [Fact]
    public void GetThumbnailFiles_SkipsWhenNotMarkedThumbnail()
    {
        var storage = new RecordingUploadStorage();
        storage.GetFilesResult.Add("a/file_t.jpg");
        storage.Metadata["a/file_t.jpg"] = new Dictionary<string, string>
        {
            [FileMetadataKeys.IsThumbnail] = "false"
        };

        Assert.Empty(storage.GetThumbnailFiles("a/file.jpg"));
    }

    [Fact]
    public void GetThumbnailFiles_AcceptsLegacyThumbsWithoutMetadata()
    {
        var storage = new RecordingUploadStorage();
        storage.GetFilesResult.Add("a/file_t.jpg");

        Assert.Single(storage.GetThumbnailFiles("a/file.jpg"));
    }

    [Fact]
    public void CopyTemporaryFile_Throws_ForNulls()
    {
        var storage = new RecordingUploadStorage();
        Assert.Throws<ArgumentNullException>(() => storage.CopyTemporaryFile(null!));
        Assert.Throws<ArgumentNullException>(() => ((IUploadStorage)null!).CopyTemporaryFile(new CopyTemporaryFileOptions { Format = "{4}" }));
    }

    [Fact]
    public void CopyTemporaryFile_CopiesAndRegistersFiles()
    {
        var storage = new RecordingUploadStorage();
        var filesToDelete = new RecordingFilesToDelete();
        var options = new CopyTemporaryFileOptions
        {
            TemporaryFile = "temporary/x.jpg",
            OriginalName = "orig.jpg",
            Format = "{4}",
            FilesToDelete = filesToDelete
        };

        var result = storage.CopyTemporaryFile(options);

        Assert.Equal(42, result.FileSize);
        Assert.Equal("copied/orig.jpg", result.Path);
        Assert.Equal("orig.jpg", result.OriginalName);
        Assert.False(result.HasThumbnail);
        Assert.Contains("copied/orig.jpg", filesToDelete.NewFiles);
        Assert.Contains("temporary/x.jpg", filesToDelete.OldFiles);
    }

    [Fact]
    public void CopyTemporaryFile_DetectsThumbnail()
    {
        var storage = new RecordingUploadStorage();
        storage.Files.Add("temporary/x_t.jpg");
        var options = new CopyTemporaryFileOptions
        {
            TemporaryFile = "temporary/x.jpg",
            OriginalName = "orig.jpg",
            Format = "{4}"
        };

        Assert.True(storage.CopyTemporaryFile(options).HasThumbnail);
    }

    [Fact]
    public void ReadAllFileBytes_ReadsStream()
    {
        var storage = new RecordingUploadStorage();
        Assert.Equal("hello", Encoding.UTF8.GetString(storage.ReadAllFileBytes("a.txt")));
        Assert.Throws<ArgumentNullException>(() => ((IUploadStorage)null!).ReadAllFileBytes("a.txt"));
    }

    [Fact]
    public void GetOriginalName_ReadsMetadata()
    {
        var storage = new RecordingUploadStorage();
        Assert.Null(storage.GetOriginalName("a.txt"));

        storage.Metadata["a.txt"] = new Dictionary<string, string>
        {
            [FileMetadataKeys.OriginalName] = "orig.txt"
        };
        Assert.Equal("orig.txt", storage.GetOriginalName("a.txt"));
    }

    [Fact]
    public void SetOriginalName_WritesMetadata()
    {
        var storage = new RecordingUploadStorage();
        storage.SetOriginalName("a.txt", "orig.txt");
        Assert.Equal("orig.txt", storage.Metadata["a.txt"][FileMetadataKeys.OriginalName]);
        Assert.Throws<ArgumentNullException>(() => ((IUploadStorage)null!).SetOriginalName("a.txt", "x"));
    }

    [Fact]
    public void ObsoleteCopyFrom_MapsAutoRename()
    {
#pragma warning disable CS0618
        var target = new RecordingUploadStorage();
        var source = new RecordingUploadStorage();
        target.CopyFrom(source, "s", "t", null);
        target.CopyFrom(source, "s", "t", true);
        target.CopyFrom(source, "s", "t", false);
#pragma warning restore CS0618
        Assert.Equal(3, target.Calls.Count(x => x.Method == "CopyFrom"));
    }

    [Fact]
    public void ObsoleteWriteFile_MapsAutoRename()
    {
#pragma warning disable CS0618
        var storage = new RecordingUploadStorage();
        using var stream = new MemoryStream();
        storage.WriteFile("a.jpg", stream, null);
        storage.WriteFile("a.jpg", stream, true);
        storage.WriteFile("a.jpg", stream, false);
#pragma warning restore CS0618
        Assert.Equal(3, storage.Calls.Count(x => x.Method == "WriteFile"));
    }
}
