using Serenity.Reflection;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        protected override void HandleMemberType(Type memberType, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            if (memberType == typeof(String))
            {
                sb.Append("String");
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(memberType);
            if (nullableType != null)
                memberType = nullableType;

            if (memberType == typeof(DateTime) || 
                memberType == typeof(TimeSpan))
            {
                sb.Append("String");
                return;
            }

            if (GeneratorUtils.IsSimpleType(memberType))
            {
                sb.Append(memberType.Name);
                if (nullableType != null)
                    sb.Append("?");
                return;
            }

            if (nullableType != null)
            {
                HandleMemberType(nullableType, codeNamespace, sb);
                sb.Append("?");
                return;
            }

            if (memberType == typeof(SortBy[]))
            {
                sb.Append("SortBy[]");
                return;
            }

            if (memberType == typeof(Stream))
            {
                sb.Append("byte[]");
                return;
            }

            if (memberType == typeof(Object))
            {
                sb.Append("Object");
                return;
            }

            if (memberType.IsArray)
            {
                sb.Append("List<");
                HandleMemberType(memberType.GetElementType(), codeNamespace, sb);
                sb.Append(">");
                return;
            }

            if (memberType.GetIsGenericType() &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) ||
                memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                sb.Append("List<");
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append(">");
                return;
            }

            if (memberType.GetIsGenericType() &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                sb.Append("JsDictionary<");
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append(",");
                HandleMemberType(memberType.GenericTypeArguments[1], codeNamespace, sb);
                sb.Append(">");
                return;
            }

            EnqueueType(memberType);

            MakeFriendlyReference(memberType, codeNamespace);
        }

        protected string ShortenFullName(ExternalType type, string codeNamespace)
        {
            if (type.FullName == "Serenity.Widget")
                return "Serenity.Widget<any>";

            var ns = ShortenNamespace(type, codeNamespace);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + type.Name;
            else
                return type.Name;
        }
    }
}