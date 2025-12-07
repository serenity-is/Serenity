namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
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
                    AppendMappedType(requestType, codeNamespace);

                sb.Append(", onSuccess?: (response: ");
                AppendMappedType(responseType, codeNamespace);
                var serviceOptions = ImportFromQ("ServiceOptions");

                sb.Append($") => void, opt?: {serviceOptions}<any>): PromiseLike<");
                AppendMappedType(responseType, codeNamespace);
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
                        sb.AppendLine(",");

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

    protected virtual string GetControllerIdentifier(TypeReference controller)
    {
        string className = controller.Name;

        if (className.EndsWith("Controller", StringComparison.Ordinal))
            className = className[0..^10];
        else if (className.EndsWith("Endpoint", StringComparison.Ordinal))
            className = className[0..^8];
        else if (className.EndsWith("Service", StringComparison.Ordinal))
            className = className[0..^7];

        return className + "Service";
    }

    protected static bool IsPublicServiceMethod(MethodDefinition method, out TypeReference requestType, out TypeReference responseType,
        out string requestParam)
    {
        ArgumentExceptionHelper.ThrowIfNull(method);

        responseType = null;
        requestType = null;
        requestParam = null;

        if ((TypingsUtils.FindAttr(method.GetAttributes(), "System.Web.Mvc", "NonActionAttribute") ??
             TypingsUtils.FindAttr(method.GetAttributes(), "Microsoft.AspNetCore.Mvc", "NonActionAttribute") ??
             TypingsUtils.FindAttr(method.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute")) != null)
            return false;

        if (!TypingsUtils.IsSubclassOf(
#if ISSOURCEGENERATOR
                method.ContainingType,
#else
                method.DeclaringType,
#endif
                "System.Web.Mvc", "Controller") &&
            !TypingsUtils.IsSubclassOf(
#if ISSOURCEGENERATOR
                method.ContainingType,
#else
                method.DeclaringType,
#endif
                "Microsoft.AspNetCore.Mvc", "ControllerBase"))
            return false;

#if ISSOURCEGENERATOR
        if ((method.MethodKind == MethodKind.PropertySet ||
             method.MethodKind == MethodKind.PropertyGet) &&
#else
        if (method.IsSpecialName &&
#endif
            (method.Name.StartsWith("set_", StringComparison.Ordinal) || method.Name.StartsWith("get_", StringComparison.Ordinal)))
            return false;

        var parameters = method.Parameters.Where(x =>
#if ISSOURCEGENERATOR
            x.Type.TypeKind != TypeKind.Interface &&
#else
            !x.ParameterType.Resolve().IsInterface &&
#endif
            TypingsUtils.FindAttr(x.GetAttributes(), "Microsoft.AspNetCore.Mvc", "FromServicesAttribute") == null).ToArray();

        if (parameters.Length > 1)
            return false;

        if (parameters.Length == 1)
        {
#if ISSOURCEGENERATOR
            requestType = parameters[0].Type;
#else
            requestType = parameters[0].ParameterType;
#endif
        }
        else
            requestType = null;

        requestParam = parameters.Length == 0 ? "request" : parameters[0].Name;

        responseType = method.ReturnType;
        if (responseType != null &&
            responseType.IsGenericInstanceType(out var originalDefinition) &&
            originalDefinition.FullNameOf().StartsWith("System.Threading.Tasks.Task`1", StringComparison.Ordinal))
        { 
#if ISSOURCEGENERATOR
                responseType = (responseType as INamedTypeSymbol).TypeArguments[0];
#else
                responseType = (responseType as GenericInstanceType).GenericArguments[0];
#endif
        }

        if (responseType != null &&
            responseType.IsGenericInstanceType(out originalDefinition) &&
            (originalDefinition.FullNameOf().StartsWith("Serenity.Services.Result`1", StringComparison.Ordinal) ||
             originalDefinition.FullNameOf().StartsWith("Serenity.Services.ResultWithStatus`1", StringComparison.Ordinal)))
        {
#if ISSOURCEGENERATOR
            responseType = (responseType as INamedTypeSymbol).TypeArguments[0];
#else
            responseType = (responseType as GenericInstanceType).GenericArguments[0];
#endif
        }

        if (TypingsUtils.IsOrSubClassOf(responseType, "System.Web.Mvc", "ActionResult") ||
            TypingsUtils.IsAssignableFrom("Microsoft.AspNetCore.Mvc.IActionResult",
#if ISSOURCEGENERATOR
            responseType))
#else
            responseType.Resolve()))
#endif
            return false;
        else if (responseType == null || TypingsUtils.IsVoid(responseType))
            return false;

        return true;
    }

    protected static string GetServiceUrlFromRoute(TypeDefinition controller)
    {
        ArgumentExceptionHelper.ThrowIfNull(controller);

        var route = TypingsUtils.GetAttr(controller, "System.Web.Mvc", "RouteAttribute") ??
            TypingsUtils.GetAttr(controller, "Microsoft.AspNetCore.Mvc", "RouteAttribute");
        string url = route == null ||
            route.ConstructorArguments()?.Count == 0 || route.ConstructorArguments()[0].Value is not string ?
            ("Services/HasNoRoute/" + controller.Name) : (route.ConstructorArguments()[0].Value as string ?? "");

        url = url.Replace("[controller]", controller.Name[..^"Controller".Length]
#if ISSOURCEGENERATOR
            );
#else
            , StringComparison.Ordinal);
#endif
        url = url.Replace("/[action]", ""
#if ISSOURCEGENERATOR
            );
#else
            , StringComparison.Ordinal);
#endif

        if (!url.StartsWith("~/", StringComparison.Ordinal) && !url.StartsWith('/'))
            url = "~/" + url;

        while (true)
        {
            var idx1 = url.IndexOf('{', StringComparison.Ordinal);
            if (idx1 <= 0)
                break;

            var idx2 = url.IndexOf('}', idx1 + 1);
            if (idx2 <= 0)
                break;

            url = url[..idx1] + url[(idx2 + 1)..];
        }

        if (url.StartsWith("~/Services/", StringComparison.OrdinalIgnoreCase))
            url = url["~/Services/".Length..];

        if (url.Length > 1 && url.EndsWith('/'))
            url = url[0..^1];

        return url;
    }
}