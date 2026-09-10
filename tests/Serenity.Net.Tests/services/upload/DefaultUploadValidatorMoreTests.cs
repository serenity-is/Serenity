using Microsoft.Extensions.Options;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Serenity.Web;

public class DefaultUploadValidatorMoreTests
{
    private static readonly DefaultUploadValidator validator =
        new(new MockImageProcessor(), NullTextLocalizer.Instance);

    private static readonly DefaultUploadValidator imageValidator =
        new(new DefaultImageProcessor(), NullTextLocalizer.Instance);

    [Fact]
    public void Enforces_Min_And_Max_Size()
    {
        Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions { MaxSize = 4 }, new MemoryStream(new byte[5]), "a.txt", out _));

        Assert.Throws<ValidationError>(() => validator.ValidateFile(
            new UploadOptions { MinSize = 4 }, new MemoryStream(new byte[1]), "a.txt", out _));
    }

    [Fact]
    public void Enforces_Allowed_Extensions()
    {
        var options = new UploadOptions { AllowedExtensions = ".txt", AllowNonImage = true };

        Assert.Throws<ValidationError>(() => validator.ValidateFile(
            options, new MemoryStream(), "a.png", out _));

        validator.ValidateFile(options, new MemoryStream(), "a.txt", out _);
    }

    [Fact]
    public void Allows_Dot_Extension_For_NoExtension_Files()
    {
        var settings = new UploadSettings { ExtensionBlacklist = "", ExtensionWhitelist = "" };
        var v = new DefaultUploadValidator(new MockImageProcessor(), NullTextLocalizer.Instance,
            null, Options.Create(settings));

        v.ValidateFile(new UploadOptions { AllowedExtensions = ".", AllowNonImage = true },
            new MemoryStream(), "noext", out _);
    }

    [Fact]
    public void Throws_When_ImageExtensions_Empty()
    {
        var options = new UploadOptions { ImageExtensions = "", AllowNonImage = false };

        var ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            options, new MemoryStream(), "a.txt", out _));

        Assert.Equal(FileUploadTexts.NotAnImageFile.ToString(NullTextLocalizer.Instance), ex.Message);
    }

    [Fact]
    public void Throws_When_NotAnImageWithExtensions()
    {
        var options = new UploadOptions { ImageExtensions = ".jpg", AllowNonImage = false };

        var ex = Assert.Throws<ValidationError>(() => validator.ValidateFile(
            options, new MemoryStream(), "a.txt", out _));

        Assert.False(string.IsNullOrEmpty(ex.Message));
    }

    [Fact]
    public void ValidateImage_Returns_Image_For_Valid_File()
    {
        using var image = new Image<Rgba32>(2, 2);
        using var ms = PngStream(image);

        imageValidator.ValidateImage(new UploadOptions(), ms, "a.png", out var imageObj);
        Assert.NotNull(imageObj);
    }

    [Fact]
    public void ValidateImage_Throws_For_Extension_Mismatch()
    {
        using var image = new Image<Rgba32>(2, 2);
        using var ms = PngStream(image);

        Assert.Throws<ValidationError>(() => imageValidator.ValidateImage(
            new UploadOptions { IgnoreExtensionMismatch = false }, ms, "a.jpg", out _));
    }

    [Fact]
    public void ValidateImage_Throws_For_Nulls()
    {
        Assert.Throws<ArgumentNullException>(() => imageValidator.ValidateImage(null!, new MemoryStream(), "a.png", out _));
        Assert.Throws<ArgumentNullException>(() => imageValidator.ValidateImage(new UploadOptions(), null!, "a.png", out _));
        Assert.Throws<ArgumentNullException>(() => imageValidator.ValidateImage(new UploadOptions(), new MemoryStream(), null!, out _));
    }

    [Fact]
    public void ValidateImage_Throws_For_Invalid_Image()
    {
        Assert.Throws<ValidationError>(() => imageValidator.ValidateImage(
            new UploadOptions(), new MemoryStream([1, 2, 3]), "a.png", out _));
    }

    [Fact]
    public void ValidateImage_Ignores_Invalid_Image_When_Allowed()
    {
        imageValidator.ValidateImage(new UploadOptions { IgnoreInvalidImage = true },
            new MemoryStream([1, 2, 3]), "a.png", out var image);
        Assert.Null(image);
    }

    [Fact]
    public void ValidateImage_Ignores_Empty_Image_When_Allowed()
    {
        var emptyValidator = new DefaultUploadValidator(new EmptyImageProcessor(), NullTextLocalizer.Instance);
        emptyValidator.ValidateImage(new UploadOptions { IgnoreEmptyImage = true },
            new MemoryStream([1, 2, 3]), "a.png", out var image);
        Assert.Null(image);
    }

    private static MemoryStream PngStream(Image image)
    {
        var ms = new MemoryStream();
        image.Save(ms, PngFormat.Instance);
        ms.Position = 0;
        return ms;
    }

    private class EmptyImageProcessor : IImageProcessor
    {
        public (int width, int height) GetImageSize(object image) => (0, 0);

        public object? Load(Stream source, out ImageFormatInfo? formatInfo)
        {
            formatInfo = new ImageFormatInfo { MimeType = "image/png", FileExtensions = [".png"] };
            return new object();
        }

        public void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams) { }

        public object Scale(object image, int width, int height, ImageScaleMode mode, string? backgroundColor, bool inplace)
            => new object();
    }
}
