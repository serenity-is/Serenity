using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class Program
{
    public static void Main(string[] args)
    {
        var exitCode = Run(args, new PhysicalGeneratorFileSystem(), () => new MSBuild.MSBuildProjectSystem());
        if (exitCode != ExitCodes.Success &&
            exitCode != ExitCodes.Help)
            Environment.Exit((int)exitCode);
    }

    public static ExitCodes Run(string[] args, IFileSystem fileSystem,
        Func<IBuildProjectSystem> projectSystemFactory)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        string projectFile = null;
        var csprojIdx = Array.FindIndex(args, x => x == "-p");
        if (csprojIdx >= 0)
        {
            if (csprojIdx >= args.Length - 1)
            {
                WriteHelp();
                return ExitCodes.InvalidArguments;
            }
            projectFile = args[csprojIdx + 1];
            args = args.Where((x, i) => i != csprojIdx && i != csprojIdx + 1).ToArray();
        }

        string[] prjRefs = null;
        var prjRefsIdx = Array.FindIndex(args, x => x == "--projectrefs");
        if (prjRefsIdx >= 0)
        {
            if (prjRefsIdx >= args.Length - 1)
            {
                WriteHelp();
                return ExitCodes.InvalidArguments;
            }

            prjRefs = args[prjRefsIdx + 1].Split(';', StringSplitOptions.RemoveEmptyEntries);
            args = args.Where((x, i) => i != prjRefsIdx && i != prjRefsIdx + 1).ToArray();
        }

        args = ProjectFileInfo.FilterPropertyParams(args, out var props).ToArray();
        var errorProp = props.FirstOrDefault(x => x.Value == null);
        if (errorProp.Key != null)
        {
            Console.Error.WriteLine($"Property values should be passed " +
                $"in format '-prop:Name=Value'. Argument '{errorProp.Value}' is invalid!");
            return ExitCodes.InvalidArguments;
        }

        string command = null;
        if (args.Length > 0)
            command = args[0].ToLowerInvariant().TrimToEmpty();

        if (string.IsNullOrEmpty(command))
        {
            WriteHelp();
            return ExitCodes.NoCommand;
        }

        if (command == "-?" ||
            command == "--help")
        {
            WriteHelp();
            return ExitCodes.Help;
        }

        if (projectFile == null)
        {
            var csprojs = fileSystem.GetFiles(".", "*.csproj");
            if (csprojs.Length == 0)
            {
                Console.Error.WriteLine("Can't find a project file in current directory!");
                Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                return ExitCodes.NoProjectFiles;
            }

            if (csprojs.Length > 1)
            {
                Console.Error.WriteLine("Multiple project files found in current directory!");
                Console.Error.WriteLine("Please run Sergen in a folder that contains only one Asp.Net Core project.");
                return ExitCodes.MultipleProjectFiles;
            }

            projectFile = csprojs[0];
        }

        if (!fileSystem.FileExists(projectFile))
        {
            Console.Error.WriteLine($"Can't find a project file at: {projectFile}");
            return ExitCodes.ProjectNotFound;
        }

        var projectDir = fileSystem.GetDirectoryName(fileSystem.GetFullPath(projectFile));
        var project = new ProjectFileInfo(fileSystem, projectFile, props);
        try
        {
            if ("restore".StartsWith(command, StringComparison.Ordinal))
                return new RestoreCommand(project, projectSystemFactory(), prjRefs).Run();

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
                        TSConfigHelper.LocateTSConfigFiles(fileSystem, projectDir,
                            out string modulesPath, out string namespacesPath);

                        if (modulesPath is null &&
                            namespacesPath is null)
                            namespacesPath = fileSystem.Combine(projectDir, "tsconfig.json");

                        if (namespacesPath is not null)
                        {
                            var nsLister = new TSTypeLister(fileSystem, namespacesPath);
                            tsTypesNamespaces = nsLister.List();
                        }

                        if (modulesPath is not null)
                        {
                            var mdLister = new TSTypeLister(fileSystem, modulesPath);
                            tsTypesModules = mdLister.List();
                        }
                    }
                }

                bool transformAll = "transform".StartsWith(command, StringComparison.Ordinal);

                if (transformAll ||
                    "mvct".StartsWith(command, StringComparison.Ordinal))
                {
                    new MvcCommand(project).Run();
                }

                if (transformAll ||
                    "clienttypes".StartsWith(command, StringComparison.Ordinal) || command == "mvct")
                {
                    ensureTSTypes();
                    new ClientTypesCommand(project).Run([.. (tsTypesNamespaces ?? []), .. tsTypesModules ?? []]);
                }

                if (transformAll ||
                    "servertypings".StartsWith(command, StringComparison.Ordinal))
                {
                    ensureTSTypes();

                    if (tsTypesNamespaces is not null)
                        new ServerTypingsCommand(project, modules: false)
                            .Run(tsTypesNamespaces);

                    if (tsTypesModules is not null)
                        new ServerTypingsCommand(project, modules: true)
                            .Run(tsTypesModules);
                }

                return ExitCodes.Success;
            }

            if ("generate".StartsWith(command, StringComparison.Ordinal))
            {
                new GenerateCommand(project, Spectre.Console.AnsiConsole.Console)
                    .Run(args.Skip(1).ToArray());
                return ExitCodes.Success;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return ExitCodes.Exception;
        }

        WriteHelp();
        return ExitCodes.InvalidCommand;
    }

    private static void WriteHelp()
    {
        Console.WriteLine($"""""
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