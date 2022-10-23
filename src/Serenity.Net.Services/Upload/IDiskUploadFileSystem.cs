namespace Serenity.Web;

public interface IDiskUploadFileSystem : IFileSystem
{
    void Delete(string path, IO.DeleteType deleteType);
    void PurgeDirectory(string directoryToClean, TimeSpan? autoExpireTime = null, int? maxFilesInDirectory = null, string checkFileName = null);
    void TryDeleteMarkedFiles(string path);
    void TryDeleteOrMark(string path);
}