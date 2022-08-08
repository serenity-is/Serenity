#if !ISSOURCEGENERATOR
using Mono.Cecil.Cil;
#endif

namespace Serenity.CodeGeneration
{
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

        private void GenerateForm(TypeDefinition type, CustomAttribute formScriptAttribute,
            string identifier, bool module)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export interface ");
            sb.Append(identifier);

            var propertyNames = new List<string>();
            var propertyTypes = new List<string>();

            TypeDefinition basedOnRow = null;
            var basedOnRowAttr = TypingsUtils.GetAttr(type, "Serenity.ComponentModel", "BasedOnRowAttribute");
            if (basedOnRowAttr != null &&
                basedOnRowAttr.ConstructorArguments().Count > 0 &&
                basedOnRowAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.Type")
                basedOnRow = (basedOnRowAttr.ConstructorArguments[0].Value as TypeReference).Resolve();

            var rowAnnotations = basedOnRow != null ? GetAnnotationTypesFor(basedOnRow) : null;

            ILookup<string, PropertyDefinition> basedOnByName = null;
            if (basedOnRowAttr != null)
            {
                basedOnByName = basedOnRow.PropertiesOf().Where(x => TypingsUtils.IsPublicInstanceProperty(x))
                    .ToLookup(x => x.Name);
            }

            cw.InBrace(delegate
            {
                foreach (var item in type.PropertiesOf())
                {
                    if (!TypingsUtils.IsPublicInstanceProperty(item))
                        continue;

                    PropertyDefinition basedOnField = null;
                    if (basedOnByName != null)
                        basedOnField = basedOnByName[item.Name].FirstOrDefault();

                    if (TypingsUtils.FindAttr(item.GetAttributes(), "Serenity.ComponentModel", "IgnoreAttribute") != null)
                        continue;

                    if (basedOnField != null)
                    {
                        if (TypingsUtils.FindAttr(basedOnField.GetAttributes(), "Serenity.ComponentModel", "IgnoreAttribute") != null)
                            continue;

                        bool ignored = false;
                        foreach (var annotationType in rowAnnotations)
                        {
                            if (annotationType.PropertyByName.TryGetValue(item.Name, out PropertyDefinition annotation) &&
                                TypingsUtils.FindAttr(annotation.GetAttributes(), "Serenity.ComponentModel", "IgnoreAttribute") != null)
                            {
                                ignored = true;
                                break;
                            }
                        }

                        if (ignored)
                            continue;
                    }

                    var editorTypeAttr = TypingsUtils.FindAttr(item.GetAttributes(), "Serenity.ComponentModel", "EditorTypeAttribute");
                    if (editorTypeAttr == null && basedOnField != null)
                        editorTypeAttr = TypingsUtils.FindAttr(basedOnField.GetAttributes(), "Serenity.ComponentModel", "EditorTypeAttribute");

                    if (editorTypeAttr == null && basedOnRow != null)
                    {
                        foreach (var annotationType in rowAnnotations)
                        {
                            if (!annotationType.PropertyByName.TryGetValue(item.Name, out PropertyDefinition annotation))
                                continue;

                            editorTypeAttr = TypingsUtils.FindAttr(annotation.GetAttributes(),
                                "Serenity.ComponentModel", "EditorTypeAttribute");
                            if (editorTypeAttr != null)
                                break;
                        }
                    }

                    var editorType = GetEditorTypeKeyFrom(item.PropertyType(), basedOnField?.PropertyType(), editorTypeAttr);

                    ExternalType scriptType = null;

                    foreach (var rootNamespace in RootNamespaces)
                    {
                        string wn = rootNamespace + "." + editorType;
                        if ((scriptType = (GetScriptType(wn) ?? GetScriptType(wn + "Editor"))) != null)
                            break;
                    }

                    if (scriptType == null &&
                        (scriptType = (GetScriptType(editorType) ?? GetScriptType(editorType + "Editor"))) == null)
                        continue;

                    var fullName = ShortenFullName(scriptType, codeNamespace, module);
                    var shortName = fullName;
                    if (fullName.StartsWith("Serenity.", StringComparison.Ordinal))
                        shortName = "s." + fullName["Serenity.".Length..];

                    propertyNames.Add(item.Name);
                    propertyTypes.Add(shortName);

                    cw.Indented(item.Name);
                    sb.Append(": ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }
            });

            sb.AppendLine();
            cw.Indented("export class ");
            sb.Append(identifier);

            sb.Append(" extends Serenity.PrefixedContext");
            cw.InBrace(delegate
            {
                cw.Indented("static formKey = '");
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

                            cw.IndentedLine("var s = Serenity;");
                            var typeNumber = new Dictionary<string, int>();
                            foreach (var s in propertyTypes)
                            {
                                if (!typeNumber.ContainsKey(s))
                                {
                                    cw.Indented("var w");
                                    sb.Append(typeNumber.Count);
                                    sb.Append(" = ");
                                    sb.Append(s);
                                    sb.AppendLine(";");
                                    typeNumber[s] = typeNumber.Count;
                                }
                            }
                            sb.AppendLine();

                            cw.Indented("Q.initFormType(");
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
                                    sb.Append(typeNumber[propertyTypes[i]]);
                                    sb.Append("");
                                }

                                sb.AppendLine();
                            });
                            cw.IndentedLine("]);");
                        });
                    });
                }
            });

            RegisterGeneratedType(codeNamespace, identifier, module, typeOnly: false);
        }
    }
}