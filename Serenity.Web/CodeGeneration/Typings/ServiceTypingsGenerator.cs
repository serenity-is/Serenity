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

namespace Serenity.CodeGeneration
{
    public class ServiceTypingsGenerator
    {
        private StringBuilder sb;
        private CodeWriter cw;
        private HashSet<Type> visited;
        private Queue<Type> generateQueue;
        private List<Type> lookupScripts;

        public ServiceTypingsGenerator(params Assembly[] assemblies)
        {
            RootNamespaces = new HashSet<string>
            {
            };

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

            return ns;
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
                        fromType.GetCustomAttribute<ScriptIncludeAttribute>() != null)
                    {
                        EnqueueType(fromType);
                        continue;
                    }

                    if (fromType.GetCustomAttribute<FormScriptAttribute>() != null ||
                        fromType.GetCustomAttribute<ColumnsScriptAttribute>() != null)
                    {
                        EnqueueTypeMembers(fromType);
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
                var filename = ns + "." + type.Name + ".ts";

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

        public static void MakeFriendlyName(StringBuilder sb, Type type, string codeNamespace,
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
            }
            else
                sb.Append(type.Name);
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
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);

            cw.Indented("export enum ");
            sb.Append(enumType.Name);
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
            sb.Append(", new Serenity.EnumKeyAttribute(\"");
            sb.Append(enumKey);
            sb.AppendLine("\"));");
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
                    cw.Indented("export const idProperty = \"");
                    var field = ((Field)idRow.IdField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("\";");
                    anyMetadata = true;
                }


                if (isActiveRow != null)
                {
                    cw.Indented("export const isActiveProperty = \"");
                    var field = (isActiveRow.IsActiveField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("\";");
                    anyMetadata = true;
                }

                if (nameRow != null)
                {
                    cw.Indented("export const nameProperty = \"");
                    var field = (nameRow.NameField);
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("\";");
                    anyMetadata = true;
                }

                var localTextPrefix = row.GetFields().LocalTextPrefix;
                if (!string.IsNullOrEmpty(localTextPrefix))
                {
                    cw.Indented("export const localTextPrefix = \"");
                    sb.Append(localTextPrefix);
                    sb.AppendLine("\";");
                    anyMetadata = true;
                }

                if (lookupAttr != null)
                {
                    cw.Indented("export const lookupKey = \"");
                    sb.Append(lookupAttr.Key);
                    sb.AppendLine("\";");

                    sb.AppendLine();
                    cw.Indented("export function lookup()");
                    cw.InBrace(delegate
                    {
                        cw.Indented("return Q.getLookup(\"");
                        sb.Append(lookupAttr.Key);
                        sb.AppendLine("\");");
                    });

                    //sb.AppendLine();
                    //cw.Indented("public static Lookup<");
                    //sb.Append(MakeFriendlyName(rowType, null));
                    //sb.Append("> Lookup { [InlineCode(\"Q.getLookup('");
                    //sb.Append(attr.Key);
                    //sb.AppendLine("')\")] get { return null; } }");

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
                        sb.Append(": \"");
                        sb.Append(field.PropertyName ?? field.Name);
                        sb.AppendLine("\";");
                    }
                });

                sb.AppendLine();
                cw.Indented("[");
                int i = 0;
                foreach (var field in row.GetFields())
                {
                    if (i++ > 0)
                        sb.Append(',');
                    sb.Append('"');
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.Append('"');
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

                cw.Indented("export interface ");

                MakeFriendlyName(sb, type, codeNamespace, enqueueType: (t) => EnqueueType(t));

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
    }
}