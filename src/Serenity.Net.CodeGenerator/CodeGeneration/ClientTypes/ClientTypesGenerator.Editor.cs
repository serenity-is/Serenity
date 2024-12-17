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
                    (isServiceLookupEditor ? serviceLookupEditorBaseOptions : null));
        });
    }
}
