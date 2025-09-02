namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    private void GenerateService(TypeDefinition type, string identifier)
    {
        var codeNamespace = ScriptNamespaceFor(type);

        cw.Indented("export namespace ");
        sb.Append(identifier);
        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);

        cw.InBrace(delegate
        {
            var serviceUrl = GetServiceUrlFromRoute(type);
            serviceUrl ??= ScriptNamespaceFor(type).Replace(".", "/", StringComparison.Ordinal);

            cw.Indented("export const baseUrl = '");
            sb.Append(serviceUrl);
            sb.AppendLine("';");
            sb.AppendLine();

            var methods = type.MethodsOf().Where(method => method.IsPublic() &&
                !method.IsStatic && !method.IsAbstract)
#if ISSOURCEGENERATOR
                    .OrderBy(m => m.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree?.FilePath)
#endif
                    .ToList();

            var methodNames = new List<string>();
            foreach (var method in methods)
            {
                if (methodNames.Contains(method.Name) ||
                    !IsPublicServiceMethod(method, out TypeReference requestType, out TypeReference responseType, out string requestParam))
                    continue;

                methodNames.Add(method.Name);

                cw.Indented("export declare function ");
                sb.Append(method.Name);

                sb.Append("(request: ");
                if (requestType == null)
                {
                    var serviceRequest = ImportFromQ("ServiceRequest");
                    sb.Append(serviceRequest);
                }
                else
                    HandleMemberType(requestType, codeNamespace);

                sb.Append(", onSuccess?: (response: ");
                HandleMemberType(responseType, codeNamespace);
                var serviceOptions = ImportFromQ("ServiceOptions");

                sb.Append($") => void, opt?: {serviceOptions}<any>): PromiseLike<");
                HandleMemberType(responseType, codeNamespace);
                sb.AppendLine(">;");
            }

            sb.AppendLine();
            cw.Indented($"export const ");
            sb.Append("Methods =");

            cw.InBrace(delegate
            {
                var inserted = 0;
                foreach (var methodName in methodNames)
                {
                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented(methodName);
                    sb.Append(": \"");
                    sb.Append(serviceUrl);
                    sb.Append('/');
                    sb.Append(methodName);
                    sb.Append('"');

                    inserted++;
                }

                sb.AppendLine();
            }, endLine: false);

            sb.AppendLine(" as const;");
            sb.AppendLine();

            if (methodNames.Count > 0)
            {
                cw.IndentedLine("[");
                int i = 0;
                foreach (var methodName in methodNames)
                {
                    if (i++ > 0)
                        sb.AppendLine(", ");

                    cw.Indented("    '");
                    sb.Append(methodName);
                    sb.Append('\'');
                }
                if (i > 0)
                    sb.AppendLine();
                cw.IndentedLine("].forEach(x => {");
                cw.Block(delegate ()
                {
                    cw.Indented("(<any>");
                    sb.Append(identifier);
                    sb.AppendLine(")[x] = function (r, s, o) {");
                    var serviceRequest = ImportFromQ("serviceRequest");
                    cw.IndentedLine($"    return {serviceRequest}(baseUrl + '/' + x, r, s, o);");
                    cw.IndentedLine("};");
                });
                cw.IndentedLine("});");
            }
        });
    }
}