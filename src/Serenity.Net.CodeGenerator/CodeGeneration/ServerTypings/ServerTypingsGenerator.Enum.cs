namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    private string GetEnumKeyFor(TypeDefinition enumType)
    {
        string enumKey = enumType.FullNameOf();
        var enumKeyAttr = TypingsUtils.FindAttr(enumType.GetAttributes(), "Serenity.ComponentModel", "EnumKeyAttribute");
        if (enumKeyAttr != null &&
            enumKeyAttr.ConstructorArguments().Count >= 1 &&
            enumKeyAttr.ConstructorArguments()[0].Type.FullNameOf() == "System.String")
            enumKey = enumKeyAttr.ConstructorArguments[0].Value as string;
        return enumKey;
    }

    private void GenerateEnum(TypeDefinition enumType, bool module)
    {
        var codeNamespace = GetNamespace(enumType);
        var enumKey = GetEnumKeyFor(enumType);

        cw.Indented("export enum ");
        var identifier = MakeFriendlyName(enumType, codeNamespace, module);
        var fullName = (string.IsNullOrEmpty(codeNamespace) ? "" : codeNamespace + ".") + identifier;
        RegisterGeneratedType(codeNamespace, identifier, module, typeOnly: false);

        cw.InBrace(delegate
        {
            var fields = enumType.FieldsOf().Where(x => x.IsStatic &&
                !x.IsSpecialName() && x.Constant() != null &&
                (!x.HasCustomAttributes() ||
                    (TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "IgnoreAttribute") == null &&
                     TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "ScriptSkipAttribute") == null)));
            fields = fields.OrderBy(x => Convert.ToInt64(x.Constant(), CultureInfo.InvariantCulture));

            var inserted = 0;
            foreach (var field in fields)
            {
                if (inserted > 0)
                    sb.AppendLine(",");

                cw.Indented(field.Name);
                sb.Append(" = ");
                sb.Append(Convert.ToInt64(field.Constant(), CultureInfo.InvariantCulture));
                inserted++;
            }

            sb.AppendLine();
        });

        if (module)
        {
            var decorators = ImportFromCorelib("Decorators");
            cw.Indented($"{decorators}.registerEnumType(");
        }
        else
        {
            cw.Indented("Serenity.Decorators.registerEnumType(");
        }
        sb.Append(enumType.Name);
        sb.Append(", '");
        sb.Append(fullName);
        sb.Append('\'');
        if (enumKey != fullName)
        {
            sb.Append(", '");
            sb.Append(enumKey);
            sb.AppendLine("');");
        }
        else
            sb.AppendLine(");");
    }
}