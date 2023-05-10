namespace Serenity.Tests;

public class MockDiskUploadFileSystem : MockFileSystem, IDiskUploadFileSystem
{
    public MockDiskUploadFileSystem(string currentDirectory = "")
        : base(currentDirectory)
    {
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
                using (var sr = new System.IO.StreamReader(OpenRead(name)))
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

    private void TryDelete(string path)
    {
        if (File.Exists(path))
            DeleteFile(path);
    }

    public void Delete(string path, Serenity.IO.DeleteType deleteType)
    {
        if (deleteType == Serenity.IO.DeleteType.Delete)
            DeleteFile(path);
        else if (deleteType == Serenity.IO.DeleteType.TryDelete)
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

    public void PurgeDirectory(string directoryToClean, TimeSpan? autoExpireTime = null, int? maxFilesInDirectory = null, string checkFileName = null)
    {
        // not implemented
    }
}