using Serenity.CodeGeneration;
using System.IO;

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

        if (Modules && config.ServerTypings?.ModuleTypings == false)
            return ExitCodes.Success;

        if (!Modules && config.ServerTypings?.NamespaceTypings == false)
            return ExitCodes.Success;

        var assemblyFiles = Project.GetAssemblyList(config.ServerTypings?.Assemblies);

        if (assemblyFiles == null || assemblyFiles.Length == 0)
            return ExitCodes.CantDetermineOutputAssemblies;

        if (string.IsNullOrEmpty(config.RootNamespace))
            config.RootNamespace = config.GetRootNamespaceFor(Project);

        var generator = new ServerTypingsGenerator(FileSystem, assemblyFiles.ToArray())
        {
            LocalTexts = config.ServerTypings != null && config.ServerTypings.LocalTexts,
            ModuleTypings = Modules && config?.ServerTypings?.ModuleTypings != false
        };
        generator.ModuleReExports = generator.ModuleTypings && config?.ServerTypings?.ModuleReExports != false;
        generator.NamespaceTypings = !Modules && config?.ServerTypings?.NamespaceTypings != false;

        string outDir = Modules ? Path.Combine(generator.DetermineModulesRoot(
            FileSystem, ProjectFile, config.RootNamespace), "ServerTypes") :
            FileSystem.Combine(projectDir, PathHelper.ToPath(
                config.ServerTypings?.OutDir.TrimToNull() ?? "Imports/ServerTypings"));

        generator.SetLocalTextFiltersFrom(FileSystem, FileSystem.Combine(projectDir, "appsettings.json"));

        Console.WriteLine("Transforming " + (Modules ? "ServerTypes" : "ServerTypings"), ConsoleColor.Cyan);

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