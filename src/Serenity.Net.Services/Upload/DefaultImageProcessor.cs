using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Default implementation for the <see cref="IImageProcessor"/>
/// </summary>
public class DefaultImageProcessor : IImageProcessor
{
    /// <inheritdoc/>
    public (int width, int height) GetImageSize(object imageObj)
    {
        if (imageObj == null)
            throw new ArgumentNullException(nameof(imageObj));

        if (imageObj is not Image image)
            throw new ArgumentOutOfRangeException(nameof(imageObj));

        return (image.Width, image.Height);
    }

    /// <inheritdoc/>
    public object Load(Stream source, out ImageFormatInfo formatInfo)
    {
        formatInfo = null;
        var image = Image.Load(source, out var format);
        try
        {
            formatInfo = new()
            {
                MimeType = format.DefaultMimeType,
                FileExtensions = format.FileExtensions
            };
            return image;
        }
        catch
        {
            image.Dispose();
            throw;
        }
    }

    /// <inheritdoc/>
    public void Save(object imageObj, Stream target, string mimeType, ImageEncoderParams encoderParams)
    {
        if (imageObj == null)
            throw new ArgumentNullException(nameof(imageObj));

        if (imageObj is not Image image)
            throw new ArgumentOutOfRangeException(nameof(imageObj));

        if (target is null)
            throw new ArgumentNullException(nameof(target));

        if (mimeType is null)
            throw new ArgumentNullException(nameof(mimeType));

        IImageEncoder encoder = mimeType switch
        {
            "image/jpeg" => new JpegEncoder() { Quality = ((encoderParams?.Quality ?? 0) == 0) ? (int?)null : encoderParams.Quality },
            "image/png" => new PngEncoder(),
            "image/gif" => new GifEncoder(),
            "image/webp" => new WebpEncoder(),
            _ => throw new ArgumentOutOfRangeException(nameof(mimeType)),
        };

        image.Save(target, encoder);
    }

    /// <inheritdoc/>
    public object Scale(object imageObj, int scaleWidth, int scaleHeight, ImageScaleMode mode, string backgroundColor, bool inplace)
    {
        if (imageObj == null)
            throw new ArgumentNullException(nameof(imageObj));

        if (imageObj is not Image image)
            throw new ArgumentOutOfRangeException(nameof(imageObj));

        int imageWidth = image.Width;
        int imageHeight = image.Height;

        Color padColor = string.IsNullOrEmpty(backgroundColor) ? Color.Black : Color.Parse(backgroundColor);

        // if image or thumb width and height is zero, return an empty image
        if (imageWidth <= 0 || imageHeight <= 0 || (scaleWidth <= 0 && scaleHeight <= 0))
            return GenerateEmptyBitmap(imageWidth, imageHeight, padColor);

        // if thumb width is zero, thumb height is not zero
        // so calculate width by aspect ratio, do similar
        // if thumb height is zero. for both states, aspect
        // ratio of source and thumb will be same
        if (scaleWidth == 0)
        {
            scaleWidth = Convert.ToInt32(imageWidth * scaleHeight / ((double)imageHeight));
            mode = ImageScaleMode.StretchToFit;
        }
        else if (scaleHeight == 0)
        {
            scaleHeight = Convert.ToInt32(imageHeight * scaleWidth / ((double)imageWidth));
            mode = ImageScaleMode.StretchToFit;
        }

        var resizeMode = mode switch
        {
            ImageScaleMode.PreserveRatioNoFill => ResizeMode.Max,
            ImageScaleMode.PreserveRatioWithFill => ResizeMode.Pad,
            ImageScaleMode.CropSourceImage => ResizeMode.Crop,
            _ => ResizeMode.Stretch,
        };

        void operation(IImageProcessingContext x)
        {
            x.Resize(new ResizeOptions
            {
                Mode = resizeMode,
                Size = new Size(scaleWidth, scaleHeight),
                PremultiplyAlpha = false,
                PadColor = padColor
            });
        }

        if (inplace)
        {
            image.Mutate(operation);
            return image;
        }

        return image.Clone(operation);

    }

    private Image GenerateEmptyBitmap(int width, int height, Color color)
    {
        return new Image<Rgb24>(width, height, color);
    }
}
