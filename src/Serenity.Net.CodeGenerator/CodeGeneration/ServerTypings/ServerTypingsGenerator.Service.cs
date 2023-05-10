namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    private void GenerateService(TypeDefinition type, string identifier, bool module)
    {
        var codeNamespace = GetNamespace(type);

        cw.Indented("export namespace ");
        sb.Append(identifier);
        RegisterGeneratedType(codeNamespace, identifier, module, typeOnly: false);

        cw.InBrace(delegate
        {
            var serviceUrl = GetServiceUrlFromRoute(type);
            serviceUrl ??= GetNamespace(type).Replace(".", "/", StringComparison.Ordinal);

            cw.Indented("export const baseUrl = '");
            sb.Append(serviceUrl);
            sb.AppendLine("';");
            sb.AppendLine();


            var methodNames = new List<string>();
            foreach (var method in type.MethodsOf())
            {
                if (!method.IsPublic() || method.IsStatic || method.IsAbstract)
                    continue;

                if (methodNames.Contains(method.Name))
                    continue;

                if (!IsPublicServiceMethod(method, out TypeReference requestType, out TypeReference responseType, out string requestParam))
                    continue;

                methodNames.Add(method.Name);

                cw.Indented("export declare function ");
                sb.Append(method.Name);

                sb.Append("(request: ");
                if (requestType == null)
                {
                    if (module) 
                    {
                        var serviceRequest = ImportFromQ("ServiceRequest");
                        sb.Append(serviceRequest);
                    }
                    else
                        sb.Append(ShortenFullName("Serenity", "ServiceRequest", codeNamespace, module, "Serenity.Net.Core"));
                }
                else
                    HandleMemberType(requestType, codeNamespace, module);

                sb.Append(", onSuccess?: (response: ");
                HandleMemberType(responseType, codeNamespace, module);
                var serviceOptions = module ? ImportFromQ("ServiceOptions") : "Q.ServiceOptions";

                sb.AppendLine($") => void, opt?: {serviceOptions}<any>): JQueryXHR;");
            }

            sb.AppendLine();
            cw.Indented($"export {(module ? "const" : "declare const enum")} ");
            sb.Append("Methods");
            if (module)
                sb.Append(" =");

            cw.InBrace(delegate
            {
                var inserted = 0;
                foreach (var methodName in methodNames)
                {
                    if (inserted > 0)
                        sb.AppendLine(",");

                    cw.Indented(methodName);
                    sb.Append(module ? ": \"" : " = \"");
                    sb.Append(serviceUrl);
                    sb.Append('/');
                    sb.Append(methodName);
                    sb.Append('"');

                    inserted++;
                }

                sb.AppendLine();
            }, endLine: !module);

            if (module)
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
                    if (module)
                    {
                        var serviceRequest = ImportFromQ("serviceRequest");
                        cw.IndentedLine($"    return {serviceRequest}(baseUrl + '/' + x, r, s, o);");
                    }
                    else
                        cw.IndentedLine("    return Q.serviceRequest(baseUrl + '/' + x, r, s, o);");
                    cw.IndentedLine("};");
                });
                cw.IndentedLine("});");
            }
        });
    }
}