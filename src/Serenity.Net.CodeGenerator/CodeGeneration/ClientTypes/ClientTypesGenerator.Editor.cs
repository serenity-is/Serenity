using System;

namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        private void GenerateEditor(ExternalType type, string name)
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
                sb.Append(type.FullName);
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

        private bool IsEditorType(ExternalType type)
        {
            if (type.IsAbstract)
                return false;

            if (type.GenericParameters.Count > 0)
                return false;

            if (!HasBaseType(type, "Serenity.Widget") &&
                !HasBaseType(type, "Serenity.Widget<any>"))
                return false;

            return
                GetAttribute(type, "Serenity.EditorAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.ElementAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.registerEditor", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.editor", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.element", inherited: true) != null ||
                HasBaseType(type, "Serenity.Extensions.GridEditorBase") ||
                HasBaseType(type, "Serenity.LookupEditorBase");
        }
    }
}
