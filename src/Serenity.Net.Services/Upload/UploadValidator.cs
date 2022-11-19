using SixLabors.ImageSharp;
using System.IO;

namespace Serenity.Web
{
    public static class UploadValidator
    {
        public static void CheckFileConstraints(IUploadFileConstraints fileConstraints,
            long fileSize, string fileExtension, ITextLocalizer localizer,
            out bool isImageExtension)
        {
            isImageExtension = false;

            if ((fileConstraints.ExtensionBlacklist ?? ImageUploadEditorAttribute.DefaultExtensionBlacklist)
                .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(x => string.Equals(x.Trim(), fileExtension, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                    UploadTexts.Controls.ImageUpload.ExtensionBlacklisted.ToString(localizer),
                    fileExtension));

            var minSize = fileConstraints?.MinSize ?? 0;
            if (minSize != 0 && fileSize < minSize)
                throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                    UploadTexts.Controls.ImageUpload.UploadFileTooSmall.ToString(localizer),
                    UploadFormatting.FileSizeDisplay(minSize)));

            var maxSize = fileConstraints?.MaxSize ?? 0;
            if (maxSize != 0 && fileSize > maxSize)
                throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                    UploadTexts.Controls.ImageUpload.UploadFileTooBig.ToString(localizer),
                    UploadFormatting.FileSizeDisplay(maxSize)));

            var allowedExtensions = fileConstraints?.AllowedExtensions;
            if (!string.IsNullOrEmpty(allowedExtensions))
            {
                if (!allowedExtensions.Split(',', ';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Any(x => string.Equals(x, fileExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                        UploadTexts.Controls.ImageUpload.ExtensionNotAllowed.ToString(localizer),
                        fileExtension, fileConstraints.AllowedExtensions));
                }
            }

            var imageExtensions = fileConstraints?.ImageExtensions ?? ImageUploadEditorAttribute.DefaultImageExtensions;
            if (string.IsNullOrEmpty(imageExtensions) ||
                !imageExtensions.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Any(x => string.Equals(x, fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                if (fileConstraints?.AllowNonImage == true)
                    return;

                if (string.IsNullOrEmpty(imageExtensions))
                    throw new ValidationError(
                        UploadTexts.Controls.ImageUpload.NotAnImageFile.ToString(localizer));

                throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                    UploadTexts.Controls.ImageUpload.NotAnImageWithExtensions.ToString(localizer),
                    fileExtension, fileConstraints.ImageExtensions));
            }

            isImageExtension = true;
        }

        public static void CheckImageConstraints(IUploadImageContrains imageConstraints,
            Stream fileStream, string fileExtension, ITextLocalizer localizer,
            IExceptionLogger logger, out Image image)
        {
            image = null;
            try
            {
                var checker = new ImageChecker
                {
                    MinWidth = imageConstraints?.MinWidth ?? 0,
                    MaxWidth = imageConstraints?.MaxWidth ?? 0,
                    MinHeight = imageConstraints?.MinHeight ?? 0,
                    MaxHeight = imageConstraints?.MaxHeight ?? 0
                };

                ImageCheckResult result = checker.CheckStream(fileStream, true, out image,
                    out var mimeType, out var fileExtensions, logger);

                if (result != ImageCheckResult.Valid)
                {
                    if (imageConstraints?.IgnoreInvalidImage == true &&
                        result == ImageCheckResult.InvalidImage)
                        return;

                    if (imageConstraints?.IgnoreEmptyImage == true &&
                        result == ImageCheckResult.ImageIsEmpty)
                        return;

                    var error = checker.FormatErrorMessage(result, localizer);
                    throw new ValidationError(error);
                }

                if (imageConstraints?.IgnoreExtensionMismatch != true &&
                    !fileExtensions.Any(x => string.Equals(x, fileExtension,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                        UploadTexts.Controls.ImageUpload.ImageExtensionMismatch.ToString(localizer),
                        fileExtension, mimeType));
                }
            }
            catch
            {
                image?.Dispose();
                throw;
            }

        }

    }
}
