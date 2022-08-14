namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        private void GenerateEditor(ExternalType type, string name, string key)
        {
            cw.Indented("public partial class ");
            sb.Append(name);

            bool isLookupEditor = HasBaseType(type, "Serenity.LookupEditorBase`1") ||
                HasBaseType(type, "Serenity.LookupEditorBase");

            bool isServiceLookupEditor = HasBaseType(type, "Serenity.ServiceLookupEditorBase`1") ||
                HasBaseType(type, "Serenity.ServiceLookupEditorBase");

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

        static readonly string[] editorAttributeNames = new[]
        {
            "Serenity.EditorAttribute",
            "@serenity-is/corelib/serenity:EditorAttribute",
            "@serenity-is/corelib:Serenity.EditorAttribute",
            "Serenity.ElementAttribute",
            "@serenity-is/corelib/serenity:ElementAttribute",
            "@serenity-is/corelib:Serenity.ElementAttribute",
            "Serenity.Decorators.registerEditor",
            "@serenity-is/corelib/serenity:Decorators.registerEditor",
            "@serenity-is/corelib:Serenity.Decorators.registerEditor",
            "Decorators.registerEditor",
            "registerEditor",
            "Serenity.Decorators.element",
            "@serenity-is/corelib/serenity:Decorators.element",
            "@serenity-is/corelib:Serenity.Decorators.element",
            "Decorators.element"
        };

        static readonly string[] editorBaseClasses = new[]
        {
            "Serenity.Extensions.GridEditorBase",
            "GridEditorBase",
            "@serenity-is/extensions:GridEditorBase",
            "Serenity.LookupEditorBase",
            "LookupEditorBase",
            "@serenity-is/corelib/serenity:LookupEditorBase",
            "@serenity-is/corelib:Serenity.LookupEditorBase",
            "LookupEditor",
            "@serenity-is/corelib/serenity:LookupEditor",
            "@serenity-is/corelib:Serenity.LookupEditor",
            "ServiceLookupEditor",
            "@serenity-is/corelib/serenity:ServiceLookupEditor",
            "@serenity-is/corelib:Serenity.ServiceLookupEditor",
            "ServiceLookupEditorBase",
            "@serenity-is/corelib/serenity:ServiceLookupEditorBase",
            "@serenity-is/corelib:Serenity.ServiceLookupEditorBase",
        };

        static readonly string[] widgetBaseClasses = new[]
        {
            "Serenity.Widget",
            "Serenity.Widget<any>",
            "@serenity-is/corelib/serenity:Widget",
            "@serenity-is/corelib/serenity:Widget<any>",
            "@serenity-is/corelib:Serenity.Widget",
            "@serenity-is/corelib:Serenity.Widget<any>"
        };

        private bool IsEditorType(ExternalType type)
        {
            if (type.IsAbstract == true)
                return false;

            if (type.GenericParameters?.Count > 0)
                return false;

            if (!HasBaseType(type, widgetBaseClasses))
                return false;

            return
                GetAttribute(type, inherited: true, attributeNames: editorAttributeNames) != null ||
                HasBaseType(type, editorBaseClasses);
        }
    }
}
