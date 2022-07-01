using System.IO;
using Serenity.IO;

namespace Serenity.Web;

public class PhysicalDiskUploadFileSystem : IDiskUploadFileSystem
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CopyFile(Stream sourceStream, string destPath, bool overwrite)
    {
        if (FileExists(destPath) && !overwrite)
            return;

        using var stream = File.Open(destPath, FileMode.Create);
        sourceStream.CopyTo(stream);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public void TryDeleteMarkedFiles(string folderPath)
    {
        TemporaryFileHelper.TryDeleteMarkedFiles(folderPath);
    }

    public void Delete(string metaFilePath, DeleteType deleteType)
    {
        TemporaryFileHelper.Delete(metaFilePath, deleteType);
    }

    public void TryDeleteOrMark(string metaFilePath)
    {
        TemporaryFileHelper.TryDeleteOrMark(metaFilePath);
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public long GetFileSize(string path)
    {
        return new FileInfo(path).Length;
    }

    public Stream FileOpenRead(string path)
    {
        return File.OpenRead(path);
    }
}