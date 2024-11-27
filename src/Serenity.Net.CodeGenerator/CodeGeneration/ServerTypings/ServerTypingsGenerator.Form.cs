#if !ISSOURCEGENERATOR
using Mono.Cecil.Cil;
#endif
using Microsoft.CodeAnalysis;

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    const string requestSuffix = "Request";

    private static string AutoDetermineEditorType(TypeReference valueType, TypeReference basedOnFieldType)
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

    private static string GetEditorTypeKeyFrom(TypeReference propertyType, TypeReference basedOnFieldType, CustomAttribute editorTypeAttr)
    {
        if (editorTypeAttr == null)
            return AutoDetermineEditorType(propertyType, basedOnFieldType);

        if (editorTypeAttr.AttributeType().FullNameOf() == "Serenity.ComponentModel.EditorTypeAttribute" ||
            editorTypeAttr.AttributeType().FullNameOf() == "Serenity.ComponentModel.CustomEditorAttribute")
        {
            if (editorTypeAttr.ConstructorArguments().Count == 1 &&
                editorTypeAttr.ConstructorArguments[0].Type.FullNameOf() == "System.String" &&
                editorTypeAttr.ConstructorArguments[0].Value is string)
                return editorTypeAttr.ConstructorArguments[0].Value as string;
        }

        var keyConstant = editorTypeAttr.AttributeType().Resolve().FieldsOf().FirstOrDefault(x =>
            x.IsStatic &&
            x.IsPublic() &&
            x.Name == "Key" &&
            x.HasConstant() &&
            x.Constant() is string &&
            x.DeclaringType().FullNameOf() == editorTypeAttr.AttributeType().FullNameOf());
        
        if (keyConstant != null && keyConstant.Constant() as string != null)
            return keyConstant.Constant() as string;

        string editorType;
#if !ISSOURCEGENERATOR
        editorType = editorTypeAttr.AttributeType().Resolve().MethodsOf()
            .Where(x => x.IsConstructor())
            .SelectMany(m => m.Body.Instructions
                .Where(i => i.OpCode == OpCodes.Call &&
                    (i.Operand is Mono.Cecil.MethodReference) &&
                    (i.Operand as Mono.Cecil.MethodReference).Resolve().IsConstructor &&
                    i.Previous.OpCode == OpCodes.Ldstr &&
                    i.Previous.Operand is string)
                .Select(x => x.Previous.Operand as string)).FirstOrDefault();

        if (editorType != null)
            return editorType;
#endif

        editorType = editorTypeAttr.AttributeType().FullNameOf();
        if (editorType.EndsWith("Attribute", StringComparison.Ordinal))
            editorType = editorType[..^"Attribute".Length];

        return editorType;
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

    private static IEnumerable<string> GetDialogTypeKeyRefs(CustomAttribute editorTypeAttr)
    {
        if (editorTypeAttr != null)
        {
            var dialogType = editorTypeAttr.NamedArguments().FirstOrDefault(x => string.Equals(x.Name(), "DialogType")).ArgumentValue() as string;
            if (!string.IsNullOrEmpty(dialogType))
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

    private ExternalType FindTypeInLookup(ILookup<string, ExternalType> lookup, string key, string suffix, string containingAssembly = null)
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

    private TypeDefinition GetBasedOnRowAndAnnotations(TypeDefinition type, 
        out ILookup<string, PropertyDefinition> basedOnByName,
        out List<AnnotationTypeInfo> rowAnnotations)
    {
        TypeDefinition basedOnRow = null;
        var basedOnRowAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "BasedOnRowAttribute");
        if (basedOnRowAttr != null &&
            basedOnRowAttr.ConstructorArguments().Count > 0 &&
            basedOnRowAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.Type")
            basedOnRow = (basedOnRowAttr.ConstructorArguments[0].Value as TypeReference).Resolve();

        rowAnnotations = basedOnRow != null ? GetAnnotationTypesFor(basedOnRow) : null;

        basedOnByName = null;
        if (basedOnRowAttr != null)
        {
            basedOnByName = EnumerateProperties(basedOnRow).Where(TypingsUtils.IsPublicInstanceProperty)
                .ToLookup(x => x.Name);
        }

        return basedOnRow;
    }

    private static CustomAttribute GetAttribute(PropertyDefinition item, PropertyDefinition basedOnField,
                IEnumerable<AnnotationTypeInfo> rowAnnotations, string ns, string name)
    {
        var attr = TypingsUtils.FindAttr(item.GetAttributes(), ns, name);

        if (attr == null && rowAnnotations != null)
        {
            foreach (var annotationType in rowAnnotations)
            {
                if (!annotationType.PropertyByName.TryGetValue(item.Name, out PropertyDefinition annotation))
                    continue;

                attr = TypingsUtils.FindAttr(annotation.GetAttributes(), ns, name);
                if (attr != null)
                    return attr;
            }
        }

        if (attr == null && basedOnField != null)
            attr = TypingsUtils.FindAttr(basedOnField.GetAttributes(), ns, name);

        return attr;
    }

    private void TryReferenceEnumType(TypeReference itemType, TypeReference basedOnFieldType, string codeNamespace,
        HashSet<(string group, string key)> referencedTypeKeys,
        List<(string group, string alias)> referencedTypeAliases)
    {
        TypeDefinition enumType = null;
        if (itemType != null)
            enumType = TypingsUtils.GetEnumTypeFrom(itemType);
        if (enumType is null && basedOnFieldType != null)
            enumType = TypingsUtils.GetEnumTypeFrom(basedOnFieldType);

        if (enumType is null)
            return;

        var enumKey = GetEnumKeyFor(enumType) ?? enumType.FullNameOf();
        if (!referencedTypeKeys.Add(("Enum", enumKey)))
            return;

        string containingAssembly = GetAssemblyNameFor(enumType);
        if (string.IsNullOrEmpty(containingAssembly) ||
            !assemblyNames.Contains(containingAssembly))
        {
            ExternalType enumScriptType = TryFindModuleType(enumType.FullNameOf(), containingAssembly) ??
                TryFindModuleType(enumKey, containingAssembly);

            if (enumScriptType != null)
                referencedTypeAliases.Add(("Enum", ReferenceScriptType(enumScriptType, codeNamespace)));
        }
        else
        {
            var moduleName = GetTypingFileNameFor(ScriptNamespaceFor(enumType), enumType.Name);
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

                PropertyDefinition basedOnField = null;
                if (basedOnByName != null)
                    basedOnField = basedOnByName[item.Name].FirstOrDefault();

                if (GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "IgnoreAttribute") != null ||
                    GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                    continue;

                var editorTypeAttr = GetAttribute(item, basedOnField, rowAnnotations, "Serenity.ComponentModel", "EditorTypeAttribute");
                var editorTypeKey = GetEditorTypeKeyFrom(item.PropertyType(), basedOnField?.PropertyType(), editorTypeAttr);

                ExternalType editorScriptType = null;

                editorScriptType = FindTypeInLookup(modularEditorTypeByKey, editorTypeKey, "Editor", containingAssembly: null);

                foreach (var typeKey in GetDialogTypeKeyRefs(editorTypeAttr))
                {
                    if (!referencedTypeKeys.Add(("Dialog", typeKey)))
                        continue;

                    var dialogType = FindTypeInLookup(modularDialogTypeByKey, typeKey, "Dialog", containingAssembly: null);
                    if (dialogType != null)
                        referencedTypeAliases.Add(("Dialog", ReferenceScriptType(dialogType, codeNamespace)));
                }

                TryReferenceEnumType(item.PropertyType(), basedOnField?.PropertyType(), codeNamespace, referencedTypeKeys, referencedTypeAliases);

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

                var editorFullName = ReferenceScriptType(editorScriptType, codeNamespace);
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
                cw.IndentedLine("private static init: boolean;");
                sb.AppendLine();
                cw.Indented("constructor(prefix: string)");
                cw.InBrace(delegate
                {
                    cw.IndentedLine("super(prefix);");
                    sb.AppendLine();
                    cw.Indented("if (!");
                    sb.Append(identifier);
                    sb.Append(".init) ");


                    cw.InBrace(delegate
                    {
                        cw.Indented(identifier);
                        sb.AppendLine(".init = true;");
                        sb.AppendLine();

                        var typeNumber = new Dictionary<string, int>();
                        foreach (var s in propertyTypes)
                        {
                            var typeName = s;
                            SplitGenericArguments(ref typeName);

                            if (!typeNumber.ContainsKey(typeName))
                            {
                                cw.Indented("var w");
                                sb.Append(typeNumber.Count);
                                sb.Append(" = ");
                                sb.Append(typeName);
                                sb.AppendLine(";");
                                typeNumber[typeName] = typeNumber.Count;
                            }
                        }
                        sb.AppendLine();

                        var initFormType = ImportFromQ("initFormType");
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
                                sb.Append("', w");
                                var typeName = propertyTypes[i];
                                SplitGenericArguments(ref typeName);
                                sb.Append(typeNumber[typeName]);
                                sb.Append("");
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