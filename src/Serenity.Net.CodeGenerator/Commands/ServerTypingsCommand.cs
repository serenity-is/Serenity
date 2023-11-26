using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ServerTypingsCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public bool Modules { get; set; }
    public List<ExternalType> TsTypes { get; set; }

    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        if (Modules && config.ServerTypings?.ModuleTypings == false ||
            !Modules && config.ServerTypings?.NamespaceTypings == false)
            return ExitCodes.Success;

        var transformType = Modules ? "Modular Server Types" : "Namespace Server Typings";
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
            ModuleTypings = Modules && config?.ServerTypings?.ModuleTypings != false
        };
        generator.ModuleReExports = generator.ModuleTypings && config?.ServerTypings?.ModuleReExports != false;
        generator.NamespaceTypings = !Modules && config?.ServerTypings?.NamespaceTypings != false;

        string outDir = Modules ? FileSystem.Combine(generator.DetermineModulesRoot(
            FileSystem, ProjectFile, config.RootNamespace), "ServerTypes") :
            FileSystem.Combine(projectDir, PathHelper.ToPath(
                config.ServerTypings?.OutDir.TrimToNull() ?? "Imports/ServerTypings"));

        generator.SetLocalTextFiltersFrom(FileSystem, FileSystem.Combine(projectDir, "appsettings.json"));
        generator.RootNamespaces.Add(config.RootNamespace);

        foreach (var type in TsTypes)
            generator.AddTSType(type);

        var generatedSources = generator.Run();

        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir,
            generatedSources.Where(x => x.Module == Modules)
                .Select(x => (x.Filename, x.Text)),
            deleteExtraPattern: ["*.ts"],
            endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }
}