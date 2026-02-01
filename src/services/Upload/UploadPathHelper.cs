using Path = System.IO.Path;

namespace Serenity.Web;

/// <summary>
/// Contains helper functions for upload paths
/// </summary>
public static class UploadPathHelper
{
    /// <summary>
    /// Gets thumb file name
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="width">Thumb width</param>
    /// <param name="height">Thumb height</param>
    /// <returns></returns>
    public static string GetThumbnailName(string path, int? width = null, int? height = null)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        path = Path.ChangeExtension(path, null);

        if (width != null && height != null)
            return path + string.Format(CultureInfo.InvariantCulture, SizedThumbFormat, width, height) + ThumbExtension;

        return path + ThumbBaseSuffix + ThumbExtension;
    }

    /// <summary>
    /// Tries to parse a thumbnail filename suffix, e.g. it ends with "_t.jpg",
    /// or "_tNxN.jpg" where N is a number
    /// </summary>
    /// <param name="path">File path, e.g. some/file_t.jpg</param>
    /// <param name="baseName">Base name of the file, e.g. some/file. May include folder</param>
    /// <param name="suffix">Thumb suffix</param>
    /// <param name="width">Thumb width</param>
    /// <param name="height">Thumb height</param>
    public static bool TryParseThumbSuffix(string path,
        out string baseName, out string suffix, out int width, out int height)
    {
        width = -1;
        height = -1;
        baseName = null;
        suffix = null;

        if (string.IsNullOrEmpty(path))
            return false;

        var filename = Path.GetFileName(path);
        if (string.IsNullOrEmpty(filename))
            return false;

        if (!filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            return false;

        var tIndex = filename.LastIndexOf("_t", StringComparison.OrdinalIgnoreCase);
        if (tIndex < 0)
            return false;

        var folder = Path.GetDirectoryName(path);
        baseName = (string.IsNullOrEmpty(folder) ? "" : (PathHelper.ToUrl(folder) + "/")) + filename[..tIndex];
        suffix = filename[tIndex..];
        
        if (string.Equals(suffix, "_t.jpg", StringComparison.OrdinalIgnoreCase))
            return true;

        if (suffix.Length < 9 || 
            !suffix.StartsWith("_t", StringComparison.OrdinalIgnoreCase) || 
            !suffix.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            return false;

        var idx = suffix.IndexOf('x', StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return false;

        if (!int.TryParse(suffix[2..idx], out width) ||
            width <= 0)
            return false;

        if (!int.TryParse(suffix[idx..^4], out height) ||
            height <= 0)
            return false;

        return true;
    }

    /// <summary>
    /// Checks file name security, e.g. it is a relative file (not rooted) that 
    /// does not contain ".." etc.
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static void CheckFileNameSecurity(string path)
    {
        if (!PathHelper.IsSecureRelativeFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));
    }

    /// <summary>
    /// Finds an available name for a file
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="exists">Function that returns if a file exists</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Path or exists is null</exception>
    public static string FindAvailableName(string path, Func<string, bool> exists)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));

        if (exists == null)
            throw new ArgumentNullException(nameof(exists));

        var extension = Path.GetExtension(path);
        string baseFileName = null;
        int tries = 0;
        while (exists(path) && ++tries < 10000)
        {
            baseFileName ??= Path.ChangeExtension(path, null);

            path = baseFileName + " (" + tries + ")" + (extension ?? "");
        }

        return path;
    }

    /// <summary>
    /// Represents the prefix used to identify temporary upload files.
    /// </summary>
    /// <remarks>Use this constant to distinguish files that are stored temporarily during the upload process.
    /// Files with this prefix are typically subject to cleanup or special handling.</remarks>
    public const string TemporaryFilePrefix = "temporary/";

    /// <summary>
    /// Determines whether the specified file name represents a temporary upload file.
    /// </summary>
    /// <param name="fileName">The name of the file to evaluate. Can be null or empty.</param>
    /// <returns>true if the file name is not null or empty and indicates a temporary upload file; otherwise, false.</returns>
    public static bool IsTemporaryFile(string fileName)
    {
        return !string.IsNullOrEmpty(fileName) &&
            fileName.StartsWith(TemporaryFilePrefix, StringComparison.Ordinal);
    }

    /// <summary>
    /// The thumbnail suffix used for thumbnail images. Does not include file extension.
    /// </summary>
    public const string ThumbBaseSuffix = "_t";
    
    /// <summary>
    /// Represents the format string used to generate a thumbnail size suffix with width and height placeholders.
    /// It does not include the file extension.
    /// </summary>
    /// <remarks>This format string can be used with string formatting methods to create a suffix indicating
    /// the dimensions of a thumbnail image. For example, using string.Format(SizedThumbFormat, 100, 200) produces the
    /// string "_t100x200".</remarks>
    public const string SizedThumbFormat = "_t{0}x{1}";

    /// <summary>
    /// Represents the file extension used for thumbnail images in JPEG format.
    /// </summary>
    public const string ThumbExtension = ".jpg";

    /// <summary>
    /// Represents the file extension used for metadata files.
    /// </summary>
    public const string MetaFileExtension = ".meta";
}
