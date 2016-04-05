using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public partial class ServerImportsGenerator : ServerImportGeneratorBase
    {
        private void GenerateService(Type type)
        {
            var codeNamespace = GetNamespace(type);

            var identifier = GetControllerIdentifier(type);
            generatedTypes.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);

            cw.IndentedLine("[Imported, PreserveMemberCase]");
            cw.Indented("public partial class ");
            sb.AppendLine(identifier);

            cw.InBrace(delegate
            {
                var serviceUrl = GetServiceUrlFromRoute(type);
                if (serviceUrl == null)
                    serviceUrl = GetNamespace(type).Replace(".", "/");

                cw.Indented("[InlineConstant] public const string BaseUrl = \"");
                sb.Append(serviceUrl);
                sb.AppendLine("\";");

                Type responseType;
                Type requestType;
                string requestParam;

                var methodNames = new List<string>();
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (methodNames.Contains(method.Name))
                        continue;

                    if (!IsPublicServiceMethod(method, out requestType, out responseType, out requestParam))
                        continue;

                    methodNames.Add(method.Name);

                    sb.AppendLine();
                    cw.Indented("[InlineCode(\"Q.serviceRequest(\'");
                    sb.Append(UriHelper.Combine(serviceUrl, method.Name));
                    sb.Append("\', {");
                    sb.Append(requestParam);
                    sb.AppendLine("}, {onSuccess}, {options})\")]");
                    cw.Indented("public static jQueryXmlHttpRequest ");
                    sb.Append(method.Name);

                    sb.Append("(");

                    MakeFriendlyReference(requestType, codeNamespace);

                    sb.Append(' ');
                    sb.Append(requestParam);
                    sb.Append(", Action<");

                    MakeFriendlyReference(responseType, codeNamespace);

                    sb.Append("> onSuccess = null, ServiceCallOptions options = null");
                    sb.AppendLine(")");

                    cw.InBrace(delegate
                    {
                        cw.IndentedLine("return null;");
                    });
                }

                sb.AppendLine();
                cw.IndentedLine("[Imported, PreserveMemberCase]");
                cw.IndentedLine("public static class Methods");
                cw.InBrace(delegate
                {
                    foreach (var method in methodNames)
                    {
                        cw.Indented("[InlineConstant] public const string ");
                        sb.Append(method);
                        sb.Append(" = \"");
                        sb.Append(UriHelper.Combine(serviceUrl, method));
                        sb.AppendLine("\";");
                    }
                });
            });
        }
    }
}