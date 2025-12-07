namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    protected virtual void AppendMappedType(TypeReference type, string codeNamespace)
    {
        if (type == null)
        {
            sb.Append("unknown /* null type */");
            return;
        }

        var ns = type.NamespaceOf();
        bool isSystem = ns == "System";

        if (isSystem && type.Name == "String")
        {
            sb.Append("string");
            return;
        }

        if (type.Name == "dynamic")
        {
            sb.Append("any");
            return;
        }

        if (isSystem &&
            type is GenericInstanceType &&
            type.MetadataName() == "Nullable`1")
        {
            type = (type as GenericInstanceType).GenericArguments()[0];
            isSystem = type.NamespaceOf() == "System";
        }

        var name = type.Name;

        if (isSystem &&
            type.IsPrimitive())
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

        if (type.IsArray())
        {
            var elementType = type.ElementType();
            if (elementType.NamespaceOf() == "Serenity.Services" &&
                elementType.Name == "SortBy")
            {
                sb.Append("string[]");
                return;
            }
        }

        if (name == "Stream" &&
            type.NamespaceOf() == "System.IO")
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

        if (type.IsArray())
        {
            AppendMappedType(type.ElementType(), codeNamespace);
            sb.Append("[]");
            return;
        }

        if (type.IsGenericInstanceType(out _))
        {
            var gi = type as GenericInstanceType;
            if (gi.ElementType().NamespaceOf() == "System.Collections.Generic")
            {
                if (gi.ElementType().MetadataName() == "List`1" ||
                    gi.ElementType().MetadataName() == "HashSet`1" ||
                    gi.ElementType().MetadataName() == "IList`1" ||
                    gi.ElementType().MetadataName() == "IEnumerable`1" ||
                    gi.ElementType().MetadataName() == "ISet`1")
                {
                    AppendMappedType(gi.GenericArguments()[0], codeNamespace);
                    sb.Append("[]");
                    return;
                }

                if (gi.ElementType().MetadataName() == "Dictionary`2" ||
                    gi.ElementType().MetadataName() == "IDictionary`2")
                {
                    sb.Append("{ [key: ");
                    AppendMappedType(gi.GenericArguments()[0], codeNamespace);
                    sb.Append("]: ");
                    AppendMappedType(gi.GenericArguments()[1], codeNamespace);
                    sb.Append(" }");
                    return;
                }
            }
        }

#if ISSOURCEGENERATOR
        if (type is Microsoft.CodeAnalysis.ITypeParameterSymbol)
#else
        if (type.IsGenericParameter)
#endif
        {
            sb.Append(type.Name);
            return;
        }

        var resolvedType = type.Resolve();

        if (resolvedType != null && TypingsUtils.GetAttr(resolvedType, "Serenity.ComponentModel", "TransformIgnoreAttribute") != null)
        {
            sb.Append("any");
            return;
        }
        else
        {
            EnqueueType(type.Resolve());
            MakeFriendlyReference(type, codeNamespace);
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