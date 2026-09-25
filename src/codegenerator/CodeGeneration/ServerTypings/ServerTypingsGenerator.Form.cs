#if !ISSOURCEGENERATOR
using Mono.Cecil.Cil;
#endif
using Microsoft.CodeAnalysis;

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    const string requestSuffix = "Request";

    private static string AutoDetermineEditorType(TypeReference valueType, TypeReference? basedOnFieldType)
    {
        if (TypingsUtils.GetEnumTypeFrom(valueType) != null)
            return "Enum";

        if (basedOnFieldType != null &&
            TypingsUtils.GetEnumTypeFrom(basedOnFieldType) != null)
            return "Enum";

        valueType = (TypingsUtils.GetNullableUnderlyingType(valueType) ?? valueType).Resolve();

        if (valueType.NamespaceOf() == "System")
        {
            if (valueType.Name == "String")
                return "String";

            if (valueType.Name == "Int32" ||
                valueType.Name == "Int16")
                return "Integer";

            if (valueType.Name == "DateTime")
                return "Date";

            if (valueType.Name == "Boolean")
                return "Boolean";

            if (valueType.Name == "Decimal" ||
                valueType.Name == "Double" ||
                valueType.Name == "Single")
                return "Decimal";
        }

        return "String";
    }

#if ISSOURCEGENERATOR
    private string? GetAttributeKeyViaBaseCtorCall(TypeReference attributeType)
    {
        return TypingsUtils.GetAttributeKeyViaBaseCtorCall(attributeType, Compilation, cancellationToken);
    }
#else
#pragma warning disable CA1822 // Mark members as static
    private string? GetAttributeKeyViaBaseCtorCall(TypeReference attributeType)
    {
        return TypingsUtils.GetAttributeKeyViaBaseCtorCall(attributeType);
    }
