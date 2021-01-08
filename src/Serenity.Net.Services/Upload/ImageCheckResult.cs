using System.ComponentModel;

namespace Serenity.Web
{
    /// <summary>
    ///   <see cref="ImageChecker"/>'s result codes.</summary>
    public enum ImageCheckResult
    {
        /// <summary>
        ///   Checked image is a GIF</summary>
        [Description("GIF Image")]
        GIFImage,
        /// <summary>
        ///   Checked image is a JPEG</summary>
        [Description("JPEG Image")]
        JPEGImage,
        /// <summary>
        ///   Checked image is a PNG</summary>
        [Description("PNG Image")]
        PNGImage,
        /// <summary>
        ///   Checked file is a FLASH .SWF, ImageChecker itself won't return such a result,
        ///   used by TemporaryImage</summary>
        [Description("Flash Movie")]
        FlashMovie,
        /// <summary>
        ///   Unsupported format</summary>
        [Description("Uploaded image format is not supported!")]
        UnsupportedFormat,
        /// <summary>
        ///   Error occurred during reading</summary>
        [Description("Error occurred during reading file stream!")]
        StreamReadError,
        /// <summary>
        ///   File size too high</summary>
        [Description("File size must be lower than {0}, but it is {1}!")]
        DataSizeTooHigh,
        /// <summary>
        ///   Image file is invalid</summary>
        [Description("File is not a valid image!")]
        InvalidImage,
        /// <summary>
        ///   Image is empty, its height or width is zero</summary>
        [Description("Image is empty, its height or width is zero!")]
        ImageIsEmpty,
        /// <summary>
        ///   Image is not at exact pixel width/height</summary>
        [Description("Image size must be exactly {0}x{1} pixels but it is {2}x{3} pixels!")]
        SizeMismatch,
        /// <summary>
        ///   Image doesn't have an exact width</summary>
        [Description("Image width must be exactly {0} pixels, but it is {1} pixels!")]
        WidthMismatch,
        /// <summary>
        ///   Image is wider than maximum allowed</summary>
        [Description("Image width must be lower than {0} pixels, but it is {1} pixels!")]
        WidthTooHigh,
        /// <summary>
        ///   Image is shorter than minimum allowed</summary>
        [Description("Image width must be higher than {0} pixels, but it is {1} pixels!")]
        WidthTooLow,
        /// <summary>
        ///   Image doesn't have an exact height</summary>
        [Description("Image height must be exactly {0} pixels, but it is {1} pixels!")]
        HeightMismatch,
        /// <summary>
        ///   Image is taller than maximum allowed</summary>
        [Description("Image height must be lower than {0} pixels, but it is {1} pixels!")]
        HeightTooHigh,
        /// <summary>
        ///   Image is shorter than minimum allowed</summary>
        [Description("Image width must be higher than {0} pixels, but it is {1} pixels!")]
        HeightTooLow
    }
}
