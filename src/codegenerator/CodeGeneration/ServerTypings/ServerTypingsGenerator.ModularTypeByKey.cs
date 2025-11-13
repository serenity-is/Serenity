namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected ILookup<string, ExternalType> modularEditorTypeByKey;
    protected ILookup<string, ExternalType> modularFormatterTypeByKey;
    protected ILookup<string, ExternalType> modularDialogTypeByKey;

    protected void InitModularTypeByKey()
    {
        static string fixRegName(ExternalType type, string text)
        {
            if (text != null && text[^1] == '.' && !string.IsNullOrEmpty(type?.Name))
                return text + type.Name;

            return text;
        }

        static (string, string) extractTypeNameViaTypeInfo(ExternalType type)
        {
            var field = type.Fields?.FirstOrDefault(x =>
                x.IsStatic == true &&
                x.Value is string &&
                x.Name == "[Symbol.typeInfo]" &&
                x.Type?.EndsWith("TypeInfo", StringComparison.Ordinal) == true);
            return (fixRegName(type, field?.Value as string), field?.Type);
        }

        modularEditorTypeByKey = TSTypes.Values.Select(type =>
        {
            if (type.IsAbstract != true &&
                type.IsInterface != true &&
                type.IsEnum != true &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (extractTypeNameViaTypeInfo(type) is var (tiKey, tiType) &&
                    !string.IsNullOrEmpty(tiKey) &&
                    tiType == "EditorTypeInfo")
                    return (key: tiKey, type);

                if (type.SourceFile?.EndsWith(".d.ts") == true &&
                    (HasBaseType(type, ClientTypesGenerator.EditorBaseClasses) ||
                     (HasBaseType(type, "@serenity-is/corelib:Widget", "Widget") &&
                      type.Name?.EndsWith("Editor", StringComparison.Ordinal) == true)))
                {
                    return (key: type.Name, type);
                }

                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerEditor" ||
                             attr.Type.EndsWith(".registerEditor", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key: fixRegName(type, key), type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);

        modularFormatterTypeByKey = TSTypes.Values.Select(type =>
        {
            if (type.IsAbstract != true &&
                type.IsEnum != true &&
                type.IsInterface != true &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (extractTypeNameViaTypeInfo(type) is var (tiKey, tiType) &&
                    !string.IsNullOrEmpty(tiKey) &&
                    tiType == "FormatterTypeInfo")
                    return (key: tiKey, type);

                if (type.SourceFile?.EndsWith(".d.ts") == true &&
                    type.Interfaces != null &&
                    type.Interfaces.Any(x => x == "ISlickFormatter" ||
                        x?.EndsWith(".ISlickFormatter", StringComparison.Ordinal) == true))
                {
                    return (key: type.Name, type);
                }

                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerFormatter" ||
                             attr.Type.EndsWith(".registerFormatter", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key: fixRegName(type, key), type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);

        modularDialogTypeByKey = TSTypes.Values.Select(type =>
        {
            // add dialog base class / IDialog / dialogOpen check?
            if (type.IsAbstract != true &&
                type.IsEnum != true &&
                type.IsInterface != true &&
                !string.IsNullOrEmpty(type.Module))
            {
                if (extractTypeNameViaTypeInfo(type) is var (tiKey, tiType) &&
                    !string.IsNullOrEmpty(tiKey) &&
                    tiType == "ClassTypeInfo")
                    return (key: tiKey, type);

                if (type.Attributes != null)
                {
                    foreach (var attr in type.Attributes)
                    {
                        if (attr.Type is not null &&
                            (attr.Type == "registerClass" ||
                             attr.Type.EndsWith(".registerClass", StringComparison.Ordinal)) &&
                            attr.Arguments?.Count > 0 &&
                            attr.Arguments[0]?.Value is string key &&
                            !string.IsNullOrEmpty(key))
                            return (key: fixRegName(type, key), type);
                    }
                }
            }

            return (null, type);
        }).Where(x => x.key != null)
            .ToLookup(x => x.key, x => x.type);
    }


    protected ExternalType TryFindModuleType(string fullName, string containingAssembly)
    {
        var nonGeneric = fullName;
        var genericIdx = nonGeneric.IndexOf('`', StringComparison.Ordinal);
        if (genericIdx >= 0)
            nonGeneric = nonGeneric[..genericIdx];

        ExternalType scriptType;
        if (nonGeneric.Contains(':'))
        {
            scriptType = GetScriptType(nonGeneric);
            if (scriptType != null)
                return scriptType;
        }

        string ns = "";
        var dotIdx = nonGeneric.LastIndexOf('.');
        if (dotIdx >= 0)
        {
            ns = nonGeneric[0..dotIdx];
            nonGeneric = nonGeneric[(dotIdx + 1)..];
        }

        if (!string.IsNullOrEmpty(containingAssembly))
        {
            if (SerenityNetAssemblies.Contains(containingAssembly))
            {
                if ((scriptType = GetScriptType("@serenity-is/corelib:" + nonGeneric)) != null)
                    return scriptType;
            }
            else if (containingAssembly.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase))
            {
                if ((scriptType = GetScriptType("@serenity-is/" + containingAssembly[9..].ToLowerInvariant() + ":" + nonGeneric)) != null)
                    return scriptType;
            }
            else if ((scriptType = GetScriptType(containingAssembly.ToLowerInvariant() + ":" + nonGeneric)) != null)
                return scriptType;
        }

        if (ns == null)
            return null;

        if (ns.StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) &&
            (scriptType = GetScriptType("@serenity-is/" + ns[9..].ToLowerInvariant() + ":" + nonGeneric)) != null)
            return scriptType;

        if (ns.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var moduleColon in CommonSerenityModulesColon)
            {
                if ((scriptType = GetScriptType(moduleColon + nonGeneric)) != null)
                    return scriptType;
            }
        }

        if ((scriptType = GetScriptType(ns.ToLowerInvariant() + ":" + nonGeneric)) != null)
            return scriptType;

        return null;
    }

    private static readonly string[] CommonSerenityModulesColon = [
        "@serenity-is/corelib:",
        "@serenity-is/extensions:",
        "@serenity-is/pro.extensions:",
    ];

    private static readonly HashSet<string> SerenityNetAssemblies = new([
        "Serenity.Net.Core",
        "Serenity.Net.Services",
        "Serenity.Net.Web"
    ], StringComparer.OrdinalIgnoreCase);
}