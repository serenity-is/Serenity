using Mono.Cecil;
using System.Text;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : CecilImportGenerator
    {
        protected override void HandleMemberType(TypeReference memberType, string codeNamespace, 
            StringBuilder sb = null)
        {
            sb = sb ?? this.sb;
            bool isSystem = memberType.Namespace == "System";

            if (isSystem && memberType.Name == "String")
            {
                sb.Append("string");
                return;
            }

            if (memberType is GenericInstanceType &&
                memberType.Name == "Nullable`1" &&
                isSystem)
            {
                memberType = (memberType as GenericInstanceType).GenericArguments[0];
                isSystem = memberType.Namespace == "System";
            }

            var name = memberType.Name;

            if (isSystem &&
                memberType.IsPrimitive)
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

            if (memberType.IsArray)
            {
                var elementType = memberType.GetElementType();
                if (elementType.Namespace == "Serenity.Services" &&
                    elementType.Name == "SortBy")
                {
                    sb.Append("string[]");
                    return;
                }
            }

            if (name == "Stream" &&
                memberType.Namespace == "System.IO")
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

            if (memberType.IsArray)
            {
                HandleMemberType(memberType.GetElementType(), codeNamespace, sb);
                sb.Append("[]");
                return;
            }

            if (memberType.IsGenericInstance)
            {
                var gi = memberType as GenericInstanceType;
                if (gi.ElementType.Namespace == "System.Collections.Generic")
                {
                    if (gi.ElementType.Name == "List`1" ||
                        gi.ElementType.Name == "HashSet`1")
                    {
                        HandleMemberType(gi.GenericArguments[0], codeNamespace, sb);
                        sb.Append("[]");
                        return;
                    }

                    if (gi.ElementType.Name == "Dictionary`2")
                    {
                        sb.Append("{ [key: ");
                        HandleMemberType(gi.GenericArguments[0], codeNamespace, sb);
                        sb.Append("]: ");
                        HandleMemberType(gi.GenericArguments[1], codeNamespace, sb);
                        sb.Append(" }");
                        return;
                    }
                }
            }

            EnqueueType(memberType.Resolve());
            MakeFriendlyReference(memberType, codeNamespace);
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