
namespace Serenity.Web;

[NestedLocalTexts]
internal static partial class UploadTexts
{
    public static partial class Controls
    {
        public static class ImageUpload
        {
            public static LocalText NotAnImageFile = "Uploaded file is not an image!";
            public static LocalText NotAnImageWithExtensions = "Uploaded file extension is not in the list of allowed image extensions: '{1}'!";
            public static LocalText ImageExtensionMismatch = "Uploaded image extension ({0}) does not match its format ({1})!";
            public static LocalText ExtensionBlacklisted = "Uploaded file extension '{0}' is not allowed!";
            public static LocalText ExtensionNotAllowed = "Uploaded file extension '{0}' is not in the list of allowed extensions: '{1}'!";
            public static LocalText UploadFileTooSmall = "Uploaded file must be at least {0}!";
            public static LocalText UploadFileTooBig = "Uploaded file must be smaller than {0}!";
            public static LocalText MaxWidth = "Uploaded image should be under {0} pixels in width!";
            public static LocalText MinWidth = "Uploaded image should be at least {0} pixels in width!";
            public static LocalText MaxHeight = "Uploaded image should be under {0} pixels in height!";
            public static LocalText MinHeight = "Uploaded image should be at least {0} pixels in height!";
        }
    }

    public static partial class Enums
    {
        public static class ImageCheckResult
        {
            public static LocalText UnsupportedFormat = "Uploaded image format is not supported!";
            public static LocalText StreamReadError = "Error occurred during reading file stream!";
            public static LocalText DataSizeTooHigh = "File size must be lower than {0} but it is {1}!";
            public static LocalText InvalidImage = "File is not a valid image!";
            public static LocalText ImageIsEmpty = "Image is empty. Its height or width is zero!";
            public static LocalText SizeMismatch = "Image size must be exactly {0}x{1} pixels but it is {2}x{3} pixels!";
            public static LocalText WidthMismatch = "Image width must be exactly {0} pixels but it is {1} pixels!";
            public static LocalText WidthTooHigh = "Image width must be lower than {0} pixels but it is {1} pixels!";
            public static LocalText WidthTooLow = "Image width must be higher than {0} pixels but it is {1} pixels!";
            public static LocalText HeightMismatch = "Image height must be exactly {0} pixels but it is {1} pixels!";
            public static LocalText HeightTooHigh = "Image height must be lower than {0} pixels but it is {1} pixels!";
            public static LocalText HeightTooLow = "Image height must be higher than {0} pixels but it is {1} pixels!";
        }
    }
}