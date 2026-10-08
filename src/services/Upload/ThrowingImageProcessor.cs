using System.IO;

namespace Serenity.Web;

/// <summary>
/// A placeholder implementation of <see cref="IImageProcessor"/> that throws <see cref="NotImplementedException"/> for all methods.
/// </summary>
public class ThrowingImageProcessor : IImageProcessor
{
    const string NotImplementedMessage = "Please add your own IImageProcessor implementation in Startup.cs.";

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