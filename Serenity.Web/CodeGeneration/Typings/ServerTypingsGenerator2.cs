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
using System.Text;

namespace Serenity.CodeGeneration
{
    public class ServerTypingsGenerator2
    {
        private StringBuilder sb;
        private CodeWriter cw;
        private HashSet<string> visited;
        private Queue<ExternalType> generateQueue;
        private List<ExternalType> lookupScripts;

        public ServerTypingsGenerator2()
        {
            RootNamespaces = new HashSet<string>
            {
            };
        }

        public Dictionary<string, ExternalType> ServerTypes { get; set; }

        public Dictionary<string, ExternalType> SharpScriptTypes { get; set; }
        public Dictionary<string, ExternalType> TypeScriptTypes { get; set; }

        public HashSet<string> RootNamespaces { get; private set; }

        private bool EnqueueType(string fullName)
        {
            if (visited.Contains(fullName))
                return false;

            ExternalType type;
            if (!ServerTypes.TryGetValue(fullName, out type))
                return false;

            visited.Add(fullName);
            generateQueue.Enqueue(type);
            return true;
        }

        private void EnqueueTypeMembers(ExternalType type)
        {
            if (type.Members == null)
                return;

            foreach (var member in type.Members)
            {
                //if (member.IsEnum)
                //    EnqueueType(member.Type);
            }
        }

        private static string GetNamespace(ExternalType type)
        {
            var ns = type.Namespace;
            if (ns.EndsWith(".Entities"))
                return ns.Substring(0, ns.Length - ".Entities".Length);

            if (ns.EndsWith(".Endpoints"))
                return ns.Substring(0, ns.Length - ".Endpoints".Length);

            return ns;
        }

        private string GetControllerIdentifier(ExternalType controller)
        {
            string className = controller.Name;

            if (className.EndsWith("Controller"))
                className = className.Substring(0, className.Length - 10);

            return className + "Service";
        }

        private bool IsSubclassOf<TType>(ExternalType type)
        {
            return 
                type.Origin == ExternalTypeOrigin.Server &&
                type.BaseClasses != null &&
                type.BaseClasses.Any(x => x == typeof(TType).FullName);
        }

        private bool IsSubclassOf(ExternalType type, string baseType)
        {
            return
                type.Origin == ExternalTypeOrigin.Server &&
                type.BaseClasses != null &&
                type.BaseClasses.Any(x => x == baseType);
        }

        private ExternalAttribute GetAttribute<TAttribute>(ExternalType type)
            where TAttribute: Attribute
        {
            if (type.Origin != ExternalTypeOrigin.Server ||
                type.Attributes == null)
                return null;

            return type.Attributes.FirstOrDefault(x => x.AttributeType ==
                typeof(TAttribute).FullName);
        }