#pragma warning restore CA1822 // Mark members as static
#endif

    private string? GetEditorTypeFromAttribute(CustomAttribute editorTypeAttr)
    {
        if (editorTypeAttr.AttributeType() is not { } attributeType)
            return null;

        string attrFullName = attributeType.FullNameOf();

        if (attrFullName is "Serenity.ComponentModel.EditorTypeAttribute"
                or "Serenity.ComponentModel.CustomEditorAttribute" &&
                TypingsUtils.GetAttributeKeyViaConstructorArgument(editorTypeAttr) is string ctorArgKey)
            return ctorArgKey;

        if ((TypingsUtils.GetAttributeKeyViaKeyConstant(attributeType) ??
             GetAttributeKeyViaBaseCtorCall(attributeType)) is string typeKey)
            return typeKey;

        if (attrFullName is not null &&
            attrFullName.EndsWith("Attribute", StringComparison.Ordinal))
            return attrFullName[..^"Attribute".Length];

        return attrFullName;
    }

    private string? GetEditorTypeKeyFrom(TypeReference propertyType, TypeReference? basedOnFieldType, CustomAttribute? editorTypeAttr)
    {
        if (editorTypeAttr == null)
            return AutoDetermineEditorType(propertyType, basedOnFieldType);

        return GetEditorTypeFromAttribute(editorTypeAttr);
    }

    public static int IndexOf<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (T item in source)
        {
            if (predicate(item))
                return index;
            index++;
        }
        return -1;
    }

    private IEnumerable<string> GetDialogTypeKeyRefs(CustomAttribute? editorTypeAttr)
    {
        if (editorTypeAttr != null)
        {
            var dialogType = editorTypeAttr.NamedArguments().FirstOrDefault(x => string.Equals(x.Name(), "DialogType")).ArgumentValue() as string;
            if (dialogType is { Length: > 0 })
                yield return dialogType;
            else
            {
                var inplaceAdd = editorTypeAttr.NamedArguments().FirstOrDefault(x => string.Equals(x.Name(), "InplaceAdd")).ArgumentValue() as bool?;
                if (inplaceAdd == true)
                {
#if ISSOURCEGENERATOR
                    var lookupType = editorTypeAttr.ConstructorArguments().Select(x => x.Value()).OfType<ITypeSymbol>().FirstOrDefault();
#else
                    var lookupType = editorTypeAttr.ConstructorArguments().Select(x => x.Value()).OfType<TypeReference>().FirstOrDefault()?.Resolve();
#endif

                    if (lookupType != null)
                    {
                        var lookupKey = AutoLookupKeyFor(lookupType);
                        if (!string.IsNullOrEmpty(lookupKey))
                            yield return lookupKey;
                    }
                    else
                    {
                        var lookupKey = editorTypeAttr.ConstructorArguments().Select(x => x.Value()).OfType<string>().FirstOrDefault();
                        if (!string.IsNullOrEmpty(lookupKey))
                            yield return lookupKey;
                    }
                }
            }
        }
    }

    private ExternalType? FindTypeInLookup(ILookup<string, ExternalType> lookup, string key, string suffix, string? containingAssembly = null)
    {
        var type = lookup[key].FirstOrDefault() ??
            lookup[key + suffix].FirstOrDefault() ??
            lookup["Serenity." + key + suffix].FirstOrDefault() ??
            lookup["Serenity." + key].FirstOrDefault();

        if (type is not null)
            return type;

        foreach (var rootNamespace in RootNamespaces)
        {
            string wn = rootNamespace + "." + key;
            type = lookup[wn].FirstOrDefault() ??
                lookup[wn + suffix].FirstOrDefault();
            if (type != null)
                return type;
        }

        if (type is null && key.IndexOfAny(['.', ':']) >= 0)
            type = TryFindModuleType(key, containingAssembly);

        return type;
    }

    private TypeDefinition? GetBasedOnRowAndAnnotations(TypeDefinition type,
        out ILookup<string, PropertyDefinition>? basedOnByName,
        out List<AnnotationTypeInfo>? rowAnnotations)
    {
        TypeDefinition? basedOnRow = null;
        var basedOnRowAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "BasedOnRowAttribute");
        if (basedOnRowAttr != null &&
            basedOnRowAttr.ConstructorArguments().Count > 0 &&
            basedOnRowAttr.ConstructorArguments()[0].Type?.FullNameOf() == "System.Type")
            basedOnRow = (basedOnRowAttr.ConstructorArguments[0].Value as TypeReference)!.Resolve();

        rowAnnotations = basedOnRow != null ? GetAnnotationTypesFor(basedOnRow) : null;

        basedOnByName = null;
        if (basedOnRowAttr != null)
        {
            basedOnByName = EnumerateProperties(basedOnRow!).Where(TypingsUtils.IsPublicInstanceProperty)
                .ToLookup(x => x.Name);
        }

        return basedOnRow;
    }

    private static CustomAttribute? GetAttribute(PropertyDefinition item, PropertyDefinition? basedOnField,
        IEnumerable<AnnotationTypeInfo>? rowAnnotations, string ns, string name)
    {
        var attr = TypingsUtils.FindAttr(item.GetAttributesWithIntrinsic(), ns, name);

        if (attr == null && rowAnnotations != null)
        {
            foreach (var annotationType in rowAnnotations)
            {
                if (!annotationType.PropertyByName.TryGetValue(item.Name, out PropertyDefinition? annotation))
                    continue;

                attr = TypingsUtils.FindAttr(annotation!.GetAttributesWithIntrinsic(), ns, name);
                if (attr != null)
                    return attr;
            }
        }

        if (attr == null && basedOnField != null)
            attr = TypingsUtils.FindAttr(basedOnField.GetAttributesWithIntrinsic(), ns, name);

        return attr;
    }

    private void TryReferenceEnumType(TypeReference? itemType, TypeReference? basedOnFieldType,
        HashSet<(string group, string key)> referencedTypeKeys,
        List<(string group, string alias)> referencedTypeAliases)
    {
        TypeDefinition? enumType = null;
        if (itemType != null)
            enumType = TypingsUtils.GetEnumTypeFrom(itemType);
        if (enumType is null && basedOnFieldType != null)
            enumType = TypingsUtils.GetEnumTypeFrom(basedOnFieldType);

        if (enumType is null)
            return;

        var enumKey = GetEnumKeyFor(enumType) ?? enumType.FullNameOf();
        if (!referencedTypeKeys.Add(("Enum", enumKey)))
            return;

        string? containingAssembly = GetAssemblyNameFor(enumType);
        if (containingAssembly is null or "" ||
            !assemblyNames.Contains(containingAssembly))
        {
            ExternalType? enumScriptType = TryFindModuleType(enumType.FullNameOf(), containingAssembly) ??
                TryFindModuleType(enumKey, containingAssembly);

            if (enumScriptType != null)
                referencedTypeAliases.Add(("Enum", ReferenceScriptType(enumScriptType)));
        }
        else
        {
            var moduleName = GetTypingFileNameFor(ScriptNamespaceFor(enumType!), enumType!.Name);
            referencedTypeAliases.Add(("Enum", AddModuleImport(moduleName, enumType.Name, external: false)));
        }
    }

    private void GenerateForm(TypeDefinition type, CustomAttribute formScriptAttribute,
        string identifier)
    {
        var codeNamespace = ScriptNamespaceFor(type);

        cw.Indented("export interface ");
        sb.Append(identifier);

        var propertyNames = new List<string>();
        var propertyTypes = new List<string>();
        var referencedTypeKeys = new HashSet<(string group, string key)>();
        var referencedTypeAliases = new List<(string group, string alias)>();
        var basedOnRow = GetBasedOnRowAndAnnotations(type, out var basedOnByName, out var rowAnnotations);

        cw.InBrace(delegate
        {
            foreach (var item in type.PropertiesOf())
            {
                if (!TypingsUtils.IsPublicInstanceProperty(item))
                    continue;

                PropertyDefinition? basedOnField = null;
                if (basedOnByName != null)
                    basedOnField = basedOnByName[item.Name].FirstOrDefault();

                if (GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "IgnoreUIFieldAttribute") != null ||
                    GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "TransformIgnoreAttribute") != null)
                    continue;

                var editorTypeAttr = GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "EditorTypeAttribute");
                var editorTypeKey = GetEditorTypeKeyFrom(item.PropertyType(), basedOnField?.PropertyType(), editorTypeAttr);

                ExternalType? editorScriptType = null;

                editorScriptType = FindTypeInLookup(modularEditorTypeByKey, editorTypeKey!, "Editor", containingAssembly: null);

                foreach (var typeKey in GetDialogTypeKeyRefs(editorTypeAttr))
                {
                    if (!referencedTypeKeys.Add(("Dialog", typeKey)))
                        continue;

                    var dialogType = FindTypeInLookup(modularDialogTypeByKey, typeKey, "Dialog", containingAssembly: null);
                    if (dialogType != null)
                        referencedTypeAliases.Add(("Dialog", ReferenceScriptType(dialogType)));
                }

                TryReferenceEnumType(item.PropertyType(), basedOnField?.PropertyType(), referencedTypeKeys, referencedTypeAliases);

                if (editorScriptType is null)
                {
                    foreach (var rootNamespace in RootNamespaces)
                    {
                        string wn = rootNamespace + "." + editorTypeKey;

                        if (rootNamespace == "Serenity" &&
                            (editorScriptType = (GetScriptType(wn + "Editor") ?? GetScriptType(wn))) != null)
                            break;

                        if ((editorScriptType = (GetScriptType(wn) ?? GetScriptType(wn + "Editor"))) != null)
                            break;
                    }
                }

                if (editorScriptType == null &&
                    (editorScriptType = (GetScriptType(editorTypeKey) ?? GetScriptType(editorTypeKey + "Editor"))) == null)
                {
                    editorScriptType = GetScriptType("@serenity-is/corelib:Widget");
                    if (editorScriptType is null)
                        continue;
                }

                var editorFullName = ReferenceScriptType(editorScriptType);
                var editorShortName = editorFullName;

                propertyNames.Add(item.Name);
                propertyTypes.Add(editorShortName);

                cw.Indented(item.Name);
                sb.Append(": ");
                sb.Append(editorFullName);
                sb.AppendLine(";");
            }
        });

        sb.AppendLine();
        cw.Indented("export class ");
        sb.Append(identifier);

        var prefixedContext = ImportFromCorelib("PrefixedContext");
        sb.Append($" extends {prefixedContext}");

        cw.InBrace(delegate
        {
            cw.Indented("static readonly formKey = '");
            var key = formScriptAttribute.ConstructorArguments() != null &&
                formScriptAttribute.ConstructorArguments().Count > 0 ? formScriptAttribute.ConstructorArguments[0].Value as string : null;
            key ??= type.FullNameOf();

            sb.Append(key);
            sb.AppendLine("';");

            if (propertyNames.Count > 0)
            {
                cw.IndentedLine("declare private static init: boolean;");
                sb.AppendLine();
                cw.Indented("constructor(...args: ConstructorParameters<typeof PrefixedContext>)");
                cw.InBrace(delegate
                {
                    cw.IndentedLine("super(...args);");
                    sb.AppendLine();
                    cw.Indented("if (!");
                    sb.Append(identifier);
                    sb.Append(".init)");

                    cw.InBrace(delegate
                    {
                        cw.Indented(identifier);
                        sb.AppendLine(".init = true;");
                        sb.AppendLine();
                        var initFormType = ImportFromCorelib("initFormType");
                        cw.Indented($"{initFormType}(");

                        sb.Append(identifier);
                        sb.AppendLine(", [");
                        cw.Block(delegate
                        {
                            for (var i = 0; i < propertyNames.Count; i++)
                            {
                                if (i > 0)
                                    sb.AppendLine(",");

                                cw.Indented("'");
                                sb.Append(propertyNames[i]);
                                sb.Append("', ");
                                var typeName = propertyTypes[i];
                                SplitGenericArguments(ref typeName);
                                sb.Append(typeName);
                            }

                            sb.AppendLine();
                        });
                        cw.IndentedLine("]);");
                    });
                });
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
}