using Newtonsoft.Json.Linq;
using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class ServerTypingsCommand
    {
        public static void Run(string csproj, List<ExternalType> tsTypes)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            string[] assemblyFiles = null;

            if (config.ServerTypings == null ||
                config.ServerTypings.Assemblies.IsEmptyOrNull())
            {
                var targetFramework = ProjectFileHelper.ExtractTargetFrameworkFrom(csproj);

                if (string.IsNullOrEmpty(targetFramework))
                {
                    Console.Error.WriteLine("Couldn't read TargetFramework from " +
                        "project file for server typings generation!");
                    Environment.Exit(1);
                }

                string outputName = ProjectFileHelper.ExtractAssemblyNameFrom(csproj)
                    ?? Path.ChangeExtension(Path.GetFileName(csproj), null);

                var outputExtension = ".dll";
                if (targetFramework.StartsWith("net", StringComparison.OrdinalIgnoreCase) &&
                    !targetFramework.StartsWith("netcoreapp", StringComparison.Ordinal) &&
                    targetFramework.IndexOf('.', StringComparison.Ordinal) < 0)
                    outputExtension = ".exe";

                var outputPath1 = Path.Combine(Path.GetDirectoryName(csproj),
                    PathHelper.ToPath("bin/Debug/" + targetFramework + "/" + outputName + outputExtension));
                var outputPath2 = Path.Combine(Path.GetDirectoryName(csproj), 
                    PathHelper.ToPath("bin/Release/" + targetFramework + "/" + outputName + outputExtension));

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
                    Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture,
                        "Couldn't find output file for server typings generation at {0}!" + Environment.NewLine + 
                        "Make sure project is built successfully before running Sergen", outputPath1));
                    Environment.Exit(1);
                }
            }

            if (assemblyFiles == null)
            {
                if (config.ServerTypings == null)
                {
                    Console.Error.WriteLine("ServerTypings is not configured in sergen.json file!");
                    Environment.Exit(1);
                }

                if (config.ServerTypings.Assemblies.IsEmptyOrNull())
                {
                    Console.Error.WriteLine("ServerTypings has no assemblies configured in sergen.json file!");
                    Environment.Exit(1);
                }

                assemblyFiles = config.ServerTypings.Assemblies;
                for (var i = 0; i < assemblyFiles.Length; i++)
                {
                    var assemblyFile1 = PathHelper.ToUrl(Path.GetFullPath(PathHelper.ToPath(assemblyFiles[i])));
                    var binDebugIdx = assemblyFile1.IndexOf("/bin/Debug/", StringComparison.OrdinalIgnoreCase);
                    string assemblyFile2 = assemblyFile1;
                    if (binDebugIdx >= 0)
                        assemblyFile2 = string.Concat(assemblyFile1.AsSpan(0, binDebugIdx), "/bin/Release/", assemblyFile1[(binDebugIdx + "/bin/Release".Length)..]);

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
                        Console.Error.WriteLine(string.Format(CultureInfo.CurrentCulture, string.Format(CultureInfo.CurrentCulture, 
                            "Assembly file '{0}' specified in sergen.json is not found! " +
                            "This might happen when project is not successfully built or file name doesn't match the output DLL." +
                            "Please check path in sergen.json and try again.", assemblyFile1)));
                        Environment.Exit(1);
                    }
                }
            }

            if (config.RootNamespace.IsEmptyOrNull())
                config.RootNamespace = config.GetRootNamespaceFor(csproj);

            var generator = new ServerTypingsGenerator(assemblyFiles.ToArray())
            {
                LocalTexts = config.ServerTypings != null && config.ServerTypings.LocalTexts
            };

            var appSettings = Path.Combine(projectDir, "appsettings.json");
            if (generator.LocalTexts && File.Exists(appSettings))
            {
                try
                {
                    var obj = JObject.Parse(File.ReadAllText(appSettings));
                    if ((obj["AppSettings"] as JObject)?["LocalTextPackages"] is JObject packages)
                    {
                        foreach (var p in packages.PropertyValues())
                            foreach (var x in p.Values<string>())
                                generator.LocalTextFilters.Add(x);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while parsing appsettings.json!" + Environment.NewLine + 
                        ex.ToString());
                }
            }

            var outDir = Path.Combine(projectDir, PathHelper.ToPath((config.ServerTypings?.OutDir.TrimToNull() ?? "Imports/ServerTypings")));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming ServerTypings at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            generator.RootNamespaces.Add(config.RootNamespace);

            foreach (var type in tsTypes)
                generator.AddTSType(type);

            var codeByFilename = generator.Run();
            MultipleOutputHelper.WriteFiles(outDir, codeByFilename, "*.ts");
        }
    }
}