namespace Serenity.Web;

/// <summary>
/// Interface for a files to delete container (<see cref="FilesToDelete"/>)
/// </summary>
public interface IFilesToDelete
{
    /// <summary>
    /// Registers a new file
    /// </summary>
    /// <param name="file">File path</param>
    void RegisterNewFile(string file);

    /// <summary>
    /// Registers an old file
    /// </summary>
    /// <param name="file">File path</param>
    void RegisterOldFile(string file);

    /// <summary>
    /// Keeps new files, while removing old files
    /// </summary>
    void KeepNewFiles();
}