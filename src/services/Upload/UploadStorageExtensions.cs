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

    private static readonly string[] PrimaryExtensionsForLegacyThumbs = 
        [".jpg", ".png", ".gif", ".jpeg", ".bmp", ".tiff", ".webp"];

    /// <summary>
    /// Gets the primary file path from a thumbnail path
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="thumbPath">Thumb file path</param>
    public static string GetPrimaryFileFromThumb(this IUploadStorage uploadStorage, string thumbPath)
    {
        if (!UploadPathHelper.TryParseThumbSuffix(thumbPath,
            out var baseName, out _, out _, out _))
            return null;

        var metadata = uploadStorage.GetFileMetadata(thumbPath);
        if (metadata != null &&
            metadata.TryGetValue(FileMetadataKeys.PrimaryFileExtension, out var primaryExt) &&
            primaryExt != null)
            return baseName + primaryExt;

        if (metadata == null ||
            metadata.Count == 0)
        {
            // previous thumbs did not have any metadata, so accept them for compat
            foreach (var ext in PrimaryExtensionsForLegacyThumbs)
            {
                if (uploadStorage.FileExists(baseName + ext))
                    return baseName + ext;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets thumbnail files for a source file
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <param name="path">Full source file path</param>
    public static IEnumerable<string> GetThumbnailFiles(this IUploadStorage uploadStorage, string path)
    {
        var sourceBase = Path.ChangeExtension(path, null);

        foreach (var thumbPath in uploadStorage.GetThumbnailFiles(path))
        {
            if (!UploadPathHelper.TryParseThumbSuffix(thumbPath,
                out var baseName, out _, out _, out _) ||
                !string.Equals(sourceBase, baseName, StringComparison.OrdinalIgnoreCase))
                continue;

            var thumbMetadata = uploadStorage.GetFileMetadata(thumbPath);

            // previous thumbs did not have any metadata, so accept them for compat
            if (thumbMetadata != null && thumbMetadata.Count > 0)
            {
                // thumb must be marked as thumbnail in metadata
                if (!thumbMetadata.TryGetValue(FileMetadataKeys.IsThumbnail, out var isThumb) ||
                    !string.Equals(isThumb, "true", StringComparison.OrdinalIgnoreCase))
                    continue;

                // if primary file extension is set, it must match source file extension
                if (thumbMetadata.TryGetValue(FileMetadataKeys.PrimaryFileExtension, out var primaryExt) &&
                    !string.Equals(primaryExt, Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            yield return thumbPath;
        }
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
        ArgumentNullException.ThrowIfNull(uploadStorage);
        ArgumentNullException.ThrowIfNull(options);

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
        ArgumentNullException.ThrowIfNull(uploadStorage);

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
        ArgumentNullException.ThrowIfNull(uploadStorage);

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
        ArgumentNullException.ThrowIfNull(uploadStorage);

        var metadata = new Dictionary<string, string>()
        {
            [FileMetadataKeys.OriginalName] = originalName
        };

        uploadStorage.SetFileMetadata(path, metadata, overwriteAll: false);
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
