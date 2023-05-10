using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Serenity.Web;

/// <summary>
///   Static class that contains thumbnail generator methods</summary>
public static class ThumbnailGenerator
{
    /// <summary>
    ///   Generates a thumbnail of the source image based on parameters.</summary>
    /// <remarks>
    ///   <p>Thumbnail width or height must be greater than 0. Otherwise an empty image
    ///   is generated.</p>
    ///   <p>When ImageScaleMode.PreserveRatioNoFill used and both dimensions are set,
    ///   if aspect ratio of source image and thumbnail doesn't match, thumbnail's horizontal or vertical
    ///   size may be different than requested one. In, PreserveRatioWithFill mode thumbnail size will
    ///   be at requested size but empty parts are filled with a solid color.</p></remarks>
    /// <param name="image">
    ///   Image object to generate thumbnail for (required)</param>
    /// <param name="thumbWidth">
    ///   Thumbnail width. If 0, width is calculated by source aspect ratio. Only one of
    ///   width or height can be zero.</param>
    /// <param name="thumbHeight">
    ///   Thumbnail height. If 0, height is calculated by source aspect ratio. Only one of
    ///   width or height can be zero.</param>
    /// <param name="mode">
    ///   Thumbnail generation mode. It is only important when both dimensions are specified and
    ///   source aspect ratio is different than thumbnail (see <see cref="ImageScaleMode"/>).</param>
    /// <param name="backgroundColor">
    ///   Specifies fill color for PreserveRatioWithFill mode.</param>
    /// <param name="inplace">True if the original image should be modified inplace</param>
    /// <returns>
    ///   Generated thumbnail image. Should be disposed by caller.</returns>
    [Obsolete("Please inject IImageProcessor from services and use that instead")]
    public static Image Generate(Image image, int thumbWidth, int thumbHeight,
        ImageScaleMode mode, Color? backgroundColor = null, bool inplace = false)
    {
        return (Image)new DefaultImageProcessor().Scale(image, thumbWidth, thumbHeight, mode, backgroundColor?.ToHex(), inplace);
    }

    /// <summary>
    /// Generates an empty bitmap
    /// </summary>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="color">Background color</param>
    /// <returns></returns>
    public static Image GenerateEmptyBitmap(int width, int height, Color color)
    {
        return new Image<Rgb24>(width, height, color);
    }
}