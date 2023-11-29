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
                Console.Error(Texts.NoProjectFiles);
                return ExitCodes.NoProjectFiles;
            }

            if (csprojs.Length > 1)
            {
                Console.Error(Texts.MultipleProjectFiles);
                return ExitCodes.MultipleProjectFiles;
            }

            projectFile = csprojs[0];
        }

        if (!FileSystem.FileExists(projectFile))
        {
            Console.Error(string.Format(Texts.CantFindProject, projectFile));
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

        if (isCommand(CommandKeys.Restore, CommandAliases.Restore))
        {
            ArgumentNullException.ThrowIfNull(BuildSystemFactory);

            return RunCommand(new RestoreCommand(project, Console)
            {
                BuildSystem = BuildSystemFactory(),
                ProjectReferences = projectRefs
            });
        }

        if (isCommand(CommandKeys.Generate, CommandAliases.Generate))
        {
            return RunCommand(new GenerateCommand(project, Console)
            {
                Arguments = arguments
            });
        }

        bool transform = isCommand(CommandKeys.Transform, CommandAliases.Transform);
        bool mvcAndClientTypes = isCommand(CommandKeys.MvcAndClientTypes);
        bool clientTypes = transform || mvcAndClientTypes ||
            isCommand(CommandKeys.ClientTypes, CommandAliases.ClientTypes);
        bool serverTypings = transform ||
            isCommand(CommandKeys.ServerTypings, CommandAliases.ServerTypings);
        bool mvc = transform || mvcAndClientTypes ||
            isCommand(CommandKeys.Mvc, CommandAliases.Mvc);

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
                var fileSystem = new TSCachingFileSystem(FileSystem);

                TSConfigHelper.LocateTSConfigFiles(fileSystem, projectDir,
                    out string modulesPath, out string namespacesPath);

                if (modulesPath is null &&
                    namespacesPath is null)
                    namespacesPath = fileSystem.Combine(projectDir, "tsconfig.json");

                if (namespacesPath is not null)
                {
                    var nsLister = new TSTypeLister(fileSystem, namespacesPath, new());
                    tsTypesNamespaces = nsLister.List();
                }

                if (modulesPath is not null)
                {
                    var mdLister = new TSTypeLister(fileSystem, modulesPath, new());
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
        console.ShowHelp(Texts.Help);
    }

    public static class CommandKeys
    {
        public const string ClientTypes = "clienttypes";
        public const string Generate = "generate";
        public const string Mvc = "mvc";
        public const string MvcAndClientTypes = "mvct";
        public const string Restore = "restore";
        public const string ServerTypings = "servertypings";
        public const string Transform = "transform";
    }

    public static class CommandAliases
    {
        public const string ClientTypes = "c";
        public const string Generate = "g";
        public const string Transform = "t";
        public const string Mvc = "m";
        public const string Restore = "r";
        public const string ServerTypings = "s";
    }
}