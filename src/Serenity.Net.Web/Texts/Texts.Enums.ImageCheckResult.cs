
namespace Serenity.Web
{
    internal static partial class Texts
    {
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
}
