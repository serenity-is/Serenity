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

            GenerateOptionMembers(type, skip: null);
        });
    }


}
