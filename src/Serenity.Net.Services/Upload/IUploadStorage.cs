using System.IO;

namespace Serenity.Web;

/// <summary>
/// Abstraction for an upload storage
/// </summary>
public interface IUploadStorage
{
    /// <summary>
    /// Copies the file to the archive folder
    /// </summary>
    /// <param name="path">Source file path</param>
    /// <returns>The archived file path</returns>
    string ArchiveFile(string path);

    /// <summary>
    /// Copies a file from another upload storage and returns the resulting file path
    /// </summary>
    /// <param name="sourceStorage">Source upload storage</param>
    /// <param name="sourcePath">Source file path</param>
    /// <param name="targetPath">Target file path</param>
    /// <param name="overwrite">Overwrite option</param>
    string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, OverwriteOption overwrite);
    
    /// <summary>
    /// Deletes the file at path
    /// </summary>
    /// <param name="path">File path</param>
    void DeleteFile(string path);

    /// <summary>
    /// Returns if a file at path exists
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns></returns>
    bool FileExists(string path);

    /// <summary>
    /// Gets the URL for a file
    /// </summary>
    /// <param name="path">File path</param>
    string GetFileUrl(string path);

    /// <summary>
    /// Gets file size
    /// </summary>
    /// <param name="path">File path</param>
    /// <returns></returns>
    long GetFileSize(string path);

    /// <summary>
    /// Gets list of files matching a search pattern
    /// </summary>
    /// <param name="path">Path of the directory to perform search</param>
    /// <param name="searchPattern">Search pattern</param>
    string[] GetFiles(string path, string searchPattern);

    /// <summary>
    /// Gets metadata dictionary for a file
    /// </summary>
    /// <param name="path">File path</param>
    IDictionary<string, string> GetFileMetadata(string path);
    
    /// <summary>
    /// Sets file metadata for a file
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="metadata">Metadata dictionary</param>
    /// <param name="overwriteAll">True to override any existing metadata for file,
    /// false to merge existing metadata by their key.</param>
    void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll);
    
    /// <summary>
    /// Opens a file stream. The caller should dispose the stream.
    /// </summary>
    /// <param name="path">File path</param>
    Stream OpenFile(string path);
    
    /// <summary>
    /// Purges temporary files. Only useful for temporary storage.
    /// </summary>
    void PurgeTemporaryFiles();

    /// <summary>
    /// Writes a file
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="source">Source stream</param>
    /// <param name="overwrite">Overwrite option</param>
    string WriteFile(string path, Stream source, OverwriteOption overwrite);
}