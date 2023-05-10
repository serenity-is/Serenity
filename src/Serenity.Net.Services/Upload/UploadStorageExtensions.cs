using System.IO;

namespace Serenity.Web;

/// <summary>
/// Extension methods for <see cref="IUploadStorage"/> and related classes
/// </summary>
public static class UploadStorageExtensions
{
    /// <summary>
    /// Gets thumbnail URL for the file path
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">Path</param>
    /// <returns></returns>
    public static string GetThumbnailUrl(this IUploadStorage uploadStorage, string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        var thumb = UploadPathHelper.GetThumbnailName(path);

        return uploadStorage.GetFileUrl(thumb);
    }
  
    /// <summary>
    /// Copies a temporary file to its target location
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="options">Copy options</param>
    /// <exception cref="ArgumentNullException">uploadStorage is null</exception>
    public static CopyTemporaryFileResult CopyTemporaryFile(this IUploadStorage uploadStorage, 
        CopyTemporaryFileOptions options)
    {
        if (uploadStorage is null)
            throw new ArgumentNullException(nameof(uploadStorage));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        long size = uploadStorage.GetFileSize(options.TemporaryFile);
        string path = PathHelper.ToUrl(UploadFormatting.FormatFilename(options));
        path = uploadStorage.CopyFrom(uploadStorage, options.TemporaryFile, path, OverwriteOption.AutoRename);
        bool hasThumbnail = uploadStorage.FileExists(UploadPathHelper.GetThumbnailName(options.TemporaryFile));

        var result = new CopyTemporaryFileResult()
        {
            FileSize = size,
            Path = path,
            OriginalName = options.OriginalName,
            HasThumbnail = hasThumbnail
        };

        options.FilesToDelete?.RegisterNewFile(path);
        options.FilesToDelete?.RegisterOldFile(options.TemporaryFile);
        return result;
    }

    /// <summary>
    /// Reads all file bytes
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">File path</param>
    /// <exception cref="ArgumentNullException">Upload storage is null</exception>
    public static byte[] ReadAllFileBytes(this IUploadStorage uploadStorage, string path)
    {
        if (uploadStorage is null)
            throw new ArgumentNullException(nameof(uploadStorage));

        using var ms = new MemoryStream();
        using (var fs = uploadStorage.OpenFile(path))
            fs.CopyTo(ms);

        return ms.ToArray();
    }

    /// <summary>
    /// Gets original name of a file
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">File path</param>
    /// <exception cref="ArgumentNullException">uploadStorage is null</exception>
    public static string GetOriginalName(this IUploadStorage uploadStorage, string path)
    {
        if (uploadStorage is null)
            throw new ArgumentNullException(nameof(uploadStorage));

        var metadata = uploadStorage.GetFileMetadata(path);
        if (metadata != null &&
            metadata.TryGetValue(FileMetadataKeys.OriginalName, out string originalName))
            return originalName;

        return null;
    }

    /// <summary>
    /// Sets original name for a file
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">File path</param>
    /// <param name="originalName">Original name</param>
    /// <exception cref="ArgumentNullException">Upload storage is null</exception>
    public static void SetOriginalName(this IUploadStorage uploadStorage, string path, string originalName)
    {
        if (uploadStorage is null)
            throw new ArgumentNullException(nameof(uploadStorage));

        var metadata = new Dictionary<string, string>()
        {
            [FileMetadataKeys.OriginalName] = originalName
        };

        uploadStorage.SetFileMetadata(path, metadata, overwriteAll: false);
    }

    /// <summary>
    /// Scales an image and saves it to an upload storage file
    /// </summary>
    /// <param name="image">Source image</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="width">Target width</param>
    /// <param name="height">Target height</param>
    /// <param name="mode">Scale mode</param>
    /// <param name="backgroundColor">Pad color</param>
    /// <param name="mimeType">Mime type of target image file</param>
    /// <param name="encoderParams">Encoder parameters for target image</param>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">Path</param>
    /// <param name="overwrite">Overwrite option</param>
    /// <exception cref="ArgumentNullException">One of inputs is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Width or height is less than zero</exception>
    public static string ScaleImageAs(object image, IImageProcessor imageProcessor,
        int width, int height, ImageScaleMode mode, string backgroundColor, 
        string mimeType, ImageEncoderParams encoderParams,
        IUploadStorage uploadStorage, string path, OverwriteOption overwrite)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (imageProcessor is null)
            throw new ArgumentNullException(nameof(imageProcessor));

