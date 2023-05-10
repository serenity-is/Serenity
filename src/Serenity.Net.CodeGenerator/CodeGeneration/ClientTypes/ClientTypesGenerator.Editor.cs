namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator : ImportGeneratorBase
{
    private void GenerateEditor(ExternalType type, string name, string key)
    {
        cw.Indented("public partial class ");
        sb.Append(name);

        bool isLookupEditor = HasBaseType(type, "Serenity.LookupEditorBase`1") ||
            HasBaseType(type, "Serenity.LookupEditorBase", "@serenity-is/corelib:LookupEditorBase", "LookupEditorBase");

        bool isServiceLookupEditor = HasBaseType(type, "Serenity.ServiceLookupEditorBase`1") ||
            HasBaseType(type, "Serenity.ServiceLookupEditorBase", "@serenity-is/corelib:ServiceLookupEditorBase", "ServiceLookupEditorBase");

        sb.Append(" : ");
        sb.AppendLine(isLookupEditor ? "LookupEditorBaseAttribute" : 
            (isServiceLookupEditor ? "ServiceLookupEditorBaseAttribute" : "CustomEditorAttribute"));

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

            GenerateOptionMembers(type, 
                skip: isLookupEditor ? lookupEditorBaseOptions : 
                    (isServiceLookupEditor ? serviceLookupEditorBaseOptions : null),
                isWidget: true);
        });
    }

    static readonly string[] EditorAttributeNames = new[]
    {
        "Serenity.EditorAttribute",
        "@serenity-is/corelib:EditorAttribute",
        "Serenity.ElementAttribute",
        "@serenity-is/corelib:ElementAttribute",
        "Serenity.Decorators.registerEditor",
        "@serenity-is/corelib:Decorators.registerEditor",
        "Decorators.registerEditor",
        "registerEditor",
        "Serenity.Decorators.element",
        "@serenity-is/corelib:Decorators.element",
        "Decorators.element"
    };

    public static readonly string[] EditorBaseClasses = new[]
    {
        "Serenity.Extensions.GridEditorBase",
        "GridEditorBase",
        "@serenity-is/extensions:GridEditorBase",
        "Serenity.LookupEditorBase",
        "LookupEditorBase",
        "@serenity-is/corelib:LookupEditorBase",
        "LookupEditor",
        "@serenity-is/corelib:LookupEditor",
        "ServiceLookupEditor",
        "@serenity-is/corelib:ServiceLookupEditor",
        "ServiceLookupEditorBase",
        "@serenity-is/corelib:ServiceLookupEditorBase",
    };

    //static readonly string[] widgetBaseClasses = new[]
    //{
    //    "Serenity.Widget",
    //    "Serenity.Widget<any>",
    //    "@serenity-is/corelib:Widget",
    //    "@serenity-is/corelib:Widget<any>"
    //};

    private bool IsEditorType(ExternalType type)
    {
        if (type.IsAbstract == true)
            return false;

        if (type.GenericParameters?.Count > 0)
            return false;

        if (HasBaseType(type, EditorBaseClasses))
            return true;

        if (GetAttribute(type, inherited: true, attributeNames: EditorAttributeNames) != null)
            return true;

        return false;
    }
}
