using System.IO;

namespace Serenity.Web;

/// <summary>
/// A combined upload storage implementation that uses two upload storage instances,
/// while serving one of them from a sub path like "/temporary/"
/// </summary>
public class CombinedUploadStorage : IUploadStorage
{
    private readonly IUploadStorage mainStorage;
    private readonly IUploadStorage subStorage;
    private readonly string subPrefix;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="mainStorage">The main storage</param>
    /// <param name="subStorage">The sub storage</param>
    /// <param name="subPrefix">Prefix path for the substorage like "temporary/"</param>
    /// <exception cref="ArgumentNullException">One of the arguments is null</exception>
    public CombinedUploadStorage(IUploadStorage mainStorage, IUploadStorage subStorage, string subPrefix)
    {
        this.mainStorage = mainStorage ?? throw new ArgumentNullException(nameof(mainStorage));
        this.subStorage = subStorage ?? throw new ArgumentNullException(nameof(subStorage));
        this.subPrefix = PathHelper.ToUrl(subPrefix ?? throw new ArgumentNullException(nameof(subPrefix)));

        if (!this.subPrefix.EndsWith("/", StringComparison.Ordinal))
            this.subPrefix += "/";
    }

    /// <summary>
    /// Returns if the path is under sub path
    /// </summary>
    /// <param name="path">Path</param>
    protected bool UnderSubPath(string path)
    {
        return path != null && PathHelper.ToUrl(path).StartsWith(subPrefix, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns true if the path is the subpath
    /// </summary>
    /// <param name="path">Path</param>
    protected bool IsSubPath(string path)
    {
        return path != null && path.Length == subPrefix.Length - 1 &&
            subPrefix.StartsWith(path, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public string ArchiveFile(string path)
    {
        if (UnderSubPath(path))
            return subPrefix + subStorage.ArchiveFile(path[subPrefix.Length..]);
        
        return mainStorage.ArchiveFile(path);
    }

    /// <inheritdoc/>
    public string CopyFrom(IUploadStorage source, string path, string targetPath, OverwriteOption overwrite)
    {
        if (UnderSubPath(targetPath))
            return subPrefix + subStorage.CopyFrom(source, path, targetPath[subPrefix.Length..], overwrite);
            
        return mainStorage.CopyFrom(source, path, targetPath, overwrite);
    }

    /// <inheritdoc/>
    public void DeleteFile(string path)
    {
        if (UnderSubPath(path))
            subStorage.DeleteFile(path[subPrefix.Length..]);
        else
            mainStorage.DeleteFile(path);
    }

    /// <inheritdoc/>
    public bool FileExists(string path)
    {
        if (UnderSubPath(path))
            return subStorage.FileExists(path[subPrefix.Length..]);
            
        return mainStorage.FileExists(path);
    }

    /// <inheritdoc/>
    public string[] GetFiles(string path, string searchPattern)
    {
        if (UnderSubPath(path))
            return subStorage.GetFiles(path[subPrefix.Length..], searchPattern)
                .Select(x => subPrefix + x).ToArray();
        else if (IsSubPath(path))
            return subStorage.GetFiles("", searchPattern)
                .Select(x => subPrefix + x).ToArray();
            
        return mainStorage.GetFiles(path, searchPattern);
    }

    /// <inheritdoc/>
    public long GetFileSize(string path)
    {
        if (UnderSubPath(path))
            return subStorage.GetFileSize(path[subPrefix.Length..]);
            
        return mainStorage.GetFileSize(path);
    }

    /// <inheritdoc/>
    public string GetFileUrl(string path)
    {
        if (UnderSubPath(path))
            return subStorage.GetFileUrl(path[subPrefix.Length..]);
            
        return mainStorage.GetFileUrl(path);
    }

    /// <inheritdoc/>
    public Stream OpenFile(string path)
    {
        if (UnderSubPath(path))
            return subStorage.OpenFile(path[subPrefix.Length..]);
        
        return mainStorage.OpenFile(path);
    }

    /// <inheritdoc/>
    public void PurgeTemporaryFiles()
    {
        mainStorage.PurgeTemporaryFiles();
        subStorage.PurgeTemporaryFiles();
    }

    /// <inheritdoc/>
    public string WriteFile(string path, Stream source, OverwriteOption overwrite)
    {
        if (UnderSubPath(path))
            return subPrefix + subStorage.WriteFile(path[subPrefix.Length..], source, overwrite);
            
        return mainStorage.WriteFile(path, source, overwrite);
    }

    /// <inheritdoc/>
    public IDictionary<string, string> GetFileMetadata(string path)
    {
        if (UnderSubPath(path))
            return subStorage.GetFileMetadata(path[subPrefix.Length..]);
            
        return mainStorage.GetFileMetadata(path);
    }

    /// <inheritdoc/>
    public void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
    {
        if (UnderSubPath(path))
           subStorage.SetFileMetadata(path[subPrefix.Length..], metadata, overwriteAll);
        else
           mainStorage.SetFileMetadata(path, metadata, overwriteAll);
    }
}