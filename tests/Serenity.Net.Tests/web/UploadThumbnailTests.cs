using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Serenity.Web;

public class UploadThumbnailTests
{
    [Fact]
    public void ProcessedUploadInfo_Properties()
    {
        var info = new ProcessedUploadInfo
        {
            ImageHeight = 10,
            ImageWidth = 20,
            IsImage = true,
            FileSize = 1024,
            TemporaryFile = "temp.png"
        };

        Assert.Equal(10, info.ImageHeight);
        Assert.Equal(20, info.ImageWidth);
        Assert.True(info.IsImage);
        Assert.Equal(1024, info.FileSize);
        Assert.Equal("temp.png", info.TemporaryFile);

#pragma warning disable CS0618
        Assert.Null(info.ErrorMessage);
        Assert.True(info.Success);
#pragma warning restore CS0618
    }

    [Fact]
    public void GenerateEmptyBitmap_Creates_Bitmap()
    {
        using var image = ThumbnailGenerator.GenerateEmptyBitmap(4, 6, Color.Red);

        Assert.Equal(4, image.Width);
        Assert.Equal(6, image.Height);
    }

    [Fact]
    public void Generate_Creates_Thumbnail()
    {
        using var source = new Image<Rgb24>(20, 10);

#pragma warning disable CS0618
        using var thumb = ThumbnailGenerator.Generate(source, 10, 5, ImageScaleMode.PreserveRatioNoFill);
#pragma warning restore CS0618

        Assert.Equal(10, thumb.Width);
        Assert.Equal(5, thumb.Height);
    }
}
