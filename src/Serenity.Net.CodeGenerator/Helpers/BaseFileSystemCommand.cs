namespace Serenity.CodeGenerator;

public abstract class BaseFileSystemCommand
{
    protected readonly IGeneratorFileSystem fileSystem;

    protected BaseFileSystemCommand(IGeneratorFileSystem fileSystem)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
}