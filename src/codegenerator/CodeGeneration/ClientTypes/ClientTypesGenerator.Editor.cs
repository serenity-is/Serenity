namespace Serenity.CodeGeneration;

public partial class ClientTypesGenerator
{
    private void GenerateEditor(ExternalType type, string name, string key)
    {
        if (!OmitComments)
            cw.IndentedLine($"/// <summary>An attribute that sets the editor type to <c>{key ?? type.FullName}</c> and its options.</summary>");
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
            if (!OmitComments)
                cw.IndentedLine("/// <summary>The editor type key.</summary>");
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

            GenerateOptionMembers(type, 
                skip: isLookupEditor ? lookupEditorBaseOptions : 
                    (isServiceLookupEditor ? serviceLookupEditorBaseOptions : null));
        });
    }
}
