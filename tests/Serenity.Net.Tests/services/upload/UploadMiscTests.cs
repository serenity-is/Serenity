using SixLabors.ImageSharp;

namespace Serenity.Web;

public class UploadMiscTests
{
    [Fact]
    public void TempUploadStorage_CreatesRootAndPurges()
    {
        var fs = new MockDiskUploadFileSystem();
        var storage = new TempUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "/temp",
            RootUrl = "/temp/"
        }, fs);

        Assert.True(fs.DirectoryExists(storage.RootPath));
        storage.PurgeTemporaryFiles();
    }

    [Fact]
    public void ImageCheckResultTexts_AreAccessible()
    {
        Assert.NotNull(ImageCheckResultTexts.UnsupportedFormat);
        Assert.NotNull(ImageCheckResultTexts.StreamReadError);
        Assert.NotNull(ImageCheckResultTexts.DataSizeTooHigh);
        Assert.NotNull(ImageCheckResultTexts.InvalidImage);
        Assert.NotNull(ImageCheckResultTexts.ImageIsEmpty);
        Assert.NotNull(ImageCheckResultTexts.SizeMismatch);
        Assert.NotNull(ImageCheckResultTexts.WidthMismatch);
        Assert.NotNull(ImageCheckResultTexts.WidthTooHigh);
        Assert.NotNull(ImageCheckResultTexts.WidthTooLow);
        Assert.NotNull(ImageCheckResultTexts.HeightMismatch);
        Assert.NotNull(ImageCheckResultTexts.HeightTooHigh);
        Assert.NotNull(ImageCheckResultTexts.HeightTooLow);
    }

    [Fact]
    public void ThumbnailGenerator_GeneratesEmptyBitmap()
    {
        using var image = ThumbnailGenerator.GenerateEmptyBitmap(20, 10, Color.Red);
        Assert.Equal(20, image.Width);
        Assert.Equal(10, image.Height);
    }

    [Fact]
    public void FormatFilename_Throws_ForNulls()
    {
        Assert.Throws<ArgumentNullException>(() => UploadFormatting.FormatFilename(null!));
        Assert.Throws<ArgumentNullException>(() => UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{4}",
            OriginalName = null
        }));
    }

    [Fact]
    public void FormatFilename_UsesOriginalName()
    {
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{4}",
            OriginalName = "file.txt"
        });

        Assert.Equal("file.txt", result);
    }

    [Fact]
    public void FormatFilename_WithGuidEntityId_UsesFirstTwoCharsAsGroupKey()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789012");
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{1}-{4}",
            OriginalName = "file.txt",
            EntityId = guid
        });

        Assert.Equal("12-file.txt", result);
    }

    [Fact]
    public void FormatFilename_WithLongEntityId_UsesThousandsGroup()
    {
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{1}",
            OriginalName = "file.txt",
            EntityId = 1234567L
        });

        Assert.Equal("1234.txt", result);
    }

    [Fact]
    public void FormatFilename_WithStringEntityId_UsesFirstTwoChars()
    {
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{1}",
            OriginalName = "file.txt",
            EntityId = "abcdef"
        });

        Assert.Equal("ab.txt", result);
    }

    [Fact]
    public void FormatFilename_WithEmptyStringEntityId_UsesUnderscore()
    {
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{1}",
            OriginalName = "file.txt",
            EntityId = ""
        });

        Assert.Equal("_.txt", result);
    }

    [Fact]
    public void FormatFilename_AppliesPostFormat()
    {
        var result = UploadFormatting.FormatFilename(new FormatFilenameOptions
        {
            Format = "{4}",
            OriginalName = "file.txt",
            PostFormat = x => x.ToUpperInvariant()
        });

        Assert.Equal("FILE.TXT", result);
    }

    [Fact]
    public void FileSizeDisplay_FormatsSizes()
    {
        Assert.Equal("a (1.00 KB)", UploadFormatting.FileNameSizeDisplay("a", 1024));
        Assert.Contains("KB", UploadFormatting.FileSizeDisplay(1024));
        Assert.Contains("MB", UploadFormatting.FileSizeDisplay(5 * 1024 * 1024));
    }
}