        private ExternalAttribute GetAttribute<TAttribute>(ExternalMember member)
            where TAttribute : Attribute
        {
            if (member.Attributes == null)
                return null;

            return member.Attributes.FirstOrDefault(x => x.AttributeType ==
                typeof(TAttribute).FullName);
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            this.sb = new StringBuilder(4096);
            this.cw = new CodeWriter(sb, 4);
            this.cw.BraceOnSameLine = true;

            this.generateQueue = new Queue<ExternalType>();
            this.visited = new HashSet<string>();
            this.lookupScripts = new List<ExternalType>();

            this.ServerTypes = this.ServerTypes ?? new Dictionary<string, ExternalType>();
            foreach (var type in this.ServerTypes.Values)
                type.Origin = ExternalTypeOrigin.Server;

            this.SharpScriptTypes = this.SharpScriptTypes ?? new Dictionary<string, ExternalType>();
            foreach (var type in this.SharpScriptTypes.Values)
                type.Origin = ExternalTypeOrigin.SharpScript;

            this.TypeScriptTypes = this.TypeScriptTypes ?? new Dictionary<string, ExternalType>();
            foreach (var type in this.TypeScriptTypes.Values)
                type.Origin = ExternalTypeOrigin.TypeScript;

            foreach (var fromType in this.ServerTypes.Values)
            {
                if (fromType.IsAbstract)
                    continue;

                if (IsSubclassOf<ServiceRequest>(fromType) ||
                    IsSubclassOf<ServiceResponse>(fromType) ||
                    IsSubclassOf<Row>(fromType) ||
                    GetAttribute<ScriptIncludeAttribute>(fromType) != null ||
                    IsSubclassOf(fromType, "Serenity.Services.ServiceEndpoint") ||
                    (IsSubclassOf(fromType, "System.Web.Mvc.Controller") && // backwards compability
                        fromType.Namespace.EndsWith(".Endpoints"))) 
                {
                    EnqueueType(fromType.FullName);
                    continue;
                }

                if (GetAttribute<FormScriptAttribute>(fromType) != null ||
                    GetAttribute<ColumnsScriptAttribute>(fromType) != null)
                {
                    EnqueueTypeMembers(fromType);
                    continue;
                }

                if (GetAttribute<LookupScriptAttribute>(fromType) != null)
                {
                    lookupScripts.Add(fromType);
                    continue;
                }
            }

            SortedDictionary<string, string> generatedCode = new SortedDictionary<string, string>();
            while (generateQueue.Count > 0)
            {
                var type = generateQueue.Dequeue();

                if (type.Origin != ExternalTypeOrigin.Server)
                    continue;

                var ns = GetNamespace(type);
                bool isController = IsSubclassOf(type, "System.Web.Mvc.Controller");

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

            return generatedCode;
        }

        private void HandleMemberType(ExternalType memberType, string codeNamespace)
        {
            HandleMemberType(sb, memberType, codeNamespace, t => EnqueueType(t));
        }

        public static void HandleMemberType(StringBuilder code, ExternalType memberType, string codeNamespace,
            Action<string> enqueueType = null)
        {
            if (memberType.FullName == typeof(String).FullName)
            {
                code.Append("string");
                return;
            }

            if (memberType.FullName == typeof(Int16).FullName ||
                memberType.FullName == typeof(Int32).FullName ||
                memberType.FullName == typeof(Int64).FullName ||
                memberType.FullName == typeof(UInt16).FullName ||
                memberType.FullName == typeof(UInt32).FullName ||
                memberType.FullName == typeof(UInt64).FullName ||
                memberType.FullName == typeof(Single).FullName ||
                memberType.FullName == typeof(Double).FullName ||
                memberType.FullName == typeof(Decimal).FullName)
            {
                code.Append("number");
                return;
            }

            if (memberType.FullName == typeof(Boolean).FullName)
            {
                code.Append("boolean");
                return;
            }

            if (memberType.FullName == typeof(TimeSpan).FullName)
            {
                code.Append("string");
                return;
            }

            if (memberType.FullName == typeof(DateTime).FullName ||
                memberType.FullName == typeof(TimeSpan).FullName)
            {
                code.Append("string"); // for now transfer datetime as string, as its ISO formatted
                return;
            }

            if (memberType.FullName == typeof(SortBy[]).FullName)
            {
                code.Append("string[]");
                return;
            }

            if (memberType.FullName == typeof(Stream).FullName)
            {
                code.Append("number[]");
                return;
            }

            if (memberType.FullName == typeof(Object).FullName)
            {
                code.Append("any");
                return;
            }

            //if (memberType.IsArray)
            //{
            //    HandleMemberType(code, memberType.GetElementType(), codeNamespace, enqueueType);
            //    code.Append("[]");
            //    return;
            //}

            //if (memberType.IsGenericType &&
            //    (memberType.GetGenericTypeDefinition() == typeof(List<>) ||
            //    memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            //{
            //    HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, enqueueType);
            //    code.Append("[]");
            //    return;
            //}

            //if (memberType.IsGenericType &&
            //    memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            //{
            //    code.Append("{ [key: ");
            //    HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, enqueueType);
            //    code.Append("]: ");
            //    HandleMemberType(code, memberType.GenericTypeArguments[1], codeNamespace, enqueueType);
            //    code.Append(" }");
            //    return;
            //}

            if (enqueueType != null)
                enqueueType(memberType.FullName);

            MakeFriendlyReference(code, memberType, codeNamespace, enqueueType);
        }

        public static bool CanHandleType(ExternalType memberType)
        {
            if (memberType.IsInterface)
                return false;

            if (memberType.IsAbstract)
                return false;

            //if (typeof(Delegate).IsAssignableFrom(memberType))
            //    return false;

            return true;
        }

        public static string ShortenNamespace(ExternalType type, string codeNamespace)
        {
            string ns = type.Namespace;

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

        public static void MakeFriendlyName(StringBuilder sb, ExternalType type, string codeNamespace,
            Action<string> enqueueType)
        {
            //if (type.IsGenericType)
            //{
            //    var gtd = type.GetGenericTypeDefinition();
            //    var name = gtd.Name;
            //    var idx = name.IndexOf('`');
            //    if (idx >= 0)
            //        name = name.Substring(0, idx);

            //    sb.Append(name);
            //    sb.Append("<");

            //    int i = 0;
            //    foreach (var argument in type.GetGenericArguments())
            //    {
            //        if (i++ > 0)
            //            sb.Append(", ");

            //        HandleMemberType(sb, argument, codeNamespace, enqueueType);
            //    }

            //    sb.Append(">");
            //}
            //else
                sb.Append(type.Name);
        }

        public static void MakeFriendlyReference(StringBuilder sb, ExternalType type, string codeNamespace,
            Action<string> enqueueType)
        {
            string ns;

            //if (type.IsGenericType)
            //{
            //    var gtd = type.GetGenericTypeDefinition();
            //    ns = ShortenNamespace(gtd, codeNamespace);

            //    if (!string.IsNullOrEmpty(ns))
            //    {
            //        sb.Append(ns);
            //        sb.Append(".");
            //    }

            //    var name = gtd.Name;
            //    var idx = name.IndexOf('`');
            //    if (idx >= 0)
            //        name = name.Substring(0, idx);

            //    sb.Append(name);
            //    sb.Append("<");

            //    int i = 0;
            //    foreach (var argument in type.GetGenericArguments())
            //    {
            //        if (i++ > 0)
            //            sb.Append(", ");

            //        HandleMemberType(sb, argument, codeNamespace, enqueueType);
            //    }

            //    sb.Append(">");
            //    return;
            //}

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

        private void GenerateEnum(ExternalType enumType)
        {
            //var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            //cw.Indented("export enum ");
            //sb.Append(enumType.Name);
            //cw.InBrace(delegate
            //{
            //    var names = Enum.GetNames(enumType);
            //    var values = Enum.GetValues(enumType);

            //    int i = 0;
            //    foreach (var name in names)
            //    {
            //        if (i > 0)
            //            sb.AppendLine(",");

            //        cw.Indented(name);
            //        sb.Append(" = ");
            //        sb.Append(Convert.ToInt32(((IList)values)[i]));
            //        i++;
            //    }

            //    sb.AppendLine();
            //});

            //cw.Indented("Serenity.Decorators.addAttribute(");
            //sb.Append(enumType.Name);
            //sb.Append(", new Serenity.EnumKeyAttribute('");
            //sb.Append(enumKey);
            //sb.AppendLine("'));");
        }

        private void GenerateRowMetadata(ExternalType rowType)
        {
            //Row row = (Row)rowType.GetInstance();

            //var idRow = row as IIdRow;
            //var isActiveRow = row as IIsActiveRow;
            //var nameRow = row as INameRow;
            //var lookupAttr = rowType.GetCustomAttribute<LookupScriptAttribute>();
            //if (lookupAttr == null)
            //{
            //    var script = lookupScripts.FirstOrDefault(x =>
            //        x.BaseType != null &&
            //        x.BaseType.IsGenericType &&
            //        x.BaseType.GetGenericArguments().Any(z => z == rowType));

            //    if (script != null)
            //        lookupAttr = script.GetCustomAttribute<LookupScriptAttribute>();
            //}

            //sb.AppendLine();
            //cw.Indented("export namespace ");
            //sb.Append(rowType.Name);

            //cw.InBrace(delegate
            //{
            //    bool anyMetadata = false;

            //    if (idRow != null)
            //    {
            //        cw.Indented("export const idProperty = '");
            //        var field = ((Field)idRow.IdField);
            //        sb.Append(field.PropertyName ?? field.Name);
            //        sb.AppendLine("';");
            //        anyMetadata = true;
            //    }


            //    if (isActiveRow != null)
            //    {
            //        cw.Indented("export const isActiveProperty = '");
            //        var field = (isActiveRow.IsActiveField);
            //        sb.Append(field.PropertyName ?? field.Name);
            //        sb.AppendLine("';");
            //        anyMetadata = true;
            //    }

            //    if (nameRow != null)
            //    {
            //        cw.Indented("export const nameProperty = '");
            //        var field = (nameRow.NameField);
            //        sb.Append(field.PropertyName ?? field.Name);
            //        sb.AppendLine("';");
            //        anyMetadata = true;
            //    }

            //    var localTextPrefix = row.GetFields().LocalTextPrefix;
            //    if (!string.IsNullOrEmpty(localTextPrefix))
            //    {
            //        cw.Indented("export const localTextPrefix = '");
            //        sb.Append(localTextPrefix);
            //        sb.AppendLine("';");
            //        anyMetadata = true;
            //    }

            //    if (lookupAttr != null)
            //    {
            //        cw.Indented("export const lookupKey = '");
            //        sb.Append(lookupAttr.Key);
            //        sb.AppendLine("';");

            //        sb.AppendLine();
            //        cw.Indented("export function lookup()");
            //        cw.InBrace(delegate
            //        {
            //            cw.Indented("return Q.getLookup('");
            //            sb.Append(lookupAttr.Key);
            //            sb.AppendLine("');");
            //        });

            //        anyMetadata = true;

            //    }

            //    if (anyMetadata)
            //        sb.AppendLine();

            //    cw.Indented("export namespace ");
            //    sb.Append("Fields");

            //    cw.InBrace(delegate
            //    {
            //        foreach (var field in row.GetFields())
            //        {
            //            cw.Indented("export declare const ");
            //            sb.Append(field.PropertyName ?? field.Name);
            //            sb.Append(": '");
            //            sb.Append(field.PropertyName ?? field.Name);
            //            sb.AppendLine("';");
            //        }
            //    });

            //    sb.AppendLine();
            //    cw.Indented("[");
            //    int i = 0;
            //    foreach (var field in row.GetFields())
            //    {
            //        if (i++ > 0)
            //            sb.Append(", ");
            //        sb.Append("'");
            //        sb.Append(field.PropertyName ?? field.Name);
            //        sb.Append("'");
            //    }
            //    sb.AppendLine("].forEach(x => (<any>Fields)[x] = x);");
            //});
        }

        private void GenerateRowMembers(ExternalType rowType)
        {
            var codeNamespace = GetNamespace(rowType);

            //Row row = (Row)rowType.GetInstance();

            //foreach (var field in row.GetFields())
            //{
            //    cw.Indented(field.PropertyName ?? field.Name);
            //    sb.Append("?: ");
            //    var enumField = field as IEnumTypeField;
            //    if (enumField != null && enumField.EnumType != null)
            //    {
            //        HandleMemberType(enumField.EnumType, codeNamespace);
            //    }
            //    else
            //    {
            //        var dataType = field.ValueType;
            //        HandleMemberType(dataType, codeNamespace);
            //    }

            //    sb.AppendLine(";");
            //}
        }

        private Type GetBaseClass(ExternalType type)
        {
            //Type derived;

            //if (IsSubclassOf<ListRequest>(type))
            //    return typeof(ListRequest);
            //else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(ListResponse<>), out derived))
            //    return typeof(ListResponse<>).MakeGenericType(derived.GetGenericArguments()[0]);
            //else if (typeof(RetrieveRequest).IsAssignableFrom(type))
            //    return typeof(RetrieveRequest);
            //else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(RetrieveResponse<>), out derived))
            //    return typeof(RetrieveResponse<>).MakeGenericType(derived.GetGenericArguments()[0]);
            //else if (GeneratorUtils.GetFirstDerivedOfGenericType(type, typeof(SaveRequest<>), out derived))
            //    return typeof(SaveRequest<>).MakeGenericType(derived.GetGenericArguments()[0]);
            //else if (typeof(DeleteRequest).IsAssignableFrom(type))
            //    return typeof(DeleteRequest);
            //else if (typeof(DeleteResponse).IsAssignableFrom(type))
            //    return typeof(DeleteResponse);
            //else if (typeof(UndeleteRequest).IsAssignableFrom(type))
            //    return typeof(UndeleteRequest);
            //else if (typeof(UndeleteResponse).IsAssignableFrom(type))
            //    return typeof(UndeleteResponse);
            //else if (typeof(SaveResponse).IsAssignableFrom(type))
            //    return typeof(SaveResponse);
            //else if (typeof(ServiceRequest).IsAssignableFrom(type))
            //    return typeof(ServiceRequest);
            //else if (typeof(ServiceResponse).IsAssignableFrom(type))
            //    return typeof(ServiceResponse);
            //else
                return null;
        }

        private void GenerateCodeFor(ExternalType type)
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

                if (IsSubclassOf(type, "System.Web.Mvc.Controller"))
                {
                    //GenerateService(type);
                    return;
                }

                cw.Indented("export interface ");

                MakeFriendlyName(sb, type, codeNamespace, enqueueType: (t) => EnqueueType(t));

                var baseClass = GetBaseClass(type);
                if (baseClass != null)
                {
                    sb.Append(" extends ");
                    //MakeFriendlyReference(sb, baseClass, GetNamespace(type), t => EnqueueType(t));
                }

                cw.InBrace(delegate
                {
                    if (IsSubclassOf<Row>(type))
                        GenerateRowMembers(type);
                    else
                    {
                        foreach (var member in type.Members.Where(x => !x.IsPrivate))
                        {
                            if (GetAttribute<JsonIgnoreAttribute>(member) != null)
                                continue;

                            ExternalType memberType;
                            if (!ServerTypes.TryGetValue(member.Type, out memberType))
                                continue;

                            //if (baseClass != null && member.DeclaringType.IsAssignableFrom(baseClass))
                            //    continue;

                            //var pi = member as PropertyInfo;
                            //var fi = member as FieldInfo;
                            //if (pi == null && fi == null)
                            //    continue;

                            //var memberType = pi != null ? pi.PropertyType : fi.FieldType;

                            if (!CanHandleType(memberType))
                                continue;

                            //var memberName = pi != null ? pi.Name : fi.Name;
                            var memberName = member.Name;

                            object jsonName;
                            var jsonProperty = GetAttribute<JsonPropertyAttribute>(member);
                            if (jsonProperty != null && jsonProperty.Arguments != null &&
                                jsonProperty.Arguments.TryGetValue("PropertyName", out jsonName) &&
                                !string.IsNullOrEmpty(jsonName as string))
                                memberName = jsonName as string; 

                            cw.Indented(memberName);
                            sb.Append("?: ");
                            HandleMemberType(memberType, codeNamespace);
                            sb.AppendLine();
                        }
                    }
                });

                if (IsSubclassOf<Row>(type))
                    GenerateRowMetadata(type);
            });
        }

