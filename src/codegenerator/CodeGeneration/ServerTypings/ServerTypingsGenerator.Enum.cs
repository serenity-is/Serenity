namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private static string? GetEnumKeyFor(TypeDefinition enumType)
    {
        string? enumKey = enumType.FullNameOf();
        var enumKeyAttr = TypingsUtils.FindAttr(enumType.GetAttributes(), "Serenity.ComponentModel", "EnumKeyAttribute");
        if (enumKeyAttr != null &&
            enumKeyAttr.ConstructorArguments().Count >= 1 &&
            enumKeyAttr.ConstructorArguments()[0].Type?.FullNameOf() == "System.String")
            enumKey = enumKeyAttr.ConstructorArguments[0].Value as string;
        return enumKey;
    }

    private static string? GetEnumIconFor(FieldDefinition field)
    {
        var iconAttr = field.HasCustomAttributes() ?
            TypingsUtils.FindAttr(field.GetAttributes(), "Serenity.ComponentModel", "IconClassAttribute") : null;
        if (iconAttr != null &&
            iconAttr.ConstructorArguments().Count >= 1 &&
            iconAttr.ConstructorArguments()[0].Type?.FullNameOf() == "System.String")
            return iconAttr.ConstructorArguments()[0].Value as string;

        return null;
    }

    private void GenerateEnum(TypeDefinition enumType)
    {
        var codeNamespace = ScriptNamespaceFor(enumType);
        var enumKey = GetEnumKeyFor(enumType);

        var fields = enumType.FieldsOf().Where(x => x.IsStatic &&
            !x.IsSpecialName() && x.Constant() != null &&
            (!x.HasCustomAttributes() ||
                (TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "IgnoreUIFieldAttribute") == null &&
                 TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute") == null)))
            .Select(x => (Field: x, Value: Convert.ToInt64(x.Constant()!, CultureInfo.InvariantCulture), Icon: GetEnumIconFor(x)))
            .OrderBy(x => x.Value)
            .ToList();

        cw.Indented("export enum ");
        var identifier = MakeFriendlyName(enumType, codeNamespace);
        var fullName = (string.IsNullOrEmpty(codeNamespace) ? "" : codeNamespace + ".") + identifier;
        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);

        cw.InBrace(delegate
        {
            var inserted = 0;
            foreach (var field in fields)
            {
                if (inserted > 0)
                    sb.AppendLine(",");

                cw.Indented(field.Field.Name);
                sb.Append(" = ");
                sb.Append(field.Value);
                inserted++;
            }

            sb.AppendLine();
        });

        var registerEnum = ImportFromCorelib("registerEnum");
        cw.Indented($"{registerEnum}(");

        sb.Append(enumType.Name);
        sb.Append(", '");
        sb.Append(fullName);
        sb.Append('\'');

        // the icon classes are only included when a member has one, so the others keep the simpler calls.
        // C# allows aliases with the same value, in which case the icon of the first one is used
        var icons = fields.Where(x => !string.IsNullOrEmpty(x.Icon))
            .GroupBy(x => x.Value)
            .Select(group => (Value: group.Key, Icon: group.First().Icon!))
            .OrderBy(x => x.Value)
            .ToList();
        if (icons.Count == 0)
        {
            if (enumKey != fullName)
            {
                sb.Append(", '");
                sb.Append(enumKey);
                sb.AppendLine("');");
            }
            else
                sb.AppendLine(");");

            return;
        }

        sb.AppendLine(", {");
        cw.Block(delegate
        {
            if (enumKey != fullName)
            {
                cw.Indented("enumKey: '");
                sb.Append(enumKey);
                sb.AppendLine("',");
            }

            cw.IndentedLine("enumIcons: {");
            cw.Block(delegate
            {
                var inserted = 0;
                foreach (var icon in icons)
                {
                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented($"'{icon.Value}': ");
                    // a negative number is not valid as an object key, so the values are quoted
                    sb.Append(icon.Icon.ToSingleQuoted());
                    inserted++;
                }

                sb.AppendLine();
            });
            cw.Indented("}");
            sb.AppendLine();
        });
        cw.IndentedLine("});");
    }
}
