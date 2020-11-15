using Mono.Cecil;
using Serenity.Data;
using Serenity.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Serenity.CodeGeneration
{
    public abstract class CecilImportGenerator : ImportGeneratorBase
    {
        private HashSet<string> visited;
        private Queue<TypeDefinition> generateQueue;
        protected List<TypeDefinition> lookupScripts;
        protected HashSet<string> localTextKeys;
        protected HashSet<string> generatedTypes;
        protected string fileIdentifier;
        protected List<AnnotationTypeInfo> annotationTypes;

        public CecilImportGenerator(params Assembly[] assemblies)
            : this(CecilUtils.ToDefinitions(assemblies == null ? (string[])null : assemblies.Select(x => x.Location)))
        {
        }

        public CecilImportGenerator(params string[] assemblyLocations)
            : this(CecilUtils.ToDefinitions(assemblyLocations))
        {
        }

        public CecilImportGenerator(params AssemblyDefinition[] assemblies)
            : base()
        {
            generatedTypes = new HashSet<string>();
            annotationTypes = new List<AnnotationTypeInfo>();

            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assembly");

            this.Assemblies = assemblies;
        }

        public AssemblyDefinition[] Assemblies { get; private set; }

        protected virtual bool EnqueueType(TypeDefinition type)
        {
            if (visited.Contains(type.FullName))
                return false;

            visited.Add(type.FullName);
            generateQueue.Enqueue(type);
            return true;
        }

        private void EnqueueMemberType(TypeReference memberType)
        {
            if (memberType == null)
                return;

            var enumType = CecilUtils.GetEnumTypeFrom(memberType);
            if (enumType != null)
                EnqueueType(enumType);
        }

        protected virtual void EnqueueTypeMembers(TypeDefinition type)
        {
            foreach (var field in type.Fields)
            {
                if (field.IsStatic | !field.IsPublic)
                    continue;

                EnqueueMemberType(field.FieldType);
            }

            foreach (var property in type.Properties)
            {
                if (!CecilUtils.IsPublicInstanceProperty(property))
                    continue;

                EnqueueMemberType(property.PropertyType);
            }
        }

        protected virtual string GetNamespace(TypeReference type)
        {
            var ns = type.Namespace ?? "";
            if (string.IsNullOrEmpty(ns) && type.IsNested)
                ns = type.DeclaringType.Namespace;

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

        protected virtual string GetControllerIdentifier(TypeReference controller)
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
            this.generateQueue = new Queue<TypeDefinition>();
            this.visited = new HashSet<string>();
            this.lookupScripts = new List<TypeDefinition>();
            this.localTextKeys = new HashSet<string>();
        }

        protected abstract bool IsTS();

        protected override void GenerateAll()
        {
            foreach (var assembly in this.Assemblies)
            {
                foreach (var module in assembly.Modules)
                {
                    TypeDefinition[] types;
                    try
                    {
                        types = module.Types.ToArray();
                    }
                    catch
                    {
                        // skip assemblies that doesn't like to list its types (e.g. some SignalR reported in #2340)
                        continue;
                    }

                    TypeDefinition[] emptyTypes = new TypeDefinition[0];

                    foreach (var fromType in types)
                    {
                        var nestedLocalTexts = CecilUtils.GetAttr(fromType, "Serenity.Extensibility",
                            "NestedLocalTextsAttribute", emptyTypes);
                        if (nestedLocalTexts != null)
                        {
                            string prefix = null;
                            if (nestedLocalTexts.HasProperties)
                                prefix = nestedLocalTexts.Properties.FirstOrDefault(x => x.Name == "Prefix").Argument.Value as string;

                            AddNestedLocalTexts(fromType, prefix ?? "");
                        }

                        ScanAnnotationTypeAttributes(fromType);

                        if (fromType.IsAbstract ||
                            CecilUtils.GetAttr(fromType, "Serenity.ComponentModel", "ScriptSkipAttribute") != null)
                            continue;

                        var baseClasses = CecilUtils.EnumerateBaseClasses(fromType).ToArray();

                        if (CecilUtils.Contains(baseClasses, "Serenity.Services", "ServiceRequest") ||
                            CecilUtils.Contains(baseClasses, "Serenity.Services", "ServiceResponse") ||
                            CecilUtils.Contains(baseClasses, "Serenity.Data", "Row") ||
                            CecilUtils.Contains(baseClasses, "Serenity.Services", "ServiceEndpoint") ||
                            CecilUtils.GetAttr(fromType, "Serenity.ComponentModel", "ScriptIncludeAttribute", baseClasses) != null ||
                            CecilUtils.GetAttr(fromType, "Serenity.ComponentModel", "FormScriptAttribute", baseClasses) != null ||
                            CecilUtils.GetAttr(fromType, "Serenity.ComponentModel", "ColumnsScriptAttribute", baseClasses) != null ||
                            CecilUtils.GetAttr(fromType, "Serenity.Extensibility", "NestedPermissionKeysAttribute", emptyTypes) != null ||
                            ((CecilUtils.Contains(baseClasses, "Microsoft.AspNetCore.Mvc", "Controller") ||
                              CecilUtils.Contains(baseClasses, "System.Web.Mvc", "Controller")) && // backwards compability
                             fromType.Namespace != null &&
                             fromType.Namespace.EndsWith(".Endpoints")))
                        {
                            EnqueueType(fromType);
                            continue;
                        }

                        if (CecilUtils.GetAttr(fromType, "Serenity.ComponentModel", "LookupScriptAttribute", baseClasses) != null)
                            lookupScripts.Add(fromType);
                    }
                }
            }

            while (generateQueue.Count > 0)
            {
                var typeDef = generateQueue.Dequeue();

                if (!this.Assemblies.Any(x => x.FullName == typeDef.Module.Assembly.FullName))
                    continue;

                var ns = GetNamespace(typeDef);
                this.fileIdentifier = typeDef.Name;

                GenerateCodeFor(typeDef);

                AddFile(RemoveRootNamespace(ns, this.fileIdentifier + (IsTS() ? ".ts" : ".cs")));
            }
        }

        private void ScanAnnotationTypeAttributes(TypeDefinition fromType)
        {
            var annotationTypeAttrs = CecilUtils.GetAttrs(fromType.CustomAttributes,
                                        "Serenity.ComponentModel", "AnnotationTypeAttribute", null);
            if (!annotationTypeAttrs.Any())
                return;

            var typeInfo = new AnnotationTypeInfo(fromType);
            foreach (var attr in annotationTypeAttrs)
            {
                var attrInfo = new AnnotationTypeInfo.AttributeInfo();
                var annotatedType = attr.ConstructorArguments?.FirstOrDefault(x =>
                    x.Type.FullName == "System.Type").Value as TypeReference;
                if (annotatedType == null)
                    continue;

                attrInfo.AnnotatedType = annotatedType.Resolve();

                if (attr.HasProperties)
                {
                    attrInfo.Inherited = attr.Properties.FirstOrDefault(x =>
                        x.Name == "Inherited").Argument.Value as Boolean? ?? true;

                    attrInfo.Namespaces = attr.Properties.FirstOrDefault(x =>
                        x.Name == "Namespaces").Argument.Value as string[];

                    attrInfo.Properties = attr.Properties.FirstOrDefault(x =>
                        x.Name == "Properties").Argument.Value as string[];
                }
                else
                    attrInfo.Inherited = true;
                typeInfo.Attributes.Add(attrInfo);
            }

            if (typeInfo.Attributes.Count > 0)
                annotationTypes.Add(typeInfo);
        }

        protected List<AnnotationTypeInfo> GetAnnotationTypesFor(TypeDefinition type)
        {
            var list = new List<AnnotationTypeInfo>();
            TypeDefinition[] baseClasses = null;
            foreach (var annotationType in annotationTypes)
            {
                var annotationMatch = false;

                foreach (var attr in annotationType.Attributes)
                {
                    baseClasses = baseClasses ?? CecilUtils.EnumerateBaseClasses(type).ToArray();

                    if (CecilUtils.IsOrSubClassOf(attr.AnnotatedType.Resolve(), "System", "Attribute"))
                    {
                        if (CecilUtils.GetAttr(type, attr.AnnotatedType.Namespace, 
                            attr.AnnotatedType.Name, baseClasses) == null) 
                            continue;
                    }
                    else if (attr.Inherited || attr.AnnotatedType.IsInterface)
                    {
                        if (!CecilUtils.IsAssignableFrom(attr.AnnotatedType, type))
                            continue;
                    }
                    else if (type != attr.AnnotatedType)
                        continue;

                    if (attr.Namespaces != null && attr.Namespaces.Length > 0)
                    {
                        bool namespaceMatch = false;
                        foreach (var ns in attr.Namespaces)
                        {
                            if (type.Namespace == ns)
                            {
                                namespaceMatch = true;
                                break;
                            }

                            if (ns.Length > 2 &&
                                ns.EndsWith(".*", StringComparison.OrdinalIgnoreCase) &&
                                type.Namespace != null)
                            {
                                if (type.Namespace == ns.Substring(0, ns.Length - 2) ||
                                    type.Namespace.StartsWith(ns.Substring(0, ns.Length - 1), StringComparison.OrdinalIgnoreCase))
                                {
                                    namespaceMatch = true;
                                    break;
                                }
                            }
                        }

                        if (!namespaceMatch)
                            continue;
                    }

                    if (attr.Properties != null &&
                        attr.Properties.Length > 0 &&
                        attr.Properties.Any(name => !type.Properties.Any(p => 
                            p.Name == name && CecilUtils.IsPublicInstanceProperty(p))))
                        continue;

                    annotationMatch = true;
                    break;
                }

                if (annotationMatch)
                    list.Add(annotationType);
            }

            return list;
        }


        protected abstract void HandleMemberType(TypeReference memberType, string codeNamespace, StringBuilder sb = null);

        public static bool CanHandleType(TypeDefinition memberType)
        {
            if (memberType.IsInterface)
                return false;

            if (memberType.IsAbstract)
                return false;

            if (CecilUtils.IsOrSubClassOf(memberType, "System", "Delegate"))
                return false;

            return true;
        }

        public virtual string ShortenNamespace(TypeReference type, string codeNamespace)
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

        protected virtual string MakeFriendlyName(TypeReference type, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            if (type.IsGenericInstance)
            {
                var name = type.Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                int i = 0;
                foreach (var argument in (type as GenericInstanceType).GenericArguments)
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    HandleMemberType(argument, codeNamespace, sb);
                }

                sb.Append(">");

                return name + "`" + (type as GenericInstanceType).GenericArguments.Count;
            }
            else if (type.HasGenericParameters)
            {
                var name = type.Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                int i = 0;
                foreach (var argument in type.GenericParameters)
                {
                    if (i++ > 0)
                        sb.Append(", ");

                    sb.Append(argument.Name);
                }

                sb.Append(">");

                return name + "`" + type.GenericParameters.Count;
            }
            else
            {
                sb.Append(type.Name);
                return type.Name;
            }
        }

        protected virtual void MakeFriendlyReference(TypeReference type, string codeNamespace, StringBuilder sb = null)
        {
            sb = sb ?? this.sb;

            string ns;

            if (type.IsGenericInstance)
            {
                ns = ShortenNamespace(type, codeNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    sb.Append(ns);
                    sb.Append(".");
                }

                var name = type.Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                int i = 0;
                foreach (var argument in (type as GenericInstanceType).GenericArguments)
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

        protected TypeReference GetBaseClass(TypeDefinition type)
        {
            foreach (var t in CecilUtils.SelfAndBaseClasses(type))
            {
                if (t.BaseType != null &&
                    t.BaseType.IsGenericInstance &&
                    (t.BaseType as GenericInstanceType).ElementType.Namespace == "Serenity.Services")
                {
                    var n = (t.BaseType as GenericInstanceType).ElementType.Name;
                    if (n == "ListResponse`1" || n == "RetrieveResponse`1" || n == "SaveRequest`1")
                        return t.BaseType;
                }

                if (t.Namespace != "Serenity.Services")
                    continue;

                if (t.Name == "ListRequest" ||
                    t.Name == "RetrieveRequest" ||
                    t.Name == "DeleteRequest" ||
                    t.Name == "DeleteResponse" ||
                    t.Name == "UndeleteRequest" ||
                    t.Name == "UndeleteResponse" ||
                    t.Name == "SaveResponse" ||
                    t.Name == "ServiceRequest" ||
                    t.Name == "ServiceResponse")
                    return t;
            }

            return null;
        }

        protected abstract void GenerateCodeFor(TypeDefinition type);

        protected bool IsPublicServiceMethod(MethodDefinition method, out TypeReference requestType, out TypeReference responseType,
            out string requestParam)
        {
            responseType = null;
            requestType = null;
            requestParam = null;

            if ((CecilUtils.FindAttr(method.CustomAttributes, "System.Web.Mvc", "NonActionAttribute") ??
                 CecilUtils.FindAttr(method.CustomAttributes, "Microsoft.AspNetCore.Mvc", "NonActionAttribute") ??
                 CecilUtils.FindAttr(method.CustomAttributes, "Serenity.ComponentModel", "ScriptSkipAttribute")) != null)
                return false;

            if (!CecilUtils.IsSubclassOf(method.DeclaringType, "System.Web.Mvc", "Controller") &&
                !CecilUtils.IsSubclassOf(method.DeclaringType, "Microsoft.AspNetCore.Mvc", "Controller"))
                return false;

            if (method.IsSpecialName && (method.Name.StartsWith("set_") || method.Name.StartsWith("get_")))
                return false;

            var parameters = method.Parameters.Where(x => !x.ParameterType.Resolve().IsInterface).ToArray();
            if (parameters.Length > 1)
                return false;

            if (parameters.Length == 1)
            {
                requestType = parameters[0].ParameterType;
                if (requestType.IsPrimitive || !CanHandleType(requestType.Resolve()))
                    return false;
            }
            else
                requestType = null;

            requestParam = parameters.Length == 0 ? "request" : parameters[0].Name;

            responseType = method.ReturnType == null ? null : method.ReturnType;
            if (responseType != null &&
                responseType.IsGenericInstance &&
                (responseType as GenericInstanceType).ElementType.FullName.StartsWith("Serenity.Services.Result`1"))
            {
                responseType = (responseType as GenericInstanceType).GenericArguments[0];
                return true;
            }
            else if (CecilUtils.IsOrSubClassOf(responseType.Resolve(), "System.Web.Mvc", "ActionResult") ||
                CecilUtils.IsOrSubClassOf(responseType.Resolve(), "Microsoft.AspNetCore.Mvc", "ActionResult"))
                return false;
            else if (responseType == null || CecilUtils.IsVoid(responseType))
                return false;

            return true;
        }

        protected string GetServiceUrlFromRoute(TypeDefinition controller)
        {
            var route = CecilUtils.GetAttr(controller, "System.Web.Mvc", "RouteAttribute") ??
                CecilUtils.GetAttr(controller, "Microsoft.AspNetCore.Mvc", "RouteAttribute");
            string url = route == null || route.ConstructorArguments.Count == 0 || !(route.ConstructorArguments[0].Value is string) ? 
                ("Services/HasNoRoute/" + controller.Name) : (route.ConstructorArguments[0].Value as string ?? "");

#if !ASPNETMVC
            url = url.Replace("[controller]", controller.Name.Substring(0, controller.Name.Length - "Controller".Length));
            url = url.Replace("/[action]", "");
#else
            if (!url.StartsWith("~/"))
            {
                var routePrefix = CecilUtils.GetAttr(controller, "System.Web.Mvc", "RoutePrefixAttribute");
                if (routePrefix != null && routePrefix.ConstructorArguments.Count > 0 && routePrefix.ConstructorArguments[0].Value is string)
                    url = UriHelper.Combine(routePrefix.ConstructorArguments[0].Value as string, url);
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

        protected virtual void AddNestedLocalTexts(TypeDefinition type, string prefix)
        {
        }

        protected class AnnotationTypeInfo
        {
            public TypeDefinition AnnotationType { get; private set; }
            public List<AttributeInfo> Attributes { get; private set; }
            public Dictionary<string, PropertyDefinition> PropertyByName { get; private set; }

            public AnnotationTypeInfo(TypeDefinition annotationType)
            {
                this.AnnotationType = annotationType;
                this.PropertyByName = new Dictionary<string, PropertyDefinition>();
                this.Attributes = new List<AttributeInfo>();

                foreach (var property in annotationType.Properties)
                    if (CecilUtils.IsPublicInstanceProperty(property))
                        PropertyByName[property.Name] = property;
            }

            public class AttributeInfo
            {
                public TypeDefinition AnnotatedType { get; set; }
                public bool Inherited { get; set; }
                public string[] Namespaces { get; set; }
                public string[] Properties { get; set; }
            }
        }
    }
}