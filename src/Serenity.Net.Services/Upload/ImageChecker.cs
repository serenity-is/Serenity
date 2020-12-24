using System;
using System.Drawing;
using System.IO;

namespace Serenity.Web
{
    /// <summary>
    ///   checks stream data if valid image file and validate required conditions.
    ///   </summary>
    public class ImageChecker
    {
        /// <summary>
        ///   Checks if the given image if it is a valid or not. If so, controls its compliance to constraints</summary> 
        /// <param name="inputStream">
        ///   Stream which contains image data</param>
        /// <param name="returnImage">
        ///   Does image required to be returned? If not requested, it will be disposed at the end of processing</param>
        /// <param name="image">
        ///   When method returns contains the image object. If returnImage false it will contain null</param>
        /// <returns>
        ///   Image validation result. One of <see cref="ImageCheckResult"/> values. If the result is one of
        ///   GIFImage, JPEGImage, PNGImage, the checking is successful. Rest of results are invalid.</returns>
        public ImageCheckResult CheckStream(Stream inputStream, bool returnImage, out Image image)
        {
            // store the start time of validation
            DateTime startTime = DateTime.Now;

            // initialize result variables
            DataSize = 0;
            Width = 0;
            Height = 0;
            Milliseconds = -1;
            image = null;
            
            try
            {
                // set stream position to start
                inputStream.Seek(0, SeekOrigin.Begin);

                // set file data size to input stream length
                DataSize = inputStream.Length;
            }
            catch
            {
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
                image = Image.FromStream(inputStream, true, true);
            }
            catch
            {
                // couldn't load image
                return ImageCheckResult.InvalidImage;
            }

            bool isImageOK = false;

            try
            {
                ImageCheckResult checkResult;

                // check image format, if not JPEG, PNG or GIF return UnsupportedFormat error
                if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                    checkResult = ImageCheckResult.JPEGImage;
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                    checkResult = ImageCheckResult.GIFImage;
                else if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                    checkResult = ImageCheckResult.PNGImage;
                else
                    return ImageCheckResult.UnsupportedFormat;

                // read image size
                Width = image.Width;
                Height = image.Height;

                var constraintResult = CheckSizeConstraints(Width, Height);

                if (constraintResult != ImageCheckResult.JPEGImage)
                    return constraintResult;

                TimeSpan span = DateTime.Now.Subtract(startTime);
                Milliseconds = span.TotalMilliseconds;
                
                isImageOK = true;

                return checkResult;
            }
            finally
            {
                if (!(isImageOK && returnImage))
                {
                    image.Dispose();
                    image = null;
                }
            }
        }

        /// <summary>
        ///   Checks an image width and height against size constraints</summary>
        /// <param name="width">
        ///   Image width.</param>
        /// <param name="height">
        ///   Image height.</param>
        /// <returns>
        ///   One of ImageCheckResult values. ImageCheckResult.JPEGImage if image size is validated.</returns>
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

            return ImageCheckResult.JPEGImage;
        }

        /// <summary>
        ///   Use this overload to check an image from a stream.</summary>
        /// <param name="inputStream">
        ///   Stream with image data</param>
        /// <returns>
        ///   Image checking result</returns>
        /// <seealso cref="CheckStream(Stream, bool, out Image)"/>
        public ImageCheckResult CheckStream(Stream inputStream)
        {
            var result = CheckStream(inputStream, false, out Image image);
            if (image != null)
                image.Dispose();
            return result;
        }

        /// <summary>
        ///   Gets data size of the validated image</summary>
        public long DataSize { get; private set; }

        /// <summary>
        ///   Gets width of the validated image</summary>
        public int Width { get; private set; }

        /// <summary>
        ///   Gets height of the validate image</summary>
        public int Height { get; private set; }

        /// <summary>
        ///   Gets the time passed during validating the image</summary>
        public double Milliseconds { get; private set; }

        /// <summary>
        ///   Gets/sets maximum file size allowed</summary>
        public int MaxDataSize { get; set; }

        /// <summary>
        ///   Gets/sets maximum width allowed. 0 means any width.</summary>
        public int MaxWidth { get; set; }

        /// <summary>
        ///   Gets/sets maximum height allowed. 0 means any height.</summary>
        public int MaxHeight { get; set; }

        /// <summary>
        ///   Gets/sets minimum width allowed. 0 means any width.</summary>
        public int MinWidth { get; set; }

        /// <summary>
        ///   Gets/sets minimum height allowed. 0 means any height.</summary>
        public int MinHeight { get; set; }

        public string FormatErrorMessage(ImageCheckResult result, ITextLocalizer localizer)
        {
            var format = localizer.Get("Enums.ImageCheckResult." + Enum.GetName(typeof(ImageCheckResult), result));
            switch (result)
            {
                case ImageCheckResult.GIFImage:
                case ImageCheckResult.PNGImage:
                case ImageCheckResult.JPEGImage:
                case ImageCheckResult.FlashMovie:
                case ImageCheckResult.InvalidImage:
                case ImageCheckResult.ImageIsEmpty:
                    return format;
                case ImageCheckResult.DataSizeTooHigh:
                    return string.Format(format, MaxDataSize, DataSize);
                case ImageCheckResult.SizeMismatch:
                    return string.Format(format, MinWidth, MinHeight, Width, Height);
                case ImageCheckResult.WidthMismatch:
                case ImageCheckResult.WidthTooHigh:
                    return string.Format(format, MaxWidth, Width);
                case ImageCheckResult.WidthTooLow:
                    return string.Format(format, MinWidth, Width);
                case ImageCheckResult.HeightMismatch:
                case ImageCheckResult.HeightTooHigh:
                    return string.Format(format, MaxHeight, MaxHeight);
                case ImageCheckResult.HeightTooLow:
                    return string.Format(format, MinHeight, MaxHeight);
                default:
                    throw new ArgumentOutOfRangeException("ImageCheckResult");
            }
        }
    }
}