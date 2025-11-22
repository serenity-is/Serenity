#if ISSOURCEGENERATOR
using PathHelper = Serenity.CodeGenerator.PathHelper;
#endif

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private readonly List<ModuleImport> moduleImports = [];
    private readonly HashSet<string> moduleImportAliases = [];

    public string ModulesPathAlias { get; set; } = "@/";
    public string ModulesPathFolder { get; set; } = "Modules";
    public string RootPathAlias { get; set; } = "@/../";

    protected void ClearImports()
    {
        moduleImports.Clear();
    }

    protected void HandleModuleImportsOnAddFile(string filename)
    {
        if (moduleImports.Count == 0)
            return;
        var dotTsIndex = filename.IndexOf(".ts", StringComparison.Ordinal);
        var currentFrom = filename;

        if (dotTsIndex >= 0)
            currentFrom = filename[..dotTsIndex];

        var moduleImportsLookup = moduleImports
            .Where(x => x.From != currentFrom)
            .ToLookup(x => (x.From, x.External));

        if (moduleImportsLookup.Count != 0)
        {
            sb.Insert(0, string.Join(Environment.NewLine, moduleImportsLookup
                .Select(z =>
                {
                    var from = z.Key.From;
                    if (!z.Key.External)
                        from = RelativeModulePath(filename, from);

                    var importList = string.Join(", ", z.OrderBy(p => p.Name ?? "", StringComparer.InvariantCultureIgnoreCase).Select(p =>
                        p.Name + (p.Alias != p.Name ? (" as " + p.Alias) : "")));

                    return (from, importList);
                }).OrderBy(x => FromOrderKey(x.from), StringComparer.OrdinalIgnoreCase).Select(x => $"import {{ {x.importList} }} from \"{x.from}\";")) +
                Environment.NewLine + Environment.NewLine);
        }

        moduleImports.Clear();
        moduleImportAliases.Clear();
    }

    private static string FromOrderKey(string from)
    {
        if (from == null)
            return null;

        /// local imports ordered last
        return (from.StartsWith('.') ||
            from.StartsWith('/')) ?
            char.MaxValue + from : from;
    }


    protected string ImportFromQ(string name)
    {
        return AddExternalImport("@serenity-is/corelib", name);
    }

    protected string ImportFromCorelib(string name)
    {
        return AddExternalImport("@serenity-is/corelib", name);
    }

    protected string AddExternalImport(string from, string name)
    {
        return AddModuleImport(from, name, external: true);
    }

    protected string AddModuleImport(string from, string name, bool external = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(name);
        ArgumentExceptionHelper.ThrowIfNull(from);

        var existing = moduleImports.FirstOrDefault(x => x.From == from && x.Name == name && x.External == external);
        if (existing != null)
            return existing.Alias;

        var i = 0; string alias;
        while (moduleImportAliases.Contains(alias = i == 0 ? name : (name + "_" + i)))
            i++;

        moduleImportAliases.Add(alias);
        moduleImports.Add(new ModuleImport
        {
            From = from,
            Name = name,
            Alias = alias,
            External = external
        });

        return alias;
    }

    public string DetermineModulesRoot(IFileSystem fileSystem, string projectFile,
        string rootNamespace)
    {
        ArgumentExceptionHelper.ThrowIfNull(fileSystem, nameof(fileSystem));
        ArgumentExceptionHelper.ThrowIfNull(projectFile, nameof(projectFile));

        var projectDir = fileSystem.GetDirectoryName(projectFile);
        var modulesDir = fileSystem.Combine(projectDir, "Modules");

        if (!fileSystem.DirectoryExists(modulesDir) ||
            fileSystem.GetFiles(modulesDir, "*.*").Length == 0)
        {
            var rootNsDir = fileSystem.Combine(projectDir,
                fileSystem.GetFileName(projectFile));
            if (fileSystem.DirectoryExists(rootNsDir))
            {
                modulesDir = rootNsDir;
                ModulesPathFolder = fileSystem.GetFileName(projectFile);
            }
            else
            {
                rootNsDir = fileSystem.Combine(projectDir, rootNamespace);
                if (fileSystem.DirectoryExists(rootNsDir))
                {
                    modulesDir = rootNsDir;
                    ModulesPathFolder = rootNamespace;
                }
            }
        }

        return modulesDir;
    }

    protected string GetTypingFileNameFor(string ns, string name)
    {
        var filename = RemoveRootNamespace(ns, name);
        var idx = filename.IndexOf('.');
        if (idx >= 0)
            filename = filename[..idx] + '/' + filename[(idx + 1)..];

        return filename;
    }

    protected string RelativeModulePath(string fromModule, string toModule)
    {
        if (string.IsNullOrEmpty(toModule) ||
            toModule.StartsWith('.'))
            return toModule;

        if (toModule.StartsWith("@/") &&
            !string.IsNullOrEmpty(ModulesPathFolder))
            toModule = "/" + ModulesPathFolder + "/" + toModule;

        if (!toModule.StartsWith('/'))
        {
            if (System.IO.Path.GetDirectoryName(fromModule) ==
                System.IO.Path.GetDirectoryName(toModule))
                return "./" + System.IO.Path.GetFileName(toModule);
            else
                return "../" + toModule;
        }

        if (!string.IsNullOrEmpty(ModulesPathFolder) &&
            !string.IsNullOrEmpty(ModulesPathAlias) &&
            toModule.StartsWith("/" + ModulesPathFolder + "/", StringComparison.Ordinal))
        {
            return ModulesPathAlias + toModule[(ModulesPathFolder.Length + 2)..];
        }

        if (!string.IsNullOrEmpty(RootPathAlias))
            return RootPathAlias + toModule[1..];

        var relativeTo = PathHelper.ToPath("/Modules/ServerTypes/") +
            PathHelper.ToPath(System.IO.Path.GetDirectoryName(fromModule));

#if ISSOURCEGENERATOR
        toModule = PathHelper.GetRelativePath(relativeTo, toModule);
#else
        toModule = System.IO.Path.GetRelativePath(relativeTo, toModule);
#endif
        return PathHelper.ToUrl(toModule);
    }

}
