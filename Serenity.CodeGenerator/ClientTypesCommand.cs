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

        public void Run(string projectJson, List<ExternalType> tsTypes)
        {
            var root = Path.GetDirectoryName(projectJson);
            var configFile = Path.Combine(root, "sergen.json");
            if (!File.Exists(configFile))
            {
                System.Console.Error.WriteLine("Can't find sergen.json in current directory!");
                Environment.Exit(1);
            }

            var config = GeneratorConfig.LoadFromJson(File.ReadAllText(configFile));
            config.ClientTypes = config.ClientTypes ?? new GeneratorConfig.ClientTypesConfig();

            if (config.RootNamespace.IsEmptyOrNull())
            {
                System.Console.Error.WriteLine("Please set RootNamespace option in sergen.json file!");
                Environment.Exit(1);
            }

            var outDir = Path.Combine(root, (config.ClientTypes.OutDir.TrimToNull() ?? "Imports/ClientTypes")
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