        if (uploadStorage is null)
            throw new ArgumentNullException(nameof(uploadStorage));

        if (width < 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        if (height < 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        if (width == 0 && height == 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        var scaledImage = imageProcessor.Scale(image, width, height, mode, backgroundColor, inplace: false);
        try
        {
            using var ms = new MemoryStream();
            imageProcessor.Save(scaledImage, ms, mimeType, encoderParams);
            ms.Seek(0, SeekOrigin.Begin);
            return uploadStorage.WriteFile(path, ms, overwrite);
        }
        finally
        {
            (scaledImage as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Scales the temporary image with provided upload image options if required 
    /// based on the options and saves the result to the target upload storage file
    /// </summary>
    /// <param name="image">Image object</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="options">Image upload options</param>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="temporaryFile">Temporary input file</param>
    /// <param name="overwrite">Overwrite </param>
    /// <returns>The resulting image file path</returns>
    /// <exception cref="ArgumentNullException">image or options is null</exception>
    public static string ScaleImage(object image,
        IImageProcessor imageProcessor, IUploadImageOptions options,
        IUploadStorage uploadStorage, string temporaryFile, OverwriteOption overwrite)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(temporaryFile))
            throw new ArgumentNullException(nameof(temporaryFile));

        var (imageWidth, imageHeight) = imageProcessor.GetImageSize(image);
        var scaleSmaller = options.ScaleSmaller == true;

        var baseFile = Path.ChangeExtension(temporaryFile, null);
        if ((options.ScaleWidth > 0 || options.ScaleHeight > 0) &&
            (options.ScaleWidth != imageWidth || options.ScaleHeight != imageHeight) &&
            ((options.ScaleWidth > 0 && (scaleSmaller || options.ScaleWidth < imageWidth)) ||
             (options.ScaleHeight > 0 && (scaleSmaller || options.ScaleHeight < imageHeight))))
        {
            var originalName = uploadStorage.GetOriginalName(temporaryFile);
            temporaryFile = ScaleImageAs(image, imageProcessor,
                options.ScaleWidth, options.ScaleHeight, options.ScaleMode, options.ScaleBackColor,
                mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ScaleQuality },
                uploadStorage, baseFile + ".jpg", overwrite);
            if (!string.IsNullOrEmpty(originalName))
                uploadStorage.SetOriginalName(temporaryFile, Path.ChangeExtension(originalName, ".jpg"));
        }
        return temporaryFile;
    }

    /// <summary>
    /// Creates the default thumbnail for image if the size is provided
    /// in the upload image options (ThumbWidth and ThumbHeight >= 0) and saves it to the target upload storage file
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="options">Upload image options</param>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="temporaryFile">Input temporary file</param>
    /// <param name="overwrite">Overwrite option</param>
    /// <exception cref="ArgumentNullException">image, options or temporaryFile is null</exception>
    public static string CreateDefaultThumb(object image,
        IImageProcessor imageProcessor, IUploadImageOptions options,
        IUploadStorage uploadStorage, string temporaryFile, OverwriteOption overwrite)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(temporaryFile))
            throw new ArgumentNullException(nameof(temporaryFile));

        if ((options.ThumbWidth > 0 || options.ThumbHeight > 0) &&
            (options.ThumbWidth >= 0 && options.ThumbHeight >= 0))
        {
            return ScaleImageAs(image, imageProcessor,
                options.ThumbWidth, options.ThumbHeight, options.ThumbMode, options.ThumbBackColor,
                mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ThumbQuality },
                uploadStorage, Path.ChangeExtension(temporaryFile, null) + "_t.jpg", overwrite);
        }

