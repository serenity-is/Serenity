namespace Serenity.Web;

[NestedLocalTexts(Prefix = "Controls.ImageUpload.")]
public static class FileUploadTexts
{
    public static readonly LocalText AddFileButton = "Select File";
    public static readonly LocalText ColorboxClose = "close";
    public static readonly LocalText ColorboxCurrent = "image {current} / {total}";
    public static readonly LocalText ColorboxNext = "next";
    public static readonly LocalText ColorboxPrior = "prior";
    public static readonly LocalText DeleteButtonHint = "Remove";
    public static readonly LocalText ExtensionBlacklisted = "Uploaded file extension '{0}' is not allowed!";
    public static readonly LocalText ExtensionNotAllowed = "Uploaded file extension '{0}' is not in the list of allowed extensions: '{1}'!";
    public static readonly LocalText FailedScan = "An error occurred while scanning the uploaded file for viruses! For more information, visit https://serenity.is/docs/av";
    public static readonly LocalText ImageExtensionMismatch = "Uploaded image extension ({0}) does not match its format ({1})!";
    public static readonly LocalText InfectedFile = "Uploaded file may contain a virus!";
    public static readonly LocalText InfectedFileOrError = "Uploaded file may contain a virus or an error occurred during the AV scan!";
    public static readonly LocalText MaxHeight = "Uploaded image should be under {0} pixels in height!";
    public static readonly LocalText MaxWidth = "Uploaded image should be under {0} pixels in width!";
    public static readonly LocalText MinHeight = "Uploaded image should be at least {0} pixels in height!";
    public static readonly LocalText MinWidth = "Uploaded image should be at least {0} pixels in width!";
    public static readonly LocalText NotAnImageFile = "Uploaded file is not an image!";
    public static readonly LocalText NotAnImageWithExtensions = "Uploaded file extension is not in the list of allowed image extensions: '{1}'!";
    public static readonly LocalText UploadFileTooBig = "Uploaded file must be smaller than {0}!";
    public static readonly LocalText UploadFileTooSmall = "Uploaded file must be at least {0}!";
}