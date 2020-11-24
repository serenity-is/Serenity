using System.IO;

namespace Serenity.Web
{
    public interface IUploadStorage
    {
        string ArchiveFile(string path);
        string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, bool autoRename);
        void DeleteFile(string path);
        bool FileExists(string path);
        string GetFileUrl(string path);
        long GetFileSize(string path);
        string[] GetFiles(string path, string searchPattern);
        string GetOriginalName(string path);
        Stream OpenFile(string path);
        void PurgeTemporaryFiles();
        string WriteFile(string path, Stream source, bool autoRename);
    }
}