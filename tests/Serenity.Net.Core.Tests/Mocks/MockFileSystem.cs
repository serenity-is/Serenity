using Stream = System.IO.Stream;
using IOException = System.IO.IOException;
using SearchOption = System.IO.SearchOption;

namespace Serenity.Tests;

public class MockFileSystem : System.IO.Abstractions.TestingHelpers.MockFileSystem, IFileSystem
{
    public MockFileSystem(string currentDirectory = "")
        : base(null, currentDirectory)
    {
    }

    /// <inheritdoc/>
    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    /// <inheritdoc/>
    public Stream CreateFile(string path, bool overwrite = true)
    {
        if (!overwrite && FileExists(path))
            throw new IOException("Target file exists!");

        return File.Create(path);
    }

    /// <inheritdoc/>
    public void DeleteDirectory(string path, bool recursive)
    {
        Directory.Delete(path, recursive);
    }

    /// <inheritdoc/>
    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    /// <inheritdoc/>
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <inheritdoc/>
    bool IFileSystem.FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <inheritdoc/>
    public string[] GetDirectories(string path, string searchPattern = "*", bool recursive = false)
    {
        return Directory.GetDirectories(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    /// <inheritdoc/>
    public string[] GetFiles(string path, string searchPattern = "*", bool recursive = false)
    {
        return Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    /// <inheritdoc/>
    public long GetFileSize(string path)
    {
        return FileInfo.New(path).Length;
    }

    /// <inheritdoc/>
    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    /// <inheritdoc/>
    public virtual string GetRelativePath(string relativeTo, string path)
    {
        return Path.GetRelativePath(relativeTo, path);
    }

    /// <inheritdoc/>
    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    /// <inheritdoc/>
    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    /// <inheritdoc/>
    public string ReadAllText(string path, Encoding encoding = null)
    {
        return encoding != null ?
            File.ReadAllText(path, encoding) :
            File.ReadAllText(path);
    }

    /// <inheritdoc/>
    public void WriteAllBytes(string path, byte[] content)
    {
        File.WriteAllBytes(path, content);
    }

    /// <inheritdoc/>
    public void WriteAllText(string path, string content, Encoding encoding = null)
    {
        if (encoding != null)
        {
            File.WriteAllText(path, content, encoding);
            return;
        }

        File.WriteAllText(path, content);
    }
}