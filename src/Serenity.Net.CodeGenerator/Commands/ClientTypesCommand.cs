using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class ClientTypesCommand
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void Run(string csproj, List<ExternalType> tsTypes)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            config.ClientTypes = config.ClientTypes ?? new GeneratorConfig.ClientTypesConfig();

            if (config.RootNamespace.IsEmptyOrNull())
            {
                Console.Error.WriteLine("Please set RootNamespace option in sergen.json file!");
                Environment.Exit(1);
            }

            var outDir = Path.Combine(projectDir, (config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes")
                .Replace('/', Path.DirectorySeparatorChar));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ClientTypes at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            var generator = new ClientTypesGenerator();
            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            new MultipleOutputHelper().WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}