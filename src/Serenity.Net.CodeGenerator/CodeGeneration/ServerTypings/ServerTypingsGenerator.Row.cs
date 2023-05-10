#if !ISSOURCEGENERATOR
using Mono.Cecil;
using Mono.Cecil.Cil;
#endif

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GenerateRowType(TypeDefinition type, bool module)
    {
        var codeNamespace = module ? null : GetNamespace(type);

        cw.Indented("export interface ");

        var identifier = MakeFriendlyName(type, codeNamespace, module);

        RegisterGeneratedType(codeNamespace, identifier, module, typeOnly: module);

        cw.InBrace(() =>
        {
            GenerateRowMembers(type, codeNamespace, module);
        });
    }

    private void GenerateRowMembers(TypeDefinition rowType, string codeNamespace, bool module)
    {
        foreach (var property in EnumerateFieldProperties(rowType))
        {
            cw.Indented(property.Name);
            sb.Append("?: ");

            var enumType = TypingsUtils.GetEnumTypeFrom(property.PropertyType());
            if (enumType != null)
            {
                HandleMemberType(enumType, codeNamespace, module);
            }
            else
            {
                HandleMemberType(TypingsUtils.GetNullableUnderlyingType(property.PropertyType()) ?? property.PropertyType(), codeNamespace, module);
            }

            sb.AppendLine(";");
        }
    }

    private static IEnumerable<PropertyDefinition> EnumerateFieldProperties(TypeDefinition rowType)
    {
        do
        {
            var propertyByName = rowType.PropertiesOf().Where(x =>
                TypingsUtils.IsPublicInstanceProperty(x) &&
                (!x.PropertyType().Name.EndsWith("Field", StringComparison.Ordinal) ||
                  x.PropertyType().NamespaceOf() != "Serenity.Data")).ToLookup(x => x.Name);

            var fieldsType = rowType.NestedTypes().FirstOrDefault(x =>
                TypingsUtils.IsSubclassOf(x, "Serenity.Data", "RowFieldsBase"));

            if (fieldsType == null &&
#if ISSOURCEGENERATOR
                rowType is Microsoft.CodeAnalysis.INamedTypeSymbol rowTypeNT &&
                rowTypeNT.TypeParameters.Any())
            {
                var gp = rowTypeNT.TypeParameters.FirstOrDefault(x =>
                    x.ConstraintTypes.Any(c => TypingsUtils.IsSubclassOf(c, "Serenity.Data", "RowFieldsBase")));
                if (gp != null)
                    fieldsType = gp.ConstraintTypes.First(c => TypingsUtils.IsSubclassOf(c, "Serenity.Data", "RowFieldsBase"));
            }
#else
                rowType.HasGenericParameters)
            {
                var gp = rowType.GenericParameters.FirstOrDefault(x => 
                    x.HasConstraints &&
                    x.Constraints.Any(c => TypingsUtils.IsSubclassOf(c.ConstraintType, "Serenity.Data", "RowFieldsBase")));
                if (gp != null)
                    fieldsType = gp.Constraints.First(c => TypingsUtils.IsSubclassOf(c.ConstraintType, "Serenity.Data", "RowFieldsBase"))
                        .ConstraintType.Resolve();
            }
#endif

            if (fieldsType != null)
            {
                foreach (var fieldName in fieldsType.FieldsOf()
                    .Where(x => x.IsPublic())
                    .Select(x => x.Name))
                {
                    var property = propertyByName[fieldName].FirstOrDefault();
                    if (property != null)
                    {
                        var attrs = property.GetAttributes();
                        if (TypingsUtils.FindAttr(attrs, "Serenity.ComponentModel", "ScriptSkipAttribute") is null &&
                            TypingsUtils.FindAttr(attrs, "Serenity.ComponentModel", "IgnoreAttribute") is null)
                            yield return property;
                    }
                }
            }
        }
        while ((rowType = (rowType.BaseType?.Resolve())) != null && 
            rowType.FullNameOf() != "Serenity.Data.Row" &&
            rowType.FullNameOf() != "Serenity.Data.Row`1");
    }

    private static IEnumerable<PropertyDefinition> EnumerateProperties(TypeDefinition rowType)
    {
        do
        {
            foreach (var property in rowType.PropertiesOf().Where(x =>
                TypingsUtils.IsPublicInstanceProperty(x)))
                yield return property;
        }
        while ((rowType = (rowType.BaseType?.Resolve())) != null &&
            rowType.FullNameOf() is not "Serenity.Data.Row" and
                not "Serenity.Data.Row`1");
    }

    private static string ExtractInterfacePropertyFromRow(TypeDefinition rowType, string[] interfaceTypes, 
        string propertyType, string propertyName, string getMethodFullName)
    {
        do
        {
            if (rowType.Interfaces.Any(x => interfaceTypes.Contains(
#if ISSOURCEGENERATOR
                x.FullNameOf()
#else
                x.InterfaceType.FullName
#endif
                )))
            {
#if ISSOURCEGENERATOR
                foreach (var method in rowType.MethodsOf())
                {
                    if (!(method.ExplicitInterfaceImplementations.Any(intfImpl =>
                        interfaceTypes.Any(intfType =>
                            intfImpl.ReceiverType.FullNameOf() == intfType &&
                            intfImpl.Name == "get_" + propertyName)) ||
                        (method.MethodKind == Microsoft.CodeAnalysis.MethodKind.PropertyGet &&
                         method.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public &&
                         method.Name == "get_" + propertyName &&
                         method.ReturnType != null &&
                         method.ReturnType.FullNameOf() == propertyType)))
                    {
                        continue;
                    }

                    foreach (var syntaxRef in method.DeclaringSyntaxReferences)
                    {
                        var syntax = syntaxRef.GetSyntax();

                        foreach (var memberAccess in syntax.DescendantNodes()
                            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MemberAccessExpressionSyntax>())
                        {
                            if (memberAccess.Expression is not Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax idNameLeft)
                                continue;

                            if (idNameLeft.Identifier.Text != "fields")
                                continue;

                            if (memberAccess.Name is not Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax idNameRight)
                                continue;

                            return idNameRight.Identifier.Text;
                        }
                    }
                }
#else
                var name = rowType.Methods.Where(x =>
                        x.Overrides.Any(z => z.FullName == getMethodFullName) ||
                        (x.IsSpecialName && x.Name == "get_" + propertyName && x.ReturnType != null && x.ReturnType.FullName == propertyType))
                    .SelectMany(x => x.Body.Instructions.Where(z =>
                        z.OpCode == OpCodes.Ldfld &&
                        z.Operand is FieldReference &&
                        TypingsUtils.IsSubclassOf((z.Operand as FieldReference).DeclaringType, "Serenity.Data", "RowFieldsBase"))
                        .Select(z => (z.Operand as FieldReference).Name))
                    .FirstOrDefault();

                if (name != null)
                    return name;
#endif
            }
        }
        while ((rowType = (rowType.BaseType?.Resolve())) != null && 
            rowType.FullNameOf() != "Serenity.Data.Row" &&
            rowType.FullNameOf() != "Serenity.Data.Row`1");

        return null;
    }

    private static string DetermineModuleIdentifier(TypeDefinition rowType)
    {
        var moduleAttr = TypingsUtils.GetAttr(rowType, "Serenity.ComponentModel", "ModuleAttribute");
        if (moduleAttr != null)
            return moduleAttr.ConstructorArguments[0].Value as string;

        var ns = rowType.NamespaceOf() ?? "";

        if (ns.EndsWith(".Entities", StringComparison.Ordinal))
            ns = ns[0..^9];

        var idx = ns.IndexOf(".", StringComparison.Ordinal);
        if (idx >= 0)
            ns = ns[(idx + 1)..];

        return ns;
    }

    private static string DetermineRowIdentifier(TypeDefinition rowType)
    {
        var name = rowType.Name;
        if (name.EndsWith("Row", StringComparison.Ordinal))
            name = name[0..^3];

        var moduleIdentifier = DetermineModuleIdentifier(rowType);
        return string.IsNullOrEmpty(moduleIdentifier) ? name : 
            moduleIdentifier + "." + name;
    }

    private static string DetermineLocalTextPrefix(TypeDefinition rowType)
    {
        string localTextPrefix = null;

        var fieldsType = rowType.NestedTypes().FirstOrDefault(x =>
                TypingsUtils.IsSubclassOf(x, "Serenity.Data", "RowFieldsBase"));

        if (fieldsType != null)
        {
#if ISSOURCEGENERATOR
            foreach (var ctor in fieldsType.MethodsOf().Where(x => x.IsConstructor()))
            {
                foreach (var syntaxRef in ctor.DeclaringSyntaxReferences)
                {
                    var syntax = syntaxRef.GetSyntax();
                    foreach (var assignment in syntax.DescendantNodes()
                        .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax>())
                    {
                        if (assignment.Left is not Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax idLeft ||
                            idLeft.Identifier.Text != "LocalTextPrefix")
                            continue;

                        if (assignment.Right is not Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpr)
                            continue;

                        var text = literalExpr.ToString();
                        if (text.Length < 2 &&
                            (text[0] != '"' ||
                             text[^1] != '"'))
                            continue;

                        return Newtonsoft.Json.JsonConvert.DeserializeObject<string>(text);
                    }
                }
            }
#else
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
#endif
        }

        var ltp = TypingsUtils.GetAttr(rowType, "Serenity.ComponentModel", "LocalTextPrefixAttribute");
        if (ltp != null)
        {
            localTextPrefix = ltp.ConstructorArguments[0].Value as string;
            if (!string.IsNullOrEmpty(localTextPrefix))
                return localTextPrefix;
        }

        return DetermineRowIdentifier(rowType);
    }

    private static string DeterminePermission(TypeDefinition rowType, params string[] attributeNames)
    {
        CustomAttribute permissionAttr = null;
        foreach (var attributeName in attributeNames)
        {
            permissionAttr = TypingsUtils.GetAttr(rowType, "Serenity.Data", attributeName + "PermissionAttribute");
            if (permissionAttr != null)
                break;
        }

        if (permissionAttr == null)
            return null;

#if ISSOURCEGENERATOR
        return string.Join(":", permissionAttr.ConstructorArguments.Where(x => (x.Value as string) != null)
            .Select(x => x.Value as string));

#else
        return string.Join(":", permissionAttr.ConstructorArguments.Where(x => (x.Value as string) != null || 
            (x.Value is Mono.Cecil.CustomAttributeArgument))
            .Select(x => (x.Value as string) ?? (((Mono.Cecil.CustomAttributeArgument)x.Value).Value.ToString())));
#endif
    }

    private static string AutoLookupKeyFor(TypeDefinition type)
    {
        string module;
        var moduleAttr = TypingsUtils.GetAttr(type,
            "Serenity.ComponentModel", "ModuleAttribute");
        if (moduleAttr != null)
        {
            if (moduleAttr.ConstructorArguments().Count == 1 &&
                moduleAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.String")
                module = moduleAttr.ConstructorArguments[0].Value as string;
            else
                module = null;
        }
        else
        {
            module = type.NamespaceOf() ?? "";

            if (module.EndsWith(".Entities", StringComparison.Ordinal))
                module = module[0..^9];
            else if (module.EndsWith(".Scripts", StringComparison.Ordinal))
                module = module[0..^8];
            else if (module.EndsWith(".Lookups", StringComparison.Ordinal))
                module = module[0..^8];

            var idx = module.IndexOf(".", StringComparison.Ordinal);
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        if (name.EndsWith("Row", StringComparison.Ordinal))
            name = name[0..^3];
        else if (name.EndsWith("Lookup", StringComparison.Ordinal))
            name = name[0..^6];

        return string.IsNullOrEmpty(module) ? name :
            module + "." + name;
    }

    public string DetermineLookupKey(TypeDefinition rowType)
    {
        var lookupAttr = TypingsUtils.GetAttr(rowType, 
            "Serenity.ComponentModel", "LookupScriptAttribute");

        TypeDefinition autoFrom = rowType;
        if (lookupAttr == null)
        {
            var script = lookupScripts.FirstOrDefault(x =>
#if ISSOURCEGENERATOR
                x.BaseType is not null &&
                x.BaseType
#else
                x.BaseType is GenericInstanceType &&
                (x.BaseType as GenericInstanceType)
#endif
                    .GenericArguments().Any(z =>
                    z.Name == rowType.Name && z.NamespaceOf() == rowType.NamespaceOf()) &&
                DetermineLookupKey(x) == AutoLookupKeyFor(rowType));

            if (script != null)
            {
                lookupAttr = TypingsUtils.GetAttr(script, "Serenity.ComponentModel",
                    "LookupScriptAttribute");
                autoFrom = script;
            }
        }
        else if (lookupAttr.ConstructorArguments().Count > 0 &&
            lookupAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.Type")
        {
            autoFrom = ((TypeReference)lookupAttr.ConstructorArguments[0].Value).Resolve();
            lookupAttr = TypingsUtils.GetAttr(autoFrom, 
                "Serenity.ComponentModel", "LookupScriptAttribute");
        }

        if (lookupAttr == null)
            return null;

        if (lookupAttr.ConstructorArguments().Count == 1 &&
            lookupAttr.ConstructorArguments[0].Type.FullNameOf() == "System.String")
            return lookupAttr.ConstructorArguments[0].Value as string;

        if (lookupAttr.ConstructorArguments().Count == 1 &&
            lookupAttr.ConstructorArguments[0].Type.FullNameOf() == "System.Type")
        {
            return AutoLookupKeyFor(
                (lookupAttr.ConstructorArguments[0].Value as TypeReference).Resolve());
        }

        if (lookupAttr.ConstructorArguments().Count == 1 &&
            lookupAttr.ConstructorArguments[0].Type.FullNameOf() == "System.Type")
        {
            return AutoLookupKeyFor(
                (lookupAttr.ConstructorArguments[0].Value as TypeReference).Resolve());
        }

        if (lookupAttr.ConstructorArguments().Count == 0)
            return AutoLookupKeyFor(autoFrom);

        return null;
    }

    protected class RowMetadata
    {
        public string IdProperty { get; set; }
        public string NameProperty { get; set; }
        public string IsActiveProperty { get; set; }
        public string IsDeletedProperty { get; set; }
        public string LocalTextPrefix { get; set; }
        public string LookupKey { get; set; }
        public string ReadPermission { get; set; }
        public string DeletePermission { get; set; }
        public string InsertPermission { get; set; }
        public string UpdatePermission { get; set; }
    }

    protected RowMetadata ExtractRowMetadata(TypeDefinition rowType)
    {
        var metadata = new RowMetadata
        {
            IdProperty = ExtractInterfacePropertyFromRow(rowType, new[] { "Serenity.Data.IIdRow" },
                "Serenity.Data.IIdField", "IdField",
                "Serenity.Data.IIdField Serenity.Data.IIdRow::get_IdField()")
        };

        var properties = EnumerateProperties(rowType).ToList();

        metadata.IdProperty ??= properties.FirstOrDefault(x =>
                x.HasCustomAttributes() && TypingsUtils.FindAttr(x.GetAttributes(),
                    "Serenity.Data", "IdPropertyAttribute") != null)?.Name;

        if (metadata.IdProperty == null)
        {
            var identities = properties.Where(x =>
                x.HasCustomAttributes() && TypingsUtils.FindAttr(x.GetAttributes(),
                    "Serenity.Data.Mapping", "IdentityAttribute") != null);

            if (identities.Count() == 1)
                metadata.IdProperty = identities.First().Name;
            else if (!identities.Any())
            {
                var primaryKeys = properties.Where(x =>
                    x.HasCustomAttributes() && TypingsUtils.FindAttr(x.GetAttributes(),
                        "Serenity.Data.Mapping", "PrimaryKeyAttribute") != null);

                if (primaryKeys.Count() == 1)
                    metadata.IdProperty = primaryKeys.First().Name;
            }
        }

        metadata.NameProperty = ExtractInterfacePropertyFromRow(rowType, new[] { "Serenity.Data.INameRow" },
                "Serenity.Data.StringField", "NameField",
                "Serenity.Data.StringField Serenity.Data.INameRow::get_NameField()");

        metadata.NameProperty ??= properties.FirstOrDefault(x =>
                x.HasCustomAttributes() && TypingsUtils.FindAttr(x.GetAttributes(),
                    "Serenity.Data", "NamePropertyAttribute") != null)?.Name;

        metadata.IsActiveProperty = ExtractInterfacePropertyFromRow(rowType,
            new[] { "Serenity.Data.IIsActiveRow", "Serenity.Data.IIsActiveDeletedRow" },
            "Serenity.Data.Int16Field", "IsActiveField",
            "Serenity.Data.Int16Field Serenity.Data.IIsActiveRow::get_IsActiveField()");

        metadata.IsDeletedProperty = ExtractInterfacePropertyFromRow(rowType,
            new[] { "Serenity.Data.IIsDeletedRow", "Serenity.Data.IIsDeletedRow" },
            "Serenity.Data.BooleanField", "IsDeletedField",
            "Serenity.Data.BooleanField Serenity.Data.IIsDeletedRow::get_IsDeletedField()");

        metadata.LookupKey = DetermineLookupKey(rowType);

        metadata.DeletePermission = DeterminePermission(rowType, "Delete", "Modify", "Read");
        metadata.InsertPermission = DeterminePermission(rowType, "Insert", "Modify", "Read");
        metadata.ReadPermission = DeterminePermission(rowType, "Read") ?? "";
        metadata.UpdatePermission = DeterminePermission(rowType, "Update", "Modify", "Read");
        metadata.LocalTextPrefix = DetermineLocalTextPrefix(rowType);

        AddRowTexts(rowType, "Db." + (string.IsNullOrEmpty(metadata.LocalTextPrefix) ? "" : 
            (metadata.LocalTextPrefix + ".")));

        return metadata;
    }

    protected void GenerateRowMetadata(TypeDefinition rowType, RowMetadata meta, bool module)
    { 
        sb.AppendLine();
        cw.Indented($"export {(module ? "abstract class " : "namespace ")}");
        sb.Append(rowType.Name);

        string export = module ? "static readonly " : "export const ";

        static string sq(string s) => s == null ? "null" : s.ToSingleQuoted();
        string dq(string s) => s == null ? "null" : module ? s.ToSingleQuoted() : s.ToDoubleQuoted();

        cw.InBrace(delegate
        {
            if (meta.IdProperty != null)
                cw.IndentedLine($"{export}idProperty = {sq(meta.IdProperty)};");

            if (meta.IsActiveProperty != null)
                cw.IndentedLine($"{export}isActiveProperty = {sq(meta.IsActiveProperty)};");

            if (meta.IsDeletedProperty != null)
                cw.IndentedLine($"{export}isDeletedProperty = {sq(meta.IsDeletedProperty)};");

            if (meta.NameProperty != null)
                cw.IndentedLine($"{export}nameProperty = {sq(meta.NameProperty)};");

            if (!string.IsNullOrEmpty(meta.LocalTextPrefix))
                cw.IndentedLine($"{export}localTextPrefix = {sq(meta.LocalTextPrefix)};");

            if (!string.IsNullOrEmpty(meta.LookupKey))
            {
                cw.IndentedLine($"{export}lookupKey = {sq(meta.LookupKey)};");
                sb.AppendLine();

                if (module)
                {
                    var getLookup = ImportFromQ("getLookup");
                    var getLookupAsync = ImportFromQ("getLookupAsync");
                    cw.IndentedLine("/** @deprecated use getLookupAsync instead */");
                    cw.IndentedLine($"static getLookup() {{ return {getLookup}<{rowType.Name}>({sq(meta.LookupKey)}) }}");
                    cw.IndentedLine($"static async getLookupAsync() {{ return {getLookupAsync}<{rowType.Name}>({sq(meta.LookupKey)}) }}");
                    sb.AppendLine();
                }
                else
                {
                    cw.Indented($"export function getLookup(): Q.Lookup<{rowType.Name}>");
                    cw.InBrace(() => cw.IndentedLine(
                        $"return Q.getLookup<{rowType.Name}>({sq(meta.LookupKey)});"));
                }
            }

            cw.IndentedLine($"{export}deletePermission = {sq(meta.DeletePermission)};");
            cw.IndentedLine($"{export}insertPermission = {sq(meta.InsertPermission)};");
            cw.IndentedLine($"{export}readPermission = {sq(meta.ReadPermission)};");
            cw.IndentedLine($"{export}updatePermission = {sq(meta.UpdatePermission)};");
            sb.AppendLine();

            if (module)
            {
                var fieldsProxy = ImportFromQ("fieldsProxy");
                cw.IndentedLine($"static readonly Fields = {fieldsProxy}<{rowType.Name}>();");
            }
            else
            {
                cw.Indented(module ? "static readonly Fields =" : "export declare const enum Fields");

                cw.InBrace(delegate
                {
                    var inserted = 0;
                    foreach (var property in EnumerateFieldProperties(rowType))
                    {
                        if (inserted > 0)
                            sb.AppendLine(",");

                        cw.Indented($"{property.Name}{(module ? ": " : " = ")}{dq(property.Name)}");

                        inserted++;
                    }

                    sb.AppendLine();
                });
            }
        });
    }
}