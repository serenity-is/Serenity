namespace Serenity.Web;

/// <summary>
/// A container that stores list of file paths to delete (old files) if 
/// the upload related operation is successful. If it is not, the old
/// files will be kept while the new files will be deleted.
/// </summary>
public class FilesToDelete : List<string>, IDisposable, IFilesToDelete
{
    private readonly IUploadStorage storage;
    private readonly List<string> OldFiles;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="storage">Upload storage</param>
    /// <exception cref="ArgumentNullException">storage is null</exception>
    public FilesToDelete(IUploadStorage storage)
    {
        OldFiles = new List<string>();
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    /// <summary>
    /// Deletes the new files if <see cref="KeepNewFiles"/> is not called.
    /// </summary>
    public void Dispose()
    {
        foreach (var file in this)
            storage.DeleteFile(file);

        Clear();
    }

    /// <summary>
    /// Registers a new file
    /// </summary>
    /// <param name="file">File path</param>
    public void RegisterNewFile(string file)
    {
        Add(file);
    }

    /// <summary>
    /// Registers an old file
    /// </summary>
    /// <param name="file">File path</param>
    public void RegisterOldFile(string file)
    {
        OldFiles.Add(file);
    }

    /// <summary>
    /// Keeps new files, while removing old files
    /// </summary>
    public void KeepNewFiles()
    {
        Clear();
        AddRange(OldFiles);
        Dispose();
    }
}