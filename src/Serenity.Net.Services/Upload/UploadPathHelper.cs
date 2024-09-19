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
    /// <param name="thumbSuffix">Thumb suffix, default is "_t.jpg"</param>
    /// <returns></returns>
    public static string GetThumbnailName(string path, string thumbSuffix = "_t.jpg")
    {
        if (string.IsNullOrEmpty(path))
            return path;

        return Path.ChangeExtension(path, null) + thumbSuffix;
    }

    /// <summary>
    /// Tries to parse a thumbnail filename suffix, e.g. it ends with "_t.jpg",
    /// or "_tNxN.jpg" where N is a number
    /// </summary>
    /// <param name="filename">Filename</param>
    /// <param name="baseName">Base name of the file</param>
    /// <param name="suffix">Thumb suffix</param>
    /// <param name="width">Thumb width</param>
    /// <param name="height">Thumb height</param>
    public static bool TryParseThumbSuffix(string filename,
        out string baseName, out string suffix, out int width, out int height)
    {
        width = -1;
        height = -1;
        baseName = null;
        suffix = null;

        if (string.IsNullOrEmpty(filename))
            return false;

        if (!filename.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            return false;

        var tIndex = filename.LastIndexOf("_t", StringComparison.OrdinalIgnoreCase);
        if (tIndex < 0)
            return false;

        baseName = filename[..tIndex];
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
}
