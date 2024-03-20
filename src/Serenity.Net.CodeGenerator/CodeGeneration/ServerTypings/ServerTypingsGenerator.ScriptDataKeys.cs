namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void PreVisitTypeForScriptData(TypeDefinition type)
    {
        var attrs = type.GetAttributes();
        if (TypingsUtils.FindAttr(attrs, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
            return;

        var dataScriptAttr = TypingsUtils.FindAttr(attrs, "Serenity.ComponentModel", "DataScriptAttribute");
        if (dataScriptAttr != null)
        {
            var key = dataScriptAttr.ConstructorArguments().Count > 0 ?
                dataScriptAttr.ConstructorArguments()[0].Value as string : null;
            if (key == null)
                AutoDataScriptKeyFor(type);
            key = "RemoteData." + key;
            if (!scriptDataKeys.ContainsKey(key))
                scriptDataKeys[key] = type;
        }
    }

    private static string AutoDataScriptKeyFor(TypeDefinition type)
    {
        string module;
        var moduleAttr = TypingsUtils.GetAttr(type,
            "Serenity.ComponentModel", "ModuleAttribute");
        if (moduleAttr != null)
        {
            if (moduleAttr.ConstructorArguments().Count == 1 &&
                moduleAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.String")
                module = moduleAttr.ConstructorArguments[0].Value as string;
            else
                module = null;
        }
        else
        {
            module = type.NamespaceOf() ?? "";
            if (module.EndsWith(".Scripts", StringComparison.Ordinal))
                module = module[0..^8];
            else if (module.EndsWith(".Lookups", StringComparison.Ordinal))
                module = module[0..^8];

            var idx = module.IndexOf('.');
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        return string.IsNullOrEmpty(module) ? name :
            module + "." + name;
    }

    protected void GenerateScriptDataKeys(bool module)
    {
        GenerateScriptDataKeysFor("RemoteDataKeys", "RemoteData.", module);
    }

    protected void GenerateScriptDataKeysFor(string name, string startsWith, bool module)
    {
        if (!module)
            return;

        var list = scriptDataKeys.Keys.Where(x => !string.IsNullOrWhiteSpace(x) &&
            x.StartsWith(startsWith, StringComparison.Ordinal))
            .Select(x => x[(startsWith.Length)..])
            .Where(x => x.Split('.').All(p => SqlSyntax.IsValidIdentifier(p)))
                .ToList();

        if (list.Count == 0)
            return;

        list.Sort((i1, i2) => string.CompareOrdinal(i1, i2));

        cw.Indented($"export namespace {name}");
        cw.InBrace(delegate
        {
            List<string> stack = [];
            int stackCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var key = list[i];
                var parts = key.Split('.');

                int same = 0;

                if (stackCount > 0)
                {
                    while (same < stackCount && same < parts.Length && stack[same] == parts[same])
                        same++;

                    for (int level = same; level < stackCount; level++)
                    {
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
                    sb.AppendLine();
                    cw.Indented("export namespace ");
                    sb.Append(part);
                    cw.StartBrace();
                }
                stackCount = parts.Length - 1;

                if (same != parts.Length)
                {
                    string part = parts[^1];

                    bool nextStartsWithThis = false;
                    if (i + 1 < list.Count)
                    {
                        var next = list[i + 1];
                        if (next.StartsWith(key, StringComparison.Ordinal) &&
                            next.Length > key.Length &&
                            next[key.Length] == '.')
                            nextStartsWithThis = true;
                    }

                    if (nextStartsWithThis)
                    {
                        stackCount = parts.Length;
                        if (stack.Count > stackCount - 1)
                            stack[stackCount - 1] = part;
                        else
                            stack.Add(part);
                        cw.Indented("export namespace ");
                        sb.Append(part);
                        cw.StartBrace();
                        cw.Indented("__ = ");
                    }
                    else
                    {
                        cw.Indented("export const ");
                        sb.Append(part);
                        sb.Append(" = ");
                    }

                    sb.Append(key.ToDoubleQuoted());
                    sb.AppendLine(";");
                }
            }
            for (int i = 0; i < stackCount; i++)
            {
                cw.EndBrace();
                sb.AppendLine();
            }
        });

        AddFile(name + ".ts", module);
    }
}