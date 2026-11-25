namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Directory under ServerTypes for texts files of nested local text classes that
    /// does not map to a module directory. No module re-export file is generated for it.
    /// </summary>
    protected const string TextsDirectory = "Texts";

    public bool LocalTexts { get; set; }

    /// <summary>
    /// Determines what the root Texts.ts contains. Nested local texts classes and
    /// module Db texts are generated in their own files regardless of this setting.
    /// </summary>
    public ServerTypingsTextsMode TextsMode { get; set; }

    protected HashSet<string> localTextKeys = [];
    protected Dictionary<string, string> localTextNestedClasses = [];
    protected Dictionary<string, NestedLocalTextsClass> nestedLocalTextsClasses = [];

    protected void PreVisitTypeForTexts(TypeDefinition fromType)
    {
        var nestedLocalTexts = TypingsUtils.GetAttr(fromType, "Serenity.Extensibility",
            "NestedLocalTextsAttribute", emptyTypes) ??
            TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel",
                "NestedLocalTextsAttribute", emptyTypes);
        if (nestedLocalTexts == null)
            return;
        string? prefix = null;
#if ISSOURCEGENERATOR
        prefix = nestedLocalTexts.NamedArguments.FirstOrDefault(x => x.Key == "Prefix").Value.Value as string;
#else
        if (nestedLocalTexts.HasProperties)
            prefix = nestedLocalTexts.Properties.FirstOrDefault(x => x.Name == "Prefix").Argument.Value as string;
