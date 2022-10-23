using Serenity.IO;

namespace Serenity.Web;

public class PhysicalDiskUploadFileSystem : PhysicalFileSystem, IDiskUploadFileSystem
{
    public void TryDeleteMarkedFiles(string path)
    {
        TemporaryFileHelper.TryDeleteMarkedFiles(path);
    }

    public void Delete(string path, DeleteType deleteType)
    {
        TemporaryFileHelper.Delete(path, deleteType);
    }

    public void PurgeDirectory(string directoryToClean, TimeSpan? autoExpireTime = null, int? maxFilesInDirectory = null, string checkFileName = null)
    {
        TemporaryFileHelper.PurgeDirectory(directoryToClean, 
            autoExpireTime ?? TemporaryFileHelper.DefaultAutoExpireTime, 
            maxFilesInDirectory ?? TemporaryFileHelper.DefaultMaxFilesInDirectory, 
            checkFileName ?? TemporaryFileHelper.DefaultTemporaryCheckFile);
    }

    public void TryDeleteOrMark(string path)
    {
        TemporaryFileHelper.TryDeleteOrMark(path);
    }
}