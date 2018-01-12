using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        private void GenerateRowMembers(Type rowType)
        {
            var codeNamespace = GetNamespace(rowType);

            Row row = (Row)Activator.CreateInstance(rowType);

            foreach (var field in row.GetFields())
            {
                cw.Indented(field.PropertyName ?? field.Name);
                sb.Append("?: ");
                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType, codeNamespace);
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType, codeNamespace);
                }

                sb.AppendLine(";");
            }
        }

        private void GenerateRowMetadata(Type rowType)
        {
            Row row = (Row)Activator.CreateInstance(rowType);

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

            sb.AppendLine();
            cw.Indented("export namespace ");
            sb.Append(rowType.Name);

            cw.InBrace(delegate
            {
                bool anyMetadata = false;

                if (idRow != null)
                {
                    cw.Indented("export const idProperty = '");
                    var field = ((Field)idRow.IdField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (isActiveRow != null)
                {
                    cw.Indented("export const isActiveProperty = '");
                    var field = (isActiveRow.IsActiveField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (nameRow != null)
                {
                    cw.Indented("export const nameProperty = '");
                    var field = (nameRow.NameField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                var localTextPrefix = row.GetFields().LocalTextPrefix;
                if (!string.IsNullOrEmpty(localTextPrefix))
                {
                    cw.Indented("export const localTextPrefix = '");
                    sb.Append(localTextPrefix);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (lookupAttr != null)
                {
                    cw.Indented("export const lookupKey = '");
                    sb.Append(lookupAttr.Key);
                    sb.AppendLine("';");

                    sb.AppendLine();
                    cw.Indented("export function getLookup(): Q.Lookup<");
                    sb.Append(rowType.Name);
                    sb.Append(">");
                    cw.InBrace(delegate
                    {
                        cw.Indented("return Q.getLookup<");
                        sb.Append(rowType.Name);
                        sb.Append(">('");
                        sb.Append(lookupAttr.Key);
                        sb.AppendLine("');");
                    });

                    anyMetadata = true;

                }

                if (anyMetadata)
                    sb.AppendLine();

                cw.Indented("export declare const enum ");
                sb.Append("Fields");

                cw.InBrace(delegate
                {
                    var inserted = 0;
                    foreach (var field in row.GetFields())
                    {
                        if (inserted > 0)
                            sb.AppendLine(",");

                        cw.Indented(field.PropertyName ?? field.Name);
                        sb.Append(" = \"");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.Append("\"");

                        inserted++;
                    }

                    sb.AppendLine();
                });
            });
        }
    }
}