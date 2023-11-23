#nullable enable
using System.IO;

namespace Serenity;

/// <summary>
/// Physical file sytem
/// </summary>
public class PhysicalFileSystem : IFileSystem
{
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
    public bool FileExists(string path)
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
        return new FileInfo(path).Length;
    }

    /// <inheritdoc/>
    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    /// <inheritdoc/>
    public DateTime GetLastWriteTimeUtc(string path)
    {
        return File.GetLastWriteTimeUtc(path);
    }

#if ISSOURCEGENERATOR
    // https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/32113484#32113484
    public string GetRelativePath(string relativeTo, string path)
    {
        if (string.IsNullOrEmpty(relativeTo))
            throw new ArgumentNullException("fromPath");

        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException("toPath");

        relativeTo = relativeTo.Replace("/", "\\", StringComparison.Ordinal);
        path = path.Replace("/", "\\", StringComparison.Ordinal);

        if (!relativeTo.Contains(':', StringComparison.Ordinal))
            relativeTo = "z:\\" + relativeTo;

        if (!path.Contains(':', StringComparison.Ordinal))
            path = "z:\\" + path;

        Uri fromUri = new(AppendDirectorySeparatorChar(relativeTo));
        Uri toUri = new(AppendDirectorySeparatorChar(path));

        if (fromUri.Scheme != toUri.Scheme)
            return path;

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);

        return relativePath;
    }

    private static string AppendDirectorySeparatorChar(string path)
    {
        if (!System.IO.Path.HasExtension(path) &&
            !path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            return path + System.IO.Path.DirectorySeparatorChar;

        return path;
    }
#else
    /// <inheritdoc/>
    public virtual string GetRelativePath(string relativeTo, string path)
    {
        return Path.GetRelativePath(relativeTo, path);
    }
#endif

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
    public string ReadAllText(string path, Encoding? encoding = null)
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
    public void WriteAllText(string path, string content, Encoding? encoding = null)
    {
        if (encoding != null)
        {
            File.WriteAllText(path, content, encoding);
            return;
        }

        File.WriteAllText(path, content);
    }

}
