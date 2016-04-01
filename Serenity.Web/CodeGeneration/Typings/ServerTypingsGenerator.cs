using Newtonsoft.Json;
using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Reflection;
using Serenity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Serenity.CodeGeneration
{
    public class ServerTypingsGenerator
    {
        private StringBuilder sb;
        private CodeWriter cw;
        private HashSet<Type> visited;
        private Queue<Type> generateQueue;
        private List<Type> lookupScripts;
        private Dictionary<string, ExternalType> ssTypes;
        private Dictionary<string, ExternalType> ssTypeMapping;
        private HashSet<string> tsGenerated;
        private Dictionary<string, ExternalType> tsTypes;

        public ServerTypingsGenerator(params Assembly[] assemblies)
        {
            RootNamespaces = new HashSet<string>
            {
                "Serenity"
            };

            ssTypes = new Dictionary<string, ExternalType>();
            ssTypeMapping = new Dictionary<string, ExternalType>();
            tsGenerated = new HashSet<string>();
            tsTypes = new Dictionary<string, ExternalType>();

            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assembly");

            this.Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; private set; }
        public HashSet<string> RootNamespaces { get; private set; }

        private bool EnqueueType(Type type)
        {
            if (visited.Contains(type))
                return false;

            visited.Add(type);
            generateQueue.Enqueue(type);
            return true;
        }

        public void AddSSType(ExternalType type)
        {
            type.Origin = ExternalTypeOrigin.SS;

            var oldFullName = type.FullName;
            var ignoreNS = type.Attributes.FirstOrDefault(x => 
                x.Type == "System.Runtime.CompilerServices.IgnoreNamespaceAttribute");

            if (ignoreNS != null)
                type.Namespace = "";

            var scriptNS = type.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNamespaceAttribute");

            if (scriptNS != null)
                type.Namespace = scriptNS.Arguments[0].Value as string;

            var scriptName = type.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptName != null)
                type.Name = scriptName.Arguments[0].Value as string;

            if (oldFullName != type.FullName)
                ssTypeMapping[oldFullName] = type;

            ssTypes[type.FullName] = type;
        }

        public void AddTSType(ExternalType type)
        {
            type.Origin = ExternalTypeOrigin.TS;
            tsTypes[type.FullName] = type;
        }

        private void EnqueueTypeMembers(Type type)
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

        private static string GetNamespace(Type type)
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

        private string GetControllerIdentifier(Type controller)
        {
            string className = controller.Name;

            if (className.EndsWith("Controller"))
                className = className.Substring(0, className.Length - 10);

            return className + "Service";
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            this.sb = new StringBuilder(4096);
            this.cw = new CodeWriter(sb, 4);
            this.cw.BraceOnSameLine = true;

            this.generateQueue = new Queue<Type>();
            this.visited = new HashSet<Type>();
            this.lookupScripts = new List<Type>();

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

            SortedDictionary<string, string> generatedCode = new SortedDictionary<string, string>();
            while (generateQueue.Count > 0)
            {
                var type = generateQueue.Dequeue();

                if (!this.Assemblies.Contains(type.Assembly))
                    continue;

                var ns = GetNamespace(type);
                bool isController = type.IsSubclassOf(typeof(Controller));

                var identifier = isController ? GetControllerIdentifier(type) : type.Name;
                var filename = ns + "." + identifier + ".ts";

                foreach (var rn in RootNamespaces)
                {
                    if (filename.StartsWith(rn + "."))
                    {
                        filename = filename.Substring(rn.Length + 1);
                        break;
                    }
                }

                GenerateCodeFor(type);

                generatedCode[filename] = sb.ToString();
                sb.Clear();
            }

            GenerateSSDeclarations();
            generatedCode["SSDeclarations.ts"] = sb.ToString();

            return generatedCode;
        }

        private void HandleMemberType(Type memberType, string codeNamespace)
        {
            HandleMemberType(sb, memberType, codeNamespace, t => EnqueueType(t));
        }

        public static void HandleMemberType(StringBuilder code, Type memberType, string codeNamespace,
            Action<Type> enqueueType = null)
        {
            if (memberType == typeof(String))
            {
                code.Append("string");
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(memberType);
            if (nullableType != null)
                memberType = nullableType;

            if (memberType == typeof(Int16) ||
                memberType == typeof(Int32) ||
                memberType == typeof(Int64) ||
                memberType == typeof(UInt16) ||
                memberType == typeof(UInt32) ||
                memberType == typeof(UInt64) ||
                memberType == typeof(Single) ||
                memberType == typeof(Double) ||
                memberType == typeof(Decimal))
            {
                code.Append("number");
                return;
            }

            if (memberType == typeof(Boolean))
            {
                code.Append("boolean");
                return;
            }

            if (memberType == typeof(TimeSpan))
            {
                code.Append("string");
                return;
            }

            if (memberType == typeof(DateTime?) || memberType == typeof(DateTime) ||
                memberType == typeof(TimeSpan) || memberType == typeof(TimeSpan?))
            {
                code.Append("string"); // for now transfer datetime as string, as its ISO formatted
                return;
            }

            if (memberType == typeof(SortBy[]))
            {
                code.Append("string[]");
                return;
            }

            if (memberType == typeof(Stream))
            {
                code.Append("number[]");
                return;
            }

            if (memberType == typeof(Object))
            {
                code.Append("any");
                return;
            }

            if (memberType.IsArray)
            {
                HandleMemberType(code, memberType.GetElementType(), codeNamespace, enqueueType);
                code.Append("[]");
                return;
            }

            if (memberType.IsGenericType &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) ||
                memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, enqueueType);
                code.Append("[]");
                return;
            }

            if (memberType.IsGenericType &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                code.Append("{ [key: ");
                HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, enqueueType);
                code.Append("]: ");
                HandleMemberType(code, memberType.GenericTypeArguments[1], codeNamespace, enqueueType);
                code.Append(" }");
                return;
            }

            if (enqueueType != null)
                enqueueType(memberType);

            MakeFriendlyReference(code, memberType, codeNamespace, enqueueType);
        }

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

        public static string ShortenNamespace(Type type, string codeNamespace)
        {
            string ns = GetNamespace(type);

            if (ns == "Serenity.Services")
                return "Serenity";

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith((ns + "."))))
            {
                return "";
            }

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.');
                if (idx >= 0 && ns.StartsWith(codeNamespace.Substring(0, idx + 1)))
                    return ns.Substring(idx + 1);
            }

            return ns;
        }

        public static string ShortenFullName(ExternalType type, string codeNamespace)
        {
            if (type.FullName == "Serenity.Widget")
                return "Serenity.Widget<any>";

            var ns = ShortenNamespace(type, codeNamespace);
            if (!string.IsNullOrEmpty(ns))
                return ns + "." + type.Name;
            else
                return type.Name;
        }

        public static string ShortenNamespace(ExternalType type, string codeNamespace)
        {
            string ns = type.Namespace ?? "";

            if ((codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith((ns + "."))))
            {
                return "";
            }

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.');
                if (idx >= 0 && ns.StartsWith(codeNamespace.Substring(0, idx + 1)))
                    return ns.Substring(idx + 1);
            }

            return ns;
        }

        public static string MakeFriendlyName(StringBuilder sb, Type type, string codeNamespace,
            Action<Type> enqueueType)
        {
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

                    HandleMemberType(sb, argument, codeNamespace, enqueueType);
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

        public static void MakeFriendlyReference(StringBuilder sb, Type type, string codeNamespace,
            Action<Type> enqueueType)
        {
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

                    HandleMemberType(sb, argument, codeNamespace, enqueueType);
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


        private void GenerateEnum(Type enumType)
        {
            var codeNamespace = GetNamespace(enumType);
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            cw.Indented("export enum ");
            var generatedName = MakeFriendlyName(sb, enumType, codeNamespace, enqueueType: (t) => EnqueueType(t));
            tsGenerated.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

            cw.InBrace(delegate
            {
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                int i = 0;
                foreach (var name in names)
                {
                    if (i > 0)
                        sb.AppendLine(",");

                    cw.Indented(name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt32(((IList)values)[i]));
                    i++;
                }

                sb.AppendLine();
            });

            cw.Indented("Serenity.Decorators.addAttribute(");
            sb.Append(enumType.Name);
            sb.Append(", new Serenity.EnumKeyAttribute('");
            sb.Append(enumKey);
            sb.AppendLine("'));");
        }

        private void GenerateRowMetadata(Type rowType)
        {
            Row row = (Row)rowType.GetInstance();

            var idRow = row as IIdRow;
            var isActiveRow = row as IIsActiveRow;
            var nameRow = row as INameRow;
            var lookupAttr = rowType.GetCustomAttribute<LookupScriptAttribute>();
            if (lookupAttr == null)
            {
                var script = lookupScripts.FirstOrDefault(x =>
                    x.BaseType != null &&
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericArguments().Any(z => z == rowType));

                if (script != null)
                    lookupAttr = script.GetCustomAttribute<LookupScriptAttribute>();
            }

            sb.AppendLine();
            cw.Indented("export namespace ");
            sb.Append(rowType.Name);

            cw.InBrace(delegate
            {
                bool anyMetadata = false;

                if (idRow != null)
                {
                    cw.Indented("export const idProperty = '");
                    var field = ((Field)idRow.IdField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }


                if (isActiveRow != null)
                {
                    cw.Indented("export const isActiveProperty = '");
                    var field = (isActiveRow.IsActiveField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (nameRow != null)
                {
                    cw.Indented("export const nameProperty = '");
                    var field = (nameRow.NameField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                var localTextPrefix = row.GetFields().LocalTextPrefix;
                if (!string.IsNullOrEmpty(localTextPrefix))
                {
                    cw.Indented("export const localTextPrefix = '");
                    sb.Append(localTextPrefix);
                    sb.AppendLine("';");
                    anyMetadata = true;
                }

                if (lookupAttr != null)
                {
                    cw.Indented("export const lookupKey = '");
                    sb.Append(lookupAttr.Key);
                    sb.AppendLine("';");

                    sb.AppendLine();
                    cw.Indented("export function lookup()");
                    cw.InBrace(delegate
                    {
                        cw.Indented("return Q.getLookup('");
                        sb.Append(lookupAttr.Key);
                        sb.AppendLine("');");
                    });

                    anyMetadata = true;

                }

                if (anyMetadata)
                    sb.AppendLine();

                cw.Indented("export namespace ");
                sb.Append("Fields");

                cw.InBrace(delegate
                {
                    foreach (var field in row.GetFields())
                    {
                        cw.Indented("export declare const ");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.Append(": '");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.AppendLine("';");
                    }
                });

                sb.AppendLine();
                cw.Indented("[");
                int i = 0;
                foreach (var field in row.GetFields())
                {
                    if (i++ > 0)
                        sb.Append(", ");
                    sb.Append("'");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.Append("'");
                }
                sb.AppendLine("].forEach(x => (<any>Fields)[x] = x);");
            });
        }

        private void GenerateRowMembers(Type rowType)
        {
            var codeNamespace = GetNamespace(rowType);

            Row row = (Row)rowType.GetInstance();

            foreach (var field in row.GetFields())
            {
                cw.Indented(field.PropertyName ?? field.Name);
                sb.Append("?: ");
                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType, codeNamespace);
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType, codeNamespace);
                }

                sb.AppendLine(";");
            }
        }

        private Type GetBaseClass(Type type)
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

        private void GenerateCodeFor(Type type)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("namespace ");
            sb.Append(codeNamespace);

            cw.InBrace(delegate
            {
                if (type.IsEnum)
                {
                    GenerateEnum(type);
                    return;
                }

                if (type.IsSubclassOf(typeof(Controller)))
                {
                    GenerateService(type);
                    return;
                }

                var formScriptAttr = type.GetCustomAttribute<FormScriptAttribute>();
                if (formScriptAttr != null)
                {
                    GenerateForm(type, formScriptAttr);
                    return;
                }

                if (type.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                {
                    //GenerateColumns(type);
                    return;
                }

                cw.Indented("export interface ");

                var generatedName = MakeFriendlyName(sb, type, codeNamespace, enqueueType: (t) => EnqueueType(t));
                tsGenerated.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

                var baseClass = GetBaseClass(type);
                if (baseClass != null)
                {
                    sb.Append(" extends ");
                    MakeFriendlyReference(sb, baseClass, GetNamespace(type), t => EnqueueType(t));
                }

                cw.InBrace(delegate
                {
                    if (type.IsSubclassOf(typeof(Row)))
                        GenerateRowMembers(type);
                    else
                    {
                        foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (member.GetCustomAttribute<JsonIgnoreAttribute>(false) != null)
                                continue;

                            if (baseClass != null && member.DeclaringType.IsAssignableFrom(baseClass))
                                continue;

                            var pi = member as PropertyInfo;
                            var fi = member as FieldInfo;
                            if (pi == null && fi == null)
                                continue;

                            var memberType = pi != null ? pi.PropertyType : fi.FieldType;

                            if (!CanHandleType(memberType))
                                continue;

                            var memberName = pi != null ? pi.Name : fi.Name;

                            var jsonProperty = member.GetCustomAttribute<JsonPropertyAttribute>(false);
                            if (jsonProperty != null && !jsonProperty.PropertyName.IsEmptyOrNull())
                                memberName = jsonProperty.PropertyName;

                            cw.Indented(memberName);
                            sb.Append("?: ");
                            HandleMemberType(memberType, codeNamespace);
                            sb.AppendLine();
                        }
                    }
                });

                if (type.IsSubclassOf(typeof(Row)))
                    GenerateRowMetadata(type);
            });
        }

        private void GenerateService(Type type)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export namespace ");
            var identifier = GetControllerIdentifier(type);
            sb.Append(identifier);
            tsGenerated.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + identifier);

            cw.InBrace(delegate
            {
                var serviceUrl = GetServiceUrlFromRoute(type);
                if (serviceUrl == null)
                    serviceUrl = GetNamespace(type).Replace(".", "/");

                cw.Indented("export const baseUrl = '");
                sb.Append(serviceUrl);
                sb.AppendLine("';");
                sb.AppendLine();

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

                    cw.Indented("export declare function ");
                    sb.Append(method.Name);

                    sb.Append("(request: ");
                    MakeFriendlyReference(sb, requestType, codeNamespace, t => EnqueueType(t));

                    sb.Append(", onSuccess?: (response: ");
                    MakeFriendlyReference(sb, responseType, codeNamespace, t => EnqueueType(t));
                    sb.AppendLine(") => void, opt?: Serenity.ServiceOptions<any>): JQueryXHR;");
                }

                sb.AppendLine();
                cw.Indented("export namespace ");
                sb.Append("Methods");
                cw.InBrace(delegate
                {
                    foreach (var methodName in methodNames)
                    {
                        cw.Indented("export declare const ");
                        sb.Append(methodName);
                        sb.Append(": '");
                        sb.Append(serviceUrl);
                        sb.Append("/");
                        sb.Append(methodName);
                        sb.AppendLine("';");
                    }
                });

                sb.AppendLine();
                cw.Indented("[");
                int i = 0;
                foreach (var methodName in methodNames)
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    sb.Append("'");
                    sb.Append(methodName);
                    sb.Append("'");
                }
                sb.AppendLine("].forEach(x => {");
                cw.Block(delegate () {
                    cw.Indented("(<any>");
                    sb.Append(identifier);
                    sb.AppendLine(")[x] = function (r, s, o) { return Q.serviceRequest(baseUrl + '/' + x, r, s, o); };");
                    cw.IndentedLine("(<any>Methods)[x] = baseUrl + '/' + x;");
                });
                cw.IndentedLine("});");
            });
        }

        private bool IsPublicServiceMethod(MethodInfo method, out Type requestType, out Type responseType,
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

        private string GetServiceUrlFromRoute(Type controller)
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

        private ExternalType GetScriptType(string fullName)
        {
            ExternalType type;
            if (tsTypes.TryGetValue(fullName, out type))
                return type;

            if (ssTypes.TryGetValue(fullName, out type))
                return type;

            if (ssTypeMapping.TryGetValue(fullName, out type))
                return type;

            return null;
        }

        private void GenerateForm(Type type, FormScriptAttribute formScriptAttribute)
        {
            var codeNamespace = GetNamespace(type);

            cw.Indented("export class ");
            var generatedName = MakeFriendlyName(sb, type, codeNamespace, (t) => EnqueueType(t));
            tsGenerated.Add((codeNamespace.IsEmptyOrNull() ? "" : codeNamespace + ".") + generatedName);

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
            MakeFriendlyName(sb, type, codeNamespace, (t) => EnqueueType(t));
            sb.Append(" extends Serenity.PrefixedContext");

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
                    initializer.Append("', ");
                    initializer.Append(fullName);
                    initializer.Append("]");

                    cw.Indented(item.Name);
                    sb.AppendLine("();");
                }
            });

            initializer.Append("].forEach(x => ");
            MakeFriendlyName(initializer, type, codeNamespace, (t) => EnqueueType(t));
            initializer.Append(".prototype[<string>x[0]] = function() { return this.w(x[0], x[1]); });");

            sb.AppendLine();
            cw.IndentedLine(initializer.ToString());
        }

        private void SSTypeNameToTS(string typeName, string codeNamespace, string defaultType = "any",
            string[] leaveAsIs = null)
        {
            const string nullable = "System.Nullable<";

            if (typeName.StartsWith(nullable) &&
                typeName.EndsWith(">"))
            {
                typeName = typeName.Substring(nullable.Length, typeName.Length - nullable.Length - 1);
            }

            switch (typeName)
            {
                case "System.Type":
                    sb.Append("Function");
                    return;

                case "System.String":
                    sb.Append("string");
                    return;

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    sb.Append("number");
                    return;

                case "System.Boolean":
                    sb.Append("boolean");
                    return;

                case "System.JsDate":
                    sb.Append("Date");
                    return;

                case "jQueryApi.jQueryObject":
                    sb.Append("JQuery");
                    return;

                case "System.Action":
                    sb.Append("() => void");
                    return;

                default:

                    typeName = FixupSSGenerics(typeName);

                    Action<string> handlePart = part =>
                    {
                        if (leaveAsIs != null &&
                            leaveAsIs.Contains(part))
                            sb.Append(part);
                        else
                            SSTypeNameToTS(part, codeNamespace, "any", leaveAsIs);
                    };

                    if (IsGenericTypeName(typeName))
                    {
                        var parts = SplitGenericArguments(ref typeName);

                        var scriptType = GetScriptType(typeName);
                        if (scriptType == null)
                        {
                            if (parts.Length == 1 &&
                                typeName == "System.Collections.Generic.List`1" ||
                                typeName == "System.Collections.Generic.IList`1")
                            {
                                handlePart(parts[0]);
                                sb.Append("[]");
                                return;
                            }
                            else if (parts.Length > 0 &&
                                typeName.StartsWith("System.Func`"))
                            {
                                int k = 0;
                                sb.Append("(");
                                foreach (var part in parts.Take(parts.Length - 1))
                                {
                                    if (k++ > 0)
                                        sb.Append(", ");

                                    sb.Append("p" + k);
                                    sb.Append(": ");
                                    handlePart(part);
                                }
                                sb.Append(") => ");
                                handlePart(parts.Last());
                                return;
                            }
                            else if (typeName.StartsWith("System.Action`"))
                            {
                                int k = 0;
                                sb.Append("(");
                                foreach (var part in parts)
                                {
                                    if (k++ > 0)
                                        sb.Append(", ");

                                    sb.Append("p" + k);
                                    sb.Append(": ");
                                    handlePart(part);
                                }
                                sb.Append(") => void");
                                return;
                            }
                            else
                            {
                                sb.Append(defaultType);
                                return;
                            }
                        }
                        else
                        {
                            var ns = ShortenNamespace(scriptType, codeNamespace);
                            if (!string.IsNullOrEmpty(ns))
                            {
                                sb.Append(ns);
                                sb.Append(".");
                            }

                            sb.Append(scriptType.Name.Split('`')[0]);
                        }

                        sb.Append("<");
                        int i = 0;
                        foreach (var part in parts)
                        {
                            if (i++ > 0)
                                sb.Append(", ");

                            if (leaveAsIs != null &&
                                leaveAsIs.Contains(part))
                                sb.Append(part);
                            else
                                SSTypeNameToTS(part, codeNamespace, "any", leaveAsIs);
                        }

                        sb.Append(">");
                    }
                    else
                    {
                        var scriptType = GetScriptType(typeName);
                        if (scriptType == null)
                            sb.Append(defaultType);
                        else
                            sb.Append(ShortenFullName(scriptType, codeNamespace));
                    }
                    break;
            }
        }

        private void SSMethodArguments(IEnumerable<ExternalArgument> arguments, string codeNamespace)
        {
            int k = 0;
            foreach (var arg in arguments)
            {
                if (k++ > 0)
                    sb.Append(", ");

                sb.Append(arg.Name);
                if (arg.IsOptional || arg.HasDefault)
                    sb.Append("?");

                sb.Append(": ");

                SSTypeNameToTS(arg.Type, codeNamespace);
            }
        }

        private void SSDeclarationConstructor(ExternalMethod ctor, string codeNamespace)
        {
            cw.Indented("constructor(");
            SSMethodArguments(ctor.Arguments, codeNamespace);
            sb.AppendLine(");");
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

        private void SSDeclarationMethodInternal(ExternalMethod method, string codeNamespace,
            bool isStaticClass, bool preserveMemberCase)
        {
            if (method.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.InlineCodeAttribute"))
                return;

            var methodName = GetMethodName(method, preserveMemberCase);

            if (isStaticClass && method.IsStatic)
            {
                cw.Indented("function ");
                sb.Append(methodName);
            }
            else if (method.IsStatic)
            {
                cw.Indented("static ");
                sb.Append(methodName);
            }
            else
            {
                cw.Indented(methodName);
            }

            sb.Append("(");
            SSMethodArguments(method.Arguments, codeNamespace);
            sb.Append("): ");

            if (method.Type == null ||
                method.Type == "System.Void")
            {
                sb.Append("void");
            }
            else
            {
                SSTypeNameToTS(method.Type, codeNamespace);
            }
            sb.AppendLine(";");
        }

        private void SSDeclarationMethod(ExternalMethod method, string codeNamespace,
            bool isStaticClass, bool preserveMemberCase)
        {
            if (method.IsConstructor || method.IsOverride || method.IsGetter || method.IsSetter)
                return;

            SSDeclarationMethodInternal(method, codeNamespace, isStaticClass, preserveMemberCase);
        }

        private void SSDeclarationProperty(ExternalType type, ExternalProperty prop, string codeNamespace,
            bool isStaticClass, bool isSerializable, bool preserveMemberCase)
        {
            if (string.IsNullOrEmpty(prop.GetMethod) &&
                string.IsNullOrEmpty(prop.SetMethod))
                return;

            string propName = prop.Name;

            var scriptNameAttr = prop.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                propName = scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !prop.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                if (propName == "ID")
                    propName = "id";
                else propName = propName.Substring(0, 1).ToLowerInvariant()
                    + propName.Substring(1);
            }

            if (isSerializable ||
                prop.Attributes.FirstOrDefault(x => 
                    x.Type == "System.Runtime.CompilerServices.IntrinsicPropertyAttribute") != null)
            {
                if (isStaticClass && prop.IsStatic)
                {
                    cw.Indented("let ");
                    sb.Append(propName);
                }
                else if (prop.IsStatic)
                {
                    cw.Indented("static ");
                    sb.Append(propName);
                }
                else
                {
                    cw.Indented(propName);
                }

                sb.Append(": ");
                SSTypeNameToTS(prop.Type, codeNamespace);
                sb.AppendLine(";");
            }
            else
            {
                var getMethod = type.Methods.FirstOrDefault(x => x.Name == prop.GetMethod);

                if (getMethod != null)
                {
                    getMethod.Name = "get_" + propName;
                    SSDeclarationMethodInternal(getMethod, codeNamespace, isStaticClass, preserveMemberCase);
                }

                var setMethod = type.Methods.FirstOrDefault(x => x.Name == prop.SetMethod);
                if (setMethod != null)
                {
                    setMethod.Name = "set_" + propName;
                    SSDeclarationMethodInternal(setMethod, codeNamespace, isStaticClass, preserveMemberCase);
                }
            }
        }

        private void SSDeclarationField(ExternalType type, ExternalMember field, string codeNamespace,
            bool isStaticClass, bool isSerializable, bool preserveMemberCase)
        {
            string fieldName = field.Name;

            var scriptNameAttr = field.Attributes.FirstOrDefault(x =>
                x.Type == "System.Runtime.CompilerServices.ScriptNameAttribute");

            if (scriptNameAttr != null)
                fieldName = scriptNameAttr.Arguments[0].Value as string;
            else if (!preserveMemberCase && !field.Attributes.Any(x =>
                    x.Type == "System.Runtime.CompilerServices.PreserveCaseAttribute"))
            {
                if (fieldName == "ID")
                    fieldName = "id";
                else fieldName = fieldName.Substring(0, 1).ToLowerInvariant()
                    + fieldName.Substring(1);
            }

            if (isStaticClass && field.IsStatic)
            {
                cw.Indented("let ");
                sb.Append(fieldName);
            }
            else if (field.IsStatic)
            {
                cw.Indented("static ");
                sb.Append(fieldName);
            }
            else
            {
                cw.Indented(fieldName);
            }

            sb.Append(": ");
            SSTypeNameToTS(field.Type, codeNamespace);
            sb.AppendLine(";");
        }

        private bool IsGenericTypeName(string typeName)
        {
            return typeName.IndexOf("`") >= 0;
        }

        private string[] SplitGenericArguments(ref string typeName)
        {
            if (!IsGenericTypeName(typeName))
                return new string[0];

            var pos = typeName.IndexOf("<");
            var last = typeName.LastIndexOf(">");
            if (pos >= 0 && last > pos)
            {
                char[] c = typeName.Substring(pos + 1, last - pos - 1).ToCharArray();
                typeName = typeName.Substring(0, pos);

                int nestingLevel = 0;
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == '<')
                        nestingLevel++;
                    else if (c[i] == '>')
                        nestingLevel--;
                    else if ((c[i] == ',') && (nestingLevel == 0))
                        c[i] = '€';
                }

                return new string(c).Split(new char[] { '€' });
            }
            else
                return new string[0];
        }

        private string FixupSSGenerics(string typeName)
        {
            if (typeName == "Serenity.TemplatedDialog")
                typeName = "Serenity.TemplatedDialog`1<System.Object>";
            else if (typeName == "Serenity.EntityDialog")
                typeName = "Serenity.EntityDialog`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.EntityDialog`1<"))
            {
                typeName = typeName.Replace("Serenity.EntityDialog`1<", "Serenity.EntityDialog`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.PropertyDialog")
                typeName = "Serenity.PropertyDialog`2<System.Object, System.Object>";
            else if (typeName.StartsWith("PropertyDialog`1<"))
            {
                typeName = typeName.Replace("Serenity.PropertyDialog`1<", "Serenity.PropertyDialog`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.EntityGrid")
                typeName = "Serenity.EntityGrid`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.EntityGrid`1<"))
            {
                typeName = typeName.Replace("Serenity.EntityGrid`1<", "Serenity.EntityGrid`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.PropertyPanel")
                typeName = "Serenity.PropertyPanel`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.PropertyPanel`1<"))
            {
                typeName = typeName.Replace("Serenity.PropertyPanel`1<", "Serenity.PropertyPanel`2<");
                typeName = typeName.Substring(0, typeName.Length - 1) + ", System.Object>";
            }
            else if (typeName == "Serenity.CheckTreeEditor")
                typeName = "Serenity.CheckTreeEditor`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.CheckTreeEditor`1<"))
            {
                typeName = typeName.Replace("Serenity.CheckTreeEditor`1<", "Serenity.CheckTreeEditor`2<System.Object, ");
            }
            else if (typeName == "Serenity.LookupEditorBase")
                typeName = "Serenity.LookupEditorBase`2<System.Object, System.Object>";
            else if (typeName.StartsWith("Serenity.LookupEditorBase`1<"))
            {
                typeName = typeName.Replace("Serenity.LookupEditorBase`1<", "Serenity.LookupEditorBase`2<System.Object, ");
            }
            else if (typeName == "Serenity.TemplatedWidget")
                typeName = "Serenity.TemplatedWidget`1<System.Object>";
            else if (typeName == "Serenity.ServiceCallOptions")
            {
                typeName = "Serenity.ServiceCallOptions`1<System.Object>";
            }
            else if (typeName == "System.Promise")
            {
                typeName = "System.Promise`1<System.Object>";
            }
            return typeName;
        }

        private void SSDeclarationBaseTypeReference(ExternalType type, string baseType, string codeNamespace)
        {
            if (string.IsNullOrEmpty(baseType))
                return;

            if (baseType == "System.Object")
                return;

            sb.Append(" extends ");

            SSTypeNameToTS(baseType, codeNamespace, "Object",
                type.GenericParameters.Select(x => x.Name).ToArray());
        }

        private void GenerateSSDeclarations()
        {
            var byNamespace = 
                ssTypes.Values.Where(x => 
                    !tsTypes.ContainsKey(x.FullName) &&
                    !tsGenerated.Contains(x.FullName))
                .Where(x => !x.AssemblyName.StartsWith("Serenity.Script"))
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name)
                .ToLookup(x => x.Namespace);

            int i = 0;
            foreach (var item in byNamespace)
            {
                if (i++ > 0)
                    sb.AppendLine();

                cw.Indented("declare namespace ");
                var codeNamespace = item.Key;
                sb.Append(codeNamespace);

                cw.InBrace(delegate
                {
                    int j = 0;
                    foreach (var type in item)
                    {
                        if (j++ > 0)
                            sb.AppendLine();

                        bool isStatic = type.IsAbstract && type.IsSealed;
                        bool isClass = !type.IsInterface && !isStatic;

                        if (type.IsInterface)
                            cw.Indented("interface ");
                        else if (isStatic)
                            cw.Indented("namespace ");
                        else
                            cw.Indented("class ");

                        sb.Append(type.Name.Split('`')[0]);

                        if (isClass && type.GenericParameters.Count > 0)
                        {
                            sb.Append("<");

                            var k = 0;
                            foreach (var arg in type.GenericParameters)
                            {
                                if (k++ > 0)
                                    sb.Append(", ");

                                sb.Append(arg.Name);
                            }
                            sb.Append(">");
                        }

                        if (isClass)
                            SSDeclarationBaseTypeReference(type, type.BaseType, codeNamespace);

                        cw.InBrace(delegate
                        {

                            bool preserveMemberCase = type.Attributes.Any(x =>
                                x.Type == "System.Runtime.CompilerServices.PreserveMemberCaseAttribute");

                            bool isSerializable = type.IsSerializable ||
                                type.Attributes.Any(x =>
                                    x.Type == "System.SerializableAttribute");

                            foreach (var field in type.Fields)
                                SSDeclarationField(type, field, codeNamespace, isStatic, isSerializable, preserveMemberCase);

                            if (isClass)
                            {
                                var ctors = type.Methods.Where(x => x.IsConstructor)
                                    .OrderByDescending(x => x.Arguments.Count);

                                var ctor = ctors.FirstOrDefault();

                                if (ctor != null && ctor.Arguments.Count > 0)
                                    SSDeclarationConstructor(ctor, codeNamespace);
                            }

                            foreach (var method in type.Methods)
                            {
                                SSDeclarationMethod(method, codeNamespace, isStatic, preserveMemberCase);
                            }

                            foreach (var prop in type.Properties)
                            {
                                SSDeclarationProperty(type, prop, codeNamespace, isStatic, isSerializable, preserveMemberCase);
                            }

                        });
                    }
                });
            }
        }
    }
}