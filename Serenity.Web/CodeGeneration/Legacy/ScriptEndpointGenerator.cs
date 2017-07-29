﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Serenity.Reflection;
using Serenity.Services;
using Serenity.Web.Common;
#if ASPNETCORE
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Serenity.CodeGeneration
{
    public class ScriptEndpointGenerator
    {
        public Assembly[] Assemblies { get; private set; }
        public Func<Type, bool> IsEndpoint { get; set; }
        public Func<Type, string> GetNamespace { get; set; }
        /// <summary>
        /// Don't set this anymore, prefer route attributes
        /// </summary>
        public Func<Type, string> GetServiceUrl { get; set; }
        public HashSet<string> RootNamespaces { get; private set; }
        public HashSet<string> UsingNamespaces { get; private set; }

        public ScriptEndpointGenerator(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assemblies");

            RootNamespaces = new HashSet<string> { };
            UsingNamespaces = new HashSet<string>
            {
                "jQueryApi",
                "Serenity",
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.Runtime.CompilerServices"
            };

            this.Assemblies = assemblies;
        }

        private string DoGetNamespace(Type type)
        {
            if (GetNamespace != null)
                return GetNamespace(type);

            return type.Namespace ?? "";
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            var endpointCodes = new Dictionary<Type, string>();
            var sb = new StringBuilder();
            var cw = new CodeWriter(sb, 4);
            var result = new SortedDictionary<string, string>();

            Func<Type, string> getClassName = (t) =>
            {
                string className = t.Name;
                if (className.EndsWith("Controller"))
                    className = className.Substring(0, className.Length - 10);
                return className;
            };

            foreach (var assembly in this.Assemblies)
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(Controller)))
                        continue;

                    if (type.GetIsAbstract())
                        continue;

                    if (this.IsEndpoint != null && !this.IsEndpoint(type))
                        continue;

                    var className = getClassName(type);

                    cw.IndentedLine("[Imported, PreserveMemberCase]");
                    cw.Indented("public partial class ");
                    sb.Append(className);
                    sb.AppendLine("Service");

                    var methods = new List<string>();

                    string serviceUrl;
                    if (GetServiceUrl != null)
                    {
                        serviceUrl = GetServiceUrl(type);
                        serviceUrl = UriHelper.Combine(serviceUrl, className);
                    }
                    else
                    {
                        serviceUrl = GetServiceUrlFromRoute(type);
                        if (serviceUrl == null)
                            serviceUrl = DoGetNamespace(type).Replace(".", "/");
                    }

                    cw.InBrace(delegate
                    {
                        cw.Indented("[InlineConstant] public const string BaseUrl = \"");
                        sb.Append(serviceUrl);
                        sb.AppendLine("\";");
                        sb.AppendLine();

                        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                        {
                            if (method.GetCustomAttribute<NonActionAttribute>() != null)
                                continue;

                            if (typeof(Controller).IsSubclassOf(method.DeclaringType))
                                continue;

                            if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
                                continue;

                            // belki burada daha sonra metod listesini de verebiliriz (ayrı bir namespace de?)
                            var parameters = method.GetParameters().Where(x => !x.ParameterType.GetIsInterface()).ToArray();
                            if (parameters.Length > 1)
                            {
                                // tek parametreli olmalı
                                continue;
                            }

                            Type paramType;
                            if (parameters.Length == 1)
                            {
                                paramType = parameters[0].ParameterType;
                                if (paramType.GetIsPrimitive() || !ScriptDtoGenerator.CanHandleType(paramType))
                                    continue;
                            }
                            else
                                paramType = typeof(ServiceRequest);

                            var returnType = method.ReturnType;

                            Type responseType = returnType;
                            if (returnType != null &&
                                returnType.GetIsGenericType() &&
                                returnType.GetGenericTypeDefinition() == typeof(Result<>))
                            {
                                responseType = returnType.GenericTypeArguments[0];
                            }
                            else if (typeof(ActionResult).IsAssignableFrom(returnType))
                                continue;
                            else if (returnType == typeof(void))
                                continue;

                            if (methods.Count > 0)
                                sb.AppendLine();

                            methods.Add(method.Name);

                            var parameterName = parameters.Length == 0 ? "request" : parameters[0].Name;
                            cw.Indented("[InlineCode(\"Q.serviceRequest(\'");
                            sb.Append(UriHelper.Combine(serviceUrl, method.Name));
                            sb.Append("\', {");
                            sb.Append(parameterName);
                            sb.AppendLine("}, {onSuccess}, {options})\")]");
                            cw.Indented("public static jQueryXmlHttpRequest ");
                            sb.Append(method.Name);

                            sb.Append("(");

                            ScriptDtoGenerator.HandleMemberType(sb, paramType, 
                                codeNamespace: DoGetNamespace(type), 
                                usingNamespaces: UsingNamespaces, 
                                enqueueType: null);

                            sb.Append(' ');
                            sb.Append(parameterName);
                            sb.Append(", Action<");

                            ScriptDtoGenerator.HandleMemberType(sb, responseType,
                                codeNamespace: DoGetNamespace(type),
                                usingNamespaces: UsingNamespaces,
                                enqueueType: null);

                            sb.Append("> onSuccess, ServiceCallOptions options = null");
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
                            foreach (var method in methods)
                            {
                                cw.Indented("[InlineConstant] public const string ");
                                sb.Append(method);
                                sb.Append(" = \"");
                                sb.Append(UriHelper.Combine(serviceUrl, method));
                                sb.AppendLine("\";");
                            }
                        });

                    });

                    if (methods.Count > 0)
                        endpointCodes.Add(type, sb.ToString());

                    sb.Clear();
                }

            var ordered = endpointCodes.Keys.OrderBy(DoGetNamespace).ThenBy(x => x.Name);
            var byNameSpace = ordered.ToLookup(DoGetNamespace);

            sb.Clear();

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                Action<Type> outputType = delegate(Type type)
                {
                    var filename = ns.Key + "." + getClassName(type) + "Service.cs";

                    foreach (var rn in RootNamespaces)
                    {
                        if (filename.StartsWith(rn + "."))
                            filename = filename.Substring(rn.Length + 1);
                    }

                    result.Add(filename, sb.ToString());
                };

                foreach (var type in ns)
                {
                    cw.Indented("namespace ");
                    sb.AppendLine(ns.Key);

                    cw.InBrace(delegate
                    {
                        foreach (var nsStr in UsingNamespaces.ToArray().OrderBy(x => x))
                        {
                            cw.Indented("using ");
                            sb.Append(nsStr);
                            sb.AppendLine(";");
                        }

                        sb.AppendLine();

                        cw.IndentedMultiLine(endpointCodes[type]);
                    });

                    outputType(type);

                    sb.Clear();
                }
            }

            return result;
        }

        private string GetServiceUrlFromRoute(Type controller)
        {
            var route = AttributeReader.GetAttributeWithAssemblyVersionChecking<RouteAttribute>(controller);
            string url = route?.Template ?? "";

#if ASPNETCORE
            url = url.Replace("[controller]", controller.Name.Substring(0, controller.Name.Length - "Controller".Length));
#else
            if (!url.StartsWith("~/"))
            {
                var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
                if (routePrefix != null)
                    url = UriHelper.Combine(routePrefix.Prefix, url);
            }
#endif

            if (!url.StartsWith("~/") && !url.StartsWith("/"))
                url = "~/" + url;

            while (true)
            {
                var idx1 = url.IndexOf('{');
                if (idx1 <= 0)
                    break;

                var idx2 = url.IndexOf("}", idx1 + 1);
                if (idx2 <= 0)
                    break;

                url = url.Substring(0, idx1) + url.Substring(idx2 + 1);
            }

            if (url.StartsWith("~/Services/", StringComparison.OrdinalIgnoreCase))
                url = url.Substring("~/Services/".Length);

            if (url.Length > 1 && url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);

            return url;
        }
    }
}