#nullable enable
namespace Serenity;

/// <summary>
/// Base file system for abstracting physical disk access
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Creates a file for writing or optionally overwrites if exists
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="overwrite">Overwrite if file exists</param>
    System.IO.Stream CreateFile(string path, bool overwrite = true);

    /// <summary>
    /// Creates a directory
    /// </summary>
    /// <param name="path">Path to the directory</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Deletes the directory at path
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <param name="recursive">True to delete directory recursively</param>
    void DeleteDirectory(string path, bool recursive = false);

    /// <summary>
    /// Deletes the file at path
    /// </summary>
    /// <param name="path">The path to file</param>
    void DeleteFile(string path);

    /// <summary>
    /// Checks if directory exists
    /// </summary>
    /// <param name="path">Path to the directory</param>
    /// <returns>True if exists</returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// Checks if file exists
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <returns>True if exists</returns>
    bool FileExists(string path);

    /// <summary>
    /// Gets directories at specified path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern">Search pattern</param>
    /// <param name="recursive">Scan subdirectories recursively</param>
    /// <returns>List of directories</returns>
    string[] GetDirectories(string path, string searchPattern = "*", bool recursive = false);

    /// <summary>
    /// Gets files at specified path
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="searchPattern">Search pattern</param>
    /// <param name="recursive">Scan subdirectories recursively</param>
    string[] GetFiles(string path, string searchPattern = "*", bool recursive = false);

    /// <summary>
    /// Gets file size for specified path
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Size of the file at path</returns>
    long GetFileSize(string path);

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns>Absolute path</returns>
    string GetFullPath(string path);

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="relativeTo">The source path the result should be relative to.</param>
    /// <param name="path">File path</param>
    /// <returns>Absolute path</returns>
    string GetRelativePath(string relativeTo, string path);

    /// <summary>
    /// Opens the file at path for reading
    /// </summary>
    /// <param name="path">Path</param>
    System.IO.Stream OpenRead(string path);

    /// <summary>
    /// Reads all bytes from a file
    /// </summary>
    /// <param name="path">Path to the file</param>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Reads all text from a file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="encoding">Optional encoding</param>
    string ReadAllText(string path, Encoding? encoding = null);

    /// <summary>
    /// Writes all bytes to a file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="content">Bytes to write</param>
    void WriteAllBytes(string path, byte[] content);

    /// <summary>
    /// Writes content to a file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="content">Content</param>
    /// <param name="encoding">Optional encoding</param>
    void WriteAllText(string path, string content, Encoding? encoding = null);
}