        //private void GenerateService(Type type)
        //{
        //    var codeNamespace = GetNamespace(type);

        //    cw.Indented("export namespace ");
        //    var identifier = GetControllerIdentifier(type);
        //    sb.Append(identifier);
        //    cw.InBrace(delegate
        //    {
        //        var serviceUrl = GetServiceUrlFromRoute(type);
        //        if (serviceUrl == null)
        //            serviceUrl = GetNamespace(type).Replace(".", "/");

        //        cw.Indented("export const baseUrl = '");
        //        sb.Append(serviceUrl);
        //        sb.AppendLine("';");
        //        sb.AppendLine();

        //        Type responseType;
        //        Type requestType;
        //        string requestParam;

        //        var methodNames = new List<string>();
        //        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        //        {
        //            if (methodNames.Contains(method.Name))
        //                continue;

        //            if (!IsPublicServiceMethod(method, out requestType, out responseType, out requestParam))
        //                continue;

        //            methodNames.Add(method.Name);

        //            cw.Indented("export declare function ");
        //            sb.Append(method.Name);

        //            sb.Append("(request: ");
        //            MakeFriendlyReference(sb, requestType, codeNamespace, t => EnqueueType(t));

        //            sb.Append(", onSuccess?: (response: ");
        //            MakeFriendlyReference(sb, responseType, codeNamespace, t => EnqueueType(t));
        //            sb.AppendLine(") => void, opt?: Serenity.ServiceOptions<any>): JQueryXHR;");
        //        }

