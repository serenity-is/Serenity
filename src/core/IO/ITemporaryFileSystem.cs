#nullable enable
namespace Serenity;

/// <summary>
/// A file system abstraction used by <see cref="Serenity.IO.TemporaryFileHelper"/> that
/// additionally exposes temporary file information.
/// </summary>
public interface ITemporaryFileSystem : IFileSystem
{
    /// <summary>
    /// Gets the temporary file info objects for the files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>An array of temporary file info objects.</returns>
    TemporaryFileInfo[] GetTemporaryFileInfos(string path);
}