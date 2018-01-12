using Serenity.ComponentModel;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : ServerImportGeneratorBase
    {
        const string requestSuffix = "Request";

        private void GenerateForm(Type type, FormScriptAttribute formScriptAttribute)
        {
            var codeNamespace = GetNamespace(type);

            var identifier = type.Name;
            if (identifier.EndsWith(requestSuffix) &&
                type.IsSubclassOf(typeof(ServiceRequest)))
            {
                identifier = identifier.Substring(0,
                    identifier.Length - requestSuffix.Length) + "Form";
                this.fileIdentifier = identifier;
            }

            cw.Indented("export interface ");
            sb.Append(identifier);

            var propertyNames = new List<string>();
            var propertyTypes = new List<string>();

            cw.InBrace(delegate
            {
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
                    var shortName = fullName;
                    if (fullName.StartsWith("Serenity."))
                        shortName = "s." + fullName.Substring("Serenity.".Length);

                    propertyNames.Add(item.Name);
                    propertyTypes.Add(shortName);

                    cw.Indented(item.Name);
                    sb.Append(": ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }
            });

            sb.AppendLine();
            cw.IndentedLine("import s = Serenity;");
            sb.AppendLine();
            cw.Indented("export class ");
            sb.Append(identifier);

            sb.Append(" extends Serenity.PrefixedContext");
            cw.InBrace(delegate
            {
                cw.Indented("static formKey = '");
                sb.Append(formScriptAttribute.Key);
                sb.AppendLine("';");
                cw.IndentedLine("private static init: boolean;");

                sb.AppendLine();

                cw.Indented("constructor(prefix: string)");
                cw.InBrace(delegate
                {
                    cw.IndentedLine("super(prefix);");
                    sb.AppendLine();
                    cw.Indented("if (!");
                    sb.Append(identifier);
                    sb.Append(".init) ");

                    cw.InBrace(delegate
                    {
                        cw.Indented(identifier);
                        sb.AppendLine(".init = true;");
                        cw.IndentedLine("[");
                        cw.Block(delegate
                        {
                            for (var i = 0; i < propertyNames.Count; i++)
                            {
                                if (i > 0)
                                    sb.AppendLine(",");

                                cw.Indented("['");
                                sb.Append(propertyNames[i]);
                                sb.Append("', ");
                                sb.Append(propertyTypes[i]);
                                sb.Append("]");
                            }

                            sb.AppendLine();
                        });
                        cw.Indented("].forEach(x => Object.defineProperty(");
                        sb.Append(identifier);
                        sb.AppendLine(".prototype,");
                        cw.Block(delegate
                        {
                            cw.IndentedLine("<string>x[0], {");
                            cw.Block(delegate
                            {
                                cw.IndentedLine("get: function () { return this.w(x[0], x[1]); },");
                                cw.IndentedLine("enumerable: true,");
                                cw.IndentedLine("configurable: true");
                            });
                            cw.IndentedLine("}));");
                        });
                    });
                });
            });

            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);
        }
    }
}