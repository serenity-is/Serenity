namespace Serenity.CodeGenerator;

public abstract class BaseGeneratorCommand(IProjectFileInfo project)
{
    protected readonly IProjectFileInfo Project = project ?? throw new ArgumentNullException(nameof(project));
    protected IFileSystem FileSystem => Project.FileSystem;
    protected string ProjectFile => Project.ProjectFile;
}