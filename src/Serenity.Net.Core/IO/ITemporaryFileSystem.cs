#nullable enable
namespace Serenity;

/// <summary>
/// Base file system used by TemporaryFileHelper
/// </summary>
public interface ITemporaryFileSystem : IFileSystem
{
    /// <summary>
    /// Gets temporary file info objects
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns>Array of temporary file info objects</returns>
    TemporaryFileInfo[] GetTemporaryFileInfos(string path);

    /// <summary>
    /// Gets last write time for file at path
    /// </summary>
    /// <param name="path">File path</param>
    DateTime GetLastWriteTimeUtc(string path);
}