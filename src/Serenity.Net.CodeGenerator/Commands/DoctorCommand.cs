using Serenity.CodeGeneration;
using System.Diagnostics;

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

    void Error(string message)
    {
        Console.Write("ERROR: ", ConsoleColor.Red);
        Console.WriteLine(message, ConsoleColor.Red);
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
        CheckNodeAndNpmVersions();
        
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
            Error("Project directory must not include '#' (hash) character!");
        }

        if (projectDirectory.Contains(';'))
        {
            Error("Project directory must not include ';' (semicolon) character!");
        }
    }

    void CheckProjectFilename(string projectFilename)
    {
        if (!StartsWithCapitalRegex().IsMatch(projectFilename))
        {
            Error("Project filename should start with a capital letter!");
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
            Error("Project filename should not be 'Serenity'");
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
            Error("Root namespace should start with a capital letter!");
        }

        if (ContainsSpaceRegex().IsMatch(rootNamespace))
        {
            Error("Root namespace should not contain spaces!");
        }

        if (!ValidProjectNameCharsRegex().IsMatch(rootNamespace))
        {
            Error("Root namespace should only include letters, digits, underscore and dot characters!");
        }

        if (rootNamespace.EndsWith('.'))
        {
            Error("Root namespace should not end with a DOT!");
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

    private static Version GetNodeOrNpmVersion(bool npm)
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "cmd",
                Arguments = "/c " + (npm ? "npm.cmd" : "node") + " --version",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        string output;
        try
        {
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            if (!process.WaitForExit(5000))
                output = null;
        }
        catch (Exception)
        {
            output = null;
        }

        output = (output ?? "").Trim();

        if (output.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            output = output.Substring(1);

        if (Version.TryParse(output, out var version))
            return version;

        return new Version(-1, 0);
    }

    private void CheckNodeAndNpmVersions()
    {
        var nodeVersion = GetNodeOrNpmVersion(npm: false);
        if (nodeVersion.Major < 0)
        {
            Error("NodeJS is not installed or not in PATH. Please install NodeJS.");
            return;
        } else if (nodeVersion < new Version(20, 11, 0))
        {
            Warning($"Your NodeJS version ({nodeVersion}) is not up to date." +
                "Please install latest Node LTS version (at least 20.17.0+).");
        }

        var npmVersion = GetNodeOrNpmVersion(npm: true);
        if (npmVersion.Major < 0)
        {
            Error("NPM is not installed or not in PATH. Please install NPM.");
            return;
        }
        else if (npmVersion < new Version(10, 8, 0))
        {
            Warning($"Your NPM version ({npmVersion}) is not up to date." +
                "Please install latest NPM version (at least 10.8.2+).");
        }
    }

    [GeneratedRegex("^[A-Z]")]
    private static partial Regex StartsWithCapitalRegex();
    [GeneratedRegex(@"\s")]
    private static partial Regex ContainsSpaceRegex();
    [GeneratedRegex(@"^[A-Za-z0-9._]+$")]
    private static partial Regex ValidProjectNameCharsRegex();
}