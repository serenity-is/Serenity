using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ServerTypingsCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public List<ExternalType> TsTypes { get; set; }

    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        var transformType = "Modular Server Types";
        var transformFor = FileSystem.GetFileNameWithoutExtension(ProjectFile);
        Console.WriteLine($"Transforming {transformType} for {transformFor}", ConsoleColor.Cyan);

        var assemblyFiles = Project.GetAssemblyList(config.ServerTypings?.Assemblies);
        if (assemblyFiles == null || assemblyFiles.Length == 0)
        {
            Console.Error("Can't determine the output assemblies! Please ensure the project is built successfully.");
            return ExitCodes.CantDetermineOutputAssemblies;
        }

        if (string.IsNullOrEmpty(config.RootNamespace))
            config.RootNamespace = config.GetRootNamespaceFor(Project);

        var generator = new ServerTypingsGenerator(FileSystem, assemblyFiles.ToArray())
        {
            LocalTexts = config.ServerTypings != null && config.ServerTypings.LocalTexts,
            ModuleReExports = config?.ServerTypings?.ModuleReExports != false
        };
        if (config?.ServerTypings?.PreferRelativePaths ?? true)
        {
            generator.ModulesPathAlias = null;
            generator.RootPathAlias = null;
        }

        string outDir = FileSystem.Combine(generator.DetermineModulesRoot(
            FileSystem, ProjectFile, config.RootNamespace), "ServerTypes");

        generator.SetLocalTextFiltersFrom(FileSystem, FileSystem.Combine(projectDir, "appsettings.json"));
        generator.RootNamespaces.Add(config.RootNamespace);

        generator.AddBuiltinTSTypes();
        foreach (var type in TsTypes)
            generator.AddTSType(type);

        var generatedSources = generator.Run();

        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir,
            generatedSources.Select(x => (x.Filename, x.Text)),
            deleteExtraPattern: ["*.ts"],
            endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }
}