using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class Cli(IFileSystem fileSystem, IGeneratorConsole console)
{
    private readonly IFileSystem FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IGeneratorConsole Console = console ?? throw new ArgumentNullException(nameof(console));

    public Func<IBuildProjectSystem> BuildSystemFactory { get; set; }
    public Func<string, Func<string, string>, IProjectFileInfo> ProjectFactory { get; set; }
    public Func<BaseGeneratorCommand, ExitCodes> RunCommandCallback { get; set; }

    public ExitCodes Run(IArgumentReader arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (arguments.HasHelpSwitch())
        {
            WriteHelp(Console);
            return ExitCodes.Help;
        }

        var projectFile = arguments.GetString(["p", "project"]);
        var projectRefs = arguments.GetStrings(["projectrefs", "project-refs"]);
        var propertyArgs = arguments.GetDictionary(["prop", "props", "property"], 
            separators: [',', ';']);
        var command = arguments.GetCommand();
        if (string.IsNullOrEmpty(command))
        {
            WriteHelp(Console);
            return ExitCodes.NoCommand;
        }

        if (projectFile == null)
        {
            var csprojs = FileSystem.GetFiles(".", "*.csproj");
            if (csprojs.Length == 0)
            {
                Console.Error("Can't find a project file in current directory!\n" + 
                    "Please run Sergen in a folder that contains the Asp.Net Core project.");
                return ExitCodes.NoProjectFiles;
            }

            if (csprojs.Length > 1)
            {
                Console.Error("Multiple project files found in current directory!\n" + 
                    "Please run Sergen in a folder that contains only one Asp.Net Core project.");
                return ExitCodes.MultipleProjectFiles;
            }

            projectFile = csprojs[0];
        }

        if (!FileSystem.FileExists(projectFile))
        {
            Console.Error($"Can't find a project file at: {projectFile}");
            return ExitCodes.ProjectNotFound;
        }

        var projectDir = FileSystem.GetDirectoryName(FileSystem.GetFullPath(projectFile));
        string getPropertyArgument(string name) => propertyArgs.TryGetValue(name, out var value) ? value : null;

        var project = ProjectFactory?.Invoke(projectFile, getPropertyArgument)
            ?? new ProjectFileInfo(fileSystem, projectFile, getPropertyArgument, Console.Error);

        bool isCommand(params string[] args)
        {
            return args.Any(x => string.Equals(x, command, StringComparison.OrdinalIgnoreCase));
        }

        if (isCommand("r", "restore"))
        {
            ArgumentNullException.ThrowIfNull(BuildSystemFactory);

            return RunCommand(new RestoreCommand(project, Console)
            {
                BuildSystem = BuildSystemFactory(),
                ProjectReferences = projectRefs
            });
        }

        if (isCommand("g", "generate"))
        {
            return RunCommand(new GenerateCommand(project, Console)
            {
                Arguments = arguments
            });
        }

        bool transform = isCommand("t", "transform");
        bool mvct = isCommand("mvct");
        bool clientTypes = transform || mvct || isCommand("c", "ct", "clienttypes");
        bool serverTypings = transform || isCommand("s", "st", "serverTypings");
        bool mvc = transform || mvct || isCommand("m", "mvc");

        if (!transform && !clientTypes && !serverTypings && !mvc)
        {
            WriteHelp(Console);
            return ExitCodes.InvalidCommand;
        }

        arguments.ThrowIfRemaining();

        List<ExternalType> tsTypesNamespaces = null;
        List<ExternalType> tsTypesModules = null;

        void ensureTSTypes()
        {
            if (tsTypesNamespaces is null &&
                tsTypesModules is null)
            {
                TSConfigHelper.LocateTSConfigFiles(FileSystem, projectDir,
                    out string modulesPath, out string namespacesPath);

                if (modulesPath is null &&
                    namespacesPath is null)
                    namespacesPath = FileSystem.Combine(projectDir, "tsconfig.json");

                if (namespacesPath is not null)
                {
                    var nsLister = new TSTypeLister(FileSystem, namespacesPath);
                    tsTypesNamespaces = nsLister.List();
                }

                if (modulesPath is not null)
                {
                    var mdLister = new TSTypeLister(FileSystem, modulesPath);
                    tsTypesModules = mdLister.List();
                }
            }
        }

        ExitCodes? exitCode = null;
        if (mvc)
            exitCode = RunCommand(new MvcCommand(project, Console), exitCode);

        if (clientTypes)
        {
            ensureTSTypes();
            exitCode = RunCommand(new ClientTypesCommand(project, Console)
            {
                TsTypes = [.. (tsTypesNamespaces ?? []), .. tsTypesModules ?? []]
            }, exitCode);
        }

        if (serverTypings)
        {
            ensureTSTypes();

            if (tsTypesNamespaces is not null)
            {
                exitCode = RunCommand(new ServerTypingsCommand(project, Console)
                {
                    TsTypes = tsTypesNamespaces,
                    Modules = false
                }, exitCode);
            }

            if (tsTypesModules is not null)
            {
                exitCode = RunCommand(new ServerTypingsCommand(project, Console)
                {
                    TsTypes = tsTypesModules,
                    Modules = true
                }, exitCode);
            }
        }

        return exitCode ?? ExitCodes.InvalidCommand;
    }

    private ExitCodes RunCommand(BaseGeneratorCommand command, ExitCodes? current = null)
    {
        var result = RunCommandCallback?.Invoke(command) ?? command.Run();
        return current is null || current == ExitCodes.Success ? result : current.Value;
    }

    public static void WriteHelp(IGeneratorConsole console)
    {
        console.ShowHelp($"""""
            Serenity Code Generator {Assembly.GetEntryAssembly().GetName().Version}
            Usage: sergen [switches] [command]

            Commands:
              c[lienttypes]    Imports editor and formatter types from TypeScript to C#.
              g[enerate]        Launches the table code generator.
              m[vc]             Generates IntelliSense helpers for view locations.
              mvct              Executes client types and mvc commands simulatenously.
              r[estore]         [Obsolete] Restores content (e.g., scripts, CSS) from .nupkg.
              s[ervertypings]   Imports row, form, and service types from C# to TypeScript.
              t[ransform]       Executes clienttypes, mvc, and servertypings commands simultaneously.

            Switches:
              -p <ProjectFile>  Specifies the project file. Useful when multiple projects exist 
                                in the current directory.
              -prop:<n>=<v>     Provides hints to sergen for project-level properties, 
                                where <n> is the property name and <v> is its value.
                                Use a semicolon to separate multiple properties or specify each 
                                property separately. This is helpful for improving performance
                                as Sergen won't have to parse the project, and also for addressing 
                                cases where sergen might not determine a property correctly.

                                Examples:
                                  -prop:Configuration=Release
                                  -prop:"OutDir=..\bin\Debug\;AssemblyName=MyAssembly"
            
            """"");
    }
}