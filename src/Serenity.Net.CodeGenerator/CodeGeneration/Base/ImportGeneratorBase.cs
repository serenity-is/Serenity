namespace Serenity.CodeGeneration;

public abstract class ImportGeneratorBase : CodeGeneratorBase
{
    private readonly Dictionary<string, ExternalType> tsTypes;

    public ImportGeneratorBase()
    {
        RootNamespaces = [ "Serenity" ];
        tsTypes = [];
    }

    public IDictionary<string, ExternalType> TSTypes => tsTypes;

    public HashSet<string> RootNamespaces { get; private set; }

    private static readonly char[] genericSep = ['€'];

    public void AddTSType(ExternalType type)
    {
        tsTypes[type.FullName] = type;
    }

    public void AddBuiltinTSTypes()
    {
        foreach (var type in BuiltinTSTypes.All)
            AddTSType(type);
    }

    protected ExternalType GetScriptType(string fullName, bool fallback = true)
    {
        if (string.IsNullOrEmpty(fullName))
            return null;

        if (tsTypes.TryGetValue(fullName, out ExternalType type))
            return type;

        if (fallback &&
            fullName.StartsWith("@serenity-is/corelib:"))
        {
            var name = fullName[9..];
            if (tsTypes.TryGetValue(name, out type))
                return type;
        }

        return null;
    }

    protected static string[] SplitGenericArguments(ref string typeName)
    {
        if (!typeName.Contains('<'))
            return [];

        var pos = typeName.IndexOf('<');
        var last = typeName.LastIndexOf('>');
        if (pos >= 0 && last > pos)
        {
            char[] c = typeName.Substring(pos + 1, last - pos - 1).ToCharArray();
            typeName = typeName[..pos];

            int nestingLevel = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == '<')
                    nestingLevel++;
                else if (c[i] == '>')
                    nestingLevel--;
                else if ((c[i] == ',') && (nestingLevel == 0))
                    c[i] = '€';
            }

