using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using Texts = Serenity.Web.UploadTexts.Controls.ImageUpload;

namespace Serenity.Web;

/// <summary>
/// Default implementation for <see cref="IUploadValidator"/>
/// </summary>
public class DefaultUploadValidator : IUploadValidator
{
    private readonly IImageProcessor imageProcessor;
    private readonly ITextLocalizer localizer;
    private readonly IOptions<UploadSettings> uploadSettings;
    private readonly ILogger<DefaultUploadValidator> logger;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="uploadSettings">Upload settings</param>
    /// <param name="logger">Exception logger</param>
    /// <exception cref="ArgumentNullException">imageProcessor or localizer is null</exception>
    public DefaultUploadValidator(IImageProcessor imageProcessor, ITextLocalizer localizer, 
        ILogger<DefaultUploadValidator> logger = null,
        IOptions<UploadSettings> uploadSettings = null)
    {
        this.imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
        this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        this.uploadSettings = uploadSettings ?? new UploadSettings();
        this.logger = logger;
    }

    /// <inheritdoc/>
    public void ValidateFile(IUploadFileConstraints constraints, 
        Stream stream, string filename, out bool isImageExtension)
    {
        if (constraints is null)
            throw new ArgumentNullException(nameof(constraints));

        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (filename is null)
            throw new ArgumentNullException(nameof(filename));

        isImageExtension = false;
        var fileExtension = Path.GetExtension(filename);

        var settings = uploadSettings?.Value;

        if ((!IsExtensionIn(settings.ExtensionBlacklistExclude, fileExtension) &&
             IsExtensionIn(settings.ExtensionBlacklist, fileExtension)) ||
            IsExtensionIn(settings.ExtensionBlacklistInclude, fileExtension))
        {
            throw new ValidationError("ExtensionInBlacklist", 
                string.Format(CultureInfo.CurrentCulture,
                Texts.ExtensionBlacklisted.ToString(localizer),
                fileExtension));
        }

        if ((!string.IsNullOrEmpty(settings.ExtensionWhitelist) ||
             !string.IsNullOrEmpty(settings.ExtensionWhitelistInclude)) &&
            (IsExtensionIn(settings.ExtensionWhitelistExclude, fileExtension) || 
             !IsExtensionIn(settings.ExtensionWhitelist, fileExtension)) &&
            !IsExtensionIn(settings.ExtensionWhitelistInclude, fileExtension))
            throw new ValidationError("ExtensionNotInWhitelist", string.Format(CultureInfo.CurrentCulture,
                Texts.ExtensionBlacklisted.ToString(localizer),
                fileExtension));

        var size = stream.Length;
        if (constraints.MinSize != 0 && size < constraints.MinSize)
            throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                Texts.UploadFileTooSmall.ToString(localizer),
                UploadFormatting.FileSizeDisplay(constraints.MinSize)));

        if (constraints.MaxSize != 0 && size > constraints.MaxSize)
            throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                Texts.UploadFileTooBig.ToString(localizer),
                UploadFormatting.FileSizeDisplay(constraints.MaxSize)));

        var allowedExtensions = constraints.AllowedExtensions;
        if (!string.IsNullOrEmpty(allowedExtensions) &&
            !IsExtensionIn(allowedExtensions, fileExtension))
        {
            throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                Texts.ExtensionNotAllowed.ToString(localizer),
                fileExtension, constraints.AllowedExtensions));
        }

        var imageExtensions = constraints.ImageExtensions ?? UploadOptions.DefaultImageExtensions;
        if (string.IsNullOrEmpty(imageExtensions) ||
            !imageExtensions.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Any(x => string.Equals(x, fileExtension, StringComparison.OrdinalIgnoreCase)))
        {
            if (constraints.AllowNonImage == true)
                return;

            if (string.IsNullOrEmpty(imageExtensions))
                throw new ValidationError(
                    Texts.NotAnImageFile.ToString(localizer));

            throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                Texts.NotAnImageWithExtensions.ToString(localizer),
                fileExtension, constraints.ImageExtensions));
        }

        isImageExtension = true;
    }

    private static readonly char[] extSep = new char[] { ',', ';' };

    private bool IsExtensionIn(string extensionList, string extension)
    {
        if (string.IsNullOrEmpty(extensionList))
            return false;

        extension = extension?.Trim();

        foreach (var x in extensionList.Split(extSep, StringSplitOptions.RemoveEmptyEntries))
        {
            string ext = x.Trim();
            if (ext == "." && string.IsNullOrEmpty(extension))
                return true;

            if (string.Equals(ext, extension, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void ValidateImage(IUploadImageContrains constraints, Stream stream, 
        string filename, out object image)
    {
        if (constraints is null)
            throw new ArgumentNullException(nameof(constraints));

        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        if (filename is null)
            throw new ArgumentNullException(nameof(filename));

        var fileExtension = Path.GetExtension(filename);

        image = null;
        try
        {
            var checker = new ImageChecker
            {
                MinWidth = constraints.MinWidth,
                MaxWidth = constraints.MaxWidth,
                MinHeight = constraints.MinHeight,
                MaxHeight = constraints.MaxHeight
            };

            ImageCheckResult result = checker.CheckStream(stream, imageProcessor, returnImage: true, 
                out var imageObj, out var formatInfo, logger);

            image = imageObj;

            if (result != ImageCheckResult.Valid)
            {
                if (constraints?.IgnoreInvalidImage == true &&
                    result == ImageCheckResult.InvalidImage)
                    return;

                if (constraints?.IgnoreEmptyImage == true &&
                    result == ImageCheckResult.ImageIsEmpty)
                    return;

                var error = checker.FormatErrorMessage(result, localizer);
                throw new ValidationError(error);
            }

            if (constraints.IgnoreExtensionMismatch != true &&
                !formatInfo.FileExtensions.Any(x => string.Equals(x, fileExtension,
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationError(string.Format(CultureInfo.CurrentCulture,
                    Texts.ImageExtensionMismatch.ToString(localizer),
                    fileExtension, formatInfo.MimeType));
            }
        }
        catch
        {
            (image as IDisposable)?.Dispose();
            throw;
        }

    }

}
