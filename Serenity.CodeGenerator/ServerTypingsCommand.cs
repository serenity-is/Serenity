using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class ServerTypingsCommand
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
            if (config.ServerTypings == null)
            {
                System.Console.Error.WriteLine("ServerTypings is not configured in sergen.json file!");
                Environment.Exit(1);
            }

            if (config.ServerTypings.Assemblies.IsEmptyOrNull())
            {
                System.Console.Error.WriteLine("ServerTypings has no assemblies configured in sergen.json file!");
                Environment.Exit(1);
            }

            if (config.RootNamespace.IsEmptyOrNull())
            {
                System.Console.Error.WriteLine("Please set RootNamespace option in sergen.json file!");
                Environment.Exit(1);
            }

            var outDir = Path.Combine(root, (config.ServerTypings.OutDir.TrimToNull() ?? "Imports/ServerTypings")
                .Replace('/', Path.DirectorySeparatorChar));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ServerTypings at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in config.ServerTypings.Assemblies)
                assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(root, assembly)));

            var generator = new ServerTypingsGenerator(assemblies.ToArray());
            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            new MultipleOutputHelper().WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}