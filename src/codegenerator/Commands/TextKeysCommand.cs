using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

/// <summary>
/// Writes the constants of the text keys of the project (TextKeys.g.cs), which is a part of the mvc command, as it runs
/// before the build, see <see cref="TextKeysGenerator"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TextKeysCommand"/> class.
/// </remarks>
/// <param name="project">The project.</param>
/// <param name="console">The console.</param>
public class TextKeysCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    /// <summary>
    /// Gets or sets a value indicating whether the file is written even when the source generators of the project
    /// write it while it builds, which is what the force switch of the command asks for, as the one that this writes
    /// would be a duplicate of theirs, and the two drift from each other.
    /// </summary>
    public bool Force { get; set; }

    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(FileSystem.GetFullPath(ProjectFile));
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        // the source generators of Pro.Coder write these while the project builds, and the file written here would be
        // a duplicate of theirs, which drifts from it, unless the command is forced to write it anyway
        if (!Force && Project.UsesSourceGenerators())
        {
            Console.WriteLine(Texts.TextKeysGeneratedOnBuild, ConsoleColor.Yellow);
            return ExitCodes.Success;
        }

        Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.TextKeysTransforming,
            FileSystem.GetFileNameWithoutExtension(ProjectFile)), ConsoleColor.Cyan);

        try
        {
            new TextKeysGenerator(FileSystem, Console)
            {
                ProjectDir = projectDir,
                OutDir = FileSystem.Combine(projectDir,
                    PathHelper.ToPath(config.MVC?.OutDir.TrimToNull() ?? "Imports/MVC")),
                RootNamespace = config.GetRootNamespaceFor(Project) +
                    (config.MVC?.AsNamespace == true ? ".MVC" : ""),
                TextKeys = config.MVC?.TextKeys,
                InternalAccess = config.MVC?.InternalAccess == true,
                UsingAlias = config.MVC?.TextKeysAlias != false && !Project.HasAlias("TextKeys"),
                EndOfLine = config.EndOfLine,
                FileScopedNamespaces = config.FileScopedNamespaces == true
            }.Run();
        }
        catch (Exception exception)
        {
            // a step that runs before the build can not break it, so it says what it could not do and goes on
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.TextKeysFailed,
                exception.Message), ConsoleColor.Yellow);
        }

        return ExitCodes.Success;
    }
}
