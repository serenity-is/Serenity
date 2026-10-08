using SkiaSharp;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// An <see cref="IImageProcessor"/> implementation based on SkiaSharp.
/// </summary>
/// <remarks>
/// Register this in <c>Startup.cs</c> before calling <c>services.AddUploadStorage()</c>:
/// <code>services.AddSingleton&lt;IImageProcessor, SkiaSharpImageProcessor&gt;();</code>
/// </remarks>
public class SkiaSharpImageProcessor : IImageProcessor
{
    private static readonly SKSamplingOptions Sampling = new(SKFilterMode.Linear, SKMipmapMode.Linear);

    /// <inheritdoc/>
    public (int width, int height) GetImageSize(object image)
    {
        ArgumentNullException.ThrowIfNull(image);

        return image switch
        {
            SKBitmap bitmap => (bitmap.Width, bitmap.Height),
            SKImage skImage => (skImage.Width, skImage.Height),
            _ => throw new ArgumentOutOfRangeException(nameof(image))
        };
    }

    /// <inheritdoc/>
    public object? Load(Stream source, out ImageFormatInfo? formatInfo)
    {
        ArgumentNullException.ThrowIfNull(source);

        formatInfo = null;

        using var data = SKData.Create(source);
        using var codec = SKCodec.Create(data)
            ?? throw new ArgumentException("Invalid or unsupported image format.", nameof(source));

        var bitmap = SKBitmap.Decode(data)
            ?? throw new ArgumentException("Error loading image.", nameof(source));

        formatInfo = new ImageFormatInfo
        {
            MimeType = GetMimeType(codec.EncodedFormat),
            FileExtensions = GetFileExtensions(codec.EncodedFormat)
        };

        return bitmap;
    }

    /// <inheritdoc/>
    public object Scale(object image, int width, int height, ImageScaleMode mode, string? backgroundColor, bool inplace)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (image is not SKBitmap source)
            throw new ArgumentOutOfRangeException(nameof(image));

        if (inplace)
            throw new NotSupportedException("Scaling images in place is not supported.");

        var sourceWidth = source.Width;
        var sourceHeight = source.Height;
        var padColor = ParseColor(backgroundColor);

        if (sourceWidth <= 0 || sourceHeight <= 0 || (width <= 0 && height <= 0))
            return GenerateEmptyBitmap(sourceWidth, sourceHeight, padColor);

        if (width == 0)
        {
            width = Math.Max(1, (int)Math.Round(sourceWidth * (height / (double)sourceHeight)));
            mode = ImageScaleMode.StretchToFit;
        }
        else if (height == 0)
        {
            height = Math.Max(1, (int)Math.Round(sourceHeight * (width / (double)sourceWidth)));
            mode = ImageScaleMode.StretchToFit;
        }

        return mode switch
        {
            ImageScaleMode.PreserveRatioNoFill => ScalePreserveRatioNoFill(source, width, height),
            ImageScaleMode.PreserveRatioWithFill => ScalePreserveRatioWithFill(source, width, height, padColor),
            ImageScaleMode.CropSourceImage => ScaleCropSourceImage(source, width, height),
            _ => ScaleTo(source, width, height)
        };
    }

    /// <inheritdoc/>
    public void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(mimeType);

        if (image is not SKBitmap bitmap)
            throw new ArgumentOutOfRangeException(nameof(image));

        var format = mimeType switch
        {
            "image/jpeg" => SKEncodedImageFormat.Jpeg,
            "image/png" => SKEncodedImageFormat.Png,
            "image/gif" => SKEncodedImageFormat.Gif,
            "image/webp" => SKEncodedImageFormat.Webp,
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType))
        };

        var quality = encoderParams?.Quality ?? 0;
        if (quality <= 0 || quality > 100)
            quality = 75;

        using var skImage = SKImage.FromBitmap(bitmap);
        using var data = skImage.Encode(format, quality)
            ?? throw new InvalidOperationException($"Cannot encode image as {mimeType}.");

        data.SaveTo(target);
    }

    private static SKBitmap ScaleTo(SKBitmap source, int width, int height)
    {
        var result = new SKBitmap(width, height);
        using var canvas = new SKCanvas(result);
        using var image = SKImage.FromBitmap(source);
        canvas.DrawImage(image, new SKRect(0, 0, width, height), Sampling, null);
        return result;
    }

    private static SKBitmap ScalePreserveRatioNoFill(SKBitmap source, int width, int height)
    {
        var ratio = Math.Min(width / (double)source.Width, height / (double)source.Height);
        var scaledWidth = Math.Max(1, (int)Math.Round(source.Width * ratio));
        var scaledHeight = Math.Max(1, (int)Math.Round(source.Height * ratio));
        return ScaleTo(source, scaledWidth, scaledHeight);
    }

    private static SKBitmap ScalePreserveRatioWithFill(SKBitmap source, int width, int height, SKColor padColor)
    {
        var ratio = Math.Min(width / (double)source.Width, height / (double)source.Height);
        var scaledWidth = Math.Max(1, (int)Math.Round(source.Width * ratio));
        var scaledHeight = Math.Max(1, (int)Math.Round(source.Height * ratio));

        var result = new SKBitmap(width, height);
        using var canvas = new SKCanvas(result);
        canvas.Clear(padColor);

        var left = (width - scaledWidth) / 2f;
        var top = (height - scaledHeight) / 2f;
        using var image = SKImage.FromBitmap(source);
        canvas.DrawImage(image, new SKRect(left, top, left + scaledWidth, top + scaledHeight), Sampling, null);
        return result;
    }

    private static SKBitmap ScaleCropSourceImage(SKBitmap source, int width, int height)
    {
        var ratio = Math.Max(width / (double)source.Width, height / (double)source.Height);
        var scaledWidth = source.Width * ratio;
        var scaledHeight = source.Height * ratio;

        var result = new SKBitmap(width, height);
        using var canvas = new SKCanvas(result);

        var left = (width - scaledWidth) / 2.0;
        var top = (height - scaledHeight) / 2.0;
        using var image = SKImage.FromBitmap(source);
        canvas.DrawImage(image, new SKRect((float)left, (float)top,
            (float)(left + scaledWidth), (float)(top + scaledHeight)), Sampling, null);
        return result;
    }

    private static SKBitmap GenerateEmptyBitmap(int width, int height, SKColor color)
    {
        var result = new SKBitmap(Math.Max(1, width), Math.Max(1, height));
        using var canvas = new SKCanvas(result);
        canvas.Clear(color);
        return result;
    }

    private static SKColor ParseColor(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return SKColors.Black;

        return SKColor.TryParse(value, out var color) ? color : SKColors.Black;
    }

    private static string? GetMimeType(SKEncodedImageFormat format) => format switch
    {
        SKEncodedImageFormat.Jpeg => "image/jpeg",
        SKEncodedImageFormat.Png => "image/png",
        SKEncodedImageFormat.Gif => "image/gif",
        SKEncodedImageFormat.Webp => "image/webp",
        SKEncodedImageFormat.Bmp => "image/bmp",
        _ => null
    };

    private static IEnumerable<string>? GetFileExtensions(SKEncodedImageFormat format) => format switch
    {
        SKEncodedImageFormat.Jpeg => [".jpg", ".jpeg", ".jpe", ".jfif"],
        SKEncodedImageFormat.Png => [".png"],
        SKEncodedImageFormat.Gif => [".gif"],
        SKEncodedImageFormat.Webp => [".webp"],
        SKEncodedImageFormat.Bmp => [".bmp"],
        _ => null
    };
}
