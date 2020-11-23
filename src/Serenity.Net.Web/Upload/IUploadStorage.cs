using System.IO;

namespace Serenity.Web
{
    public interface IUploadStorage
    {
        string ArchiveFile(string path);
        void CopyFile(IUploadStorage source, string path, string targetPath, bool overwrite);
        void DeleteFile(string path);
        bool FileExists(string path);
        string GetFileUrl(string path);
        long GetFileSize(string path);
        string[] GetFiles(string path, string searchPattern);
        string GetOriginalName(string path);
        Stream OpenFile(string path);
        void PurgeTemporaryFiles();
        void WriteFile(string path, Stream source, bool overwrite);
    }
}