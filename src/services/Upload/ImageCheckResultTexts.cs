
namespace Serenity.Web;

[NestedLocalTexts(Prefix = "Enums.ImageCheckResult.")]
public static class ImageCheckResultTexts
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
