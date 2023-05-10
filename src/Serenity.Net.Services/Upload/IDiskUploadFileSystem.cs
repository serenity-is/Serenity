namespace Serenity.Web;

/// <summary>
/// Extension for <see cref="IFileSystem"/> with disk upload storage specific methods.
/// </summary>
public interface IDiskUploadFileSystem : IFileSystem
{
    /// <summary>
    /// Deletes the file with a DeleteType option
    /// </summary>
    /// <param name="path">File path</param>
    /// <param name="deleteType">Delete type</param>
    void Delete(string path, IO.DeleteType deleteType);

    /// <summary>
    /// Purges target directory, cleaning temporary files
    /// </summary>
    /// <param name="directoryToClean">Directory to clean</param>
    /// <param name="autoExpireTime">Auto expire old files timespan</param>
    /// <param name="maxFilesInDirectory">Max files to keep in directory</param>
    /// <param name="checkFileName">A filename to check existance to confirm the target is a temporary directory.
    /// By default it is ".temporary"</param>
    void PurgeDirectory(string directoryToClean, TimeSpan? autoExpireTime = null, int? maxFilesInDirectory = null, string checkFileName = null);

    /// <summary>
    /// Tries to delete marked files in a directory
    /// </summary>
    /// <param name="path">Target path</param>
    void TryDeleteMarkedFiles(string path);

    /// <summary>
    /// Tries to delete or marks a file for future deletion
    /// </summary>
    /// <param name="path">File path</param>
    void TryDeleteOrMark(string path);
}