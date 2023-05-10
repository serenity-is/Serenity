using System.IO;

namespace Serenity.Web;

/// <summary>
/// Registrar for HTML templates (obsolete, please avoid such templates and specify
/// templates directly in the Widget's getTemplate() method
/// </summary>
public class TemplateScriptRegistrar
{
    private static readonly string[] TemplateSuffixes = new[] { ".Template.html", ".ts.html" };

    private ConcatenatedScript bundle;
    private readonly Dictionary<string, TemplateScript> scriptByKey = new(StringComparer.OrdinalIgnoreCase);

    private static string GetKey(string rootPath, string filename)
    {
        string key = Path.GetFileName(filename);
        bool isModulesFolder = rootPath.EndsWith(Path.DirectorySeparatorChar + "Modules" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

        foreach (var suffix in TemplateSuffixes)
            if (key.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                key = key.Substring(0, key.Length - suffix.Length);

                if (isModulesFolder && filename.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
                {
                    filename = filename[rootPath.Length..];

                    var moduleEnd = filename.IndexOf(Path.DirectorySeparatorChar, StringComparison.Ordinal);
                    if (moduleEnd >= 0)
                    {
                        var module = filename.Substring(0, moduleEnd);
                        if (!key.StartsWith(module + ".", StringComparison.OrdinalIgnoreCase))
                            return module + "." + key;
                    }
                }

                return key;
            }

        return null;
    }

    private void Changed(IDynamicScriptManager manager, string rootPath, string name)
    {
        string key = GetKey(rootPath, rootPath + name);
        if (key == null)
            return;

        if (scriptByKey.TryGetValue(key, out TemplateScript ts))
            manager.Changed(ts.ScriptName);

        manager.Changed("TemplateBundle");
    }

    /// <summary>
    /// Initializes the watcher
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="paths"></param>
    /// <param name="watchForChanges"></param>
    /// <returns></returns>
    public IEnumerable<FileWatcher> Initialize(IDynamicScriptManager manager, string[] paths, bool watchForChanges = true)
    {
        var watchers = new List<FileWatcher>();
        var bundleList = new List<Func<string>>();

        foreach (var p in paths)
        {
            var path = p;
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                path += Path.DirectorySeparatorChar;

            if (!Directory.Exists(path))
                continue;

            foreach (var file in Directory.EnumerateFiles(path, "*.html", SearchOption.AllDirectories))
            {
                var key = GetKey(path, file);
                if (key == null)
                    continue;

                var script = new TemplateScript(key, () => File.ReadAllText(file));
                manager.Register(script);
                scriptByKey[key.ToLowerInvariant()] = script;
                bundleList.Add(script.GetScript);
            }

            if (watchForChanges)
            {
                var watcher = new FileWatcher(path, "*.html");
                watcher.Changed += name => Changed(manager, path, name);
                watchers.Add(watcher);
            }
        }

        bundle = new ConcatenatedScript(bundleList);
        manager.Register("TemplateBundle", bundle);
        return watchers;
    }
}