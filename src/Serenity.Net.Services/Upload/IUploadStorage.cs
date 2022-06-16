using System.IO;

namespace Serenity.Web
{
    public interface IUploadStorage
    {
        string ArchiveFile(string path);
        string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, bool? autoRename);
        void DeleteFile(string path);
        bool FileExists(string path);
        string GetFileUrl(string path);
        long GetFileSize(string path);
        string[] GetFiles(string path, string searchPattern);
        IDictionary<string, string> GetFileMetadata(string path);
        void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll);
        Stream OpenFile(string path);
        void PurgeTemporaryFiles();
        string WriteFile(string path, Stream source, bool? autoRename);

    }
}