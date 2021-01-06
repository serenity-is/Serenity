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

            sb.AppendLine(isLookupEditor ?
                " : LookupEditorBaseAttribute" : " : CustomEditorAttribute");

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
                    skip: isLookupEditor ? lookupEditorBaseOptions : null,
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

            if (type.AssemblyName != null &&
                type.AssemblyName.StartsWith("Serenity.", StringComparison.Ordinal))
                return false;

            return GetAttribute(type, "Serenity.EditorAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.ElementAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.registerEditor", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.editor", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.element", inherited: true) != null;
        }
    }
}
