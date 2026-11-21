namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator
{
    /// <summary>
    /// The options of a record view that its attribute has from <c>RecordViewTypeAttribute</c>.
    /// </summary>
    private static readonly HashSet<string> recordViewBaseOptions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Below",
        "Default",
        "HideSwitch",
        "IdField",
        "Placement"
    };

    /// <summary>
    /// The names of the base class of the card views, whose attributes have the card placement.
    /// </summary>
    private static readonly string[] cardViewBaseClasses =
    [
        "CardView",
        "Serenity.Pro.Extensions.CardView",
        "@serenity-is/pro.extensions:CardView"
    ];

    private void GenerateRecordView(ExternalType type, string name, string? key)
    {
        if (!OmitComments)
            cw.IndentedLine($"/// <summary>An attribute that adds the record view <c>{key ?? type.FullName}</c> to a property, with its options.</summary>");
        cw.Indented("public partial class ");
        sb.Append(name);
        sb.AppendLine(" : RecordViewTypeAttribute");

        cw.InBrace(delegate
        {
            if (!OmitComments)
                cw.IndentedLine("/// <summary>The record view type key.</summary>");
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
            if (HasBaseType(type, cardViewBaseClasses))
                cw.IndentedLine("    Placement = RecordViewPlacement.Card;");
            cw.IndentedLine("}");

            GenerateOptionMembers(type, skip: recordViewBaseOptions);
        });
    }
}
