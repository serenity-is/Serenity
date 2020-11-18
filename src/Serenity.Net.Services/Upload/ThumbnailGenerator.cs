using System;
using System.Drawing;
using System.Drawing.Imaging;

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
            ImageScaleMode mode, Color backgroundColor, float xDPI = 0, float yDPI = 0)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            // if image or thumb width and height is zero, return an empty image
            if (imageWidth <= 0 || imageHeight <= 0 || (thumbWidth <= 0 && thumbHeight <= 0))
            {
                return GenerateEmptyBitmap(thumbWidth, thumbHeight, backgroundColor);
            }

            // calculate thumb width / source image width
            double horizontalScale = thumbWidth / ((double)imageWidth);
            // calculate thumb height / source image height
            double verticalScale = thumbHeight / ((double)imageHeight);

            // if thumb width is zero, thumb height is not zero
            // so calculate width by aspect ratio, do similar
            // if thumb height is zero. for both states, aspect
            // ratio of source and thumb will be same
            if (thumbWidth == 0)
            {
                thumbWidth = Convert.ToInt32(imageWidth * verticalScale);
                horizontalScale = verticalScale;
                mode = ImageScaleMode.StretchToFit;
            }
            else if (thumbHeight == 0)
            {
                thumbHeight = Convert.ToInt32(imageHeight * horizontalScale);
                verticalScale = horizontalScale;
                mode = ImageScaleMode.StretchToFit;
            }

            // position to generate thumb in thumbnail image, initially based on thumbWidth, thumbHeight
            Rectangle thumbRect = new Rectangle(0, 0, thumbWidth, thumbHeight);

            // source rectangle to use in source image, initially all of source image
            Rectangle imageRect = new Rectangle(0, 0, image.Width, image.Height);

            // At this point, if mode is CropSourceImage, check to see is this mode is applicable,
            // as if horizontal and vertical ratios are very close, when StretchToFit is used
            // instead of CropSourceImage, someone looking at the generated thumb won't notice difference,
            // because AspectRatio mismatch is like one in a million.
            // If thumbWidth or thumbHeight is zero, CropSourceImage won't be used
            if (mode == ImageScaleMode.CropSourceImage &&
                Math.Abs(horizontalScale - verticalScale) >= 0.0001 &&
                horizontalScale != 0 &&
                verticalScale != 0)
            {
                int cropSize;

                // if thubmnails scale to source image horizontally, is bigger than vertical scale,
                // we take all of the source image vertically, and central part of it horizontally
                // otherwise take all of the source image horizontally, and central part of it vertically
                if (horizontalScale <= verticalScale)
                {
                    cropSize = Convert.ToInt32(thumbWidth / verticalScale);
                    imageRect.X = (imageRect.Width - cropSize) / 2;
                    imageRect.Width = cropSize;
                }
                else
                {
                    cropSize = Convert.ToInt32(thumbHeight / horizontalScale);
                    imageRect.Y = (imageRect.Height - cropSize) / 2;
                    imageRect.Height = cropSize;
                }
            }
            else if (
                mode == ImageScaleMode.PreserveRatioWithFill ||
                mode == ImageScaleMode.PreserveRatioNoFill)
            {
                // In PreserveRatioWithFill ve PreserveRatioNoFill modes,
                // scaling is performed without changing aspect ratio. 
                // As there will be horizontal or vertical spaces, in WithFill mode,
                // they are filled with a solid color, while in NoFill mode
                // thumbWidth, thumbHeight are decreased as size of the space
                if (horizontalScale <= verticalScale)
                {
                    thumbRect.Height = Convert.ToInt32(horizontalScale * imageHeight);
                    if (mode == ImageScaleMode.PreserveRatioWithFill)
                        thumbRect.Y = (thumbHeight - thumbRect.Height) / 2;
                    else
                        thumbHeight = thumbRect.Height;
                }
                else
                {
                    thumbRect.Width = Convert.ToInt32(verticalScale * imageWidth);
                    if (mode == ImageScaleMode.PreserveRatioWithFill)
                        thumbRect.X = (thumbWidth - thumbRect.Width) / 2;
                    else
                        thumbWidth = thumbRect.Width;
                }
            }

            // create a 24 bit thumbnail image
            Bitmap thumb = new Bitmap(thumbWidth, thumbHeight, PixelFormat.Format24bppRgb);
            try
            {
                if ((xDPI != 0 && (thumb.HorizontalResolution != xDPI)) ||
                    (yDPI != 0 && (thumb.VerticalResolution != yDPI)))
                {
                    thumb.SetResolution(xDPI, yDPI);
                }

                using Graphics g = Graphics.FromImage(thumb);
                // high quality parameters
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                g.Clear(backgroundColor);
                g.DrawImage(image, thumbRect, imageRect, GraphicsUnit.Pixel);
            }
            catch
            {
                // dispose generated image if any errors occur
                thumb.Dispose();
                throw;
            }
            return thumb;
        }

        /// <summary>
        ///   Generates an empty bitmap</summary>
        /// <param name="width">
        ///   Width.</param>
        /// <param name="height">
        ///   Height.</param>
        /// <param name="color">
        ///   Color that empty bitmap will be filled with. If Color.Empty, it is not filled.</param>
        /// <returns>
        ///   Bitmap of requested size.</returns>
        public static Image GenerateEmptyBitmap(int width, int height, Color color)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if (width > 0 && height > 0 && color != Color.Empty)
            {
                using Graphics g = Graphics.FromImage(bitmap);
                g.Clear(color);
            }
            return bitmap;
        }
    }
}