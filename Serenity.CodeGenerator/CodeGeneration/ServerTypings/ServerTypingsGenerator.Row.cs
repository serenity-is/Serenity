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

                var fieldsType = rowType.NestedTypes.FirstOrDefault(x =>
                    CecilUtils.IsSubclassOf(x, "Serenity.Data", "RowFieldsBase"));
                
                if (fieldsType != null)
                {
                    foreach (var fieldName in fieldsType.Fields
                        .Where(x => x.IsPublic)
                        .Select(x => x.Name))
                    {
                        var property = propertyByName[fieldName].FirstOrDefault();
                        if (property != null)
                            yield return property;
                    }
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

        private string ExtractInterfacePropertyFromRow(TypeDefinition rowType, string[] interfaceTypes, 
            string propertyType, string propertyName, string getMethodFullName)
        {
            do
            {
                if (rowType.Interfaces.Any(x => interfaceTypes.Contains(x.InterfaceType.FullName)))
                {
                    var name = rowType.Methods.Where(x =>
                            x.Overrides.Any(z => z.FullName == getMethodFullName) ||
                            (x.IsSpecialName && x.Name == "get_" + propertyName && x.ReturnType != null && x.ReturnType.FullName == propertyType))
                        .SelectMany(x => x.Body.Instructions.Where(z =>
                            z.OpCode == OpCodes.Ldfld &&
                            z.Operand is FieldReference &&
                            CecilUtils.IsSubclassOf((z.Operand as FieldReference).DeclaringType.Resolve(), "Serenity.Data", "RowFieldsBase"))
                            .Select(z => (z.Operand as FieldReference).Name))
                        .FirstOrDefault();

                    if (name != null)
                        return name;
                }
            }
            while ((rowType = (rowType.BaseType?.Resolve())) != null && rowType.FullName != "Serenity.Data.Row");

            return null;
        }

        private string DetermineModuleIdentifier(TypeDefinition rowType)
        {
            var moduleAttr = CecilUtils.GetAttr(rowType, "Serenity.ComponentModel", "ModuleAttribute");
            if (moduleAttr != null)
                return moduleAttr.ConstructorArguments[0].Value as string;

            var ns = rowType.Namespace ?? "";

            if (ns.EndsWith(".Entities"))
                ns = ns.Substring(0, ns.Length - 9);

            var idx = ns.IndexOf(".");
            if (idx >= 0)
                ns = ns.Substring(idx + 1);

            return ns;
        }

        private string DetermineRowIdentifier(TypeDefinition rowType)
        {
            var name = rowType.Name;
            if (name.EndsWith("Row"))
                name = name.Substring(0, name.Length - 3);

            var moduleIdentifier = DetermineModuleIdentifier(rowType);
            return string.IsNullOrEmpty(moduleIdentifier) ? name : 
                moduleIdentifier + "." + name;
        }

        private string DetermineLocalTextPrefix(TypeDefinition rowType)
        {
            string localTextPrefix = null;
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
            
            var ltp = CecilUtils.GetAttr(rowType, "Serenity.ComponentModel", "LocalTextPrefixAttribute");
            if (ltp != null)
            {
                localTextPrefix = ltp.ConstructorArguments[0].Value as string;
                if (!string.IsNullOrEmpty(localTextPrefix))
                    return localTextPrefix;
            }

            return DetermineRowIdentifier(rowType);
        }

        private string AutoLookupKeyFor(TypeDefinition type)
        {
            string module;
            var moduleAttr = CecilUtils.GetAttr(type,
                "Serenity.ComponentModel", "ModuleAttribute");
            if (moduleAttr != null)
            {
                if (moduleAttr.ConstructorArguments.Count == 1 &&
                    moduleAttr.ConstructorArguments[0].Type.FullName == "System.String")
                    module = moduleAttr.ConstructorArguments[0].Value as string;
                else
                    module = null;
            }
            else
            {
                module = type.Namespace ?? "";

                if (module.EndsWith(".Entities"))
                    module = module.Substring(0, module.Length - 9);
                else if (module.EndsWith(".Scripts"))
                    module = module.Substring(0, module.Length - 8);
                else if (module.EndsWith(".Lookups"))
                    module = module.Substring(0, module.Length - 8);

                var idx = module.IndexOf(".");
                if (idx >= 0)
                    module = module.Substring(idx + 1);
            }

            var name = type.Name;
            if (name.EndsWith("Row"))
                name = name.Substring(0, name.Length - 3);
            else if (name.EndsWith("Lookup"))
                name = name.Substring(0, name.Length - 6);

            return string.IsNullOrEmpty(module) ? name :
                module + "." + name;
        }

        public string DetermineLookupKey(TypeDefinition rowType)
        {
            var lookupAttr = CecilUtils.GetAttr(rowType, 
                "Serenity.ComponentModel", "LookupScriptAttribute");

            TypeDefinition autoFrom = rowType;
            if (lookupAttr == null)
            {
                var script = lookupScripts.FirstOrDefault(x =>
                    x.BaseType != null &&
                    x.BaseType is GenericInstanceType &&
                    (x.BaseType as GenericInstanceType).GenericArguments.Any(z =>
                        z.Name == rowType.Name && z.Namespace == rowType.Namespace) &&
                    DetermineLookupKey(x) == AutoLookupKeyFor(rowType));

                if (script != null)
                {
                    lookupAttr = CecilUtils.GetAttr(script, "Serenity.ComponentModel",
                        "LookupScriptAttribute");
                    autoFrom = script;
                }
            }
            else if (lookupAttr.ConstructorArguments.Count > 0 &&
                lookupAttr.ConstructorArguments[0].Type.FullName == "System.Type")
            {
                autoFrom = ((TypeReference)lookupAttr.ConstructorArguments[0].Value).Resolve();
                lookupAttr = CecilUtils.GetAttr(autoFrom, 
                    "Serenity.ComponentModel", "LookupScriptAttribute");
            }

            if (lookupAttr == null)
                return null;

            if (lookupAttr.ConstructorArguments.Count == 1 &&
                lookupAttr.ConstructorArguments[0].Type.FullName == "System.String")
                return lookupAttr.ConstructorArguments[0].Value as string;

            if (lookupAttr.ConstructorArguments.Count == 1 &&
                lookupAttr.ConstructorArguments[0].Type.FullName == "System.Type")
            {
                return AutoLookupKeyFor(
                    (lookupAttr.ConstructorArguments[0].Value as TypeReference).Resolve());
            }

            if (lookupAttr.ConstructorArguments.Count == 1 &&
                lookupAttr.ConstructorArguments[0].Type.FullName == "System.Type")
            {
                return AutoLookupKeyFor(
                    (lookupAttr.ConstructorArguments[0].Value as TypeReference).Resolve());
            }

            if (lookupAttr.ConstructorArguments.Count == 0)
                return AutoLookupKeyFor(autoFrom);

            return null;
        }

        private void GenerateRowMetadata(TypeDefinition rowType)
        {
            var idProperty = ExtractInterfacePropertyFromRow(rowType, new[] { "Serenity.Data.IIdRow" }, 
                "Serenity.Data.IIdField", "IdField", 
                "Serenity.Data.IIdField Serenity.Data.IIdRow::get_IdField()");

            var nameProperty = ExtractInterfacePropertyFromRow(rowType, new[] { "Serenity.Data.INameRow" }, 
                    "Serenity.Data.StringField", "NameField",
                    "Serenity.Data.StringField Serenity.Data.INameRow::get_NameField()");

            if (nameProperty == null)
            {
                nameProperty = rowType.Properties.FirstOrDefault(x =>
                    x.HasCustomAttributes && CecilUtils.FindAttr(x.CustomAttributes,
                        "Serenity.Data", "NamePropertyAttribute") != null)?.Name;
            }

            var isActiveProperty = ExtractInterfacePropertyFromRow(rowType,
                new[] { "Serenity.Data.IIsActiveRow", "Serenity.Data.IIsActiveDeletedRow" },
                "Serenity.Data.Int16Field", "IsActiveField", 
                "Serenity.Data.Int16Field Serenity.Data.IIsActiveRow::get_IsActiveField()");

            var isDeletedProperty = ExtractInterfacePropertyFromRow(rowType,
                new[] { "Serenity.Data.IIsDeletedRow", "Serenity.Data.IIsDeletedRow" },
                "Serenity.Data.BooleanField", "IsDeletedField",
                "Serenity.Data.BooleanField Serenity.Data.IIsDeletedRow::get_IsDeletedField()");

            var lookupKey = DetermineLookupKey(rowType);

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

                if (isDeletedProperty != null)
                {
                    cw.Indented("export const isDeletedProperty = '");
                    sb.Append(isDeletedProperty);
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

                if (!string.IsNullOrEmpty(lookupKey))
                {
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