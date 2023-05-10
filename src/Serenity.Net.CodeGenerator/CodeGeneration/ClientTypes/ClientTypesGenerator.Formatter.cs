namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator : ImportGeneratorBase
{
    private void GenerateFormatter(ExternalType type, string name, string key)
    {
        cw.Indented("public partial class ");
        sb.Append(name);
        sb.AppendLine(" : CustomFormatterAttribute");

        cw.InBrace(delegate
        {
            cw.Indented("public const string Key = \"");
            sb.Append(key ?? type.FullName);
            sb.AppendLine("\";");
            sb.AppendLine();

            cw.Indented("public ");
            sb.Append(name);
            sb.AppendLine("()");
            cw.IndentedLine("    : base(Key)");
            cw.IndentedLine("{");
            cw.IndentedLine("}");

            GenerateOptionMembers(type, skip: null, isWidget: false);
        });
    }

    static readonly string[] FormatterAttributeNames = new[]
    {
        "Serenity.Decorators.registerFormatter",
        "@serenity-is/corelib:Decorators.registerFormatter",
        "Decorators.registerFormatter",
        "registerFormatter"
    };

    private bool IsFormatterType(ExternalType type)
    {
        if (type.IsAbstract == true)
            return false;

        if (type.GenericParameters?.Count > 0)
            return false;

        if (GetAttribute(type, inherited: true, attributeNames: FormatterAttributeNames) != null)
            return true;

        return type.Interfaces != null && type.Interfaces.Any(x =>
            x == "Serenity.ISlickFormatter" ||
            x == "Slick.Formatter" ||
            x == "@serenity-is/corelib:Formatter" ||
            x == "@serenity-is/corelib/slick:Formatter" ||
            x?.EndsWith("ISlickFormatter", StringComparison.Ordinal) == true);
    }

}
