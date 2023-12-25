namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void PreVisitTypeForTexts(TypeDefinition fromType)
    {
        var nestedLocalTexts = TypingsUtils.GetAttr(fromType, "Serenity.Extensibility",
            "NestedLocalTextsAttribute", emptyTypes) ??
            TypingsUtils.GetAttr(fromType, "Serenity.ComponentModel",
                "NestedLocalTextsAttribute", emptyTypes);
        if (nestedLocalTexts == null)
            return;
        string prefix = null;
#if ISSOURCEGENERATOR
        prefix = nestedLocalTexts.NamedArguments.FirstOrDefault(x => x.Key == "Prefix").Value.Value as string;
#else
        if (nestedLocalTexts.HasProperties)
            prefix = nestedLocalTexts.Properties.FirstOrDefault(x => x.Name == "Prefix").Argument.Value as string;
#endif

        AddNestedLocalTexts(fromType, prefix ?? "");
    }

    protected void AddNestedLocalTexts(TypeDefinition type, string prefix)
    {
        if (TypingsUtils.FindAttr(type.GetAttributes(), "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
            return;

        foreach (var fi in type.FieldsOf().Where(x =>
            x.IsPublic() && x.IsStatic &&
            x.DeclaringType().FullNameOf() == type.FullNameOf() &&
            x.FieldType().FullNameOf() == "Serenity.LocalText" &&
            TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "ScriptSkipAttribute") == null))
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
    }

    protected void GenerateTexts(bool module)
    {
        cw.Indented("namespace ");
        var ns = RootNamespaces.FirstOrDefault(x => x != "Serenity") ?? "App";
        sb.Append(ns + ".Texts");
        cw.InBrace(delegate
        {
            Regex filter = null;
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
                     (filter == null || filter.IsMatch(x))) &&
                    x.Split('.').All(p => SqlSyntax.IsValidIdentifier(p)))
                .ToList();

            list.Sort((i1, i2) => string.CompareOrdinal(i1, i2));

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
                        jw.WriteEndObject();
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
                    sb.AppendLine();
                    if (level == 0)
                        cw.Indented("export declare namespace ");
                    else
                        cw.Indented("namespace ");
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
                        jw.WritePropertyName(part);
                        jw.WriteStartObject();
                        cw.Indented("namespace ");
                        sb.Append(part);
                        cw.StartBrace();
                        jw.WritePropertyName("export const __");
                        jw.WriteValue(1);
                        cw.IndentedLine("__: string;");
                    }
                    else
                    {
                        cw.Indented("export const ");
                        sb.Append(part);
                        sb.AppendLine(": string;");
                    }
                }
            }
            for (int i = 0; i < stackCount; i++)
            {
                jw.WriteEndObject();
                cw.EndBrace();
                sb.AppendLine();
            }
            jw.WriteEndObject();

            cw.Indented(ns);
            if (module)
            {
                var proxyTexts = ImportFromQ("proxyTexts");
                sb.Append($"['Texts'] = {proxyTexts}(Texts, '', ");
            }
            else
                sb.Append(@"['Texts'] = Q.proxyTexts(Texts, '', ");
            jw.Flush();
            sb.Append(string.Join("\n    ", jwBuilder.ToString().Split('\n')));
            sb.AppendLine(") as any;");
        });

        if (module)
        {
            sb.AppendLine();
            sb.AppendLine($"export const Texts = {ns}.Texts;");
        }

        AddFile("Texts.ts", module);
    }
}