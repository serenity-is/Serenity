using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class ClientTypesCommand(ProjectFileInfo project) : BaseGeneratorCommand(project)
{
    public void Run(List<ExternalType> tsTypes)
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        config.ClientTypes ??= new GeneratorConfig.ClientTypesConfig();

        if (string.IsNullOrEmpty(config.RootNamespace))
            config.RootNamespace = config.GetRootNamespaceFor(new ProjectFileInfo(FileSystem, ProjectFile));

        var outDir = FileSystem.Combine(projectDir, PathHelper.ToPath(config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes"));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Transforming ClientTypes at: ");
        Console.ResetColor();
        Console.WriteLine(outDir);

        var generator = new ClientTypesGenerator()
        {
            FileScopedNamespaces = config.FileScopedNamespaces == true
        };

        if (config.IncludeGlobalUsings != null)
            generator.GlobalUsings.AddRange(config.IncludeGlobalUsings);

        generator.RootNamespaces.Add(config.RootNamespace);

        foreach (var type in tsTypes)
            generator.AddTSType(type);

        var generatedSources = generator.Run();
        MultipleOutputHelper.WriteFiles(FileSystem, outDir, 
            generatedSources.Select(x => (x.Filename, x.Text)), 
            deleteExtraPattern: null,
            endOfLine: config.EndOfLine);
    }
}