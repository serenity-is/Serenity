using System.IO.Abstractions;

namespace Serenity.CodeGenerator
{
    public abstract class BaseFileSystemCommand
    {
        protected BaseFileSystemCommand(IFileSystem fileSystem)
        {
            FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        protected IFileSystem FileSystem { get; }
        protected IDirectory Directory => FileSystem.Directory;
        protected IFile File => FileSystem.File;
        protected IPath Path => FileSystem.Path;
    }
}