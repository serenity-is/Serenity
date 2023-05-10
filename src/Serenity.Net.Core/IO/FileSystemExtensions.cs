namespace Serenity;

/// <summary>
/// IFileSystem extensions to reduce dependency on System.IO, in case these methods are also added to IFileSystem in the future
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>Changes the extension of a path string.</summary>
    /// <param name="_">File system</param>
    /// <param name="path">The path information to modify. The path cannot contain any of the characters
    /// defined in System.IO.Path.GetInvalidPathChars.</param>
    /// <param name="extension">The new extension (with or without a leading period). Specify null to remove
    /// an existing extension from path.</param>
    public static string ChangeExtension(this IFileSystem _, string path, string extension)
    {
        return System.IO.Path.ChangeExtension(path, extension);
    }

    /// <summary>
    /// Combines path strings
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path1">Path 1</param>
    /// <param name="path2">Path 2</param>
    public static string Combine(this IFileSystem _, string path1, string path2)
    {
        return System.IO.Path.Combine(path1, path2);
    }

    /// <summary>
    /// Combines path strings
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path1">Path 1</param>
    /// <param name="path2">Path 2</param>
    /// <param name="path3">Path 3</param>
    public static string Combine(this IFileSystem _, string path1, string path2, string path3)
    {
        return System.IO.Path.Combine(path1, path2, path3);
    }

    /// <summary>
    /// Combines path strings
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="paths">Paths to combine</param>
    public static string Combine(this IFileSystem _, params string[] paths)
    {
        return System.IO.Path.Combine(paths);
    }

    /// <summary>
    /// Copies a file to another destination by optionally overwriting the destination if it exists
    /// </summary>
    /// <param name="fileSystem">File system</param>
    /// <param name="sourceFileName">Source file name</param>
    /// <param name="destFileName">Destion</param>
    /// <param name="overwrite"></param>
    public static void Copy(this IFileSystem fileSystem, string sourceFileName, string destFileName, bool overwrite)
    {
        using var sourceStream = fileSystem.OpenRead(sourceFileName);
        using var destStream = fileSystem.CreateFile(destFileName, overwrite);
        sourceStream.CopyTo(destStream);
    }

    /// <summary>
    /// Gets directory name for path
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path">Path</param>
    public static string GetDirectoryName(this IFileSystem _, string path)
    {
        return System.IO.Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Gets file name for path
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path">Path</param>
    public static string GetFileName(this IFileSystem _, string path)
    {
        return System.IO.Path.GetFileName(path);
    }

    /// <summary>
    /// Gets file name without extension for path
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path">Path</param>
    public static string GetFileNameWithoutExtension(this IFileSystem _, string path)
    {
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Gets extension for path
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path">Path</param>
    public static string GetExtension(this IFileSystem _, string path)
    {
        return System.IO.Path.GetExtension(path);
    }

    /// <summary>
    /// Gets if the path is rooted
    /// </summary>
    /// <param name="_">File system</param>
    /// <param name="path">Path</param>
    public static bool IsPathRooted(this IFileSystem _, string path)
    {
        return System.IO.Path.IsPathRooted(path);
    }
}