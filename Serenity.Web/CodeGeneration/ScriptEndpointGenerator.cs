using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Serenity.Reflection;
using Serenity.Services;

namespace Serenity.CodeGeneration
{
    public class ScriptEndpointGenerator
    {
        public Assembly[] Assemblies { get; private set; }
        public Func<Type, bool> IsEndpoint { get; set; }
        public Func<Type, string> GetNamespace { get; set; }
        public Func<Type, string> GetServiceUrl { get; set; }
        public HashSet<string> RootNamespaces { get; private set; }

        public ScriptEndpointGenerator(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assemblies");

            RootNamespaces = new HashSet<string> { };

            this.Assemblies = assemblies;
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            var endpointCodes = new Dictionary<Type, string>();
            var usedNamespaces = new HashSet<string>();
            var sb = new StringBuilder();
            var cw = new CodeWriter(sb, 4);
            var result = new SortedDictionary<string, string>();

            Func<Type, string> getClassName = (t) => {
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

                if (type.IsAbstract)
                    continue;

                if (this.IsEndpoint != null && !this.IsEndpoint(type))
                    continue;

                var className = getClassName(type);

                string ns = GetNamespace != null ? GetNamespace(type) : type.Namespace;

                cw.Indented("public partial class ");
                sb.Append(className);
                sb.AppendLine("Service");

                var methods = new List<string>();
                string serviceUrl = GetServiceUrl != null ? GetServiceUrl(type) : ns.Replace(".", "/");
                serviceUrl = UriHelper.Combine(serviceUrl, className);
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
                        var parameters = method.GetParameters().Where(x => !x.ParameterType.IsInterface).ToArray();
                        if (parameters.Length > 1)
                        {
                            // tek parametreli olmalı
                            continue;
                        }

                        Type paramType;
                        if (parameters.Length == 1)
                        {
                            paramType = parameters[0].ParameterType;
                            if (paramType.IsPrimitive || !ScriptDtoGenerator.CanHandleType(paramType))
                                continue;
                        }
                        else
                            paramType = typeof(ServiceRequest);

                        var returnType = method.ReturnType;

                        Type responseType = returnType;
                        if (returnType != null &&
                            returnType.IsGenericType &&
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

                        cw.Indented("public static jQueryXmlHttpRequest ");
                        sb.Append(method.Name);
                       
                        sb.Append("(");
                        ScriptDtoGenerator.HandleMemberType(sb, paramType, enqueueType: null);
                        sb.Append(' ');
                        sb.Append(parameters.Length == 0 ? "request" : parameters[0].Name);
                        sb.Append(", Action<");
                        ScriptDtoGenerator.HandleMemberType(sb, responseType, enqueueType: null);
                        sb.Append("> onSuccess, ServiceCallOptions options = null");
                        sb.AppendLine(")");

                        cw.InBrace(delegate 
                        {
                            cw.Indented("return Q.ServiceRequest(");

                            sb.Append("Methods.");
                            sb.Append(method.Name);

                            sb.AppendLine(", request, onSuccess, options);");
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
                {
                    endpointCodes.Add(type, sb.ToString());
                    //usedNamespaces.Add(ns);
                }

                sb.Clear();
            }

            usedNamespaces.Add("jQueryApi");
            usedNamespaces.Add("Serenity");
            usedNamespaces.Add("System");
            usedNamespaces.Add("System.Collections");
            usedNamespaces.Add("System.Collections.Generic");
            usedNamespaces.Add("System.Runtime.CompilerServices");

            var ordered = endpointCodes.Keys.OrderBy(x => GetNamespace != null ? GetNamespace(x) : x.Namespace).ThenBy(x => x.Name);
            var byNameSpace = ordered.ToLookup(x => GetNamespace != null ? GetNamespace(x) : x.Namespace);

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
                        foreach (var nsStr in usedNamespaces)
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
    }
}