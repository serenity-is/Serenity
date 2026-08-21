#nullable enable
#if ISSOURCEGENERATOR
namespace Serenity.CodeGeneration;
#else
namespace Serenity;
#endif

/// <summary>
/// Base file system abstraction for accessing the physical disk, allowing
/// file and directory operations to be mocked or replaced.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Creates a file for writing, optionally overwriting it if it already exists.
    /// </summary>
    /// <param name="path">The path of the file to create.</param>
    /// <param name="overwrite"><c>true</c> to overwrite the file if it already exists.</param>
    /// <returns>A stream for writing to the created file.</returns>
    System.IO.Stream CreateFile(string path, bool overwrite = true);

    /// <summary>
    /// Creates a directory.
    /// </summary>
    /// <param name="path">The path of the directory to create.</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Deletes the directory at the specified path.
    /// </summary>
    /// <param name="path">The path of the directory to delete.</param>
    /// <param name="recursive"><c>true</c> to delete the directory recursively.</param>
    void DeleteDirectory(string path, bool recursive = false);

    /// <summary>
    /// Deletes the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file to delete.</param>
    void DeleteFile(string path);

    /// <summary>
    /// Checks whether a directory exists at the specified path.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
    bool DirectoryExists(string path);

    /// <summary>
    /// Checks whether a file exists at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Gets the directories at the specified path.
    /// </summary>
    /// <param name="path">The path of the directory to list.</param>
    /// <param name="searchPattern">The search pattern used to filter directory names.</param>
    /// <param name="recursive"><c>true</c> to scan subdirectories recursively.</param>
    /// <returns>An array of directory paths.</returns>
    string[] GetDirectories(string path, string searchPattern = "*", bool recursive = false);

    /// <summary>
    /// Gets the files at the specified path.
    /// </summary>
    /// <param name="path">The path of the directory to list.</param>
    /// <param name="searchPattern">The search pattern used to filter file names.</param>
    /// <param name="recursive"><c>true</c> to scan subdirectories recursively.</param>
    /// <returns>An array of file paths.</returns>
    string[] GetFiles(string path, string searchPattern = "*", bool recursive = false);

    /// <summary>
    /// Gets the size of the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The size of the file in bytes.</returns>
    long GetFileSize(string path);

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The absolute path.</returns>
    string GetFullPath(string path);

    /// <summary>
    /// Gets the last write time in UTC for the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The last write time of the file in UTC.</returns>
    DateTime GetLastWriteTimeUtc(string path);

    /// <summary>
    /// Returns a relative path from one path to another.
    /// </summary>
    /// <param name="relativeTo">The source path the result should be relative to.</param>
    /// <param name="path">The destination path.</param>
    /// <returns>The relative path.</returns>
    string GetRelativePath(string relativeTo, string path);

    /// <summary>
    /// Opens the file at the specified path for reading.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>A stream for reading from the file.</returns>
    System.IO.Stream OpenRead(string path);

    /// <summary>
    /// Reads all bytes from the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <returns>The bytes read from the file.</returns>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Reads all text from the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="encoding">The optional encoding to use when reading the file.</param>
    /// <returns>The text content of the file.</returns>
    string ReadAllText(string path, Encoding? encoding = null);

    /// <summary>
    /// Writes all bytes to the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="content">The bytes to write.</param>
    void WriteAllBytes(string path, byte[] content);

    /// <summary>
    /// Writes content to the file at the specified path.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="encoding">The optional encoding to use when writing the file.</param>
    void WriteAllText(string path, string content, Encoding? encoding = null);
}
