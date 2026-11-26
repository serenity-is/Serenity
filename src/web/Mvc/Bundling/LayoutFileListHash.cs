namespace Serenity.Web;

/// <summary>
/// Computes a short hash of a list of layout files, used to tell the different file sets
/// of the layout bundles apart, e.g. for users with different permissions.
/// </summary>
public static class LayoutFileListHash
{
    /// <summary>
    /// Computes the hash of the file list, which depends on the order of the files.
    /// </summary>
    /// <param name="files">The file urls</param>
    public static string Compute(IEnumerable<string> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var hash = System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(string.Join("\n", files)));
        return Convert.ToHexString(hash);
    }
}
