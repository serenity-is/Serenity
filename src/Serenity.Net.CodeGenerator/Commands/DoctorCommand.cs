using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public partial class DoctorCommand(IProjectFileInfo project, IGeneratorConsole console) 
    : BaseGeneratorCommand(project, console)
{
    public IArgumentReader Arguments { get; set; }
    public List<ExternalType> TsTypes { get; set; }

    void Info(string label, string text)
    { 
        Console.Write(label + ": ", ConsoleColor.Cyan);
        Console.WriteLine(text);
    }

    void Warning(string message)
    {
        Console.Write("WARNING: ", ConsoleColor.Yellow);
        Console.WriteLine(message, ConsoleColor.Yellow);
    }

    public override ExitCodes Run()
    {
        var projectFile = System.IO.Path.GetFullPath(ProjectFile);

        Info("Project File", projectFile);

        CheckProjectFilename(System.IO.Path.GetFileName(projectFile));
        var projectDir = FileSystem.GetDirectoryName(projectFile);
        CheckProjectDirectory(projectDir);

        var config = FileSystem.LoadGeneratorConfig(projectDir);

        var sergenJson = FileSystem.Combine(projectDir, "sergen.json");
        if (!FileSystem.FileExists(sergenJson))
        {
            Warning($"sergen.json file not found at {projectDir}!");
        }
        else
        {
            Info("sergen.json Location", sergenJson);
            Info("sergen.json Contents", FileSystem.ReadAllText(sergenJson));
        }

        Info("Using Generator Config", JSON.StringifyIndented(config));

        if (string.IsNullOrEmpty(config.RootNamespace))
        {
            config.RootNamespace = config.GetRootNamespaceFor(Project);
            Info("Using Root Namespace (auto generated)", config.RootNamespace);
        }
        else
        {
            Info("Using Root Namespace (from sergen.json)", config.RootNamespace);
        }

        CheckRootNamespace(config.RootNamespace);
        
        return ExitCodes.Success;
    }

    void CheckProjectDirectory(string projectDirectory)
    {
        if (ContainsSpaceRegex().IsMatch(projectDirectory))
        {
            Warning("It is not recommended to have the project in a directory that contains spaces!");
        }

        if (projectDirectory.Contains('#'))
        {
            Warning("Project directory must not include '#' (hash) character!");
        }

        if (projectDirectory.Contains(';'))
        {
            Warning("Project directory must not include ';' (semicolon) character!");
        }
    }

    void CheckProjectFilename(string projectFilename)
    {
        if (!StartsWithCapitalRegex().IsMatch(projectFilename))
        {
            Warning("Project filename should start with a capital letter!");
        }

        if (ContainsSpaceRegex().IsMatch(projectFilename))
        {
            Warning("Project filename should not contain spaces!");
        }

        if (!ValidProjectNameCharsRegex().IsMatch(projectFilename))
        {
            Warning("Project filename should only include letters, digits, underscore and dot characters!");
        }

        if (projectFilename.EndsWith('.'))
        {
            Warning("Project filename should not end with a DOT!");
        }

        if (projectFilename.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            Warning("Project filename should not be 'Serenity'");
        }

        if (projectFilename.StartsWith("Serenity.", StringComparison.Ordinal))
        {
            Warning("Project filename should not start with 'Serenity.' prefix");
        }
    }

    void CheckRootNamespace(string rootNamespace)
    {
        if (!StartsWithCapitalRegex().IsMatch(rootNamespace))
        {
            Warning("Root namespace should start with a capital letter!");
        }

        if (ContainsSpaceRegex().IsMatch(rootNamespace))
        {
            Warning("Root namespace should not contain spaces!");
        }

        if (!ValidProjectNameCharsRegex().IsMatch(rootNamespace))
        {
            Warning("Root namespace should only include letters, digits, underscore and dot characters!");
        }

        if (rootNamespace.EndsWith('.'))
        {
            Warning("Root namespace should not end with a DOT!");
        }

        if (rootNamespace.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            Warning("Root namespace not be 'Serenity'");
        }

        if (rootNamespace.StartsWith("Serenity.", StringComparison.Ordinal))
        {
            Warning("Root namespace should not start with 'Serenity.' prefix");
        }
    }

    [GeneratedRegex("^[A-Z]")]
    private static partial Regex StartsWithCapitalRegex();
    [GeneratedRegex(@"\s")]
    private static partial Regex ContainsSpaceRegex();
    [GeneratedRegex(@"^[A-Za-z0-9._]+$")]
    private static partial Regex ValidProjectNameCharsRegex();
}