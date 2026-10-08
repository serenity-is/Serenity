using System.IO;

namespace Serenity.Web;

/// <summary>
/// A placeholder implementation of <see cref="IImageProcessor"/> that throws <see cref="NotImplementedException"/> for all methods.
/// </summary>
public class ThrowingImageProcessor : IImageProcessor
{
    const string NotImplementedMessage =
        "No IImageProcessor implementation is registered. Image upload operations that check or resize images " +
        "require one. Register your own in Startup.cs before calling services.AddUploadStorage(), " +
        "e.g. services.AddSingleton<IImageProcessor, YourImageProcessor>(); " +
        "See SkiaSharpImageProcessor.cs in latest Serene and StartSharp for a sample implementation.";

    /// <inheritdoc/>
    public (int width, int height) GetImageSize(object image)
    {
        throw new NotImplementedException(NotImplementedMessage);
    }

    /// <inheritdoc/>
    public object? Load(Stream source, out ImageFormatInfo? formatInfo)
    {
        throw new NotImplementedException(NotImplementedMessage);
    }

    /// <inheritdoc/>
    public void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams)
    {
        throw new NotImplementedException(NotImplementedMessage);
    }

    /// <inheritdoc/>
    public object Scale(object image, int width, int height, ImageScaleMode mode, string? backgroundColor, bool inplace)
    {
        throw new NotImplementedException(NotImplementedMessage);
    }
}