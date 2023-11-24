namespace Serenity.CodeGenerator;

public abstract class BaseGeneratorCommand(IProjectFileInfo project, IGeneratorConsole console)
{
    protected readonly IProjectFileInfo Project = project ?? throw new ArgumentNullException(nameof(project));
    protected readonly IGeneratorConsole Console = console ?? throw new ArgumentNullException(nameof(console));

    protected IFileSystem FileSystem => Project.FileSystem;
    protected string ProjectFile => Project.ProjectFile;

    public abstract ExitCodes Run();
}