        return null;
    }

    /// <summary>
    /// Creates additional thumbs if specified in the upload image options,
    /// and saves them to the target upload storage
    /// </summary>
    /// <param name="image">Image</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="options">Upload image options</param>
    /// <param name="uploadStorage">Target upload storage</param>
    /// <param name="temporaryFile">Input temporary file</param>
    /// <param name="overwrite">Overwrite option</param>
    /// <exception cref="ArgumentNullException">image, options or temporaryFile is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">options.ThumbSizes contains invalid values</exception>
    public static void CreateAdditionalThumbs(object image,
        IImageProcessor imageProcessor, IUploadImageOptions options,
        IUploadStorage uploadStorage, string temporaryFile, OverwriteOption overwrite)
    {
        if (image is null)
            throw new ArgumentNullException(nameof(image));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrEmpty(temporaryFile))
            throw new ArgumentNullException(nameof(temporaryFile));

        var thumbSizes = options.ThumbSizes.TrimToNull();
        if (thumbSizes is null)
            return;

        var baseFile = Path.ChangeExtension(temporaryFile, null);

        foreach (var sizeStr in thumbSizes.Replace(";", ",", StringComparison.Ordinal).Split(new[] { ',' }))
        {
            var dims = sizeStr.ToUpperInvariant().Split(new[] { 'X' });
            if (dims.Length != 2 ||
                !int.TryParse(dims[0], out int w) ||
                !int.TryParse(dims[1], out int h) ||
                w < 0 ||
                h < 0 ||
                (w == 0 && h == 0))
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentOutOfRangeException(nameof(thumbSizes));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            var thumbFile = baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg";

            ScaleImageAs(image, imageProcessor,
                w, h, options.ThumbMode, options.ThumbBackColor,
                mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ThumbQuality },
                uploadStorage, thumbFile, overwrite);
        }
    }

    /// <summary>
    /// Depending on the image upload options, scales image, creates default and
    /// additional thumbs and saves them to the upload storage files.
    /// </summary>
    /// <param name="image">Input image</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="options">Upload image options</param>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="temporaryFile">Temporary file</param>
    /// <param name="overwrite">Overwrite option</param>
    /// <returns>Temporary file</returns>
    public static string ScaleImageAndCreateAllThumbs(object image,
        IImageProcessor imageProcessor, IUploadImageOptions options,
        IUploadStorage uploadStorage, string temporaryFile, OverwriteOption overwrite)
    {
        var orgTempFile = temporaryFile;
        temporaryFile = ScaleImage(image, imageProcessor, options, uploadStorage, temporaryFile, overwrite);
        CreateDefaultThumb(image, imageProcessor, options, uploadStorage, orgTempFile, overwrite);
        CreateAdditionalThumbs(image, imageProcessor, options, uploadStorage, orgTempFile, overwrite);
        return temporaryFile;
    }

    /// <summary>
    /// Copies a file from another upload storage and returns the resulting file path
    /// </summary>
    /// <param name="targetStorage">Target upload storage</param>
    /// <param name="sourceStorage">Source upload storage</param>
    /// <param name="sourcePath">Source file path</param>
    /// <param name="targetPath">Target file path</param>
    /// <param name="autoRename">If a file exists at target, true to auto rename,
    /// false to raise an error, and null to overwrite</param>
    [Obsolete("Please use the method with OverwriteOption enumeration")]
    public static string CopyFrom(this IUploadStorage targetStorage, IUploadStorage sourceStorage, string sourcePath, string targetPath, bool? autoRename)
    {
        return targetStorage.CopyFrom(sourceStorage, sourcePath, targetPath, ToOverwriteOption(autoRename));
    }

    /// <summary>
    /// Writes a file
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">File path</param>
    /// <param name="source">Source stream</param>
    /// <param name="autoRename">If a file exists at target, true to auto rename,
    /// false to raise an error, and null to overwrite</param>
    [Obsolete("Please use the method with OverwriteOption enumeration")]
    public static string WriteFile(this IUploadStorage uploadStorage, string path, Stream source, bool? autoRename)
    {
        return uploadStorage.WriteFile(path, source, ToOverwriteOption(autoRename));
    }

    private static OverwriteOption ToOverwriteOption(bool? autoRename)
    {
        return autoRename == null ? OverwriteOption.Overwrite :
            autoRename == true ? OverwriteOption.AutoRename :
            OverwriteOption.Disallowed; 
    }
}
