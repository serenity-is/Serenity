using System.IO;

namespace Serenity;

/// <summary>
/// Contains Path related helper functions.
/// </summary>
public static class PathHelper
{
    static readonly char[] invalidChars = Path.GetInvalidFileNameChars()
        .Where(x => x != '/' && x != '\\').ToArray();

    /// <summary>
    /// Checks whether given path is a secure relative path
    /// </summary>
    /// <param name="relativePath">Relative path</param>
    /// <returns>True if relative path looks safe</returns>
    public static bool IsSecureRelativePath(string? relativePath)
    {
        var trim = relativePath.TrimToNull();

        return !(trim == null ||
            trim == "." ||
            trim == ".." ||
            relativePath!.IndexOf("../", StringComparison.Ordinal) >= 0 ||
            relativePath!.IndexOf("..\\", StringComparison.Ordinal) >= 0 ||
            relativePath!.IndexOf(':') >= 0 ||
            trim.StartsWith("/") ||
            trim.StartsWith("\\") ||
            Path.IsPathRooted(relativePath) ||
            relativePath.IndexOfAny(invalidChars) >= 0 ||
            !Path.Combine("a/", relativePath).StartsWith("a/"));
    }

    /// <summary>
    /// Combines a path and filename checking security
    /// </summary>
    /// <param name="root">Root path</param>
    /// <param name="relativePath">Relative path. Should not be rooted, not containing .. etc.</param>
    /// <returns>Combined path</returns>
    public static string SecureCombine(string root, string relativePath)
    {
        if (string.IsNullOrEmpty(root))
            throw new ArgumentNullException(nameof(root));

        if (relativePath == null)
            throw new ArgumentNullException(nameof(relativePath));

        if (relativePath.Length > 0 && !IsSecureRelativePath(relativePath))
            throw new ArgumentOutOfRangeException(nameof(relativePath));

        return Path.Combine(root, relativePath);
    }

    /// <summary>
    /// Checks whether given path is a secure relative file
    /// </summary>
    /// <param name="relativeFile">Relative file</param>
    /// <returns>True if relative file looks safe</returns>
    public static bool IsSecureRelativeFile(string? relativeFile)
    {
        if (!IsSecureRelativePath(relativeFile))
            return false;

        var trimRight = relativeFile!.TrimEnd();
        if (trimRight.EndsWith('/') || trimRight.EndsWith('\\'))
            return false;

        return true;
    }

    /// <summary>
    /// Validates if filename is a secure relative file
    /// </summary>
    /// <param name="filename">Relative filename</param>
    public static void ValidateSecureRelativeFile(string? filename)
    {
        if (!IsSecureRelativeFile(filename))
            throw new ArgumentOutOfRangeException(nameof(filename));
    }

    /// <summary>
    ///   Converts backslashes to forward slashes</summary>
    /// <param name="fileName">
    ///   Filename.</param>
    /// <returns>
    ///   Converted filename.</returns>
    public static string? ToUrl(string? fileName)
    {
        if (fileName != null && fileName.IndexOf('\\') >= 0)
            return fileName.Replace('\\', '/');
        else
            return fileName;
    }

    /// <summary>
    ///   Converts forward slashes to backslashes</summary>
    /// <param name="fileName">
    ///   Filename.</param>
    /// <returns>
    ///   Converted filename.</returns>
    public static string? ToPath(string? fileName)
    {
        var separator = Path.DirectorySeparatorChar;
        var opposite = separator == '/' ? '\\' : '/';
        if (fileName != null && fileName.IndexOf(opposite) >= 0)
            return fileName.Replace(opposite, separator);
        else
            return fileName;
    }
}