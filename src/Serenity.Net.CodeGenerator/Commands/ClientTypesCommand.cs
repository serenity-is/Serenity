using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator
{
    public class ClientTypesCommand : BaseFileSystemCommand
    {
        public ClientTypesCommand(IGeneratorFileSystem fileSystem) 
            : base(fileSystem)
        {
        }

        public void Run(string csproj, List<ExternalType> tsTypes)
        {
            var projectDir = fileSystem.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(fileSystem,
                fileSystem.Combine(projectDir, "sergen.json"));

            config.ClientTypes ??= new GeneratorConfig.ClientTypesConfig();

            if (config.RootNamespace.IsEmptyOrNull())
                config.RootNamespace = config.GetRootNamespaceFor(fileSystem, csproj);

            var outDir = fileSystem.Combine(projectDir, PathHelper.ToPath(config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes"));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ClientTypes at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            var generator = new ClientTypesGenerator();
            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            MultipleOutputHelper.WriteFiles(fileSystem, outDir, codeByFilename, "*.ts");
        }
    }
}