#endif
        prefix ??= "";

        AddNestedLocalTexts(fromType, prefix);
        if (fromType.Name.EndsWith("Texts", StringComparison.Ordinal) &&
            !localTextNestedClasses.ContainsKey(fromType.Name))
        {
            localTextNestedClasses.Add(fromType.Name, prefix);
            nestedLocalTextsClasses.Add(fromType.Name,
                new NestedLocalTextsClass(fromType.Name, prefix, ScriptNamespaceFor(fromType)));
        }
    }

    protected void AddNestedLocalTexts(TypeDefinition type, string prefix)
    {
        if (TypingsUtils.FindAttr(type.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute") != null)
            return;

        foreach (var fi in type.FieldsOf().Where(x =>
            x.IsPublic() && x.IsStatic &&
            x.DeclaringType().FullNameOf() == type.FullNameOf() &&
            x.FieldType().FullNameOf() == "Serenity.LocalText" &&
            TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute") == null))
        {
            localTextKeys.Add(prefix + fi.Name);
        }

#if ISSOURCEGENERATOR
        foreach (var nested in type.GetTypeMembers())
#else
        if (!type.HasNestedTypes)
            return;

        foreach (var nested in type.NestedTypes)
#endif
        {
            var name = nested.Name;
            if (name.EndsWith('_'))
                name = name[0..^1];

            AddNestedLocalTexts(nested, prefix + name + ".");
        }
    }

    protected void AddRowTexts(TypeDefinition type, string prefix)
    {
        foreach (var prop in EnumerateFieldProperties(type))
            localTextKeys.Add(prefix + prop.Name);
        localTextKeys.Add(prefix + "EntityPlural");
        localTextKeys.Add(prefix + "EntitySingular");
    }


    public void SetLocalTextFiltersFrom(IFileSystem fileSystem, string appSettingsFile)
    {
        ArgumentNullException.ThrowIfNull(fileSystem, nameof(fileSystem));
        ArgumentNullException.ThrowIfNull(appSettingsFile, nameof(appSettingsFile));

        if (!LocalTexts || !fileSystem.FileExists(appSettingsFile))
            return;

        try
        {
            var obj = Newtonsoft.Json.Linq.JObject.Parse(fileSystem.ReadAllText(appSettingsFile));
            if ((obj["LocalTextPackages"] ?? ((obj["AppSettings"]
                as Newtonsoft.Json.Linq.JObject)?["LocalTextPackages"])) is
                Newtonsoft.Json.Linq.JObject packages)
            {
                foreach (var p in packages.PropertyValues())
                    foreach (var x in p.Values<string>())
                        LocalTextFilters.Add(x!);
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// Returns the local text keys that should be available in generated texts,
    /// e.g. the ones matching default site package includes or local text filters,
    /// sorted by ordinal comparison.
    /// </summary>
    protected List<string> GetFilteredLocalTextKeys()
    {
        Regex? filter = null;
        if (LocalTextFilters.Count > 0)
        {
            var fb = new StringBuilder("^(");
            bool append = false;
            foreach (string item in LocalTextFilters)
            {
                if (append)
                    fb.Append('|');
                if (!string.IsNullOrEmpty(item))
                {
                    if (item[0] == '^' && item[^1] == '$')
                        fb.Append(item[1..^1]);
                    else fb.Append(item.Replace(".", "\\."
#if ISSOURCEGENERATOR
                        ) + ".*");
#else
                            , StringComparison.Ordinal) + ".*");
#endif
                    append = true;
                }
            }
            fb.Append(")$");
            filter = new Regex(fb.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        var list = localTextKeys.Where(x =>
                !string.IsNullOrWhiteSpace(x) &&
                (Web.LocalTextPackages.DefaultSitePackageIncludes.IsMatch(x) ||
                 (filter != null && filter.IsMatch(x))) &&
                x.Split('.').All(p => SqlSyntax.IsValidIdentifier(p)))
            .ToList();

        list.Sort(string.CompareOrdinal);
        return list;
    }

    /// <summary>
    /// Returns the top level directories of generated files, e.g. module directories
    /// that will have a module re-export file like Administration.ts.
    /// </summary>
    protected Dictionary<string, string> GetModuleDirectories()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in generatedCode.Where(x => x.IsTypeScript))
        {
            var filename = file.Filename.Replace('\\', '/');
            var idx = filename.IndexOf('/');
            if (idx <= 0)
                continue;

            var directory = filename[..idx];
            if (directory != TextsDirectory &&
                !result.ContainsKey(directory))
                result.Add(directory, directory);
        }

        return result;
    }

    /// <summary>
    /// Determines the texts bundles, e.g. a file per nested local texts class, and
    /// a DbTexts file per module for the Db texts not covered by any of these classes.
    /// </summary>
    protected List<TextsBundle> GetTextsBundles(List<string> keys)
    {
        var bundles = new List<TextsBundle>();

        foreach (var nestedClass in nestedLocalTextsClasses.Values
            .Where(x => x.Name != "Texts")
            .OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            List<string> bundleKeys;
            var prefix = nestedClass.Prefix;
            // unprefixed classes are aliases for the whole tree, as they are commonly used
            // to access texts declared elsewhere, so they don't get any splitting benefit
            if (prefix.Length == 0)
                bundleKeys = keys;
            else if (prefix.EndsWith('.'))
            {
                bundleKeys = [.. keys.Where(x => x.StartsWith(prefix, StringComparison.Ordinal))];
                if (bundleKeys.Count == 0)
                    continue;
            }
            else
                continue;

            var filePath = GetTypingFileNameFor(nestedClass.Namespace, nestedClass.Name);
            var slash = filePath.IndexOf('/');
            if (slash <= 0 || filePath[..slash] == TextsDirectory)
                filePath = TextsDirectory + "/" + nestedClass.Name;

            bundles.Add(new TextsBundle(nestedClass.Name, prefix, filePath, bundleKeys, isNestedClass: true));
        }

        var moduleDirectories = GetModuleDirectories();
        var generatedFiles = new HashSet<string>(generatedCode.Select(x => x.Filename.Replace('\\', '/')),
            StringComparer.OrdinalIgnoreCase);

        var dbKeysByModule = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var key in keys)
        {
            if (!key.StartsWith("Db.", StringComparison.Ordinal) ||
                bundles.Any(x => x.Covers(key)))
                continue;

            var moduleEnd = key.IndexOf('.', 3);
            if (moduleEnd < 0)
                continue;

            var module = key[3..moduleEnd];
            if (!dbKeysByModule.TryGetValue(module, out var moduleKeys))
                dbKeysByModule[module] = moduleKeys = [];
            moduleKeys.Add(key);
        }

        foreach (var pair in dbKeysByModule.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (!moduleDirectories.TryGetValue(pair.Key, out var directory))
                continue;

            var name = directory + "DbTexts";
            var filePath = directory + "/DbTexts";
            if (nestedLocalTextsClasses.ContainsKey(name) ||
                bundles.Any(x => x.Name == name ||
                    string.Equals(x.FilePath, filePath, StringComparison.OrdinalIgnoreCase)) ||
                generatedFiles.Contains(filePath + ".ts"))
                continue;

            bundles.Add(new TextsBundle(name, "Db." + pair.Key + ".", filePath, pair.Value, isNestedClass: false));
        }

        return bundles;
    }

    /// <summary>
    /// Writes the texts tree for given sorted keys. When declarations is true, writes
    /// the type declarations to the code writer. Returns the proxyTexts template JSON
    /// for the tree, which only contains the branches, as the leaves are strings.
    /// </summary>
    protected string WriteLocalTextsTree(List<string> keys, bool declarations)
    {
        var jwBuilder = new StringBuilder();
        var jw = new Newtonsoft.Json.JsonTextWriter(new System.IO.StringWriter(jwBuilder))
        {
            QuoteName = false,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            Indentation = 4
        };

        jw.WriteStartObject();
        List<string> stack = [];
        int stackCount = 0;
        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            var parts = key.Split('.');

            int same = 0;

            if (stackCount > 0)
            {
                while (same < stackCount && same < parts.Length && stack[same] == parts[same])
                    same++;

                for (int level = same; level < stackCount; level++)
                {
                    jw.WriteEndObject();
                    if (declarations)
                        cw.EndBrace();
                }

                stackCount = same;
            }

            for (int level = same; level < parts.Length - 1; level++)
            {
                string part = parts[level];
                if (stack.Count > level)
                    stack[level] = part;
                else
                    stack.Add(part);
                jw.WritePropertyName(part);
                jw.WriteStartObject();
                if (declarations)
                {
                    if (level == 0)
                        cw.Indented("export declare namespace ");
                    else
                        cw.Indented("namespace ");
                    sb.Append(part);
                    cw.StartBrace();

                    cw.IndentedLine($"export function asKey(): typeof {part};");
                    cw.IndentedLine($"export function asTry(): typeof {part};");
                }
            }
            stackCount = parts.Length - 1;

            if (declarations && same != parts.Length)
            {
                string part = parts[^1];

                bool nextStartsWithThis = false;
                if (i + 1 < keys.Count)
                {
                    var next = keys[i + 1];
                    if (next.StartsWith(key, StringComparison.Ordinal) &&
                        next.Length > key.Length &&
                        next[key.Length] == '.')
                        nextStartsWithThis = true;
                }

                if (!nextStartsWithThis && part != "asKey" && part != "asTry")
                {
                    cw.Indented($"{(CodeWriter.IsJSKeyword(part) ? "// js keyword!: " : "")}export const ");
                    sb.Append(part);
                    sb.AppendLine(": string;");
                }
            }
        }
        for (int i = 0; i < stackCount; i++)
        {
            jw.WriteEndObject();
            if (declarations)
                cw.EndBrace();
        }
        jw.WriteEndObject();
        jw.Flush();

        return string.Join("\n", jwBuilder.ToString().Split('\n'));
    }

    /// <summary>
    /// Writes a texts export, e.g. the type declarations and the proxyTexts call for given keys.
    /// </summary>
    protected void WriteTextsExport(string name, string prefix, List<string> keys, bool export)
    {
        var exportPrefix = export ? "export " : "";
        if (keys.Count == 0)
        {
            sb.AppendLine($"{exportPrefix}const {name} = {{}} as const;");
            return;
        }

        cw.Indented("namespace texts");
        cw.InBrace(delegate
        {
            WriteLocalTextsTree(keys, declarations: true);
        });

        sb.AppendLine();
        var proxyTexts = ImportFromCorelib("proxyTexts");
        var template = WriteLocalTextsTree(prefix.Length == 0 ? keys :
            [.. keys.Select(x => x[prefix.Length..])], declarations: false);
        var textsType = prefix.Length == 0 ? "texts" : "texts." + prefix[..^1];
        sb.Append($"{exportPrefix}const {name}: typeof {textsType} = /*#__PURE__*/ {proxyTexts}({{}}, '{prefix}', ");
        sb.Append(template);
        sb.AppendLine(") as any;");
    }

    protected void GenerateTexts()
    {
        var keys = GetFilteredLocalTextKeys();
        var bundles = GetTextsBundles(keys);

        foreach (var bundle in bundles)
        {
            WriteTextsExport(bundle.Name, bundle.Prefix, bundle.Keys, export: true);
            AddFile(bundle.FilePath + ".ts");
        }

        bool exportTexts = localTextNestedClasses.ContainsKey("Texts");
        if (TextsMode == ServerTypingsTextsMode.Remaining)
        {
            if (!exportTexts)
                return;

            WriteTextsExport("Texts", "", [.. keys.Where(x => !bundles.Any(b => b.Covers(x)))], export: true);
            AddFile("Texts.ts");
            return;
        }

        if (exportTexts)
        {
            WriteTextsExport("Texts", "", keys, export: true);
            sb.AppendLine();
        }

        foreach (var bundle in bundles
            .Where(x => x.IsNestedClass)
            .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase))
        {
            sb.AppendLine($"export {{ {bundle.Name} }} from \"./{bundle.FilePath}\";");
        }

        if (sb.Length == 0)
            sb.AppendLine("const Texts = {} as const;");

        AddFile("Texts.ts");
    }
}
