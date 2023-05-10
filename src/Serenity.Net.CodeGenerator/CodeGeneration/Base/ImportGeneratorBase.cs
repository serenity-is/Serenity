namespace Serenity.CodeGeneration;

public abstract class ImportGeneratorBase : CodeGeneratorBase
{
    protected Dictionary<string, ExternalType> tsTypes;

    public ImportGeneratorBase()
    {
        RootNamespaces = new HashSet<string>
        {
            "Serenity"
        };

        tsTypes = new Dictionary<string, ExternalType>();
    }

    public HashSet<string> RootNamespaces { get; private set; }

    public void AddTSType(ExternalType type)
    {
        tsTypes[type.FullName] = type;
    }

    protected ExternalType GetScriptType(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
            return null;

        if (tsTypes.TryGetValue(fullName, out ExternalType type))
            return type;

        return null;
    }

    protected static string[] SplitGenericArguments(ref string typeName)
    {
        if (!typeName.Contains('<'))
            return Array.Empty<string>();

        var pos = typeName.IndexOf("<", StringComparison.Ordinal);
        var last = typeName.LastIndexOf(">", StringComparison.Ordinal);
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

            return new string(c).Split(new char[] { '€' });
        }
        else
            return Array.Empty<string>();
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

    protected static string GetPropertyScriptName(ExternalProperty prop, bool preserveMemberCase)
    {
        var scriptNameAttr = prop.Attributes?.FirstOrDefault(x =>
            x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

        if (scriptNameAttr != null)
            return scriptNameAttr.Arguments?[0].Value as string;
        else if (!preserveMemberCase && (prop.Attributes == null || !prop.Attributes.Any(x =>
                x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute")))
        {
            var propField = prop.Name;
            if (propField == "ID")
                return "id";
            else
                return propField[..1].ToLowerInvariant()
                    + propField[1..];
        }
        else
            return prop.Name;
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
}