        //        sb.AppendLine();
        //        cw.Indented("export namespace ");
        //        sb.Append("Methods");
        //        cw.InBrace(delegate
        //        {
        //            foreach (var methodName in methodNames)
        //            {
        //                cw.Indented("export declare const ");
        //                sb.Append(methodName);
        //                sb.Append(": '");
        //                sb.Append(serviceUrl);
        //                sb.Append("/");
        //                sb.Append(methodName);
        //                sb.AppendLine("';");
        //            }
        //        });

        //        sb.AppendLine();
        //        cw.Indented("[");
        //        int i = 0;
        //        foreach (var methodName in methodNames)
        //        {
        //            if (i++ > 0)
        //                sb.Append(", ");

        //            sb.Append("'");
        //            sb.Append(methodName);
        //            sb.Append("'");
        //        }
        //        sb.AppendLine("].forEach(x => {");
        //        cw.Block(delegate () {
        //            cw.Indented("(<any>");
        //            sb.Append(identifier);
        //            sb.AppendLine(")[x] = function (r, s, o) { return Q.serviceRequest(baseUrl + '/' + x, r, s, o); };");
        //            cw.IndentedLine("(<any>Methods)[x] = baseUrl + '/' + x;");
        //        });
        //        cw.IndentedLine("});");
        //    });
        //}

