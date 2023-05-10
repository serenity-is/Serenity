using Serenity.IO;

namespace Serenity.Web;

/// <summary>
/// Implementation of the <see cref="IDiskUploadFileSystem" />
/// </summary>
public class PhysicalDiskUploadFileSystem : PhysicalFileSystem, IDiskUploadFileSystem
{
    /// <inheritdoc/>
    public void TryDeleteMarkedFiles(string path)
    {
        TemporaryFileHelper.TryDeleteMarkedFiles(path);
    }

    /// <inheritdoc/>
    public void Delete(string path, DeleteType deleteType)
    {
        TemporaryFileHelper.Delete(path, deleteType);
    }

    /// <inheritdoc/>
    public void PurgeDirectory(string directoryToClean, TimeSpan? autoExpireTime = null, int? maxFilesInDirectory = null, string checkFileName = null)
    {
        TemporaryFileHelper.PurgeDirectory(directoryToClean, 
            autoExpireTime ?? TemporaryFileHelper.DefaultAutoExpireTime, 
            maxFilesInDirectory ?? TemporaryFileHelper.DefaultMaxFilesInDirectory, 
            checkFileName ?? TemporaryFileHelper.DefaultTemporaryCheckFile);
    }

    /// <inheritdoc/>
    public void TryDeleteOrMark(string path)
    {
        TemporaryFileHelper.TryDeleteOrMark(path);
    }
}