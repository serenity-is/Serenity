using Serenity.CodeGeneration;
using System.IO;

namespace Serenity.CodeGenerator
{
    public class ClientTypesCommand
    {
        public static void Run(string csproj, List<ExternalType> tsTypes)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            config.ClientTypes ??= new GeneratorConfig.ClientTypesConfig();

            if (config.RootNamespace.IsEmptyOrNull())
                config.RootNamespace = config.GetRootNamespaceFor(csproj);

            var outDir = Path.Combine(projectDir, PathHelper.ToPath(config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes"));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ClientTypes at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            var generator = new ClientTypesGenerator();
            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            MultipleOutputHelper.WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}