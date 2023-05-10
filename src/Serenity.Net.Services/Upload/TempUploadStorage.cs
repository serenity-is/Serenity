namespace Serenity.Web;

/// <summary>
/// A subclass of <see cref="DiskUploadStorage"/> specialized for the temporary upload folder,
/// allowing to purge temporary files
/// </summary>
public class TempUploadStorage : DiskUploadStorage
{
    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="options">Upload storage options</param>
    /// <param name="fileSystem">File system</param>
    public TempUploadStorage(DiskUploadStorageOptions options, IDiskUploadFileSystem fileSystem = null)
        : base(options, fileSystem)
    {
        if (!this.fileSystem.DirectoryExists(RootPath))
        {
            try
            {
                this.fileSystem.CreateDirectory(RootPath);
                this.fileSystem.WriteAllText(fileSystem.Combine(RootPath, ".temporary"), "");
            }
            catch
            {
                // swallow exception as this causes startup errors
                // and application pool crashes if upload folder
                // can't be accessed, better to ignore
            }
        }
    }

    /// <inheritdoc/>
    public override void PurgeTemporaryFiles()
    {
        fileSystem.PurgeDirectory(RootPath);
    }
}