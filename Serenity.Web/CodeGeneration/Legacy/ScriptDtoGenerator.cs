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
    public class ScriptDtoGenerator
    {
        private StringBuilder sb;
        private CodeWriter cw;
        private HashSet<Type> visited;
        private Queue<Type> generateQueue;
        private List<Type> lookupScripts;

        public ScriptDtoGenerator(params Assembly[] assemblies)
        {
            UsingNamespaces = new HashSet<string>
            {
                "Serenity",
                "Serenity.ComponentModel",
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.ComponentModel",
                "System.Runtime.CompilerServices"
            };

            RootNamespaces = new HashSet<string>
            {
            };

            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assembly");

            this.Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; private set; }
        public HashSet<string> UsingNamespaces { get; private set; }
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
            var ns = type.Namespace ?? "";
            if (ns.EndsWith(".Entities"))
                return ns.Substring(0, ns.Length - ".Entities".Length);

            return ns;
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            this.sb = new StringBuilder(4096);
            this.cw = new CodeWriter(sb, 4);
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

            Dictionary<Type, string> generatedCode = new Dictionary<Type, string>();
            while (generateQueue.Count > 0)
            {
                var type = generateQueue.Dequeue();

                if (!this.Assemblies.Contains(type.Assembly))
                    continue;

                GenerateCodeFor(type);
                generatedCode[type] = sb.ToString();
                sb.Clear();
            }

            sb.Clear();

            sb.AppendLine();

            var ordered = generatedCode.Keys.OrderBy(x => GetNamespace(x)).ThenBy(x => x.Name);
            var byNameSpace = ordered.ToLookup(x => GetNamespace(x));
            var byOwnerType = ordered.ToLookup(x => (x.IsNested ? x.DeclaringType : null));
            var outputted = new HashSet<Type>();

            var result = new SortedDictionary<string, string>();

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                Action<Type> outputType = delegate(Type type)
                {
                    var filename = ns.Key + "." + type.Name + ".cs";

                    foreach (var rn in RootNamespaces)
                    {
                        if (filename.StartsWith(rn + "."))
                            filename = filename.Substring(rn.Length + 1);
                    }

                    result.Add(filename, sb.ToString());
                };

                foreach (var owner in byOwnerType)
                {
                    bool skip = false;

                    sb.Clear();
                    sb.AppendLine();
                    cw.Indented("namespace ");
                    sb.AppendLine(ns.Key);

                    cw.InBrace(delegate
                    {
                        foreach (var usingNamespace in UsingNamespaces.OrderBy(x => x))
                        {
                            cw.Indented("using ");
                            sb.Append(usingNamespace);
                            sb.AppendLine(";");
                        }

                        sb.AppendLine();

                        if (owner.Key == null)
                        {
                            skip = true;
                            return;
                        }

                        if (GetNamespace(owner.Key) != ns.Key)
                        {
                            skip = true;
                            return;
                        }

                        if (outputted.Contains(owner.Key))
                        {
                            skip = true;
                            return;
                        }

                        outputted.Add(owner.Key);

                        if (!generatedCode.ContainsKey(owner.Key))
                            return;

                        string code = generatedCode[owner.Key].TrimEnd();
                        code = code.Substring(0, code.Length - 1).TrimEnd();
                        cw.IndentedMultiLine(code);

                        cw.Block(delegate
                        {
                            sb.AppendLine();

                            foreach (var subType in owner)
                            {
                                cw.IndentedMultiLine(generatedCode[subType]);
                                outputted.Add(subType);
                            }
                        });

                        cw.IndentedLine("}");
                        sb.AppendLine();
                    });

                    if (skip)
                        continue;

                    outputType(owner.Key);
                }

                foreach (var type in ns)
                {
                    if (outputted.Contains(type))
                        continue;

                    sb.Clear();
                    sb.AppendLine();
                    cw.Indented("namespace ");
                    sb.AppendLine(ns.Key);

                    cw.InBrace(() =>
                    {
                        foreach (var usingNamespace in UsingNamespaces.OrderBy(x => x))
                        {
                            cw.Indented("using ");
                            sb.Append(usingNamespace);
                            sb.AppendLine(";");
                        }

                        sb.AppendLine();

                        cw.IndentedMultiLine(generatedCode[type]);
                    });

                    outputType(type);
                    outputted.Add(type);
                }
            }

            return result;
        }

        private void HandleMemberType(Type memberType, string codeNamespace, HashSet<string> usingNamespaces)
        {
            HandleMemberType(sb, memberType, codeNamespace, usingNamespaces, t => EnqueueType(t));
        }

        public static void HandleMemberType(StringBuilder code, Type memberType, string codeNamespace, HashSet<string> usingNamespaces, 
            Action<Type> enqueueType = null)
        {
            if (memberType == typeof(DateTime?) || memberType == typeof(DateTime) || memberType == typeof(TimeSpan) || memberType == typeof(TimeSpan?))
            {
                code.Append("String"); // şu an için string, JSON tarafında ISO string formatında tarihler gidiyor, 
                // ama bunu daha sonra JSON.parse, JSON.stringify'ı değiştirip düzelteceğiz.
                return;
            }

            if (GeneratorUtils.IsSimpleType(memberType))
            {
                code.Append(memberType.Name);
                return;
            }

            var nullableType = Nullable.GetUnderlyingType(memberType);
            if (nullableType != null &&
                GeneratorUtils.IsSimpleType(nullableType))
            {
                code.Append(nullableType.Name);
                code.Append("?");
                return;
            }

            if (nullableType != null)
            {
                HandleMemberType(code, nullableType, codeNamespace, usingNamespaces, enqueueType);
                code.Append("?");
                return;
            }

            // TODO: Bunlar özel durumlar, attribute la daha sonra halledelim!
            if (memberType == typeof(SortBy[]))
            {
                code.Append("SortBy[]");
                return;
            }

            if (memberType == typeof(Stream))
            {
                code.Append("byte[]");
                return;
            }

            if (memberType == typeof(Object))
            {
                code.Append("Object");
                return;
            }

            if (memberType.IsArray)
            {
                code.Append("List<");
                HandleMemberType(code, memberType.GetElementType(), codeNamespace, usingNamespaces, enqueueType);
                code.Append(">");
                return;
            }

            if (memberType.IsGenericType &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) || memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                code.Append("List<");
                HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, usingNamespaces, enqueueType);
                code.Append(">");
                return;
            }

            if (memberType.IsGenericType &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                code.Append("JsDictionary<");
                HandleMemberType(code, memberType.GenericTypeArguments[0], codeNamespace, usingNamespaces, enqueueType);
                code.Append(",");
                HandleMemberType(code, memberType.GenericTypeArguments[1], codeNamespace, usingNamespaces, enqueueType);
                code.Append(">");
                return;
            }

            if (enqueueType != null)
                enqueueType(memberType);

            code.Append(MakeFriendlyName(memberType, codeNamespace, usingNamespaces));
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

        public static string MakeFriendlyName(Type type, string codeNamespace, HashSet<string> usingNamespaces)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();

                var name = type.GetGenericTypeDefinition().Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);
                sb.Append("<");

                foreach (var argument in type.GetGenericArguments())
                {
                    var arg = MakeFriendlyName(argument, codeNamespace, usingNamespaces);
                    sb.Append(arg);
                }

                sb.Append(">");

                return sb.ToString();
            }

            // compability with older code generators that doesn't take namespaces into account
            if (usingNamespaces == null && codeNamespace == null)
                return type.Name;

            var ns = GetNamespace(type);

            if (ns == "Serenity" ||
                ns.StartsWith("Serenity.") ||
                (usingNamespaces != null && usingNamespaces.Contains(ns)) ||
                (codeNamespace != null && (ns == codeNamespace)) ||
                (codeNamespace != null && codeNamespace.StartsWith((ns + "."))))
                return type.Name;

            if (codeNamespace != null)
            {
                var idx = codeNamespace.IndexOf('.');
                if (idx >= 0 && type.FullName.StartsWith(codeNamespace.Substring(0, idx + 1)))
                    return (type.FullName.Substring(idx + 1)).Replace(".Entities.", ".");
            }

            return type.FullName.Replace(".Entities.", ".");
        }

        private void GenerateEnum(Type enumType)
        {
            var enumKey = EnumMapper.GetEnumTypeKey(enumType);
            cw.Indented("[EnumKey(\"");
            sb.Append(enumKey);
            sb.AppendLine("\"), PreserveMemberCase]");

            cw.Indented("public enum ");
            sb.AppendLine(enumType.Name);
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
        }

        private void GenerateRowMembers(Type rowType)
        {
            bool anyMetadata = false;
            var codeNamespace = GetNamespace(rowType);

            Row row = (Row)Activator.CreateInstance(rowType);

            var idRow = row as IIdRow;
            if (idRow != null)
            {
                cw.Indented("[InlineConstant] public const string IdProperty = \"");
                var field = ((Field)idRow.IdField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            var isActiveRow = row as IIsActiveRow;
            if (isActiveRow != null)
            {
                cw.Indented("[InlineConstant] public const string IsActiveProperty = \"");
                var field = (isActiveRow.IsActiveField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            var nameRow = row as INameRow;
            if (nameRow != null)
            {
                cw.Indented("[InlineConstant] public const string NameProperty = \"");
                var field = (nameRow.NameField);
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            var localTextPrefix = row.GetFields().LocalTextPrefix;
            if (!string.IsNullOrEmpty(localTextPrefix))
            {
                cw.Indented("[InlineConstant] public const string LocalTextPrefix = \"");
                sb.Append(localTextPrefix);
                sb.AppendLine("\";");
                anyMetadata = true;
            }

            var attr = rowType.GetCustomAttribute<LookupScriptAttribute>();
            if (attr == null)
            {
                var script = lookupScripts.FirstOrDefault(x => 
                    x.BaseType != null && 
                    x.BaseType.IsGenericType &&
                    x.BaseType.GetGenericArguments().Any(z => z == rowType));

                if (script != null)
                    attr = script.GetCustomAttribute<LookupScriptAttribute>();
            }

            if (attr != null)
            {
                var lookupKey = attr.Key ?? LookupScriptAttribute.AutoLookupKeyFor(rowType);
                cw.Indented("[InlineConstant] public const string LookupKey = \"");
                sb.Append(lookupKey);
                sb.AppendLine("\";");

                sb.AppendLine();
                cw.Indented("public static Lookup<");
                sb.Append(MakeFriendlyName(rowType, null, null));
                sb.Append("> Lookup { [InlineCode(\"Q.getLookup('");
                sb.Append(lookupKey);
                sb.AppendLine("')\")] get { return null; } }");

                anyMetadata = true;
            }

            if (anyMetadata)
                sb.AppendLine();

            foreach (var field in row.GetFields())
            {
                cw.Indented("public ");
                
                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType, codeNamespace, UsingNamespaces);
                    sb.Append('?');
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType, codeNamespace, UsingNamespaces);
                }

                sb.Append(" ");
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine(" { get; set; }");
            }

            sb.AppendLine();
            cw.IndentedLine("[Imported, PreserveMemberCase]");
            cw.IndentedLine("public static class Fields");
            cw.InBrace(delegate
            {
                foreach (var field in row.GetFields())
                {
                    cw.Indented("[InlineConstant] public const string ");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.Append(" = \"");
                    sb.Append(field.PropertyName ?? field.Name);
                    sb.AppendLine("\";");
                }
            });
        }



        private Type GetParentClass(Type type)
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
            if (type.IsEnum)
            {
                GenerateEnum(type);
                return;
            }

            cw.IndentedLine("[Imported, Serializable, PreserveMemberCase]");
            cw.Indented("public partial class ");
            var codeNamespace = GetNamespace(type);

            sb.Append(MakeFriendlyName(type, null, null));

            var parentClass = GetParentClass(type);
            if (parentClass != null)
            {
                sb.Append(" : ");
                sb.Append(MakeFriendlyName(parentClass, GetNamespace(type), UsingNamespaces));
            }

            sb.AppendLine();

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

                        if (parentClass != null && member.DeclaringType.IsAssignableFrom(parentClass))
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
                        {
                            cw.Indented("[ScriptName(\"");
                            sb.Append(jsonProperty.PropertyName);
                            sb.AppendLine("\")]");
                        }

                        cw.Indented("public ");

                        HandleMemberType(memberType, codeNamespace, UsingNamespaces);

                        sb.Append(" ");
                        sb.Append(memberName);
                        sb.AppendLine(" { get; set; }");
                    }
                }
            });
        }
    }
}
