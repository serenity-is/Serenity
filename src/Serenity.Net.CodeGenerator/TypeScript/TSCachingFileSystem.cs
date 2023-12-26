using System.IO;
#if ISSOURCEGENERATOR
using System.Collections.Concurrent;
#endif

namespace Serenity.CodeGenerator;

public class TSCachingFileSystem(IFileSystem fileSystem) : IFileSystem
{
    private readonly ConcurrentDictionary<string, Lazy<string>> allText = new();
    private readonly ConcurrentDictionary<string, Lazy<bool>> fileExists = new();
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    public void CreateDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public Stream CreateFile(string path, bool overwrite = true)
    {
        throw new NotImplementedException();
    }

    public void DeleteDirectory(string path, bool recursive = false)
    {
        throw new NotImplementedException();
    }

    public void DeleteFile(string path)
    {
        throw new NotImplementedException();
    }

    public bool DirectoryExists(string path)
    {
        return fileSystem.DirectoryExists(path);
    }
    
    public bool FileExists(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        var cacheKey = TypeScript.Utilities.NormalizePath(path);

        return fileExists.GetOrAdd(cacheKey, cacheKey => new(() => fileSystem.FileExists(path))).Value;
    }

    public string[] GetDirectories(string path, string searchPattern = "*", bool recursive = false)
    {
        return fileSystem.GetDirectories(path, searchPattern, recursive);
    }

    public string[] GetFiles(string path, string searchPattern = "*", bool recursive = false)
    {
        return fileSystem.GetFiles(path, searchPattern, recursive);
    }

    public long GetFileSize(string path)
    {
        return fileSystem.GetFileSize(path);
    }

    public string GetFullPath(string path)
    {
        return fileSystem.GetFullPath(path);
    }

    public DateTime GetLastWriteTimeUtc(string path)
    {
        return fileSystem.GetLastWriteTimeUtc(path);
    }

    public string GetRelativePath(string relativeTo, string path)
    {
        return fileSystem.GetRelativePath(relativeTo, path);
    }

    public Stream OpenRead(string path)
    {
        throw new NotImplementedException();
    }

    public byte[] ReadAllBytes(string path)
    {
        throw new NotImplementedException();
    }

    public string ReadAllText(string path, Encoding encoding = null)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (encoding != null &&
            encoding != Encoding.UTF8)
            throw new NotImplementedException();

        var cacheKey = TypeScript.Utilities.NormalizePath(path);

        return allText.GetOrAdd(cacheKey, cacheKey => new(() => fileSystem.ReadAllText(path))).Value;
    }

    public void WriteAllBytes(string path, byte[] content)
    {
        throw new NotImplementedException();
    }

    public void WriteAllText(string path, string content, Encoding encoding = null)
    {
        throw new NotImplementedException();
    }
}