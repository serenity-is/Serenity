using Serenity.Web;
using System.IO;

public interface IImageProcessor
{
    (int width, int height) GetImageSize(object image);

    /// <summary>
    /// Loads the image from the source stream
    /// </summary>
    /// <param name="source">Source stream</param>
    /// <param name="formatInfo">Information about loaded image format</param>
    object Load(Stream source, out ImageFormatInfo formatInfo);

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
    /// <param name="width">
    ///   Thumbnail width. If 0, width is calculated by source aspect ratio. Only one of
    ///   width or height can be zero.</param>
    /// <param name="height">
    ///   Thumbnail height. If 0, height is calculated by source aspect ratio. Only one of
    ///   width or height can be zero.</param>
    /// <param name="mode">
    ///   Thumbnail generation mode. It is only important when both dimensions are specified and
    ///   source aspect ratio is different than thumbnail (see <see cref="ImageScaleMode"/>).</param>
    /// <param name="backgroundColor">
    ///   Specifies fill color for PreserveRatioWithFill mode.</param>
    /// <returns>
    ///   Generated thumbnail image. Should be disposed by caller.</returns>
    object Scale(object image, int width, int height,
        ImageScaleMode mode, string backgroundColor, bool inplace);

    void Save(object image, Stream target, string mimeType, ImageEncoderParams encoderParams);
}