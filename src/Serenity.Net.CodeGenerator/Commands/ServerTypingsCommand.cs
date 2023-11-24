using Newtonsoft.Json.Linq;
using Serenity.CodeGeneration;
using System.IO;

namespace Serenity.CodeGenerator;

public class ServerTypingsCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public bool Modules { get; set; }
    public List<ExternalType> TsTypes { get; set; }

    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        if (Modules && config.ServerTypings?.ModuleTypings == false)
            return ExitCodes.Success;

        if (!Modules && config.ServerTypings?.NamespaceTypings == false)
            return ExitCodes.Success;

        var assemblyFiles = Project.GetAssemblyList(config.ServerTypings?.Assemblies);

        if (assemblyFiles == null || assemblyFiles.Length == 0)
            return ExitCodes.CantDetermineOutputAssemblies;

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
                (Modules ? "Modules/ServerTypes" : "Imports/ServerTypings"))));

        generator.ModuleTypings = Modules && config?.ServerTypings?.ModuleTypings != false;
        generator.ModuleReExports = generator.ModuleTypings && config?.ServerTypings?.ModuleReExports != false;
        generator.NamespaceTypings = !Modules && config?.ServerTypings?.NamespaceTypings != false;

        if (Modules)
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

        Console.Write("Transforming " + (Modules ? "ServerTypes" : "ServerTypings") + " at: ", ConsoleColor.Cyan);
        Console.WriteLine(outDir);

        generator.RootNamespaces.Add(config.RootNamespace);

        foreach (var type in TsTypes)
            generator.AddTSType(type);

        var generatedSources = generator.Run();

        MultipleOutputHelper.WriteFiles(FileSystem, Console,
            FileSystem.Combine(projectDir, PathHelper.ToPath(outDir)),
            generatedSources.Where(x => x.Module == Modules)
                .Select(x => (x.Filename, x.Text)),
            deleteExtraPattern: ["*.ts"],
            endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }
}