using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System.IO;

namespace Serenity.CodeGeneration;

public class FileSystemDirectoryWrapper : DirectoryInfoBase
{
    private readonly string path;
    private readonly bool isParentPath;
    private readonly IFileSystem fileSystem;

    private class FileWrapper(IFileSystem fileSystem, string path) : FileInfoBase
    {
        private readonly string path = path ?? throw new ArgumentNullException(nameof(path));
        private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

        public override string Name => fileSystem.GetFileName(path);
        public override string FullName => path;
        public override DirectoryInfoBase ParentDirectory => new FileSystemDirectoryWrapper(fileSystem, fileSystem.GetDirectoryName(path));
    }

    public FileSystemDirectoryWrapper(IFileSystem fileSystem, string path)
        : this(fileSystem, path, isParentPath: false)
    {
    }

    private FileSystemDirectoryWrapper(IFileSystem fileSystem, string path, bool isParentPath)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.path = path;
        this.isParentPath = isParentPath;
    }

    /// <inheritdoc />
    public override IEnumerable<FileSystemInfoBase> EnumerateFileSystemInfos()
    {
        if (fileSystem.DirectoryExists(path))
        {
            IEnumerable<string> files;
            IEnumerable<string> dirs;
            try
            {
                files = fileSystem.GetFiles(path, "*", recursive: false);
                dirs = fileSystem.GetDirectories(path, "*", recursive: false);
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }

            foreach (var file in files)
                yield return new FileWrapper(fileSystem, file);

            foreach (var directory in dirs)
                yield return new FileSystemDirectoryWrapper(fileSystem, directory);
        }
    }

    /// <inheritdoc />
    public override DirectoryInfoBase GetDirectory(string name)
    {
        bool isParentPath = string.Equals(name, "..", StringComparison.Ordinal);

        if (isParentPath)
            return new FileSystemDirectoryWrapper(fileSystem, fileSystem.Combine(path, name), isParentPath);

        var dirs = fileSystem.GetDirectories(path, name, recursive: false);

        if (dirs.Length == 1)
            return new FileSystemDirectoryWrapper(fileSystem, dirs[0], isParentPath);
            
        if (dirs.Length == 0)
            return null;

        // This shouldn't happen. The parameter name isn't supposed to contain wild card.
        throw new InvalidOperationException(
            $"More than one sub directories are found under {path} with name {name}.");
    }

    /// <inheritdoc />
    public override FileInfoBase GetFile(string name) 
        => new FileWrapper(fileSystem, Path.Combine(path, name));

    /// <inheritdoc />
    public override string Name => isParentPath ? ".." : fileSystem.GetFileName(path);

    /// <summary>
    /// Returns the full path to the directory.
    /// </summary>
    /// <remarks>
    /// Equals the value of <seealso cref="System.IO.FileSystemInfo.FullName" />.
    /// </remarks>
    public override string FullName => path;

    /// <summary>
    /// Returns the parent directory.
    /// </summary>
    /// <remarks>
    /// Equals the value of <seealso cref="System.IO.DirectoryInfo.Parent" />.
    /// </remarks>
    public override DirectoryInfoBase ParentDirectory
        => new FileSystemDirectoryWrapper(fileSystem, fileSystem.GetDirectoryName(path));
}
