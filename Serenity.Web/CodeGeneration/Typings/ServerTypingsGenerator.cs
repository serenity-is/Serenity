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
        private HashSet<string> tsGenerated;
        private Dictionary<string, ExternalType> tsTypes;

        public ServerTypingsGenerator(params Assembly[] assemblies)
        {
            RootNamespaces = new HashSet<string>
            {
                "Serenity"
            };

            ssTypes = new Dictionary<string, ExternalType>();
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

        private string SSTypeNameToTS(string typeName)
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
                    return "Function";

                case "System.String":
                    return "string";

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Single":
                case "System.Double":
                case "System.Decimal":
                    return "number";

                case "System.Boolean":
                    return "boolean";

                case "System.JsDate":
                    return "Date";

                case "jQueryApi.jQueryObject":
                    return "JQuery";

                default:
                    return "any";
            }
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
                sb.Append(item.Key);

                cw.InBrace(delegate
                {
                    int j = 0;
                    foreach (var type in item)
                    {
                        if (type.FullName.Contains("`")) // TODO: Generics
                            continue;

                        if (j++ > 0)
                            sb.AppendLine();

                        cw.Indented(type.IsInterface ? "interface " : "class ");
                        sb.Append(type.Name);
                        cw.InBrace(delegate
                        {
                            if (!type.IsInterface)
                            {
                                var ctors = type.Methods.Where(x => x.IsConstructor)
                                    .OrderByDescending(x => x.Arguments.Count);

                                var ctor = ctors.FirstOrDefault();

                                if (ctor != null)
                                {
                                    cw.Indented("constructor(");

                                    int k = 0;
                                    foreach (var arg in ctor.Arguments)
                                    {
                                        if (k++ > 0)
                                            sb.Append(", ");

                                        sb.Append(arg.Name);
                                        if (arg.IsOptional || arg.HasDefault)
                                            sb.Append("?");

                                        sb.Append(": ");

                                        var argType = GetScriptType(arg.Type);
                                        if (argType == null)
                                        {
                                            sb.Append(SSTypeNameToTS(arg.Type));
                                        }
                                        else
                                            sb.Append(ShortenFullName(argType, item.Key));
                                    }

                                    sb.AppendLine(");");
                                }
                            }
                        });
                    }
                });
            }
        }
    }
}