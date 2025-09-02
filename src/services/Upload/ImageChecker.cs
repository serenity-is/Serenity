using Microsoft.Extensions.Logging;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Checks stream data if valid image file and validate required conditions.
/// </summary>
public class ImageChecker
{
    /// <summary>Checks if the given image if it is a valid or not. 
    /// If so, controls its compliance to constraints</summary> 
    /// <param name="inputStream">Stream which contains image data</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="returnImage">Does image required to be returned? 
    /// If not requested, it will be disposed at the end of processing</param>
    /// <param name="image">When method returns contains the image object. 
    /// If returnImage false it will contain null</param>
    /// <param name="formatInfo">Contains image format info on return</param>
    /// <param name="logger">Logger</param>
    /// <returns>Image validation result. One of <see cref="ImageCheckResult"/> values. 
    /// If the result is one of GIFImage, JPEGImage, PNGImage, the checking is successful. 
    /// Rest of results are invalid.</returns>
    public ImageCheckResult CheckStream(Stream inputStream, IImageProcessor imageProcessor, bool returnImage,
        out object image, out ImageFormatInfo formatInfo, ILogger logger = null)
    {
        if (inputStream is null)
            throw new ArgumentNullException(nameof(inputStream));

        if (imageProcessor is null)
            throw new ArgumentNullException(nameof(imageProcessor));

        // store the start time of validation
        DateTime startTime = DateTime.Now;

        // initialize result variables
        DataSize = 0;
        Width = 0;
        Height = 0;
        Milliseconds = -1;
        image = null;
        formatInfo = null;

        try
        {
            // set stream position to start
            inputStream.Seek(0, SeekOrigin.Begin);

            // set file data size to input stream length
            DataSize = inputStream.Length;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error seeking input stream");
            return ImageCheckResult.StreamReadError;
        }

        // if data size equals 0 return StreamReadError
        if (DataSize == 0)
            return ImageCheckResult.StreamReadError;

        // check file size
        if (MaxDataSize != 0 && DataSize > MaxDataSize)
            return ImageCheckResult.DataSizeTooHigh;

        // try to upload image
        try
        {
            // load image validating it
            image = imageProcessor.Load(inputStream, out formatInfo);
            var (width, height) = imageProcessor.GetImageSize(image);
            Width = width;
            Height = height;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error loading image");

            // couldn't load image
            return ImageCheckResult.InvalidImage;
        }

        bool isImageOK = false;

        try
        {
            var constraintResult = CheckSizeConstraints(Width, Height);

            if (constraintResult != ImageCheckResult.Valid)
                return constraintResult;

            TimeSpan span = DateTime.Now.Subtract(startTime);
            Milliseconds = span.TotalMilliseconds;

            isImageOK = true;

            return ImageCheckResult.Valid;
        }
        finally
        {
            if (!(isImageOK && returnImage))
            {
                (image as IDisposable)?.Dispose();
                image = null;
            }
        }
    }

    /// <summary>
    /// Checks an image width and height against size constraints
    /// </summary>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <returns>One of ImageCheckResult values. ImageCheckResult.JPEGImage if image size is validated.</returns>
    public ImageCheckResult CheckSizeConstraints(int width, int height)
    {
        // raise an error if width or height is zero
        if (width == 0 || height == 0)
            return ImageCheckResult.ImageIsEmpty;

        // check minimum height and width
        if (width < MinWidth)
        {
            if (MinWidth == MaxWidth)
            {
                if (MinHeight == MaxHeight)
                    return ImageCheckResult.SizeMismatch;
                else
                    return ImageCheckResult.WidthMismatch;
            }
            else
                return ImageCheckResult.WidthTooLow;
        }

        if (height < MinHeight)
        {
            if (MinHeight == MaxHeight)
                return ImageCheckResult.HeightMismatch;
            else
                return ImageCheckResult.HeightTooLow;
        }

        // check maximum height and width
        if (MaxWidth != 0 && width > MaxWidth)
            return ImageCheckResult.WidthTooHigh;

        if (MaxHeight != 0 && height > MaxHeight)
            return ImageCheckResult.HeightTooHigh;

        return ImageCheckResult.Valid;
    }

    /// <summary>Gets data size of the validated image</summary>
    public long DataSize { get; private set; }

    /// <summary>Gets width of the validated image</summary>
    public int Width { get; private set; }

    /// <summary>Gets height of the validate image</summary>
    public int Height { get; private set; }

    /// <summary>Gets the time passed during validating the image</summary>
    public double Milliseconds { get; private set; }

    /// <summary>Gets/sets maximum file size allowed</summary>
    public int MaxDataSize { get; set; }

    /// <summary>Gets/sets maximum width allowed. 0 means any width.</summary>
    public int MaxWidth { get; set; }

    /// <summary>Gets/sets maximum height allowed. 0 means any height.</summary>
    public int MaxHeight { get; set; }

    /// <summary>Gets/sets minimum width allowed. 0 means any width.</summary>
    public int MinWidth { get; set; }

    /// <summary>Gets/sets minimum height allowed. 0 means any height.</summary>
    public int MinHeight { get; set; }

    /// <summary>
    /// Formats an <see cref="ImageCheckResult"/> error message
    /// </summary>
    /// <param name="result">Error result</param>
    /// <param name="localizer">Text localizer</param>
    public string FormatErrorMessage(ImageCheckResult result, ITextLocalizer localizer)
    {
        var format = localizer.Get("Enums." + 
            (typeof(ImageCheckResult).GetAttribute<EnumKeyAttribute>()?.Value ??
             typeof(ImageCheckResult).FullName) + "." +
            Enum.GetName(typeof(ImageCheckResult), result));

        return result switch
        {
            ImageCheckResult.DataSizeTooHigh => string.Format(format, MaxDataSize, DataSize),
            ImageCheckResult.SizeMismatch => string.Format(format, MinWidth, MinHeight, Width, Height),
            ImageCheckResult.WidthMismatch or ImageCheckResult.WidthTooHigh => string.Format(format, MaxWidth, Width),
            ImageCheckResult.WidthTooLow => string.Format(format, MinWidth, Width),
            ImageCheckResult.HeightMismatch or ImageCheckResult.HeightTooHigh => string.Format(format, MaxHeight, Height),
            ImageCheckResult.HeightTooLow => string.Format(format, MinHeight, Height),
            _ => format,
        };
    }
}