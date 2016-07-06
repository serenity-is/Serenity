using Serenity.ComponentModel;
using System;
using System.Text;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        private void GenerateForm(Type type, FormScriptAttribute formScriptAttribute)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export class ");
            var generatedName = MakeFriendlyName(type, codeNamespace);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

            sb.Append(" extends Serenity.PrefixedContext");
            cw.InBrace(delegate
            {
                cw.Indented("static formKey = '");
                sb.Append(formScriptAttribute.Key);
                sb.AppendLine("';");
                sb.AppendLine();
            });

            sb.AppendLine();

            cw.Indented("export interface ");
            MakeFriendlyName(type, codeNamespace);

            StringBuilder initializer = new StringBuilder("[");

            cw.InBrace(delegate
            {
                int j = 0;
                foreach (var item in Serenity.PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type))
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

                    var fullName = ShortenFullName(scriptType, codeNamespace);

                    if (j++ > 0)
                        initializer.Append(", ");

                    initializer.Append("['");
                    initializer.Append(item.Name);
                    initializer.Append("', () => ");
                    initializer.Append(fullName);
                    initializer.Append("]");

                    cw.Indented(item.Name);
                    sb.Append(": ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }
            });

            initializer.Append("].forEach(x => Object.defineProperty(");
            MakeFriendlyName(type, codeNamespace, initializer);
            initializer.Append(".prototype, <string>x[0], { get: function () { return this.w(x[0], (x[1] as any)()); }, enumerable: true, configurable: true }));");

            sb.AppendLine();
            cw.IndentedLine(initializer.ToString());
        }
    }
}