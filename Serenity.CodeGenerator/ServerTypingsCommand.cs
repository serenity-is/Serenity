using JavaScriptEngineSwitcher.ChakraCore;
using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class ServerTypingsCommand
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void Run(string projectJson)
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

            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in config.ServerTypings.Assemblies)
                assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(root, assembly)));

            var generator = new ServerTypingsGenerator(assemblies.ToArray());
            generator.RootNamespaces.Add(config.RootNamespace);

            var tsTypeLister = new TSTypeLister(root);
            var tsTypes = tsTypeLister.List();
            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            var outDir = Path.Combine(root, (config.ServerTypings.OutDir.TrimToNull() ?? "Modules/Common/Imports/ServerTypings")
                .Replace('/', Path.DirectorySeparatorChar));

            foreach (var pair in codeByFilename)
            {
                var outFile = Path.Combine(outDir, pair.Key);
                if (File.Exists(outFile))
                {
                    var content = File.ReadAllText(outFile, utf8);
                    if (content.Trim().Replace("\r", "") ==
                        pair.Value.Trim().Replace("\r", ""))
                        continue;
                }

                File.WriteAllText(outFile, pair.Value, utf8);
            }
        }
    }

    public class TSTypeLister
    {
        private string projectDir;

        public TSTypeLister(string projectDir)
        {
            this.projectDir = projectDir;
        }

        private string GetEmbeddedScript(string name)
        {
            using (var sr = new StreamReader(this.GetType()
                .GetTypeInfo().Assembly.GetManifestResourceStream(name)))
            {
                return sr.ReadToEnd();
            }
        }

        private JavaScriptEngineSwitcher.Core.IJsEngine SetupJsEngine()
        {
            var jsEngine = new ChakraCoreJsEngineFactory().CreateEngine();
            try
            {
                jsEngine.Evaluate(GetEmbeddedScript("typeScriptServices.js"));
                jsEngine.Evaluate(GetEmbeddedScript("Serenity.CodeGeneration.js"));

                return jsEngine;
            }
            catch
            {
                jsEngine.Dispose();
                throw;
            }
        }

        public List<ExternalType> List()
        {
            var files = Directory.GetFiles(Path.Combine(projectDir, @"Modules"), "*.ts", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"typings"), "*.ts", SearchOption.AllDirectories))
                .Where(x => !x.EndsWith(".d.ts") || x.IndexOf("Serenity") >= 0);

            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("fileName", "/lib.d.ts");
                jsEngine.SetVariableValue("sourceText", GetEmbeddedScript("lib.d.ts_"));
                jsEngine.Evaluate("Serenity.CodeGeneration.addSourceFile(fileName, sourceText)");

                foreach (var file in files)
                {
                    var name = file.Substring(projectDir.Length - 1).Replace("\\", "/");
                    jsEngine.SetVariableValue("fileName", name);
                    jsEngine.SetVariableValue("sourceText", File.ReadAllText(file));
                    jsEngine.Evaluate("Serenity.CodeGeneration.addSourceFile(fileName, sourceText)");
                }

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseTypes(sourceText))");

                return JSON.Parse<List<ExternalType>>(json);
            }
        }
    }
}