using Serenity.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        protected override void HandleMemberType(Type memberType, string codeNamespace, 
            StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            if (memberType == typeof(String))
            {
                sb.Append("string");
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(memberType);
            if (nullableType != null)
                memberType = nullableType;

            if (memberType == typeof(Int16) ||
                memberType == typeof(Int32) ||
                memberType == typeof(Int64) ||
                memberType == typeof(UInt16) ||
                memberType == typeof(UInt32) ||
                memberType == typeof(UInt64) ||
                memberType == typeof(Single) ||
                memberType == typeof(Double) ||
                memberType == typeof(Decimal))
            {
                sb.Append("number");
                return;
            }

            if (memberType == typeof(Boolean))
            {
                sb.Append("boolean");
                return;
            }

            if (memberType == typeof(DateTime) ||
                memberType == typeof(TimeSpan) ||
                memberType == typeof(Guid))
            {
                sb.Append("string");
                return;
            }

            if (memberType == typeof(SortBy[]))
            {
                sb.Append("string[]");
                return;
            }

            if (memberType == typeof(Stream))
            {
                sb.Append("number[]");
                return;
            }

            if (memberType == typeof(Object))
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

            if (memberType.GetIsGenericType() &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) ||
                memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append("[]");
                return;
            }

            if (memberType.GetIsGenericType() &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                sb.Append("{ [key: ");
                HandleMemberType(memberType.GenericTypeArguments[0], codeNamespace, sb);
                sb.Append("]: ");
                HandleMemberType(memberType.GenericTypeArguments[1], codeNamespace, sb);
                sb.Append(" }");
                return;
            }

            EnqueueType(memberType);
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