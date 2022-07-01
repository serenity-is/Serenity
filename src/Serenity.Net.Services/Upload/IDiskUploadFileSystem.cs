using Serenity.IO;

namespace Serenity.Web;

public interface IDiskUploadFileSystem
{
    bool FileExists(string path);
    bool DirectoryExists(string path);
    void CopyFile(System.IO.Stream sourceStream, string destPath, bool overwrite);
    void CreateDirectory(string path);
    void TryDeleteMarkedFiles(string folderPath);
    void Delete(string metaFilePath, DeleteType deleteType);
    void TryDeleteOrMark(string metaFilePath);
    string ReadAllText(string path);
    void WriteAllText(string path, string content);
    string[] DirectoryGetFiles(string path, string searchPattern, System.IO.SearchOption searchOption);
    long GetFileSize(string path);
    System.IO.Stream FileOpenRead(string path);
}