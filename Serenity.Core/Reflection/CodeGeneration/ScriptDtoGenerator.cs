using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Serenity.Reflection
{
    public class ScriptDtoGenerator
    {
        private StringBuilder sb;
        private CodeWriter cw;
        private HashSet<Type> visited;
        private Queue<Type> generateQueue;

        public ScriptDtoGenerator()
        {
        }

        private bool EnqueueType(Type type)
        {
            if (visited.Contains(type))
                return false;

            visited.Add(type);
            generateQueue.Enqueue(type);
            return true;
        }

        public string GenerateCode(Assembly assembly, string anamespace)
        {
            this.sb = new StringBuilder(4096);
            this.cw = new CodeWriter(sb, 4);
            this.generateQueue = new Queue<Type>();
            this.visited = new HashSet<Type>();

            foreach (var fromType in assembly.GetTypes())
            {
                if (fromType.IsAbstract)
                    continue;

                if (fromType.IsSubclassOf(typeof(ServiceRequest)) ||
                    fromType.IsSubclassOf(typeof(ServiceResponse)) ||
                    fromType.IsSubclassOf(typeof(Row)))
                    EnqueueType(fromType);
            }

            Dictionary<Type, string> generatedCode = new Dictionary<Type, string>();
            while (generateQueue.Count > 0)
            {
                var type = generateQueue.Dequeue();

                if (type.Assembly != assembly)
                    continue;

                GenerateCodeFor(type);
                generatedCode[type] = sb.ToString();
                sb.Clear();
            }

            sb.Clear();

            foreach (var ns in
                new string[] {
                        "Serenity",
                        "Serenity.ComponentModel",
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.ComponentModel",
                        "System.Runtime.CompilerServices"
                    })
            {
                cw.Indented("using ");
                sb.Append(ns);
                sb.AppendLine(";");
            }

            sb.AppendLine();

            var byNameSpace = generatedCode.Keys.ToLookup(x => x.Namespace);
            var byOwnerType = generatedCode.Keys.ToLookup(x => (x.IsNested ? x.DeclaringType : null));
            var outputted = new HashSet<Type>();

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                sb.AppendLine();
                cw.Indented("namespace ");
                sb.AppendLine(ns.Key);
                cw.InBrace(delegate
                {
                    foreach (var owner in byOwnerType)
                    {
                        if (owner.Key == null)
                            continue;

                        if (outputted.Contains(owner.Key))
                            continue;

                        outputted.Add(owner.Key);

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
                    }

                    foreach (var type in ns)
                    {
                        if (outputted.Contains(type))
                            continue;

                        cw.IndentedMultiLine(generatedCode[type]);
                        outputted.Add(type);
                    }
                });
            }

            return sb.ToString();
        }

        private void HandleMemberType(Type memberType)
        {
            HandleMemberType(sb, memberType, t => EnqueueType(t));
        }

        public static void HandleMemberType(StringBuilder code, Type memberType, Action<Type> enqueueType = null)
        {
            if (memberType == typeof(DateTime?) || memberType == typeof(DateTime))
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
                HandleMemberType(code, nullableType, enqueueType);
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
                HandleMemberType(code, memberType.GetElementType(), enqueueType);
                code.Append(">");
                return;
            }

            if (memberType.IsGenericType &&
                (memberType.GetGenericTypeDefinition() == typeof(List<>) || memberType.GetGenericTypeDefinition() == typeof(HashSet<>)))
            {
                code.Append("List<");
                HandleMemberType(code, memberType.GenericTypeArguments[0], enqueueType);
                code.Append(">");
                return;
            }

            if (memberType.IsGenericType &&
                memberType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                code.Append("JsDictionary<");
                HandleMemberType(code, memberType.GenericTypeArguments[0], enqueueType);
                code.Append(",");
                HandleMemberType(code, memberType.GenericTypeArguments[1], enqueueType);
                code.Append(">");
                return;
            }

            if (enqueueType != null)
                enqueueType(memberType);

            code.Append(MakeFriendlyName(memberType));
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

        public static string MakeFriendlyName(Type type)
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
                    var arg = MakeFriendlyName(argument);
                    sb.Append(arg);
                }

                sb.Append(">");

                return sb.ToString();
            }

            return type.Name;
        }

        private void GenerateEnum(Type enumType)
        {
            cw.IndentedLine("[NamedValues, PreserveMemberCase]");
            cw.Indented("public enum ");
            sb.AppendLine(enumType.Name);
            cw.InBrace(delegate 
            {
                var names = Enum.GetNames(enumType);
                var values = Enum.GetValues(enumType);

                int i = 0;
                foreach (var name in names)
                {
                    cw.Indented(name);
                    sb.Append(" = ");
                    sb.Append(Convert.ToInt32(((IList)values)[i]));
                    sb.AppendLine(",");
                    i++;
                }
            });
        }

        private void GenerateRowMembers(Type rowType)
        {
            Row row = (Row)rowType.GetInstance();
            foreach (var field in row.GetFields())
            {
                cw.Indented("public ");
                
                var enumField = field as IEnumTypeField;
                if (enumField != null && enumField.EnumType != null)
                {
                    HandleMemberType(enumField.EnumType);
                    sb.Append('?');
                }
                else
                {
                    var dataType = field.ValueType;
                    HandleMemberType(dataType);
                }

                sb.Append(" ");
                sb.Append(field.PropertyName ?? field.Name);
                sb.AppendLine(" { get; set; }");
            }
        }

        static Type GetFirstDerivedOfGenericType(Type type, Type genericType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
                return type;

            if (type.BaseType != null)
                return GetFirstDerivedOfGenericType(type.BaseType, genericType);

            return null;
        }

        private Type GetParentClass(Type type)
        {
            if (typeof(ListRequest).IsAssignableFrom(type))
                return typeof(ListRequest);
            else if (GetFirstDerivedOfGenericType(type, typeof(ListResponse<>)) != null)
                return typeof(ListResponse<>).MakeGenericType(GetFirstDerivedOfGenericType(type, typeof(ListResponse<>)).GetGenericArguments()[0]);
            else if (typeof(RetrieveRequest).IsAssignableFrom(type))
                return typeof(RetrieveRequest);
            else if (GetFirstDerivedOfGenericType(type, typeof(RetrieveResponse<>)) != null)
                return typeof(RetrieveResponse<>).MakeGenericType(GetFirstDerivedOfGenericType(type, typeof(RetrieveResponse<>)).GetGenericArguments()[0]);
            else if (GetFirstDerivedOfGenericType(type, typeof(SaveRequest<>)) != null)
                return typeof(SaveRequest<>).MakeGenericType(GetFirstDerivedOfGenericType(type, typeof(SaveRequest<>)).GetGenericArguments()[0]);
            else if (typeof(DeleteRequest).IsAssignableFrom(type))
                return typeof(DeleteRequest);
            else if (typeof(DeleteResponse).IsAssignableFrom(type))
                return typeof(DeleteResponse);
            else if (typeof(UndeleteRequest).IsAssignableFrom(type))
                return typeof(UndeleteRequest);
            else if (typeof(UndeleteResponse).IsAssignableFrom(type))
                return typeof(UndeleteResponse);
            else if (typeof(UpdateResponse).IsAssignableFrom(type))
                return typeof(UpdateResponse);
            else if (typeof(CreateResponse).IsAssignableFrom(type))
                return typeof(CreateResponse);
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
            sb.Append(MakeFriendlyName(type));

            var parentClass = GetParentClass(type);
            if (parentClass != null)
            {
                sb.Append(" : ");
                sb.Append(MakeFriendlyName(parentClass));
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
                        if (jsonProperty != null)
                        {
                            cw.Indented("[ScriptName(\"");
                            sb.Append(jsonProperty.PropertyName);
                            sb.AppendLine("\")]");
                        }

                        cw.Indented("public ");

                        HandleMemberType(memberType);

                        sb.Append(" ");
                        sb.Append(memberName);
                        sb.AppendLine(" { get; set; }");
                    }
                }
            });
        }
    }
}