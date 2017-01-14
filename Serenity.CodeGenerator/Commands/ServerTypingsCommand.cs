using Serenity.CodeGeneration;
using Serenity.Data;
using Serenity.Localization;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class ServerTypingsCommand
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void Run(string projectJson, List<ExternalType> tsTypes)
        {
            var projectDir = Path.GetDirectoryName(projectJson);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

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

            var outDir = Path.Combine(projectDir, (config.ServerTypings.OutDir.TrimToNull() ?? "Imports/ServerTypings")
                .Replace('/', Path.DirectorySeparatorChar));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ServerTypings at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            var rootPath = Path.GetFullPath(config.ServerTypings.Assemblies[0].Replace('/', Path.DirectorySeparatorChar));
            var loadContext = new ProjectLoadContext(projectJson, Path.GetDirectoryName(rootPath));

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in config.ServerTypings.Assemblies)
            {
                var fullName = Path.GetFullPath(assembly.Replace('/', Path.DirectorySeparatorChar));
                assemblies.Add(loadContext.LoadFromAssemblyPath(fullName));
            }

            Extensibility.ExtensibilityHelper.SelfAssemblies = new Assembly[]
            {
                typeof(LocalTextRegistry).GetAssembly(),
                typeof(SqlConnections).GetAssembly(),
                typeof(Row).GetAssembly(),
                typeof(SaveRequestHandler<>).GetAssembly(),
                typeof(WebSecurityHelper).GetAssembly()
            }.Concat(assemblies).Distinct().ToArray();

            var generator = new ServerTypingsGenerator(assemblies.ToArray());
            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            new MultipleOutputHelper().WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}