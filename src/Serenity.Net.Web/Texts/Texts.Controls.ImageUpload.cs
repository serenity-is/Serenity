
namespace Serenity.Web
{
    internal static partial class Texts
    {
        public static partial class Controls
        {
            public static class ImageUpload
            {
                public static LocalText AddFileButton = "Select File";
                public static LocalText DeleteButtonHint = "Remove";
                public static LocalText NotAnImageFile = "Uploaded file is not an image!";
                public static LocalText NotAnImageWithExtensions = "Uploaded file extension is not in the list of allowed image extensions: '{1}'!";
                public static LocalText ImageExtensionMismatch = "Uploaded image extension ({0}) does not match its format ({1})!";
                public static LocalText ExtensionNotAllowed = "Uploaded file extension '{0}' is not in the list of allowed extensions: '{1}'!";
                public static LocalText UploadFileTooSmall = "Uploaded file must be at least {0}!";
                public static LocalText UploadFileTooBig = "Uploaded file must be smaller than {0}!";
                public static LocalText MaxWidth = "Uploaded image should be under {0} pixels in width!";
                public static LocalText MinWidth = "Uploaded image should be at least {0} pixels in width!";
                public static LocalText MaxHeight = "Uploaded image should be under {0} pixels in height!";
                public static LocalText MinHeight = "Uploaded image should be at least {0} pixels in height!";
                public static LocalText ColorboxCurrent = "image {current} / {total}";
                public static LocalText ColorboxPrior = "prior";
                public static LocalText ColorboxNext = "next";
                public static LocalText ColorboxClose = "next";
            }
        }
    }
}