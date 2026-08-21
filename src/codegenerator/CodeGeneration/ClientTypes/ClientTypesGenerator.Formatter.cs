namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator
{
    private void GenerateFormatter(ExternalType type, string name, string key)
    {
        if (!OmitComments)
            cw.IndentedLine($"/// <summary>An attribute that sets the formatter type to <c>{key ?? type.FullName}</c> and its options.</summary>");
        cw.Indented("public partial class ");
        sb.Append(name);
        sb.AppendLine(" : CustomFormatterAttribute");

        cw.InBrace(delegate
        {
            if (!OmitComments)
                cw.IndentedLine("/// <summary>The formatter type key.</summary>");
            cw.Indented("public const string Key = \"");
            sb.Append(key ?? type.FullName);
            sb.AppendLine("\";");
            sb.AppendLine();

            if (!OmitComments)
                cw.IndentedLine($"/// <summary>Creates a new instance of the <c>{name}</c> class.</summary>");
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
