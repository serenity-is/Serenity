using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ClientTypesCommand(IProjectFileInfo project, IGeneratorConsole console) 
    : BaseGeneratorCommand(project, console)
{
    public List<ExternalType> TsTypes { get; set; }

    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        config.ClientTypes ??= new GeneratorConfig.ClientTypesConfig();

        if (string.IsNullOrEmpty(config.RootNamespace))
            config.RootNamespace = config.GetRootNamespaceFor(Project);

        var transformFor = FileSystem.GetFileNameWithoutExtension(ProjectFile);
        Console.WriteLine($"Transforming Client Types for {transformFor}", ConsoleColor.Cyan);

        var generator = new ClientTypesGenerator()
        {
            FileScopedNamespaces = config.FileScopedNamespaces == true
        };

        if (config.IncludeGlobalUsings != null)
            generator.GlobalUsings.AddRange(config.IncludeGlobalUsings);

        generator.RootNamespaces.Add(config.RootNamespace);

        foreach (var type in TsTypes)
            generator.AddTSType(type);

        var outDir = FileSystem.Combine(projectDir, PathHelper.ToPath(
            config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes"));

        var generatedSources = generator.Run();
        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir,
            generatedSources.Select(x => (x.Filename, x.Text)), 
            deleteExtraPattern: null,
            endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }
}