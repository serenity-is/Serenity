using Serenity.ComponentModel;
using Serenity.Reflection;
using Serenity.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        private void GenerateRowMembers(Type rowType)
        {
            bool anyMetadata = false;
            var codeNamespace = GetNamespace(rowType);
            Row row = (Row)rowType.GetInstance();

            var idRow = row as IIdRow;
            var isActiveRow = row as IIsActiveRow;
            var nameRow = row as INameRow;
            var lookupAttr = rowType.GetCustomAttribute<LookupScriptAttribute>();
            if (lookupAttr == null)
            {
                var script = lookupScripts.FirstOrDefault(x =>
                    x.BaseType != null &&
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericArguments().Any(z => z == rowType));

                if (script != null)
                    lookupAttr = script.GetCustomAttribute<LookupScriptAttribute>();
            }

            if (idRow != null)
            {
                cw.Indented("[InlineConstant] public const string IdProperty = \"");
                var field = ((Field)idRow.IdField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            if (isActiveRow != null)
            {
                cw.Indented("[InlineConstant] public const string IsActiveProperty = \"");
                var field = (isActiveRow.IsActiveField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            if (nameRow != null)
            {
                cw.Indented("[InlineConstant] public const string NameProperty = \"");
                var field = (nameRow.NameField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            var localTextPrefix = row.GetFields().LocalTextPrefix;
            if (!string.IsNullOrEmpty(localTextPrefix))
            {
                cw.Indented("[InlineConstant] public const string LocalTextPrefix = \"");
                sb.Append(localTextPrefix);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            if (lookupAttr != null)
            {
                cw.Indented("[InlineConstant] public const string LookupKey = \"");
                sb.Append(lookupAttr.Key);
                sb.AppendLine("\";");

                sb.AppendLine();
                cw.Indented("public static Lookup<");
                MakeFriendlyName(rowType, codeNamespace, null);
                sb.Append("> Lookup { [InlineCode(\"Q.getLookup('");
                sb.Append(lookupAttr.Key);
                sb.AppendLine("')\")] get { return null; } }");

                anyMetadata = true;
            }

            if (anyMetadata)
                sb.AppendLine();

            foreach (var field in row.GetFields())
            {
                cw.Indented("public ");

                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType, codeNamespace);
                    sb.Append('?');
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType, codeNamespace);
                }

                sb.Append(" ");
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine(" { get; set; }");
            }

            sb.AppendLine();
            cw.IndentedLine("[Imported, PreserveMemberCase]");
            cw.IndentedLine("public static class Fields");
            cw.InBrace(delegate
            {
                foreach (var field in row.GetFields())
                {
                    cw.Indented("[InlineConstant] public const string ");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.Append(" = \"");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("\";");
                }
            });
        }
    }
}