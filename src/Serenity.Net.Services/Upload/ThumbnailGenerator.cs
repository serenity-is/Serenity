using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace Serenity.Web
{
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
        /// <returns>
        ///   Generated thumbnail image. Should be disposed by caller.</returns>
        public static Image Generate(Image image, int thumbWidth, int thumbHeight,
            ImageScaleMode mode, Color? backgroundColor = null, bool inplace = false)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            // if image or thumb width and height is zero, return an empty image
            if (imageWidth <= 0 || imageHeight <= 0 || (thumbWidth <= 0 && thumbHeight <= 0))
                return GenerateEmptyBitmap(imageWidth, imageHeight, backgroundColor ?? Color.White);

            // if thumb width is zero, thumb height is not zero
            // so calculate width by aspect ratio, do similar
            // if thumb height is zero. for both states, aspect
            // ratio of source and thumb will be same
            if (thumbWidth == 0)
            {
                thumbWidth = Convert.ToInt32(imageWidth * thumbHeight / ((double)imageHeight));
                mode = ImageScaleMode.StretchToFit;
            }
            else if (thumbHeight == 0)
            {
                thumbHeight = Convert.ToInt32(imageHeight * thumbWidth / ((double)imageWidth));
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
                    Size = new Size(thumbWidth, thumbHeight),
                    PremultiplyAlpha = false
                });
            }

            if (inplace)
            {
                image.Mutate(operation);
                return image;
            }

            return image.Clone(operation);
        }

        public static Image GenerateEmptyBitmap(int width, int height, Color color)
        {
            return new Image<Rgb24>(width, height, color);
        }
    }
}