        //private bool IsPublicServiceMethod(MethodInfo method, out Type requestType, out Type responseType,
        //    out string requestParam)
        //{
        //    responseType = null;
        //    requestType = null;
        //    requestParam = null;

        //    if (method.GetCustomAttribute<NonActionAttribute>() != null)
        //        return false;

        //    if (typeof(Controller).IsSubclassOf(method.DeclaringType))
        //        return false;

        //    if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
        //        return false;

        //    var parameters = method.GetParameters().Where(x => !x.ParameterType.IsInterface).ToArray();
        //    if (parameters.Length > 1)
        //        return false;

        //    if (parameters.Length == 1)
        //    {
        //        requestType = parameters[0].ParameterType;
        //        if (requestType.IsPrimitive || !CanHandleType(requestType))
        //            return false;
        //    }
        //    else
        //        requestType = typeof(ServiceRequest);

        //    requestParam = parameters.Length == 0 ? "request" : parameters[0].Name;

        //    responseType = method.ReturnType;
        //    if (responseType != null &&
        //        responseType.IsGenericType &&
        //        responseType.GetGenericTypeDefinition() == typeof(Result<>))
        //    {
        //        responseType = responseType.GenericTypeArguments[0];
        //    }
        //    else if (typeof(ActionResult).IsAssignableFrom(responseType))
        //        return false;
        //    else if (responseType == typeof(void))
        //        return false;

        //    return true;
        //}

        //private string GetServiceUrlFromRoute(Type controller)
        //{
        //    var route = controller.GetCustomAttributes<RouteAttribute>().FirstOrDefault();
        //    string url = route.Template ?? "";

        //    if (!url.StartsWith("~/"))
        //    {
        //        var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
        //        if (routePrefix != null)
        //            url = UriHelper.Combine(routePrefix.Prefix, url);
        //    }

        //    if (!url.StartsWith("~/") && !url.StartsWith("/"))
        //        url = "~/" + url;

        //    while (true)
        //    {
        //        var idx1 = url.IndexOf('{');
        //        if (idx1 <= 0)
        //            break;

        //        var idx2 = url.IndexOf("}", idx1 + 1);
        //        if (idx2 <= 0)
        //            break;

        //        url = url.Substring(0, idx1) + url.Substring(idx2 + 1);
        //    }

        //    if (url.StartsWith("~/Services/", StringComparison.OrdinalIgnoreCase))
        //        url = url.Substring("~/Services/".Length);

        //    if (url.Length > 1 && url.EndsWith("/"))
        //        url = url.Substring(0, url.Length - 1);

        //    return url;
        //}
    }
}