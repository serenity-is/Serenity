using Newtonsoft.Json.Linq;
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
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class ServerTypingsCommand
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void Run(string csproj, List<ExternalType> tsTypes)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            string[] assemblyFiles = null;

            if (config.ServerTypings == null ||
                config.ServerTypings.Assemblies.IsEmptyOrNull())
            {
                var xe = XElement.Parse(File.ReadAllText(csproj));
                var xtarget = xe.Descendants("TargetFramework").FirstOrDefault();

                if (xtarget == null || string.IsNullOrEmpty(xtarget.Value))
                {
                    System.Console.Error.WriteLine("Couldn't read TargetFramework from project file for server typings generation!");
                    Environment.Exit(1);
                }

                string outputName;
                var xasm = xe.Descendants("AssemblyName").FirstOrDefault();
                if (xasm == null || string.IsNullOrEmpty(xasm.Value))
                    outputName = Path.ChangeExtension(Path.GetFileName(csproj), null);
                else
                    outputName = xasm.Value;

                var outputExtension = ".dll";
                var targetFramework = xtarget.Value;
                if (targetFramework.StartsWith("net") &&
                    !targetFramework.StartsWith("netcoreapp"))
                    outputExtension = ".exe";

                var outputPath1 = Path.Combine(Path.GetDirectoryName(csproj), "bin/Debug/" + targetFramework + "/" + outputName + outputExtension)
                    .Replace('/', Path.DirectorySeparatorChar);
                var outputPath2 = Path.Combine(Path.GetDirectoryName(csproj), "bin/Release/" + targetFramework + "/" + outputName + outputExtension)
                    .Replace('/', Path.DirectorySeparatorChar);

                if (File.Exists(outputPath1))
                {
                    if (File.Exists(outputPath2) &&
                        File.GetLastWriteTime(outputPath1) < File.GetLastWriteTime(outputPath2))
                        assemblyFiles = new[] { outputPath2 };
                    else
                        assemblyFiles = new[] { outputPath1 };
                }
                else if (File.Exists(outputPath2))
                    assemblyFiles = new[] { outputPath2 };
                else
                {
                    System.Console.Error.WriteLine(String.Format("Couldn't find output file for server typings generation at {0}!" + Environment.NewLine + 
                        "Make sure project is built successfully before running Sergen", outputPath1));
                    Environment.Exit(1);
                }
            }

            if (assemblyFiles == null)
            {
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

                assemblyFiles = config.ServerTypings.Assemblies;
                for (var i = 0; i < assemblyFiles.Length; i++)
                {
                    var assemblyFile1 = Path.GetFullPath(assemblyFiles[i].Replace('/', Path.DirectorySeparatorChar));
                    var binDebugIdx = assemblyFile1.IndexOf("/bin/Debug/", StringComparison.OrdinalIgnoreCase);
                    string assemblyFile2 = assemblyFile1;
                    if (binDebugIdx >= 0)
                        assemblyFile2 = assemblyFile1.Substring(0, binDebugIdx) + "/bin/Release/" + assemblyFile1.Substring(binDebugIdx + "/bin/Release".Length);

                    assemblyFiles[i] = assemblyFile1;

                    if (File.Exists(assemblyFile1))
                    {
                        if (File.Exists(assemblyFile2) &&
                            File.GetLastWriteTime(assemblyFile1) < File.GetLastWriteTime(assemblyFile2))
                            assemblyFiles[i] = assemblyFile2;
                    }
                    else if (File.Exists(assemblyFile2))
                        assemblyFiles[i] = assemblyFile2;
                    else
                    {
                        System.Console.Error.WriteLine(String.Format(String.Format("Assembly file '{0}' specified in sergen.json is not found! " +
                        "This might happen when project is not successfully built or file name doesn't match the output DLL." +
                        "Please check path in sergen.json and try again.", assemblyFile1)));
                        Environment.Exit(1);
                    }
                }
            }

            if (config.RootNamespace.IsEmptyOrNull())
            {
                System.Console.Error.WriteLine("Please set RootNamespace option in sergen.json file!");
                Environment.Exit(1);
            }

            var generator = new ServerTypingsGenerator(assemblyFiles.ToArray());
            generator.LocalTexts = config.ServerTypings != null && config.ServerTypings.LocalTexts;

            var appSettings = Path.Combine(projectDir, "appsettings.json");
            if (generator.LocalTexts && File.Exists(appSettings))
            {
                try
                {
                    var obj = JObject.Parse(File.ReadAllText(appSettings));
                    var packages = ((obj["AppSettings"] as JObject)?["LocalTextPackages"] as JObject);
                    if (packages != null)
                    {
                        foreach (var p in packages.PropertyValues())
                            foreach (var x in p.Values<string>())
                                generator.LocalTextFilters.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error occured while parsing appsettings.json!" + Environment.NewLine + 
                        ex.ToString());
                }
            }

            var outDir = Path.Combine(projectDir, (config.ServerTypings.OutDir.TrimToNull() ?? "Imports/ServerTypings")
                .Replace('/', Path.DirectorySeparatorChar));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ServerTypings at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            new MultipleOutputHelper().WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}