
namespace Serenity.Web;

[NestedLocalTexts]
public static partial class UploadTexts
{
    public static partial class Controls
    {
        public static class ImageUpload
        {
            public static readonly LocalText InfectedFile = "Uploaded file may contain a virus!";
            public static readonly LocalText InfectedFileOrError = "Uploaded file may contain a virus or an error occured during the AV scan!";
            public static readonly LocalText FailedScan = "An error occured while scanning the uploaded file for viruses!";
            public static readonly LocalText NotAnImageFile = "Uploaded file is not an image!";
            public static readonly LocalText NotAnImageWithExtensions = "Uploaded file extension is not in the list of allowed image extensions: '{1}'!";
            public static readonly LocalText ImageExtensionMismatch = "Uploaded image extension ({0}) does not match its format ({1})!";
            public static readonly LocalText ExtensionBlacklisted = "Uploaded file extension '{0}' is not allowed!";
            public static readonly LocalText ExtensionNotAllowed = "Uploaded file extension '{0}' is not in the list of allowed extensions: '{1}'!";
            public static readonly LocalText UploadFileTooSmall = "Uploaded file must be at least {0}!";
            public static readonly LocalText UploadFileTooBig = "Uploaded file must be smaller than {0}!";
            public static readonly LocalText MaxWidth = "Uploaded image should be under {0} pixels in width!";
            public static readonly LocalText MinWidth = "Uploaded image should be at least {0} pixels in width!";
            public static readonly LocalText MaxHeight = "Uploaded image should be under {0} pixels in height!";
            public static readonly LocalText MinHeight = "Uploaded image should be at least {0} pixels in height!";
        }
    }

    public static partial class Enums
    {
        public static class ImageCheckResult
        {
            public static readonly LocalText UnsupportedFormat = "Uploaded image format is not supported!";
            public static readonly LocalText StreamReadError = "Error occurred during reading file stream!";
            public static readonly LocalText DataSizeTooHigh = "File size must be lower than {0} but it is {1}!";
            public static readonly LocalText InvalidImage = "File is not a valid image!";
            public static readonly LocalText ImageIsEmpty = "Image is empty. Its height or width is zero!";
            public static readonly LocalText SizeMismatch = "Image size must be exactly {0}x{1} pixels but it is {2}x{3} pixels!";
            public static readonly LocalText WidthMismatch = "Image width must be exactly {0} pixels but it is {1} pixels!";
            public static readonly LocalText WidthTooHigh = "Image width must be lower than {0} pixels but it is {1} pixels!";
            public static readonly LocalText WidthTooLow = "Image width must be higher than {0} pixels but it is {1} pixels!";
            public static readonly LocalText HeightMismatch = "Image height must be exactly {0} pixels but it is {1} pixels!";
            public static readonly LocalText HeightTooHigh = "Image height must be lower than {0} pixels but it is {1} pixels!";
            public static readonly LocalText HeightTooLow = "Image height must be higher than {0} pixels but it is {1} pixels!";
        }
    }
}