using Newtonsoft.Json;
using Serenity.Data;
using Serenity.Services;
using System;
using System.Collections;
using System.Collections.Generic;
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

            sb.Append("namespace ");
            sb.AppendLine(anamespace);
            cw.InBrace(delegate
            {
                foreach (var ns in
                    new string[] {
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

                foreach (var fromType in assembly.GetTypes())
                {
                    if (fromType.IsAbstract)
                        continue;

                    if (fromType.IsSubclassOf(typeof(ServiceRequest)) ||
                        fromType.IsSubclassOf(typeof(ServiceResponse)) ||
                        fromType.IsSubclassOf(typeof(Row)))
                        EnqueueType(fromType);
                }

                int i = 0;
                while (generateQueue.Count > 0)
                {
                    if (i++ > 0)
                        sb.AppendLine();

                    GenerateCodeFor(generateQueue.Dequeue());
                }
            });

            return sb.ToString();
        }

        private void HandleMemberType(Type memberType)
        {
            HandleMemberType(sb, memberType, t => EnqueueType(t));
        }

        public static void HandleMemberType(StringBuilder code, Type memberType, Action<Type> enqueueType = null)
        {
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
                code.Append("List<String>");
                return;
            }

            if (memberType == typeof(HashSet<string>))
            {
                code.Append("List<String>");
                return;
            }

            if (memberType == typeof(Dictionary<string, string>))
            {
                code.Append("Dictionary<String, String>");
                return;
            }

            if (memberType == typeof(Object))
            {
                code.Append("Object");
                return;
            }

            if (memberType.IsGenericType &&
                memberType.GetGenericTypeDefinition() == typeof(List<>))
            {
                code.Append("List<");
                HandleMemberType(code, memberType.GenericTypeArguments[0], enqueueType);
                code.Append(">");
                return;
            }

            if (memberType.IsArray)
            {
                code.Append("List<");
                HandleMemberType(code, memberType.GetElementType(), enqueueType);
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
                foreach (var argument in type.GetGenericArguments())
                {
                    var arg = MakeFriendlyName(argument);
                    if (arg.EndsWith("Row"))
                        arg = arg.Substring(0, arg.Length - 3);
                    sb.Append(arg);
                }

                var name = type.GetGenericTypeDefinition().Name;
                var idx = name.IndexOf('`');
                if (idx >= 0)
                    name = name.Substring(0, idx);

                sb.Append(name);

                return sb.ToString();
            }

            return type.Name;
        }

        private void GenerateEnum(Type enumType)
        {
            cw.IndentedLine("[JsonConverter(typeof(StringEnumConverter))]");
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

        private void GenerateCodeFor(Type type)
        {
            if (type.IsEnum)
            {
                GenerateEnum(type);
                return;
            }

            cw.Indented("public class ");
            sb.AppendLine(MakeFriendlyName(type));
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
                            cw.Indented("[JsonProperty(\"");
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