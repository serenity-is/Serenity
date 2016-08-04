using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Reflection;
using Serenity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Serenity.CodeGeneration
{
    public abstract class ServerImportGeneratorBase : ImportGeneratorBase
    {
        private HashSet<Type> visited;
        private Queue<Type> generateQueue;
        protected List<Type> lookupScripts;
        protected HashSet<string> generatedTypes;

        public ServerImportGeneratorBase(params Assembly[] assemblies)
            : base()
        {
            generatedTypes = new HashSet<string>();

            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assembly");

            this.Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; private set; }

        protected virtual bool EnqueueType(Type type)
        {
            if (visited.Contains(type))
                return false;

            visited.Add(type);
            generateQueue.Enqueue(type);
            return true;
        }

        protected virtual void EnqueueTypeMembers(Type type)
        {
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                var pi = member as PropertyInfo;
                var fi = member as FieldInfo;

                if (pi == null && fi == null)
                    continue;

                var memberType = pi != null ? pi.PropertyType : fi.FieldType;
                if (memberType == null)
                    continue;

                var nullableType = Nullable.GetUnderlyingType(memberType);
                if (nullableType != null)
                    memberType = nullableType;

                if (memberType.IsEnum)
                    EnqueueType(memberType);
            }
        }

        protected virtual string GetNamespace(Type type)
        {
            var ns = type.Namespace;
            if (ns.EndsWith(".Entities"))
                return ns.Substring(0, ns.Length - ".Entities".Length);

            if (ns.EndsWith(".Endpoints"))
                return ns.Substring(0, ns.Length - ".Endpoints".Length);

            if (ns.EndsWith(".Forms"))
                return ns.Substring(0, ns.Length - ".Forms".Length);

            if (ns.EndsWith(".Columns"))
                return ns.Substring(0, ns.Length - ".Columns".Length);

            return ns;
        }
        protected virtual string GetControllerIdentifier(Type controller)
        {
            string className = controller.Name;

            if (className.EndsWith("Controller"))
                className = className.Substring(0, className.Length - 10);

            return className + "Service";
        }

        protected override void Reset()
        {
            base.Reset();

            this.cw.BraceOnSameLine = IsTS();
            this.generateQueue = new Queue<Type>();
            this.visited = new HashSet<Type>();
            this.lookupScripts = new List<Type>();
        }

        protected abstract bool IsTS();

        protected override void GenerateAll()
        {
            foreach (var assembly in this.Assemblies)
                foreach (var fromType in assembly.GetTypes())
                {
                    if (fromType.IsAbstract)
                        continue;

                    if (fromType.IsSubclassOf(typeof(ServiceRequest)) ||
                        fromType.IsSubclassOf(typeof(ServiceResponse)) ||
                        fromType.IsSubclassOf(typeof(Row)) ||
                        fromType.GetCustomAttribute<ScriptIncludeAttribute>() != null ||
                        fromType.GetCustomAttribute<FormScriptAttribute>() != null ||
                        fromType.GetCustomAttribute<ColumnsScriptAttribute>() != null ||
                        fromType.IsSubclassOf(typeof(ServiceEndpoint)) ||
                        (fromType.IsSubclassOf(typeof(Controller)) && // backwards compability
                         fromType.Namespace.EndsWith(".Endpoints")))
                    {
                        EnqueueType(fromType);
                        continue;
                    }

                    if (fromType.GetCustomAttribute<LookupScriptAttribute>() != null)
                    {
                        lookupScripts.Add(fromType);
                        continue;
                    }
                }

            while (generateQueue.Count > 0)
            {
                var type = generateQueue.Dequeue();

                if (!this.Assemblies.Contains(type.Assembly))
                    continue;

                var ns = GetNamespace(type);
                bool isController = type.IsSubclassOf(typeof(Controller));

                var identifier = isController ? GetControllerIdentifier(type) : type.Name;
                GenerateCodeFor(type);

                AddFile(RemoveRootNamespace(ns, identifier + (IsTS() ? ".ts" : ".cs")));
            }
        }

        protected abstract void HandleMemberType(Type memberType, string codeNamespace, StringBuilder sb = null);

        public static bool CanHandleType(Type memberType)
        {
            if (memberType.IsInterface)
                return false;

            if (memberType.IsAbstract)
                return false;

            if (typeof(Delegate).IsAssignableFrom(memberType))
                return false;

            return true;
        }

        public virtual string ShortenNamespace(Type type, string codeNamespace)
        {
            string ns = GetNamespace(type);

            if (ns == "Serenity.Services" ||
                ns == "Serenity.ComponentModel")
            {
                if (IsUsingNamespace("Serenity"))
                    return "";
                else
                    return "Serenity";
            }

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith((ns + "."))))
            {
                return "";
            }

            if (IsUsingNamespace(ns))
                return "";

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.');
                if (idx >= 0 && ns.StartsWith(codeNamespace.Substring(0, idx + 1)))
                    return ns.Substring(idx + 1);
            }

            return ns;
        }

        protected virtual bool IsUsingNamespace(string ns)
        {
            return false;
        }

        protected virtual string ShortenNamespace(ExternalType type, string codeNamespace)
        {
            string ns = type.Namespace ?? "";

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith((ns + "."))))
            {
                return "";
            }

            if (IsUsingNamespace(ns))
                return "";

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.');
                if (idx >= 0 && ns.StartsWith(codeNamespace.Substring(0, idx + 1)))
                    return ns.Substring(idx + 1);
            }

            return ns;
        }

        protected virtual string MakeFriendlyName(Type type, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            if (type.IsGenericType)
            {
                var gtd = type.GetGenericTypeDefinition();
                var name = gtd.Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                int i = 0;
                foreach (var argument in type.GetGenericArguments())
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    HandleMemberType(argument, codeNamespace, sb);
                }

                sb.Append(">");

                return name + "`" + type.GetGenericArguments().Length;
            }
            else
            {
                sb.Append(type.Name);
                return type.Name;
            }
        }

        protected virtual void MakeFriendlyReference(Type type, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            string ns;

            if (type.IsGenericType)
            {
                var gtd = type.GetGenericTypeDefinition();
                ns = ShortenNamespace(gtd, codeNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    sb.Append(ns);
                    sb.Append(".");
                }

                var name = gtd.Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                int i = 0;
                foreach (var argument in type.GetGenericArguments())
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    HandleMemberType(argument, codeNamespace);
                }

                sb.Append(">");
                return;
            }

            if (codeNamespace != null)
            {
                ns = ShortenNamespace(type, codeNamespace);
                if (!string.IsNullOrEmpty(ns))
                    sb.Append(ns + "." + type.Name);
                else
                    sb.Append(type.Name);
            }
            else
                sb.Append(type.Name);
        }

        protected Type GetBaseClass(Type type)
        {
            Type derived;

            if (typeof(ListRequest).IsAssignableFrom(type))
                return typeof(ListRequest);
            else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(ListResponse<>), out derived))
                return typeof(ListResponse<>).MakeGenericType(derived.GetGenericArguments()[0]);
            else if (typeof(RetrieveRequest).IsAssignableFrom(type))
                return typeof(RetrieveRequest);
            else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(RetrieveResponse<>), out derived))
                return typeof(RetrieveResponse<>).MakeGenericType(derived.GetGenericArguments()[0]);
            else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(SaveRequest<>), out derived))
                return typeof(SaveRequest<>).MakeGenericType(derived.GetGenericArguments()[0]);
            else if (typeof(DeleteRequest).IsAssignableFrom(type))
                return typeof(DeleteRequest);
            else if (typeof(DeleteResponse).IsAssignableFrom(type))
                return typeof(DeleteResponse);
            else if (typeof(UndeleteRequest).IsAssignableFrom(type))
                return typeof(UndeleteRequest);
            else if (typeof(UndeleteResponse).IsAssignableFrom(type))
                return typeof(UndeleteResponse);
            else if (typeof(SaveResponse).IsAssignableFrom(type))
                return typeof(SaveResponse);
            else if (typeof(ServiceRequest).IsAssignableFrom(type))
                return typeof(ServiceRequest);
            else if (typeof(ServiceResponse).IsAssignableFrom(type))
                return typeof(ServiceResponse);
            else
                return null;
        }

        protected abstract void GenerateCodeFor(Type type);

        protected bool IsPublicServiceMethod(MethodInfo method, out Type requestType, out Type responseType,
            out string requestParam)
        {
            responseType = null;
            requestType = null;
            requestParam = null;

            if (method.GetCustomAttribute<NonActionAttribute>() != null)
                return false;

            if (typeof(Controller).IsSubclassOf(method.DeclaringType))
                return false;

            if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
                return false;

            var parameters = method.GetParameters().Where(x => !x.ParameterType.IsInterface).ToArray();
            if (parameters.Length > 1)
                return false;

            if (parameters.Length == 1)
            {
                requestType = parameters[0].ParameterType;
                if (requestType.IsPrimitive || !CanHandleType(requestType))
                    return false;
            }
            else
                requestType = typeof(ServiceRequest);

            requestParam = parameters.Length == 0 ? "request" : parameters[0].Name;

            responseType = method.ReturnType;
            if (responseType != null &&
                responseType.IsGenericType &&
                responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                responseType = responseType.GenericTypeArguments[0];
            }
            else if (typeof(ActionResult).IsAssignableFrom(responseType))
                return false;
            else if (responseType == typeof(void))
                return false;

            return true;
        }

        protected string GetServiceUrlFromRoute(Type controller)
        {
            var route = controller.GetCustomAttributes<RouteAttribute>().FirstOrDefault();
            string url = route.Template ?? "";

            if (!url.StartsWith("~/"))
            {
                var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
                if (routePrefix != null)
                    url = UriHelper.Combine(routePrefix.Prefix, url);
            }

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

        private string GetMethodName(ExternalMethod method, bool preserveMemberCase)
        {
            string methodName = method.Name;

            var scriptNameAttr = method.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                methodName = scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !method.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                if (methodName == "ID")
                    methodName = "id";
                else methodName = methodName.Substring(0, 1).ToLowerInvariant()
                    + methodName.Substring(1);
            }

            return methodName;
        }
    }
}