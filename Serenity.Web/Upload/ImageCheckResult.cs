namespace Serenity.Web
{
    /// <summary>
    ///   <see cref="ImageChecker"/>'s result codes.</summary>
    public enum ImageCheckResult
    {
        /// <summary>
        ///   Checked image is a GIF</summary>
        GIFImage,
        /// <summary>
        ///   Checked image is a JPEG</summary>
        JPEGImage,
        /// <summary>
        ///   Checked image is a PNG</summary>
        PNGImage,
        /// <summary>
        ///   Checked file is a FLASH .SWF, ImageChecker itself won't return such a result,
        ///   used by TemporaryImage</summary>
        FlashMovie,
        /// <summary>
        ///   Unsupported format</summary>
        UnsupportedFormat,
        /// <summary>
        ///   Error occurred during reading</summary>
        StreamReadError,
        /// <summary>
        ///   File size too high</summary>
        DataSizeTooHigh,
        /// <summary>
        ///   Image file is invalid</summary>
        InvalidImage,
        /// <summary>
        ///   Image is empty, its height or width is zero</summary>
        ImageIsEmpty,
        /// <summary>
        ///   Image is not at exact pixel width/height</summary>
        SizeMismatch,
        /// <summary>
        ///   Image doesn't have an exact width</summary>
        WidthMismatch,
        /// <summary>
        ///   Image is wider than maximum allowed</summary>
        WidthTooHigh,
        /// <summary>
        ///   Image is shorter than minimum allowed</summary>
        WidthTooLow,
        /// <summary>
        ///   Image doesn't have an exact height</summary>
        HeightMismatch,
        /// <summary>
        ///   Image is taller than maximum allowed</summary>
        HeightTooHigh,
        /// <summary>
        ///   Image is shorter than minimum allowed</summary>
        HeightTooLow
    }
}
