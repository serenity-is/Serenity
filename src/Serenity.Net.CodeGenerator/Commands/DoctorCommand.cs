using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class DoctorCommand(IProjectFileInfo project, IGeneratorConsole console) 
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
        
        var projectDir = FileSystem.GetDirectoryName(projectFile);
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

        return ExitCodes.Success;
    }
}