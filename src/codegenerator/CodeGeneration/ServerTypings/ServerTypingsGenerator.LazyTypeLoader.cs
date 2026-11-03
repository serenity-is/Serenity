using System.Text.RegularExpressions;

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Generates the module that loads the types implementing <c>LazyLoadInclude</c>. As an .mts file it is
    /// an entry point with a stable URL, while the literal dynamic imports make esbuild split each
    /// type into its own chunk, or each group for the types implementing <c>LazyLoadInclude&lt;"Group"&gt;</c>.
    /// It is only generated for a project that has such types.
    /// </summary>
    protected void GenerateLazyTypeLoader()
    {
        var lazyTypes = GetLazyLoadTypes();
        if (lazyTypes.Count == 0)
            return;

        // file names of groups are also compared case insensitively, as they would overwrite each other on Windows
        var groupNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? getGroup(ExternalType type)
        {
            if (string.IsNullOrEmpty(type.LazyLoadGroup))
                return null;

            // the name is used in both a file name and an identifier
            var name = Regex.Replace(type.LazyLoadGroup, "[^A-Za-z0-9_]", "_");
            if (groupNames.TryGetValue(name, out var existing))
                return existing;

            groupNames[name] = name;
            return name;
        }

        foreach (var group in lazyTypes.Where(x => getGroup(x.Type) != null).GroupBy(x => getGroup(x.Type)!))
            GenerateLazyGroup(group.Key, [.. group]);

        sb.Append("export const loaderByKey: Record<string, () => Promise<any>> =");
        cw.InBrace(() =>
        {
            for (var i = 0; i < lazyTypes.Count; i++)
            {
                var (key, type) = lazyTypes[i];
                var quotedKey = Newtonsoft.Json.JsonConvert.SerializeObject(key);
                var group = getGroup(type);
                if (group != null)
                {
                    var groupModule = Newtonsoft.Json.JsonConvert.SerializeObject("./" + GetLazyGroupModule(group));
                    cw.Indented($"{quotedKey}: async () => (await import({groupModule})).{GetLazyGroupConstant(group)}[{quotedKey}]");
                }
                else
                {
                    var module = Newtonsoft.Json.JsonConvert.SerializeObject(RelativeModulePath("LazyTypeLoader.mts", type.Module));
                    cw.Indented($"{quotedKey}: async () => (await import({module})).{type.Name}");
                }
                cw.AppendLine(i < lazyTypes.Count - 1 ? "," : "");
            }
        });

        AddFile("LazyTypeLoader.mts");
    }

    private static string GetLazyGroupModule(string group)
    {
        return "LazyTypeGroup." + group;
    }

    private static string GetLazyGroupConstant(string group)
    {
        return "lazyTypeGroup" + group;
    }

    /// <summary>
    /// Generates the module holding the types of a group, which the lazy type loader imports as a single chunk.
    /// It is not an entry point, so its name is hashed like other chunks. The types are not re-exported, as
    /// the type lister would then list them under this module too.
    /// </summary>
    private void GenerateLazyGroup(string group, List<(string Key, ExternalType Type)> types)
    {
        var filename = GetLazyGroupModule(group) + ".ts";

        // classes with the same name might exist under different folders
        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        var aliases = new List<(string Key, string Alias)>();
        foreach (var (key, type) in types)
        {
            var name = type.Name!;
            var alias = name;
            for (var i = 2; !usedNames.Add(alias); i++)
                alias = name + "_" + i;

            var module = Newtonsoft.Json.JsonConvert.SerializeObject(RelativeModulePath(filename, type.Module));
            sb.AppendLine($"import {{ {(alias == name ? name : name + " as " + alias)} }} from {module};");
            aliases.Add((key, alias));
        }

        sb.AppendLine();
        sb.Append($"export const {GetLazyGroupConstant(group)}: Record<string, any> =");
        cw.InBrace(() =>
        {
            for (var i = 0; i < aliases.Count; i++)
            {
                cw.Indented($"{Newtonsoft.Json.JsonConvert.SerializeObject(aliases[i].Key)}: {aliases[i].Alias}");
                cw.AppendLine(i < aliases.Count - 1 ? "," : "");
            }
        });

        AddFile(filename);
    }
}
