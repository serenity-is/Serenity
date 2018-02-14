using Mono.Cecil;
using Mono.Cecil.Cil;
using Serenity.Data;
using Serenity.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : CecilImportGenerator
    {
        private IEnumerable<PropertyDefinition> EnumerateFieldProperties(TypeDefinition rowType)
        {
            do
            {
                var propertyByName = rowType.Properties.Where(x =>
                    CecilUtils.IsPublicInstanceProperty(x) &&
                    (!x.PropertyType.Name.EndsWith("Field") ||
                      x.PropertyType.Namespace != "Serenity.Data")).ToLookup(x => x.Name);

                foreach (var fieldName in rowType.NestedTypes.FirstOrDefault(x =>
                    CecilUtils.IsSubclassOf(x, "Serenity.Data", "RowFieldsBase")).Fields
                        .Where(x => x.IsPublic)
                        .Select(x => x.Name))
                {
                    var property = propertyByName[fieldName].FirstOrDefault();
                    if (property != null)
                        yield return property;
                }
            }
            while ((rowType = (rowType.BaseType?.Resolve())) != null && rowType.FullName != "Serenity.Data.Row");
        }

        private void GenerateRowMembers(TypeDefinition rowType)
        {
            var codeNamespace = GetNamespace(rowType);

            foreach (var property in EnumerateFieldProperties(rowType))
            {
                cw.Indented(property.Name);
                sb.Append("?: ");

                var enumType = CecilUtils.GetEnumTypeFrom(property.PropertyType);
                if (enumType != null)
                {
                    HandleMemberType(enumType, codeNamespace);
                }
                else
                {
                    HandleMemberType(CecilUtils.GetNullableUnderlyingType(property.PropertyType) ?? property.PropertyType, codeNamespace);
                }

                sb.AppendLine(";");
            }
        }

        private string ExtractInterfacePropertyFromRow(TypeDefinition rowType, string getMethodFullName)
        {
            return rowType.Methods.Where(x => x.Overrides.Any(z => z.FullName == getMethodFullName))
                .SelectMany(x => x.Body.Instructions.Where(z =>
                    z.OpCode == OpCodes.Ldfld &&
                    z.Operand is FieldReference &&
                    CecilUtils.IsSubclassOf((z.Operand as FieldReference).DeclaringType.Resolve(), "Serenity.Data", "RowFieldsBase"))
                    .Select(z => (z.Operand as FieldReference).Name))
                .FirstOrDefault();
        }

        private string DetermineLocalTextPrefix(TypeDefinition rowType)
        {
            string localTextPrefix = null;
            var attr = CecilUtils.GetAttr(rowType, "Serenity.ComponentModel", "LocalTextPrefix");
            if (attr != null)
            {
                localTextPrefix = attr.ConstructorArguments[0].Value as string;
                if (!string.IsNullOrEmpty(localTextPrefix))
                    return localTextPrefix;
            }

            var fieldsType = rowType.NestedTypes.FirstOrDefault(x =>
                CecilUtils.IsSubclassOf(x, "Serenity.Data", "RowFieldsBase"));

            if (fieldsType != null)
            {
                var constructors = fieldsType.Resolve().Methods.Where(x => x.IsConstructor);
                localTextPrefix = constructors.SelectMany(x => x.Body.Instructions.Where(z =>
                        (z.OpCode == OpCodes.Call || z.OpCode == OpCodes.Calli ||
                        z.OpCode == OpCodes.Callvirt) &&
                        z.Operand is MethodReference &&
                        (z.Operand as MethodReference).FullName ==
                        "System.Void Serenity.Data.RowFieldsBase::set_LocalTextPrefix(System.String)" &&
                        z.Previous.OpCode == OpCodes.Ldstr &&
                        z.Previous.Operand is string)).Select(x => x.Previous.Operand as string)
                    .FirstOrDefault();

                if (localTextPrefix != null)
                    return localTextPrefix;
            }

            var parts = (rowType.Namespace ?? "").Split('.') as IEnumerable<string>;
            if (!parts.Any())
                return null;

            if (parts.Last() == "Entities")
                parts = parts.Take(parts.Count() - 1);

            if (parts.Count() > 1)
                parts = parts.Skip(1);

            return string.Join(".", parts);
        }

        private void GenerateRowMetadata(TypeDefinition rowType)
        {
            var idProperty = ExtractInterfacePropertyFromRow(rowType,
                "Serenity.Data.IIdField Serenity.Data.IIdRow::get_IdField()");
            var nameProperty = ExtractInterfacePropertyFromRow(rowType,
                "Serenity.Data.StringField Serenity.Data.INameRow::get_NameField()");
            var isActiveProperty = ExtractInterfacePropertyFromRow(rowType,
                "Serenity.Data.Int16Field Serenity.Data.IIsActiveRow::get_IsActiveField()");

            var lookupAttr = CecilUtils.GetAttr(rowType, "Serenity.ComponentModel", "LookupScriptAttribute");
            if (lookupAttr == null)
            {
                var script = lookupScripts.FirstOrDefault(x =>
                    x.BaseType != null &&
                    x.BaseType is GenericInstanceType &&
                    (x.BaseType as GenericInstanceType).GenericArguments.Any(z =>
                        z.Name == rowType.Name && z.Namespace == rowType.Namespace));

                if (script != null)
                    lookupAttr = CecilUtils.GetAttr(script, "Serenity.ComponentModel", "LookupScriptAttribute");
            }
            else if (lookupAttr.ConstructorArguments.Count > 0 &&
                lookupAttr.ConstructorArguments[0].Type.FullName == "System.Type")
            {
                lookupAttr = CecilUtils.GetAttr(((TypeReference)lookupAttr.ConstructorArguments[0].Value).Resolve(), 
                    "Serenity.ComponentModel", "LookupScriptAttribute");
            }

            sb.AppendLine();
            cw.Indented("export namespace ");
            sb.Append(rowType.Name);

            cw.InBrace(delegate
            {
                bool anyMetadata = false;

                if (idProperty != null)
                {
                    cw.Indented("export const idProperty = '");
                    sb.Append(idProperty);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (isActiveProperty != null)
                {
                    cw.Indented("export const isActiveProperty = '");
                    sb.Append(isActiveProperty);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (nameProperty != null)
                {
                    cw.Indented("export const nameProperty = '");
                    sb.Append(nameProperty);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                var localTextPrefix = DetermineLocalTextPrefix(rowType);
                if (!string.IsNullOrEmpty(localTextPrefix))
                {
                    cw.Indented("export const localTextPrefix = '");
                    sb.Append(localTextPrefix);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (lookupAttr != null && lookupAttr.ConstructorArguments.Count > 0 && 
                    lookupAttr.ConstructorArguments[0].Type.FullName == "System.String")
                {
                    var lookupKey = lookupAttr.ConstructorArguments[0].Value as string;
                    cw.Indented("export const lookupKey = '");
                    sb.Append(lookupKey);
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
                        sb.Append(lookupKey);
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
                    foreach (var property in EnumerateFieldProperties(rowType))
                    {
                        if (inserted > 0)
                            sb.AppendLine(",");

                        cw.Indented(property.Name);
                        sb.Append(" = \"");
                        sb.Append(property.Name);
                        sb.Append("\"");

                        inserted++;
                    }

                    sb.AppendLine();
                });
            });
        }
    }
}