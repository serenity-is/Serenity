namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected override void HandleMemberType(TypeReference memberType, string codeNamespace)
    {
        var ns = memberType.NamespaceOf();
        bool isSystem = ns == "System";

        if (isSystem && memberType.Name == "String")
        {
            sb.Append("string");
            return;
        }

        if (memberType.Name == "dynamic")
        {
            sb.Append("any");
            return;
        }

        if (isSystem &&
            memberType is GenericInstanceType &&
            memberType.MetadataName() == "Nullable`1")
        {
            memberType = (memberType as GenericInstanceType).GenericArguments()[0];
            isSystem = memberType.NamespaceOf() == "System";
        }

        var name = memberType.Name;

        if (isSystem &&
            memberType.IsPrimitive())
        {
            if (name == "Int16" ||
                name == "Int32" ||
                name == "Int64" ||
                name == "Double" ||
                name == "Single" ||
                name == "UInt16" ||
                name == "UInt32" ||
                name == "UInt64" ||
                name == "Byte" ||
                name == "SByte")
            {
                sb.Append("number");
                return;
            }

            if (name == "Boolean")
            {
                sb.Append("boolean");
                return;
            }
        }

        if (isSystem &&
            name == "Decimal")
        {
            sb.Append("number");
            return;
        }

        if (isSystem &&
            name == "DateTime" ||
            name == "DateOnly" ||
            name == "TimeSpan" ||
            name == "DateTimeOffset" ||
            name == "Guid")
        {
            sb.Append("string");
            return;
        }

        if (memberType.IsArray())
        {
            var elementType = memberType.ElementType();
            if (elementType.NamespaceOf() == "Serenity.Services" &&
                elementType.Name == "SortBy")
            {
                sb.Append("string[]");
                return;
            }
        }

        if (name == "Stream" &&
            memberType.NamespaceOf() == "System.IO")
        {
            sb.Append("number[]");
            return;
        }

        if (isSystem && 
            name == "Object")
        {
            sb.Append("any");
            return;
        }

        if (memberType.IsArray())
        {
            HandleMemberType(memberType.ElementType(), codeNamespace);
            sb.Append("[]");
            return;
        }

        if (memberType.IsGenericInstance())
        {
            var gi = memberType as GenericInstanceType;
            if (gi.ElementType().NamespaceOf() == "System.Collections.Generic")
            {
                if (gi.ElementType().MetadataName() == "List`1" ||
                    gi.ElementType().MetadataName() == "HashSet`1" ||
                    gi.ElementType().MetadataName() == "IList`1" ||
                    gi.ElementType().MetadataName() == "IEnumerable`1" ||
                    gi.ElementType().MetadataName() == "ISet`1")
                {
                    HandleMemberType(gi.GenericArguments()[0], codeNamespace);
                    sb.Append("[]");
                    return;
                }

                if (gi.ElementType().MetadataName() == "Dictionary`2" ||
                    gi.ElementType().MetadataName() == "IDictionary`2")
                {
                    sb.Append("{ [key: ");
                    HandleMemberType(gi.GenericArguments()[0], codeNamespace);
                    sb.Append("]: ");
                    HandleMemberType(gi.GenericArguments()[1], codeNamespace);
                    sb.Append(" }");
                    return;
                }
            }
        }

#if ISSOURCEGENERATOR
        if (memberType is Microsoft.CodeAnalysis.ITypeParameterSymbol)
#else
        if (memberType.IsGenericParameter)
#endif
        {
            sb.Append(memberType.Name);
            return;
        }

        var resolvedType = memberType.Resolve();

        if (resolvedType != null && TypingsUtils.GetAttr(resolvedType, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
        {
            sb.Append("any");
            return;
        }
        else
        {
            EnqueueType(memberType.Resolve());
            MakeFriendlyReference(memberType, codeNamespace);
        }
    }

    protected string ReferenceScriptType(ExternalType type, string codeNamespace)
    {
        if (type.FullName == "Serenity.Widget")
            return "Serenity.Widget<any>";

        if (type.FullName == "Serenity.CheckTreeItem")
            return "Serenity.CheckTreeItem<any>";

        var ns = type.Namespace;
        var name = type.Name;
        var sourceFile = type.SourceFile;

        if (!string.IsNullOrEmpty(type.Module))
        {
            return AddModuleImport(type.Module, type.Name, external:
                !type.Module.StartsWith('/') &&
                !type.Module.StartsWith('.'));
        }
        else if (sourceFile == null || !sourceFile.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase))
        {
            var filename = GetTypingFileNameFor(ns, name);
            name = AddModuleImport(filename, name, external: false);
            ns = "";
        }

        return !string.IsNullOrEmpty(ns) ? (ns + "." + name) : name;
    }
}