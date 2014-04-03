using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Services
{
    public class ScriptEndpointGenerator
    {
        public Assembly Assembly { get; private set; }
        public Func<Type, bool> IsEndpoint { get; set; }
        public Func<Type, string> GetNamespace { get; set; }
        public Func<Type, string> GetServiceUrl { get; set; }

        public ScriptEndpointGenerator(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.Assembly = assembly;
        }

        public string Generate()
        {
            var endpointCodes = new Dictionary<Type, string>();
            var usedNamespaces = new HashSet<string>();
            var sb = new StringBuilder();
            var cw = new CodeWriter(sb, 4);

            foreach (var type in this.Assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Controller)))
                    continue;

                if (type.IsAbstract)
                    continue;

                if (this.IsEndpoint != null && !this.IsEndpoint(type))
                    continue;

                string className = type.Name;
                if (className.EndsWith("Controller"))
                    className = className.Substring(0, className.Length - 10);

                string ns = GetNamespace != null ? GetNamespace(type) : type.Namespace;

                cw.Indented("public partial class ");
                sb.Append(className);
                sb.AppendLine("Service");

                bool hasAnyMethod = false;
                cw.InBrace(delegate 
                {
                    foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (method.GetCustomAttribute<NonActionAttribute>() != null)
                            continue;

                        if (typeof(Controller).IsSubclassOf(method.DeclaringType))
                            continue;

                        if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
                            continue;

                        // belki burada daha sonra metod listesini de verebiliriz (ayrı bir namespace de?)
                        var parameters = method.GetParameters();
                        if (parameters.Length != 1)
                        {
                            // tek parametreli olmalı
                            continue;
                        }

                        var paramType = parameters[0].ParameterType;
                        if (paramType.IsPrimitive || !ScriptDtoGenerator.CanHandleType(paramType))
                            continue;

                        var returnType = method.ReturnType;

                        Type responseType = returnType;
                        if (returnType != null &&
                            returnType.IsGenericType &&
                            returnType.GetGenericTypeDefinition() == typeof(Result<>))
                        {
                            responseType = returnType.GenericTypeArguments[0];
                        }
                        else if (returnType.IsAssignableFrom(typeof(ActionResult)))
                            continue;

                        if (hasAnyMethod)
                            sb.AppendLine();

                        hasAnyMethod = true;

                        cw.Indented("public static void ");
                        sb.Append(method.Name);
                       
                        sb.Append("(");
                        ScriptDtoGenerator.HandleMemberType(sb, paramType, enqueueType: null);
                        sb.Append(' ');
                        sb.Append(parameters[0].Name);
                        sb.Append(", Action<");
                        ScriptDtoGenerator.HandleMemberType(sb, responseType, enqueueType: null);
                        sb.Append("> onSuccess, ServiceCallOptions options = null");
                        sb.AppendLine(")");

                        cw.InBrace(delegate 
                        {
                            cw.Indented("Q.ServiceRequest(\"");

                            string url = GetServiceUrl != null ? GetServiceUrl(type) : ns.Replace(".", "/");
                            url = UriHelper.Combine(url, UriHelper.Combine(className, method.Name));

                            sb.Append(url);

                            sb.AppendLine("\", request, onSuccess, options);");
                        });
                    }
                });

                if (hasAnyMethod)
                {
                    endpointCodes.Add(type, sb.ToString());
                    usedNamespaces.Add(ns);
                }

                sb.Clear();
            }

            usedNamespaces.Add("Serenity");
            usedNamespaces.Add("System");
            usedNamespaces.Add("System.Collections");
            usedNamespaces.Add("System.Collections.Generic");

            foreach (var ns in usedNamespaces)
            {
                cw.Indented("using ");
                sb.Append(ns);
                sb.AppendLine(";");
            }

            sb.AppendLine();
            var ordered = endpointCodes.Keys.OrderBy(x => GetNamespace != null ? GetNamespace(x) : x.Namespace).ThenBy(x => x.Name);
            var byNameSpace = ordered.ToLookup(x => GetNamespace != null ? GetNamespace(x) : x.Namespace);

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                sb.AppendLine();
                cw.Indented("namespace ");
                sb.AppendLine(ns.Key);
                cw.InBrace(delegate
                {
                    foreach (var type in ns)
                        cw.IndentedMultiLine(endpointCodes[type]);
                });
            }

            return sb.ToString();
        }
    }
}