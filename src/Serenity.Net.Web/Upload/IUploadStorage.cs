using Serenity.IO;

namespace Serenity.Web
{
    public interface IUploadStorage
    {
        string ArchiveFileAndRelated(string dbSourceFile);
        void CopyFileAndRelated(string dbSourceFile, string dbTargetFile, bool overwrite);
        void DeleteFileAndRelated(string dbFileName, DeleteType deleteType);
        string DbFileUrl(string dbFileName);
        bool FileExists(string dbFileName);
        long GetFileSize(string dbFileName);
        string GetOriginalName(string dbTemporaryFile);
        string TempPath { get; }
    }
}