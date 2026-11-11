using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class Cli(IFileSystem fileSystem, IGeneratorConsole console, IProcessExecutor processExecutor)
{
    private readonly IFileSystem FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IGeneratorConsole Console = console ?? throw new ArgumentNullException(nameof(console));
    private readonly IProcessExecutor ProcessExecutor = processExecutor ?? throw new ArgumentNullException(nameof(processExecutor));

    public Func<string, Func<string, string?>, IProjectFileInfo>? ProjectFactory { get; set; }
    public Func<BaseGeneratorCommand, ExitCodes>? RunCommandCallback { get; set; }

    /// <summary>
    /// Gets or sets the location of the sergen that is running, to find out whether the project uses another one.
    /// </summary>
    public string? SergenLocation { get; set; } = typeof(Cli).Assembly.Location;

    /// <summary>
    /// Gets or sets the version of the sergen that is running, to compare with the Serenity.Net.Web of the project.
    /// </summary>
    public Version? SergenVersion { get; set; } = typeof(Cli).Assembly.GetName().Version;

    public ExitCodes Run(IArgumentReader arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        var projectFile = arguments.GetString(["p", "project"]);
        var globalUsings = arguments.GetString(["globalusings", "global-usings"], requiresValue: false);
        var projectRefs = arguments.GetStrings(["projectrefs", "project-refs"], requiresValue: false);
        var propertyArgs = arguments.GetDictionary(["prop", "props", "property"],
            separators: [',', ';'], requiresValue: false);

        var command = arguments.GetCommand();

        if (arguments.HasHelpSwitch())
        {
            WriteHelp(Console, command);
            return ExitCodes.Help;
        }

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
        string? getPropertyArgument(string name) => propertyArgs.TryGetValue(name, out var value) ? value : 
            string.Equals(name, "globalusings", StringComparison.OrdinalIgnoreCase) ? globalUsings : null;

        var project = ProjectFactory?.Invoke(projectFile, getPropertyArgument)
            ?? new ProjectFileInfo(fileSystem, projectFile, getPropertyArgument, Console.Error);

        bool isCommand(params string[] args)
        {
            return args.Any(x => string.Equals(x, command, StringComparison.OrdinalIgnoreCase));
        }

        if (isCommand(CommandKeys.Generate, CommandAliases.Generate))
        {
            WarnIfNotTheSergenOfTheProject(project);
            return RunCommand(new GenerateCommand(project, Console)
            {
                Arguments = arguments
            });
        }

        List<ExternalType>? tsTypes = null;

        void ensureTSTypes()
        {
            if (tsTypes is not null)
                return;

            var fileSystem = new TSCachingFileSystem(FileSystem);

            var tsConfigFile = TSConfigHelper.LocateTSConfigFile(fileSystem, projectDir) ??
                fileSystem.Combine(projectDir, "tsconfig.json");

            var typeLister = new TSTypeLister(fileSystem, tsConfigFile, new());
            tsTypes = typeLister.List();
        }

        if (isCommand(CommandKeys.Doctor))
        {
            try
            {
                ensureTSTypes();
                string path = fileSystem.Combine(projectDir, "node_modules", ".sergen", "ts-types.json");
                fileSystem.CreateDirectory(fileSystem.GetDirectoryName(path));
                fileSystem.WriteAllText(path,
                    JSON.StringifyIndented((tsTypes ?? [])
                        .OrderBy(x => x.Module ?? "", StringComparer.Ordinal)
                        .ThenBy(x => x.Namespace ?? "", StringComparer.Ordinal)
                        .ThenBy(x => x.Name ?? "", StringComparer.Ordinal)));
            }
            catch
            {
                // ignore
            }

            return RunCommand(new DoctorCommand(project, Console, ProcessExecutor)
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

        var force = arguments.GetBoolean(["force"]) == true;
        arguments.ThrowIfRemaining();

        WarnIfNotTheSergenOfTheProject(project);

        // the generators of Pro.Coder write these on build, and the files written here would be duplicates of them
        if ((mvc || clientTypes) && !force && project.UsesSourceGenerators())
        {
            Console.Error(Texts.GeneratedOnBuild);
            return ExitCodes.GeneratedOnBuild;
        }

        ExitCodes? exitCode = null;
        if (mvc)
            exitCode = RunCommand(new MvcCommand(project, Console), exitCode);

        if (clientTypes)
        {
            ensureTSTypes();
            exitCode = RunCommand(new ClientTypesCommand(project, Console)
            {
                TsTypes = [.. (tsTypes ?? [])]
            }, exitCode);
        }

        if (serverTypings)
        {
            ensureTSTypes();

            if (tsTypes is not null)
            {
                exitCode = RunCommand(new ServerTypingsCommand(project, Console)
                {
                    TsTypes = tsTypes
                }, exitCode);
            }
        }

        return exitCode ?? ExitCodes.InvalidCommand;
    }

    private void WarnIfNotTheSergenOfTheProject(IProjectFileInfo project)
    {
        if (!WarnIfProjectUsesAnotherSergen(project))
            WarnIfVersionDiffersFromSerenityWeb(project);
    }

    /// <summary>
    /// Warns when sergen does not have the version of the Serenity.Net.Web package of the project, as when the dotnet tool
    /// of the project is not restored or updated, which the doctor command tells about too. It is for the projects that
    /// run the tool, as the ones running sergen from a path are compared with it, and the ones referencing Serenity as a
    /// project have no version to compare.
    /// </summary>
    private void WarnIfVersionDiffersFromSerenityWeb(IProjectFileInfo project)
    {
        if (SergenVersion is null || project.GetSerenityWebVersion() is not { } serenityVersion)
            return;

        if (serenityVersion.Major != SergenVersion.Major ||
            serenityVersion.Minor != SergenVersion.Minor ||
            serenityVersion.Build != SergenVersion.Build)
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.DifferentSergenVersion,
                SergenVersion, serenityVersion), ConsoleColor.Yellow);
    }

    /// <summary>
    /// Warns when the project runs sergen from a path, e.g. one built from the source of Serenity, and this is not it,
    /// as it may be a released version that has not got the changes of that one.
    /// </summary>
    /// <returns>True when the project runs sergen from a path, whether or not it is this one.</returns>
    private bool WarnIfProjectUsesAnotherSergen(IProjectFileInfo project)
    {
        var command = project.GetDotNetSergen()?.Trim();
        if (string.IsNullOrEmpty(command) || string.IsNullOrEmpty(SergenLocation) ||
            !command.StartsWith("dotnet ", StringComparison.OrdinalIgnoreCase))
            return false;

        var expected = command[7..].Trim().Trim('"').Trim();
        // 'dotnet sergen' is the tool, which does not have a path to compare
        if (string.IsNullOrEmpty(expected) || string.Equals(expected, "sergen", StringComparison.OrdinalIgnoreCase))
            return false;

        string withoutExtension(string path) => FileSystem.ChangeExtension(FileSystem.GetFullPath(path), null) ?? path;

        if (!string.Equals(withoutExtension(expected), withoutExtension(SergenLocation), StringComparison.OrdinalIgnoreCase))
            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.DifferentSergen, expected, SergenLocation),
                ConsoleColor.Yellow);

        return true;
    }

    private ExitCodes RunCommand(BaseGeneratorCommand command, ExitCodes? current = null)
    {
        var result = RunCommandCallback?.Invoke(command) ?? command.Run();
        return current is null || current == ExitCodes.Success ? result : current.Value;
    }

    public static void WriteHelp(IGeneratorConsole console, string? command = null)
    {
        console.ShowHelp(command != null &&
            (string.Equals(command, CommandKeys.Generate, StringComparison.OrdinalIgnoreCase) ||
             string.Equals(command, CommandAliases.Generate, StringComparison.OrdinalIgnoreCase))
                ? Texts.GenerateHelp
                : Texts.Help);
    }

    public static class CommandKeys
    {
        public const string ClientTypes = "clienttypes";
        public const string Doctor = "doctor";
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