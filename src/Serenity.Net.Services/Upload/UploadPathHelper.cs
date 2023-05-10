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
            if (baseFileName == null)
                baseFileName = Path.ChangeExtension(path, null);

            path = baseFileName + " (" + tries + ")" + (extension ?? "");
        }

        return path;
    }
}