            return new string(c).Split(genericSep);
        }
        else
            return [];
    }

    protected string RemoveRootNamespace(string ns, string name)
    {
        if (!string.IsNullOrEmpty(ns))
            name = ns + "." + name;

        foreach (var rn in RootNamespaces)
        {
            if (name.StartsWith(rn + ".", StringComparison.Ordinal))
                return name[(rn.Length + 1)..];
        }

        return name;
    }

    protected static string TranslateJSPropertyName(string jsName)
    {
        if (string.IsNullOrEmpty(jsName))
            return jsName;

        if (char.IsLower(jsName[0]))
        {
            if (jsName == "id")
                return "ID";
                
            return char.ToUpperInvariant(jsName[0]) + jsName[1..];
        }

        return jsName;
    }

    protected static string GetBaseTypeName(ExternalType type)
    {
        var baseType = type.BaseType;

        if (string.IsNullOrEmpty(baseType))
            return null;

        SplitGenericArguments(ref baseType);
        return baseType;
    }

    protected ExternalType GetBaseType(ExternalType type)
    {
        return GetScriptType(GetBaseTypeName(type));
    }

    protected ExternalType GetScriptTypeFrom(ExternalType fromType, string typeName)
    {
        if (typeName == null)
            return null;

        if (typeName.StartsWith('[') && typeName.EndsWith(']'))
        {
            return new ExternalType
            {
                Name = typeName,
                IsInterface = true,
                IsDeclaration = true,
                Interfaces = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(typeName),
                IsIntersectionType = true
            };
        }

        if (typeName.StartsWith('{') && typeName.EndsWith('}'))
        {
            return new ExternalType
            {
                Name = typeName,
                IsInterface = true,
                IsDeclaration = true,
                Fields = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(typeName)
                    .Select(x => new ExternalMember
                    {
                        Name = x.Key,
                        Type = x.Value
                    }).ToList()
            };
        }

        var ns = fromType.Namespace;
        var scriptType = GetScriptType(typeName);
        if (scriptType != null)
            return scriptType;

        if (!string.IsNullOrEmpty(fromType.Module) &&
            typeName?.IndexOf(':') < 0)
        {
            var moduleTypeName = fromType.Module + ":" + typeName;
            scriptType = GetScriptType(moduleTypeName);
            if (scriptType != null)
                return scriptType;
        }

        if (ns != null)
        {
            var nsParts = ns.Split('.');
            for (var i = nsParts.Length; i > 0; i--)
            {
                var prefixed = string.Join(".", nsParts.Take(i)) + '.' + typeName;
                scriptType = GetScriptType(prefixed);
                if (scriptType != null)
                    return scriptType;
            }
        }

        return null;
    }

    protected bool HasBaseType(ExternalType type, params string[] typeNames)
    {
        int loop = 0;

        while (type != null)
        {
            if (loop++ > 100)
                break;

            var baseTypeName = GetBaseTypeName(type);
            if (typeNames.Contains(baseTypeName, StringComparer.Ordinal))
                return true;

            var ns = type.Namespace;
            var baseType = GetScriptType(baseTypeName);

            if (baseType == null &&
                !string.IsNullOrEmpty(type.Module) &&
                baseTypeName?.IndexOf(':') < 0)
            {
                var moduleBaseTypeName = type.Module + ":" + baseTypeName;
                if (typeNames.Contains(moduleBaseTypeName, StringComparer.Ordinal))
                    return true;

                baseType = GetScriptType(moduleBaseTypeName);
            }

            if (baseType == null && ns != null)
            {
                var nsParts = ns.Split('.');
                for (var i = nsParts.Length; i > 0; i--)
                {
                    var prefixed = string.Join(".", nsParts.Take(i)) + '.' + baseTypeName;
                    if (typeNames.Contains(prefixed, StringComparer.Ordinal))
                        return true;
                    baseType = GetScriptType(prefixed);
                    if (baseType != null)
                        break;
                }
            }

            type = baseType;
        };

        return false;
    }

    protected ExternalAttribute GetAttribute(ExternalType type, bool inherited, params string[] attributeNames)
    {
        var attr = type.Attributes?.FirstOrDefault(x => attributeNames.Contains(x.Type, StringComparer.Ordinal));
        if (attr != null)
            return attr;

        if (!inherited)
            return null;

        int loop = 0;
        while ((type = GetBaseType(type)) != null)
        {
            attr = type.Attributes?.FirstOrDefault(x => attributeNames.Contains(x.Type, StringComparer.Ordinal));
            if (attr != null)
                return attr;

            if (loop++ > 100)
                return null;
        };

        return null;
    }

    static readonly string[] EditorAttributeNames =
    [
        "Serenity.EditorAttribute",
        "@serenity-is/corelib:EditorAttribute",
        "Serenity.ElementAttribute",
        "@serenity-is/corelib:ElementAttribute",
        "Serenity.Decorators.registerEditor",
        "@serenity-is/corelib:Decorators.registerEditor",
        "Decorators.registerEditor",
        "registerEditor",
        "Serenity.Decorators.element",
        "@serenity-is/corelib:Decorators.element",
        "Decorators.element"
    ];

    public static readonly string[] EditorBaseClasses =
    [
        "Serenity.Extensions.GridEditorBase",
        "GridEditorBase",
        "@serenity-is/extensions:GridEditorBase",
        "Serenity.LookupEditorBase",
        "LookupEditorBase",
        "@serenity-is/corelib:LookupEditorBase",
        "LookupEditor",
        "@serenity-is/corelib:LookupEditor",
        "ServiceLookupEditor",
        "@serenity-is/corelib:ServiceLookupEditor",
        "ServiceLookupEditorBase",
        "@serenity-is/corelib:ServiceLookupEditorBase",
    ];

    protected bool IsEditorType(ExternalType type)
    {
        if (type.IsAbstract == true ||
            type.IsInterface == true ||
            type.IsEnum == true)
            return false;

        if (type.GenericParameters?.Any(x => string.IsNullOrEmpty(x.Default)) == true)
            return false;

        if (HasBaseType(type, EditorBaseClasses))
            return true;

        if (GetAttribute(type, inherited: true, attributeNames: EditorAttributeNames) != null)
            return true;

        return false;
    }

    static readonly string[] FormatterAttributeNames =
    [
        "Serenity.Decorators.registerFormatter",
        "@serenity-is/corelib:Decorators.registerFormatter",
        "Decorators.registerFormatter",
        "registerFormatter"
    ];

    protected bool IsFormatterType(ExternalType type)
    {
        if (type.IsAbstract == true ||
            type.IsEnum == true ||
            type.IsInterface == true)
            return false;

        if (type.GenericParameters?.Any(x => string.IsNullOrEmpty(x.Default)) == true)
            return false;

        if (GetAttribute(type, inherited: true, attributeNames: FormatterAttributeNames) != null)
            return true;

        return type.Interfaces != null && type.Interfaces.Any(x =>
            x == "Serenity.ISlickFormatter" ||
            x == "Slick.Formatter" ||
            x == "@serenity-is/corelib:Formatter" ||
            x == "@serenity-is/corelib/slick:Formatter" ||
            x?.EndsWith("ISlickFormatter", StringComparison.Ordinal) == true);
    }
}