using Microsoft.Extensions.Options;
using Serenity.IO;
using System.Net.NetworkInformation;

namespace Serenity.Web;

/// <summary>
/// A file system based upload storage implementation
/// </summary>
public class DiskUploadStorage : IUploadStorage
{
    /// <summary>
    /// File system
    /// </summary>
    protected readonly IDiskUploadFileSystem fileSystem;

    /// <summary>
    /// Root path for the uploads
    /// </summary>
    public string RootPath { get; private set; }

    /// <summary>
    /// Root URL for the uploads
    /// </summary>
    public string RootUrl { get; private set; }
    
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="options">Upload options</param>
    /// <param name="fileSystem">File system to use</param>
    /// <exception cref="ArgumentNullException">Options is null</exception>
    public DiskUploadStorage(DiskUploadStorageOptions options, IDiskUploadFileSystem fileSystem = null)
    {
        this.fileSystem = fileSystem ?? new PhysicalDiskUploadFileSystem();
        var opt = options ?? throw new ArgumentNullException(nameof(options));

        ArgumentException.ThrowIfNullOrWhiteSpace(opt.RootPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(opt.RootUrl);

        RootUrl = opt.RootUrl;
        RootPath = opt.RootPath;

        if (!this.fileSystem.IsPathRooted(RootPath))
            RootPath = this.fileSystem.Combine(AppContext.BaseDirectory, PathHelper.ToPath(RootPath));
    }

    /// <summary>
    /// Gets the full path for the file
    /// </summary>
    /// <param name="path">File path</param>
    protected string FilePath(string path)
    {
        return PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));
    }

    /// <inheritdoc/>
    public string GetFileUrl(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        return PathHelper.SecureCombine(RootUrl, PathHelper.ToUrl(path));
    }

    private bool IsInternalFile(string path)
    {
        return path != null && fileSystem.GetExtension(path)?.ToLowerInvariant()?.Trim() == ".meta";
    }

    /// <inheritdoc/>
    public bool FileExists(string path)
    {
        return fileSystem.FileExists(FilePath(path)) && !IsInternalFile(path);
    }

    /// <inheritdoc/>
    public virtual IDictionary<string, string> GetFileMetadata(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        var filePath = FilePath(path);
        var metaFile = filePath + ".meta";
        if (fileSystem.FileExists(metaFile))
        {
            var json = fileSystem.ReadAllText(metaFile)?.Trim();
            if (!string.IsNullOrEmpty(json) &&
                json[0] == '{' &&
                json[^1] == '}')
            {
                return JSON.Parse<Dictionary<string, string>>(json);
            }
        }

        return new Dictionary<string, string>();
    }

    /// <inheritdoc/>
    public virtual void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ArgumentNullException.ThrowIfNull(metadata);

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        var filePath = FilePath(path);
        var metaFile = filePath + ".meta";

        if (overwriteAll && metadata.Count == 0)
        {
            if (fileSystem.FileExists(metaFile))
                fileSystem.TryDeleteOrMark(metaFile);
        }
        else if (metadata.Count == 0)
            return;
        
        if (!overwriteAll && fileSystem.FileExists(metaFile))
        {
            var existing = GetFileMetadata(path);
            foreach (var x in metadata)
                existing[x.Key] = x.Value;
            metadata = existing;
        }

        fileSystem.WriteAllText(metaFile, JSON.StringifyIndented(metadata));
    }

    /// <inheritdoc/>
    public string CopyFrom(IUploadStorage store, string sourcePath, string targetPath, 
        OverwriteOption overwrite)
    {
        if (string.IsNullOrEmpty(sourcePath))
            throw new ArgumentNullException(sourcePath);

        if (IsInternalFile(sourcePath))
            throw new ArgumentOutOfRangeException(nameof(sourcePath));

        if (string.IsNullOrEmpty(targetPath))
            throw new ArgumentNullException(nameof(targetPath));

        if (IsInternalFile(targetPath))
            throw new ArgumentOutOfRangeException(nameof(targetPath));

        var newFiles = new List<string>();
        using var source = store.OpenFile(sourcePath);
        targetPath = WriteFile(targetPath, source, overwrite);
        newFiles.Add(targetPath);
        try
        {
            var sourceBasePath = fileSystem.ChangeExtension(sourcePath, null);
            var targetBasePath = fileSystem.ChangeExtension(targetPath, null);

            foreach (var thumbPath in store.GetThumbnailFiles(sourcePath))
            {
                if (!UploadPathHelper.TryParseThumbSuffix(thumbPath, out _, out var suffix, out _, out _))
                    continue;

                using var src = store.OpenFile(thumbPath);
                var targetThumb = targetBasePath + suffix;
                targetThumb = WriteFile(targetThumb, src, OverwriteOption.Overwrite);
                newFiles.Add(targetThumb);
                var thumbMetadata = store.GetFileMetadata(thumbPath);
                if (thumbMetadata?.Count > 0)
                    SetFileMetadata(targetThumb, thumbMetadata, true);
            }

            var metadata = store.GetFileMetadata(sourcePath);
            if (metadata?.Count > 0)
                SetFileMetadata(targetPath, metadata, true);

            return targetPath;
        }
        catch
        {
            foreach (var newFile in newFiles)
                fileSystem.TryDeleteOrMark(FilePath(newFile));
            throw;
        }
    }

    /// <inheritdoc/>
    public string ArchiveFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
        string dbHistoryFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + fileSystem.GetExtension(path);
        CopyFrom(this, path, dbHistoryFile, OverwriteOption.Disallowed);
        return dbHistoryFile;
    }

    /// <inheritdoc/>
    public void DeleteFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        var fileName = PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));

        var folder = fileSystem.GetDirectoryName(fileName);
        fileSystem.TryDeleteMarkedFiles(folder);

        fileSystem.Delete(fileName, DeleteType.TryDeleteOrMark);

        foreach (var thumbFile in this.GetThumbnailFiles(path))
        {
            var thumbPath = fileSystem.Combine(folder, fileSystem.GetFileName(thumbFile));
            fileSystem.Delete(thumbPath, DeleteType.TryDeleteOrMark);
            fileSystem.Delete(thumbPath + UploadPathHelper.MetaFileExtension, DeleteType.TryDeleteOrMark);
        }

        fileSystem.Delete(fileName + UploadPathHelper.MetaFileExtension, DeleteType.TryDeleteOrMark);
    }

    /// <inheritdoc/>
    public long GetFileSize(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        return fileSystem.GetFileSize(FilePath(path));
    }

    /// <inheritdoc/>
    public string[] GetFiles(string path, string searchPattern)
    {
        return [.. fileSystem.GetFiles(FilePath(path), searchPattern, recursive: true)
            .Where(x => !IsInternalFile(x))
            .Select(x => fileSystem.GetRelativePath(RootPath, x))];
    }

    /// <inheritdoc/>
    public System.IO.Stream OpenFile(string path)
    {
        return fileSystem.OpenRead(FilePath(path));
    }

    /// <inheritdoc/>
    public virtual void PurgeTemporaryFiles()
    {
    }

    /// <inheritdoc/>
    public string WriteFile(string path, System.IO.Stream source, OverwriteOption overwrite)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        if (IsInternalFile(path))
            throw new ArgumentOutOfRangeException(nameof(path));

        var targetFullPath = FilePath(path);

        if (fileSystem.FileExists(targetFullPath))
        {
            if (overwrite == OverwriteOption.Disallowed)
                throw new System.IO.IOException($"Target file {path} exists in storage {GetType().FullName}");

            if (overwrite == OverwriteOption.AutoRename)
            {
                path = UploadPathHelper.FindAvailableName(path, FileExists);
                targetFullPath = FilePath(path);
            }
        }

        var targetDir = fileSystem.GetDirectoryName(targetFullPath);
        if (!fileSystem.DirectoryExists(targetDir))
            fileSystem.CreateDirectory(targetDir);

        using var targetStream = fileSystem.CreateFile(targetFullPath, 
            overwrite: overwrite == OverwriteOption.Overwrite);
        source.CopyTo(targetStream);

        return path;
    }
}