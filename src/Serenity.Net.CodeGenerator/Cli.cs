using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class Cli(IFileSystem fileSystem, IGeneratorConsole console)
{
    private readonly IFileSystem FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IGeneratorConsole Console = console ?? throw new ArgumentNullException(nameof(console));

    public Func<IBuildProjectSystem> BuildSystemFactory { get; set; }
    public Func<string, Func<string, string>, IProjectFileInfo> ProjectFactory { get; set; }
    public Func<BaseGeneratorCommand, ExitCodes> RunCommandCallback { get; set; }

    public ExitCodes Run(params string[] arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        string projectFile = null;
        var csprojIdx = Array.FindIndex(arguments, x => x == "-p" || x == "--p" || x == "/p");
        if (csprojIdx >= 0)
        {
            if (csprojIdx >= arguments.Length - 1)
            {
                WriteHelp(Console);
                return ExitCodes.InvalidArguments;
            }
            projectFile = arguments[csprojIdx + 1];
            arguments = arguments.Where((x, i) => i != csprojIdx && i != csprojIdx + 1).ToArray();
        }

        string[] prjRefs = null;
        var prjRefsIdx = Array.FindIndex(arguments, x => x == "--projectrefs");
        if (prjRefsIdx >= 0)
        {
            if (prjRefsIdx >= arguments.Length - 1)
            {
                WriteHelp(Console);
                return ExitCodes.InvalidArguments;
            }

            prjRefs = arguments[prjRefsIdx + 1].Split(';', StringSplitOptions.RemoveEmptyEntries);
            arguments = arguments.Where((x, i) => i != prjRefsIdx && i != prjRefsIdx + 1).ToArray();
        }

        arguments = ProjectFileInfo.FilterPropertyParams(arguments, out var props).ToArray();
        var errorProp = props.FirstOrDefault(x => x.Value == null);
        if (errorProp.Key != null)
        {
            Console.Error($"Property values should be passed " +
                $"in format '-prop:Name=Value'. Argument '{errorProp.Value}' is invalid!");
            return ExitCodes.InvalidArguments;
        }

        string command = null;
        if (arguments.Length > 0)
            command = arguments[0].ToLowerInvariant().TrimToEmpty();

        if (string.IsNullOrEmpty(command))
        {
            WriteHelp(Console);
            return ExitCodes.NoCommand;
        }

        if (command == "-?" ||
            command == "--help")
        {
            WriteHelp(Console);
            return ExitCodes.Help;
        }

        if (projectFile == null)
        {
            var csprojs = FileSystem.GetFiles(".", "*.csproj");
            if (csprojs.Length == 0)
            {
                Console.Error("Can't find a project file in current directory!");
                Console.Error("Please run Sergen in a folder that contains the Asp.Net Core project.");
                return ExitCodes.NoProjectFiles;
            }

            if (csprojs.Length > 1)
            {
                Console.Error("Multiple project files found in current directory!");
                Console.Error("Please run Sergen in a folder that contains only one Asp.Net Core project.");
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
        string getPropertyArgument(string name) => props.TryGetValue(name, out var value) ? value : null;

        var project = ProjectFactory?.Invoke(projectFile, getPropertyArgument)
            ?? new ProjectFileInfo(fileSystem, projectFile, getPropertyArgument, Console.Error);

        try
        {
            if ("restore".StartsWith(command, StringComparison.Ordinal))
            {
                ArgumentNullException.ThrowIfNull(BuildSystemFactory);

                return RunCommand(new RestoreCommand(project, Console)
                {
                    BuildSystem = BuildSystemFactory(),
                    ProjectReferences = prjRefs
                });
            }

            if ("transform".StartsWith(command, StringComparison.Ordinal) ||
                "servertypings".StartsWith(command, StringComparison.Ordinal) ||
                "clienttypes".StartsWith(command, StringComparison.Ordinal) ||
                "mvct".StartsWith(command, StringComparison.Ordinal))
            {
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

                bool transformAll = "transform".StartsWith(command, StringComparison.Ordinal);

                if (transformAll ||
                    "mvct".StartsWith(command, StringComparison.Ordinal))
                {
                    RunCommand(new MvcCommand(project, Console));
                }

                if (transformAll ||
                    "clienttypes".StartsWith(command, StringComparison.Ordinal) || command == "mvct")
                {
                    ensureTSTypes();
                    RunCommand(new ClientTypesCommand(project, Console)
                    {
                        TsTypes = [.. (tsTypesNamespaces ?? []), .. tsTypesModules ?? []]
                    });
                }

                if (transformAll ||
                    "servertypings".StartsWith(command, StringComparison.Ordinal))
                {
                    ensureTSTypes();

                    if (tsTypesNamespaces is not null)
                        RunCommand(new ServerTypingsCommand(project, Console)
                        {
                            TsTypes = tsTypesNamespaces,
                            Modules = false
                        });

                    if (tsTypesModules is not null)
                        RunCommand(new ServerTypingsCommand(project, Console)
                        {
                            TsTypes = tsTypesModules,
                            Modules = true
                        });
                }

                return ExitCodes.Success;
            }

            if ("generate".StartsWith(command, StringComparison.Ordinal))
            {
                return RunCommand(new GenerateCommand(project, Console)
                {
                    Arguments = arguments.Skip(1).ToArray()
                });
            }
        }
        catch (Exception ex)
        {
            Console.Exception(ex);
            return ExitCodes.Exception;
        }

        WriteHelp(Console);
        return ExitCodes.InvalidCommand;
    }

    private ExitCodes RunCommand(BaseGeneratorCommand command)
    {
        return RunCommandCallback?.Invoke(command) ?? command.Run();
    }

    private static void WriteHelp(IGeneratorConsole console)
    {
        console.ShowHelp($"""""
            Serenity Code Generator {Assembly.GetEntryAssembly().GetName().Version}
            Usage: sergen [switches] [command]

            Commands:
              g[enerate]        Launches the table code generator.
              c[lienttypes]     Imports editor and formatter types from TypeScript to C#.
              m[vc]             Generates IntelliSense helpers for view locations.
              s[ervertypings]   Imports row, form, and service types from C# to TypeScript.
              t[ransform]       Executes clienttypes, mvc, and servertypings commands simultaneously.
              r[estore]         [Obsolete] Restores content (e.g., scripts, CSS) from .nupkg.

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