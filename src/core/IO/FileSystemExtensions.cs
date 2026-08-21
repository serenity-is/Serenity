namespace Serenity;

/// <summary>
/// Provides extension methods for <see cref="IFileSystem"/> that delegate to
/// <see cref="System.IO.Path"/> helpers, reducing direct dependency on System.IO
/// in case these methods are later added to <see cref="IFileSystem"/>.
/// </summary>
public static class FileSystemExtensions
{
    /// <summary>
    /// Changes the extension of a path string.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path information to modify. The path cannot contain any of the characters
    /// defined in <see cref="System.IO.Path.GetInvalidPathChars"/>.</param>
    /// <param name="extension">The new extension (with or without a leading period). Specify <c>null</c> to remove
    /// an existing extension from <paramref name="path"/>.</param>
    /// <returns>The modified path.</returns>
    public static string ChangeExtension(this IFileSystem _, string path, string extension)
    {
        return System.IO.Path.ChangeExtension(path, extension);
    }

    /// <summary>
    /// Combines two path strings.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path1">The first path.</param>
    /// <param name="path2">The second path.</param>
    /// <returns>The combined path.</returns>
    public static string Combine(this IFileSystem _, string path1, string path2)
    {
        return System.IO.Path.Combine(path1, path2);
    }

    /// <summary>
    /// Combines three path strings.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path1">The first path.</param>
    /// <param name="path2">The second path.</param>
    /// <param name="path3">The third path.</param>
    /// <returns>The combined path.</returns>
    public static string Combine(this IFileSystem _, string path1, string path2, string path3)
    {
        return System.IO.Path.Combine(path1, path2, path3);
    }

    /// <summary>
    /// Combines an array of path strings.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="paths">The paths to combine.</param>
    /// <returns>The combined path.</returns>
    public static string Combine(this IFileSystem _, params string[] paths)
    {
        return System.IO.Path.Combine(paths);
    }

    /// <summary>
    /// Copies a file to another destination, optionally overwriting the destination if it exists.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="sourceFileName">The source file name.</param>
    /// <param name="destFileName">The destination file name.</param>
    /// <param name="overwrite"><c>true</c> to overwrite the destination file if it already exists.</param>
    public static void Copy(this IFileSystem fileSystem, string sourceFileName, string destFileName, bool overwrite)
    {
        using var sourceStream = fileSystem.OpenRead(sourceFileName);
        using var destStream = fileSystem.CreateFile(destFileName, overwrite);
        sourceStream.CopyTo(destStream);
    }

    /// <summary>
    /// Gets the directory name for the specified path.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>The directory name, or <c>null</c> if the path does not contain a directory.</returns>
    public static string GetDirectoryName(this IFileSystem _, string path)
    {
        return System.IO.Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Gets the file name for the specified path.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>The file name.</returns>
    public static string GetFileName(this IFileSystem _, string path)
    {
        return System.IO.Path.GetFileName(path);
    }

    /// <summary>
    /// Gets the file name without its extension for the specified path.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>The file name without its extension.</returns>
    public static string GetFileNameWithoutExtension(this IFileSystem _, string path)
    {
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Gets the extension for the specified path.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns>The extension, including the leading period, or an empty string if there is no extension.</returns>
    public static string GetExtension(this IFileSystem _, string path)
    {
        return System.IO.Path.GetExtension(path);
    }

    /// <summary>
    /// Gets a value indicating whether the specified path is rooted.
    /// </summary>
    /// <param name="_">The file system.</param>
    /// <param name="path">The path.</param>
    /// <returns><c>true</c> if the path is rooted; otherwise, <c>false</c>.</returns>
    public static bool IsPathRooted(this IFileSystem _, string path)
    {
        return System.IO.Path.IsPathRooted(path);
    }
}