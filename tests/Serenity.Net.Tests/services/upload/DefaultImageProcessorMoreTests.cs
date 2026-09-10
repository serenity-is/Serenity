using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace Serenity.Web;

public class DefaultImageProcessorMoreTests
{
    private readonly DefaultImageProcessor processor = new();

    [Fact]
    public void GetImageSize_Throws_For_NonImage()
    {
        Assert.Throws<ArgumentNullException>(() => processor.GetImageSize(null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => processor.GetImageSize(new object()));
    }

    [Fact]
    public void Load_Returns_FormatInfo_And_Throws_For_Invalid()
    {
        using var image = new Image<Rgba32>(2, 3);
        using var ms = new MemoryStream();
        image.Save(ms, PngFormat.Instance);
        ms.Position = 0;

        var loaded = processor.Load(ms, out var formatInfo);
        Assert.NotNull(loaded);
        Assert.NotNull(formatInfo);

        using var bad = new MemoryStream([1, 2, 3]);
        Assert.ThrowsAny<Exception>(() => processor.Load(bad, out _));
    }

    [Fact]
    public void Save_Encodes_Supported_MimeTypes()
    {
        using var image = new Image<Rgba32>(1, 1);
        foreach (var mime in new[] { "image/jpeg", "image/png", "image/gif", "image/webp" })
        {
            using var ms = new MemoryStream();
            processor.Save(image, ms, mime, new ImageEncoderParams { Quality = 80 });
            Assert.True(ms.Length > 0);
        }
    }

    [Fact]
    public void Save_Validates_Arguments()
    {
        using var image = new Image<Rgba32>(1, 1);
        Assert.Throws<ArgumentNullException>(() => processor.Save(null!, new MemoryStream(), "image/png", new ImageEncoderParams()));
        Assert.Throws<ArgumentOutOfRangeException>(() => processor.Save(new object(), new MemoryStream(), "image/png", new ImageEncoderParams()));
        Assert.Throws<ArgumentNullException>(() => processor.Save(image, null!, "image/png", new ImageEncoderParams()));
        Assert.Throws<ArgumentNullException>(() => processor.Save(image, new MemoryStream(), null!, new ImageEncoderParams()));
        Assert.Throws<ArgumentOutOfRangeException>(() => processor.Save(image, new MemoryStream(), "image/tiff", new ImageEncoderParams()));
    }

    [Fact]
    public void Scale_Covers_Modes_And_Inplace()
    {
        using var image = new Image<Rgba32>(10, 5);

        using var cropped = (Image<Rgba32>)processor.Scale(image, 4, 4, ImageScaleMode.CropSourceImage, null, false);
        Assert.Equal(4, cropped.Width);

        using var stretched = (Image<Rgba32>)processor.Scale(image, 4, 4, (ImageScaleMode)999, null, false);
        Assert.Equal(4, stretched.Width);

        var inplace = processor.Scale(image, 2, 2, ImageScaleMode.PreserveRatioNoFill, "#FFFFFF", true);
        Assert.Same(image, inplace);

        Assert.Throws<ArgumentNullException>(() => processor.Scale(null!, 1, 1, ImageScaleMode.StretchToFit, null, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => processor.Scale(new object(), 1, 1, ImageScaleMode.StretchToFit, null, false));
    }

    [Fact]
    public void Scale_Computes_Zero_Dimensions_And_Empty()
    {
        using var image = new Image<Rgba32>(10, 5);

        using var widthOnly = (Image<Rgba32>)processor.Scale(image, 0, 20, ImageScaleMode.PreserveRatioNoFill, null, false);
        Assert.Equal(40, widthOnly.Width);

        using var heightOnly = (Image<Rgba32>)processor.Scale(image, 20, 0, ImageScaleMode.PreserveRatioNoFill, null, false);
        Assert.Equal(10, heightOnly.Height);

        using var empty = (Image)processor.Scale(image, 0, 0, ImageScaleMode.PreserveRatioNoFill, null, false);
        Assert.Equal(10, empty.Width);
        Assert.Equal(5, empty.Height);
    }
}
