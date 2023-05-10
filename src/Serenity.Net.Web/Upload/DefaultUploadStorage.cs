using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.IO;

namespace Serenity.Web;

/// <summary>
/// Default <see cref="IUploadStorage"/> implementation which uses a combined 
/// upload storage of two <see cref="DiskUploadStorage"/> instances.
/// </summary>
public class DefaultUploadStorage : IUploadStorage
{
    private readonly CombinedUploadStorage combined;

    /// <summary>
    /// Creates a new instance of the object
    /// </summary>
    /// <param name="options">Upload storage options</param>
    /// <param name="hostEnvironment">Web host environment</param>
    /// <param name="fileSystem">File system</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Options.URL or Options.Path is empty</exception>
    public DefaultUploadStorage(IOptions<UploadSettings> options, IWebHostEnvironment hostEnvironment = null,
        IDiskUploadFileSystem fileSystem = null)
    {
        var opt = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        if (string.IsNullOrEmpty(opt.Path))
            throw new ArgumentOutOfRangeException(nameof(opt.Path), 
                "Please set UploadSettings.Path in appsettings.json!");

        if (string.IsNullOrEmpty(opt.Url))
            throw new ArgumentOutOfRangeException(nameof(opt.Url),
                "Please set UploadSettings.Url in appsettings.json!");

        var path = opt.Path;
        if (path.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            path = path[2..]; // compatibility with old settings

        path = Path.Combine(hostEnvironment?.ContentRootPath ?? AppDomain.CurrentDomain.BaseDirectory,
            PathHelper.ToPath(path));

        fileSystem ??= new PhysicalDiskUploadFileSystem();

        combined = new CombinedUploadStorage(
            new DiskUploadStorage(new DiskUploadStorageOptions
            {
                RootPath = path,
                RootUrl = opt.Url
            }, fileSystem),
            new TempUploadStorage(new DiskUploadStorageOptions
            {
                RootPath = Path.Combine(path, "temporary"),
                RootUrl = UriHelper.Combine(opt.Url, "temporary/")
            }, fileSystem),
            "temporary/");
    }

    /// <inheritdoc/>
    public string ArchiveFile(string path)
    {
        return combined.ArchiveFile(path);
    }

    /// <inheritdoc/>
    public string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, OverwriteOption overwrite)
    {
        return combined.CopyFrom(sourceStorage, sourcePath, targetPath, overwrite);
    }

    /// <inheritdoc/>
    public void DeleteFile(string path)
    {
        combined.DeleteFile(path);
    }

    /// <inheritdoc/>
    public bool FileExists(string path)
    {
        return combined.FileExists(path);
    }

    /// <inheritdoc/>
    public string[] GetFiles(string path, string searchPattern)
    {
        return combined.GetFiles(path, searchPattern);
    }

    /// <inheritdoc/>
    public long GetFileSize(string path)
    {
        return combined.GetFileSize(path);
    }

    /// <inheritdoc/>
    public string GetFileUrl(string path)
    {
        return combined.GetFileUrl(path);
    }

    /// <inheritdoc/>
    public Stream OpenFile(string path)
    {
        return combined.OpenFile(path);
    }

    /// <inheritdoc/>
    public void PurgeTemporaryFiles()
    {
        combined.PurgeTemporaryFiles();
    }

    /// <inheritdoc/>
    public string WriteFile(string path, Stream source, OverwriteOption overwrite)
    {
        return combined.WriteFile(path, source, overwrite);
    }

    /// <inheritdoc/>
    public IDictionary<string, string> GetFileMetadata(string path)
    {
        return combined.GetFileMetadata(path);
    }

    /// <inheritdoc/>
    public void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
    {
        combined.SetFileMetadata(path, metadata, overwriteAll);
    }
}