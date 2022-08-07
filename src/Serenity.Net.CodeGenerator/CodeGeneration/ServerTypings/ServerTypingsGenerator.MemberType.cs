namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        protected override void HandleMemberType(TypeReference memberType, string codeNamespace, bool module)
        {
            bool isSystem = memberType.NamespaceOf() == "System";

            if (isSystem && memberType.Name == "String")
            {
                sb.Append("string");
                return;
            }

            if (memberType is GenericInstanceType &&
                memberType.MetadataName() == "Nullable`1" &&
                isSystem)
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
                HandleMemberType(memberType.ElementType(), codeNamespace, module);
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
                        HandleMemberType(gi.GenericArguments()[0], codeNamespace, module);
                        sb.Append("[]");
                        return;
                    }

                    if (gi.ElementType().MetadataName() == "Dictionary`2" ||
                        gi.ElementType().MetadataName() == "IDictionary`2")
                    {
                        sb.Append("{ [key: ");
                        HandleMemberType(gi.GenericArguments()[0], codeNamespace, module);
                        sb.Append("]: ");
                        HandleMemberType(gi.GenericArguments()[1], codeNamespace, module);
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

            EnqueueType(memberType.Resolve());
            MakeFriendlyReference(memberType, codeNamespace, module);
        }

        protected string ShortenFullName(ExternalType type, string codeNamespace)
        {
            if (type.FullName == "Serenity.Widget")
                return "Serenity.Widget<any>";

            if (type.FullName == "Serenity.CheckTreeItem")
                return "Serenity.CheckTreeItem<any>";

            var ns = ShortenNamespace(type, codeNamespace);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + type.Name;
            else
                return type.Name;
        }
    }
}