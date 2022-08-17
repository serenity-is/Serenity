using System.IO;
using Serenity.IO;

namespace Serenity.Tests;

public class MockDiskUploadFileSystem : MockFileSystem, IDiskUploadFileSystem
{
    public MockDiskUploadFileSystem(IDictionary<string, MockFileData> files, string currentDirectory = "")
        : base(files, currentDirectory)
    {
    }
    
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CopyFile(System.IO.Stream sourceStream, string destPath, bool overwrite)
    {
        if (FileExists(destPath) && !overwrite)
            return;

        using var stream = File.Open(destPath, System.IO.FileMode.Create);
        sourceStream.CopyTo(stream);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public void TryDeleteMarkedFiles(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;
        
        foreach (var name in Directory.GetFiles(folderPath, "*.delete"))
        {
            try
            {
                string readLine;
                using (var sr = new StreamReader(File.OpenRead(name)))
                    readLine = sr.ReadToEnd();
                var actualFile = name[..^7];
                if (File.Exists(actualFile))
                {
                    if (long.TryParse(readLine, out var fileTime))
                    {
                        if (fileTime == File.GetLastWriteTimeUtc(actualFile).ToFileTimeUtc())
                            TryDelete(actualFile);
                    }
                    TryDelete(name);
                }
                else
                    TryDelete(name);
            }
            catch
            {
            }
        }
    }

    private void Delete(string path)
    {
        File.Delete(path);
    }

    private void TryDelete(string path)
    {
        if (File.Exists(path))
            Delete(path);
    }

    public void Delete(string path, DeleteType deleteType)
    {
        if (deleteType == DeleteType.Delete)
            Delete(path);
        else if (deleteType == DeleteType.TryDelete)
            TryDelete(path);
        else
            TryDeleteOrMark(path);   
    }

    public void TryDeleteOrMark(string path)
    {
        TryDelete(path);
        if (!File.Exists(path))
            return;
        
        try
        {
            var deleteFile = path + ".delete";
            var fileTime = File.GetLastWriteTimeUtc(path).ToFileTimeUtc();
            using var sw = new System.IO.StreamWriter(File.OpenWrite(deleteFile));
            sw.Write(fileTime);
        }
        catch
        {
        }
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }

    public void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public string[] DirectoryGetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)
    {
        return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public long GetFileSize(string path)
    {
        return FileInfo.FromFileName(path).Length;
    }

    public System.IO.Stream FileOpenRead(string path)
    {
        return File.OpenRead(path);
    }
}