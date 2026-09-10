using System.IO;
using System.Text;

namespace Serenity.Web;

public class RecordingUploadStorage : IUploadStorage
{
    public List<(string Method, string Path)> Calls { get; } = [];
    public Dictionary<string, IDictionary<string, string>> Metadata { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Files { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> GetFilesResult { get; } = [];

    public string ArchiveFile(string path)
    {
        Calls.Add(("ArchiveFile", path));
        return "archive/" + path;
    }

    public string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, OverwriteOption overwrite)
    {
        Calls.Add(("CopyFrom", targetPath));
        return "copied/" + targetPath;
    }

    public void DeleteFile(string path)
    {
        Calls.Add(("DeleteFile", path));
        Files.Remove(path);
    }

    public bool FileExists(string path)
    {
        Calls.Add(("FileExists", path));
        return Files.Contains(path);
    }

    public string GetFileUrl(string path)
    {
        Calls.Add(("GetFileUrl", path));
        return "/url/" + path;
    }

    public long GetFileSize(string path)
    {
        Calls.Add(("GetFileSize", path));
        return 42;
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        Calls.Add(("GetFiles", path));
        return [.. GetFilesResult];
    }

    public IDictionary<string, string> GetFileMetadata(string path)
    {
        Calls.Add(("GetFileMetadata", path));
        return Metadata.TryGetValue(path, out var m) ? m : new Dictionary<string, string>();
    }

    public void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
    {
        Calls.Add(("SetFileMetadata", path));
        if (!overwriteAll && Metadata.TryGetValue(path, out var existing))
        {
            foreach (var pair in metadata)
                existing[pair.Key] = pair.Value;
        }
        else
            Metadata[path] = new Dictionary<string, string>(metadata);
    }

    public Stream OpenFile(string path)
    {
        Calls.Add(("OpenFile", path));
        return new MemoryStream(Encoding.UTF8.GetBytes("hello"));
    }

    public void PurgeTemporaryFiles()
    {
        Calls.Add(("PurgeTemporaryFiles", ""));
    }

    public string WriteFile(string path, Stream source, OverwriteOption overwrite)
    {
        Calls.Add(("WriteFile", path));
        return path;
    }
}
