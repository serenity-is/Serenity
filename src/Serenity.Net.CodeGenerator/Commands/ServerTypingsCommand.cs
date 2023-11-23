using Newtonsoft.Json.Linq;
using Serenity.CodeGeneration;
using System.IO;

namespace Serenity.CodeGenerator;

public class ServerTypingsCommand(IProjectFileInfo project, bool modules) : BaseGeneratorCommand(project)
{
    private readonly bool modules = modules;

    public void Run(List<ExternalType> tsTypes)
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        if (modules && config.ServerTypings?.ModuleTypings == false)
            return;

        if (!modules && config.ServerTypings?.NamespaceTypings == false)
            return;

        var assemblyFiles = DetermineAssemblyFiles(Project, config, error =>
        {
            Console.Error.WriteLine(error);
            Environment.Exit(1);
        });

        if (string.IsNullOrEmpty(config.RootNamespace))
            config.RootNamespace = config.GetRootNamespaceFor(Project);

        var generator = new ServerTypingsGenerator(FileSystem, assemblyFiles.ToArray())
        {
            LocalTexts = config.ServerTypings != null && config.ServerTypings.LocalTexts
        };

        var appSettings = FileSystem.Combine(projectDir, "appsettings.json");
        if (generator.LocalTexts && FileSystem.FileExists(appSettings))
        {
            try
            {
                var obj = JObject.Parse(FileSystem.ReadAllText(appSettings));
                if ((obj["LocalTextPackages"] ?? ((obj["AppSettings"] as JObject)?["LocalTextPackages"])) is JObject packages)
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

        var outDir = FileSystem.Combine(projectDir, 
            PathHelper.ToPath((config.ServerTypings?.OutDir.TrimToNull() ?? 
                (modules ? "Modules/ServerTypes" : "Imports/ServerTypings"))));

        generator.ModuleTypings = modules && config?.ServerTypings?.ModuleTypings != false;
        generator.ModuleReExports = generator.ModuleTypings && config?.ServerTypings?.ModuleReExports != false;
        generator.NamespaceTypings = !modules && config?.ServerTypings?.NamespaceTypings != false;

        if (modules)
        {
            var modulesDir = FileSystem.Combine(projectDir, "Modules");
            if (!FileSystem.DirectoryExists(modulesDir) ||
                FileSystem.GetFiles(modulesDir, "*.*").Length == 0)
            {
                var rootNsDir = FileSystem.Combine(projectDir, Path.GetFileName(ProjectFile));
                if (FileSystem.DirectoryExists(rootNsDir))
                {
                    outDir = Path.Combine(rootNsDir, "ServerTypes");
                    generator.ModulesPathFolder = Path.GetFileName(ProjectFile);
                }
                else
                {
                    rootNsDir = FileSystem.Combine(projectDir, config.RootNamespace);
                    if (FileSystem.DirectoryExists(rootNsDir))
                    {
                        outDir = Path.Combine(rootNsDir, "ServerTypes");
                        generator.ModulesPathFolder = config.RootNamespace;
                    }
                }
            }
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Transforming " + (modules ? "ServerTypes" : "ServerTypings") + " at: ");
        Console.ResetColor();
        Console.WriteLine(outDir);

        generator.RootNamespaces.Add(config.RootNamespace);

        foreach (var type in tsTypes)
            generator.AddTSType(type);

        var generatedSources = generator.Run();

        MultipleOutputHelper.WriteFiles(FileSystem, 
            FileSystem.Combine(projectDir, PathHelper.ToPath(outDir)),
            generatedSources.Where(x => x.Module == modules)
                .Select(x => (x.Filename, x.Text)),
            deleteExtraPattern: ["*.ts"],
            endOfLine: config.EndOfLine);
    }

    public static string[] DetermineAssemblyFiles(IProjectFileInfo projectFileInfo, 
        GeneratorConfig config, Action<string> onError)
    {
        ArgumentNullException.ThrowIfNull(projectFileInfo);
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(onError);

        var fileSystem = projectFileInfo.FileSystem;
        string projectFile = projectFileInfo.ProjectFile;

        if (config.ServerTypings == null ||
            config.ServerTypings.Assemblies.IsEmptyOrNull())
        {
            string outputDir = projectFileInfo.GetOutDir();
            string assemblyName = projectFileInfo.GetAssemblyName() ?? 
                fileSystem.ChangeExtension(fileSystem.GetFileName(projectFile), null);
            string targetFramework = projectFileInfo.GetTargetFramework();

            void couldNotFindError(string expectedPath)
            {
                onError(string.Format(CultureInfo.CurrentCulture,
                    "Couldn't find output file at {0}!" + Environment.NewLine +
                    "Make sure project is built successfully before running Sergen", expectedPath));
            }

            bool testCandidate(string path, out string outputPath)
            {
                outputPath = path + ".dll";
                if (fileSystem.FileExists(outputPath))
                    return true;

                if (fileSystem.FileExists(path + ".exe"))
                {
                    outputPath = path + ".exe";
                    return true;
                }

                return false;
            }

            if (!string.IsNullOrEmpty(outputDir) &&
                !string.IsNullOrEmpty(assemblyName))
            {
                if (!testCandidate(fileSystem.Combine(outputDir, assemblyName), out string outputPath))
                {
                    couldNotFindError(outputPath);
                    return null;
                }

                return [outputPath];
            }

            if (string.IsNullOrEmpty(targetFramework))
            {
                onError("Couldn't read TargetFramework from project file!");
                return null;
            }

            var debugExists = testCandidate(fileSystem.Combine(fileSystem.GetDirectoryName(projectFileInfo.ProjectFile),
                PathHelper.ToPath("bin/Debug/" + targetFramework + "/" + assemblyName)), out var debugPath);
            var releaseExists = testCandidate(fileSystem.Combine(fileSystem.GetDirectoryName(projectFileInfo.ProjectFile),
                PathHelper.ToPath("bin/Release/" + targetFramework + "/" + assemblyName)), out var releasePath);

            if (releaseExists && 
                (!debugExists || fileSystem.GetLastWriteTimeUtc(debugPath) < fileSystem.GetLastWriteTimeUtc(releasePath)))
                return [releasePath];

            if (debugExists)
                return [debugPath];

            couldNotFindError(debugPath);
            return null;
        }

        if (config.ServerTypings == null)
        {
            onError("ServerTypings is not configured in sergen.json file!");
            return null;
        }

        if (config.ServerTypings.Assemblies.IsEmptyOrNull())
        {
            Console.Error.WriteLine("ServerTypings has no assemblies configured in sergen.json file!");
            return null;
        }

        var assemblyFiles = config.ServerTypings.Assemblies;
        for (var i = 0; i < assemblyFiles.Length; i++)
        {
            var assemblyFile1 = PathHelper.ToUrl(fileSystem.GetFullPath(PathHelper.ToPath(assemblyFiles[i])));
            var binDebugIdx = assemblyFile1.IndexOf("/bin/Debug/", StringComparison.OrdinalIgnoreCase);
            string assemblyFile2 = assemblyFile1;
            if (binDebugIdx >= 0)
                assemblyFile2 = string.Concat(assemblyFile1[0..binDebugIdx], "/bin/Release/", 
                    assemblyFile1[(binDebugIdx + "/bin/Release".Length)..]);

            assemblyFiles[i] = assemblyFile1;

            if (fileSystem.FileExists(assemblyFile1))
            {
                if (fileSystem.FileExists(assemblyFile2) &&
                    fileSystem.GetLastWriteTimeUtc(assemblyFile1) < fileSystem.GetLastWriteTimeUtc(assemblyFile2))
                    assemblyFiles[i] = assemblyFile2;
            }
            else if (fileSystem.FileExists(assemblyFile2))
                assemblyFiles[i] = assemblyFile2;
            else
            {
                onError(string.Format(CultureInfo.CurrentCulture, string.Format(CultureInfo.CurrentCulture,
                    "Assembly file '{0}' specified in sergen.json is not found! " +
                    "This might happen when project is not successfully built or file name doesn't match the output DLL." +
                    "Please check paths in sergen.json.", assemblyFile1)));
                return null;
            }
        }

        return assemblyFiles;

    }
}