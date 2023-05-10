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

    public static ExitCodes Run(string[] args, IGeneratorFileSystem fileSystem, 
        Func<IBuildProjectSystem> projectSystemFactory)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));

        string command = null;
        if (args.Length > 0)
            command = args[0].ToLowerInvariant().TrimToEmpty();

        string[] prjRefs = null;
        var prjRefsIdx = Array.FindIndex(args, x => x == "--projectrefs");
        if (prjRefsIdx >= 0 && prjRefsIdx < args.Length - 1)
        {
            prjRefs = args[prjRefsIdx + 1].Split(';', StringSplitOptions.RemoveEmptyEntries);
            args = args.Where((x, i) => i != prjRefsIdx && i != prjRefsIdx + 1).ToArray();
        }

        string csproj = null;
        if (command == "-p" && args.Length > 2)
        {
            csproj = args[1];
            command = args.Length > 2 ? args[1] : null;
        }

        if (command.IsEmptyOrNull())
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

        if (csproj == null)
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

            csproj = csprojs[0];
        }

        if (!fileSystem.FileExists(csproj))
            return ExitCodes.ProjectNotFound;

        var projectDir = fileSystem.GetFullPath(fileSystem.GetDirectoryName(csproj));

        try
        {
            if ("restore".StartsWith(command, StringComparison.Ordinal))
            {
                return new RestoreCommand(fileSystem, 
                    projectSystemFactory(), prjRefs).Run(csproj);
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
                    new MvcCommand(fileSystem).Run(csproj);
                }

                if (transformAll ||
                    "clienttypes".StartsWith(command, StringComparison.Ordinal) || command == "mvct")
                {
                    ensureTSTypes();
                    new ClientTypesCommand(fileSystem).Run(csproj,
                        (tsTypesNamespaces ?? new List<ExternalType>())
                        .Concat(tsTypesModules ?? new List<ExternalType>()).ToList());
                }

                if (transformAll ||
                    "servertypings".StartsWith(command, StringComparison.Ordinal))
                {
                    ensureTSTypes();

                    if (tsTypesNamespaces is not null)
                        new ServerTypingsCommand(fileSystem, modules: false)
                            .Run(csproj, tsTypesNamespaces);

                    if (tsTypesModules is not null)
                        new ServerTypingsCommand(fileSystem, modules: true)
                            .Run(csproj, tsTypesModules);
                }

                return ExitCodes.Success;
            }

            if ("generate".StartsWith(command, StringComparison.Ordinal))
            {
                new GenerateCommand(fileSystem).Run(csproj, args.Skip(1).ToArray());
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
        Console.WriteLine("Serenity Code Generator " +
            Assembly.GetEntryAssembly().GetName().Version);
        Console.WriteLine();
        Console.WriteLine("Usage: sergen [command]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("    g[enerate]        Launches table code generator");
        Console.WriteLine("    r[estore]         Restores content, e.g. scripts, fonts and css from .nupkg");
        Console.WriteLine();
        Console.WriteLine("    c[lienttypes]     Imports editor, formatter types from TypeScript to CS");
        Console.WriteLine("    m[vc]             Generates intellisense helpers for view locations");
        Console.WriteLine("    s[ervertypings]   Imports row, form, service types from CS to TypeScript");
        Console.WriteLine("    t[ransform]       Runs clienttypes, mvc and servertypings commands at once");
    }
}