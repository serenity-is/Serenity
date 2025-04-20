#if !ISSOURCEGENERATOR
using Mono.Cecil.Cil;
#endif

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    private void GenerateColumns(TypeDefinition type, CustomAttribute columnsAttribute)
    {
        var codeNamespace = ScriptNamespaceFor(type);

        var identifier = type.Name;
        var referencedTypeKeys = new HashSet<(string group, string key)>();
        var referencedTypeAliases = new List<(string group, string alias)>();

        var basedOnRow = GetBasedOnRowAndAnnotations(type, out var basedOnByName, out var rowAnnotations);

        var publicProperties = type.PropertiesOf().Where(TypingsUtils.IsPublicInstanceProperty).ToArray();

        cw.Indented("export interface ");
        sb.Append(identifier);

        cw.InBrace(delegate
        {
            if (publicProperties.Length > 0)
            {
                var column = AddExternalImport("@serenity-is/sleekgrid", "Column");

                foreach (var item in publicProperties)
                {
                    cw.Indented(item.Name);
                    sb.Append(": ");
                    sb.Append(column);
                    if (basedOnRow != null)
                    {
                        sb.Append('<');
                        HandleMemberType(basedOnRow, codeNamespace);
                        sb.Append('>');
                    }
                    sb.AppendLine(";");
                }

            }
        });

        sb.AppendLine();

        cw.Indented("export class ");
        sb.Append(identifier);
        var columnsBase = ImportFromCorelib("ColumnsBase");
        sb.Append($" extends {columnsBase}");
        if (basedOnRow != null)
        {
            sb.Append('<');
            HandleMemberType(basedOnRow, codeNamespace);
            sb.Append('>');
        }

        cw.InBrace(delegate
        {
            cw.Indented("static readonly columnsKey = '");
            var key = columnsAttribute.ConstructorArguments() != null &&
                columnsAttribute.ConstructorArguments().Count > 0 ? columnsAttribute.ConstructorArguments()[0].Value as string : null;
            key ??= type.FullNameOf();

            sb.Append(key);
            sb.AppendLine("';");

            var fieldsProxy = ImportFromQ("fieldsProxy");
            cw.IndentedLine($"static readonly Fields = {fieldsProxy}<{type.Name}>();");

            foreach (var item in publicProperties)
            {
                if (!TypingsUtils.IsPublicInstanceProperty(item))
                    continue;

                PropertyDefinition basedOnField = null;
                if (basedOnByName != null)
                    basedOnField = basedOnByName[item.Name].FirstOrDefault();

                if (GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "IgnoreAttribute") != null ||
                    GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                    continue;

                var formatterTypeAttr = GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "FormatterTypeAttribute");
                var formatterTypeKey = GetFormatterTypeKeyFrom(item.PropertyType(), basedOnField?.PropertyType(), formatterTypeAttr);

                if (formatterTypeKey is null)
                    continue;

                if (!referencedTypeKeys.Add(("Formatter", formatterTypeKey)))
                    continue;

                var formatterScriptType = FindTypeInLookup(modularFormatterTypeByKey, formatterTypeKey, "Formatter", containingAssembly: null);
                if (formatterScriptType != null)
                    referencedTypeAliases.Add(("Formatter", ReferenceScriptType(formatterScriptType, codeNamespace)));

                TryReferenceEnumType(item.PropertyType(), basedOnField?.PropertyType(), codeNamespace, referencedTypeKeys, referencedTypeAliases);
            }
        });

        if (referencedTypeAliases.Count != 0)
        {
            sb.AppendLine();
            var otherTypes = referencedTypeAliases.Where(x => x.group != "Dialog");
            if (otherTypes.Any())
                sb.AppendLine($"[" + string.Join(", ", otherTypes.Select(x => x.alias)) + "]; // referenced types");

            var dialogTypes = referencedTypeAliases.Where(x => x.group == "Dialog");
            if (dialogTypes.Any())
                sb.AppendLine($"queueMicrotask(() => [" + string.Join(", ", dialogTypes.Select(x => x.alias)) + "]); // referenced dialogs");
        }

        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);
    }

    private static string AutoDetermineFormatterType(TypeReference valueType, TypeReference basedOnFieldType)
    {
        if (TypingsUtils.GetEnumTypeFrom(valueType) != null)
            return "Enum";

        if (basedOnFieldType != null &&
            TypingsUtils.GetEnumTypeFrom(basedOnFieldType) != null)
            return "Enum";

        valueType = (TypingsUtils.GetNullableUnderlyingType(valueType) ?? valueType).Resolve();

        if (valueType.NamespaceOf() == "System")
        {
            if (valueType.Name == "DateTime" ||
                valueType.Name == "DateOnly")
                return "Date";

            if (valueType.Name == "Boolean")
                return "Boolean";

            if (valueType.Name == "Int32" ||
                valueType.Name == "Int16" ||
                valueType.Name == "Decimal" ||
                valueType.Name == "Double" ||
                valueType.Name == "Single")
                return "Number";
        }

        return null;
    }

    private static string GetFormatterTypeKeyFrom(TypeReference propertyType, TypeReference basedOnFieldType, CustomAttribute formatterTypeAttr)
    {
        if (formatterTypeAttr == null)
            return AutoDetermineFormatterType(propertyType, basedOnFieldType);

        if (formatterTypeAttr.AttributeType().FullNameOf() == "Serenity.ComponentModel.FormatterTypeAttribute" ||
            formatterTypeAttr.AttributeType().FullNameOf() == "Serenity.ComponentModel.CustomFormatterAttribute")
        {
            if (formatterTypeAttr.ConstructorArguments().Count == 1 &&
                formatterTypeAttr.ConstructorArguments[0].Type.FullNameOf() == "System.String" &&
                formatterTypeAttr.ConstructorArguments[0].Value is string)
                return formatterTypeAttr.ConstructorArguments[0].Value as string;
        }

        var keyConstant = formatterTypeAttr.AttributeType().Resolve().FieldsOf().FirstOrDefault(x =>
            x.IsStatic &&
            x.IsPublic() &&
            x.Name == "Key" &&
            x.HasConstant() &&
            x.Constant() is string &&
            x.DeclaringType().FullNameOf() == formatterTypeAttr.AttributeType().FullNameOf());

        if (keyConstant != null && keyConstant.Constant() as string != null)
            return keyConstant.Constant() as string;

        string formatterType;
#if !ISSOURCEGENERATOR
            formatterType = formatterTypeAttr.AttributeType().Resolve().MethodsOf()
                .Where(x => x.IsConstructor())
                .SelectMany(m => m.Body.Instructions
                    .Where(i => i.OpCode == OpCodes.Call &&
                        (i.Operand is Mono.Cecil.MethodReference) &&
                        (i.Operand as Mono.Cecil.MethodReference).Resolve().IsConstructor &&
                        i.Previous.OpCode == OpCodes.Ldstr &&
                        i.Previous.Operand is string)
                    .Select(x => x.Previous.Operand as string)).FirstOrDefault();

            if (formatterType != null)
                return formatterType;
#endif

        formatterType = formatterTypeAttr.AttributeType().FullNameOf();
        if (formatterType.EndsWith("Attribute", StringComparison.Ordinal))
            formatterType = formatterType[..^"Attribute".Length];

        return formatterType;
    }
}