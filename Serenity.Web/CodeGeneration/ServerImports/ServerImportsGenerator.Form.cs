using Serenity.ComponentModel;
using Serenity.Services;
using System;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        const string requestSuffix = "Request";

        private void GenerateForm(Type type, FormScriptAttribute formScriptAttribute)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("public partial class ");

            var identifier = type.Name;
            if (identifier.EndsWith(requestSuffix) &&
                type.IsSubclassOf(typeof(ServiceRequest)))
            {
                identifier = identifier.Substring(0,
                    identifier.Length - requestSuffix.Length) + "Form";
                this.fileIdentifier = identifier;
            }

            sb.Append(identifier);

            sb.AppendLine(" : PrefixedContext");

            cw.InBrace(delegate
            {
                cw.Indented("[InlineConstant] public const string FormKey = \"");
                sb.Append(formScriptAttribute.Key);
                sb.AppendLine("\";");
                sb.AppendLine();

                cw.Indented("public ");
                sb.Append(identifier);
                sb.AppendLine("(string idPrefix) : base(idPrefix) {}");
                sb.AppendLine();

                foreach (var item in PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type))
                {
                    var editorType = item.EditorType ?? "String";

                    ExternalType scriptType = null;

                    foreach (var rootNamespace in RootNamespaces)
                    {
                        string wn = rootNamespace + "." + editorType;
                        if ((scriptType = (GetScriptType(wn) ?? GetScriptType(wn + "Editor"))) != null)
                            break;
                    }

                    if (scriptType == null &&
                        (scriptType = (GetScriptType(editorType) ?? GetScriptType(editorType + "Editor"))) == null)
                        continue;

                    var fullTypeName = scriptType.FullName;
                    if (type.FullName == "Serenity.Widget")
                        fullTypeName = "Serenity.Widget<any>";

                    var shortTypeName = ShortenFullName(scriptType, codeNamespace);

                    cw.Indented("public ");
                    sb.Append(shortTypeName);
                    sb.Append(" ");
                    sb.Append(item.Name);
                    sb.Append(" { ");
                    sb.Append("[InlineCode(\"{this}.w('");
                    sb.Append(item.Name);
                    sb.Append("', ");
                    sb.Append(fullTypeName);
                    sb.AppendLine(")\")] get; private set; }");
                }
            });
        }
    }
}