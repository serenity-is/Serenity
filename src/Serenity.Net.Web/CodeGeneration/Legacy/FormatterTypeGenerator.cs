using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Serenity.CodeGeneration
{
    public class FormatterTypeInfo
    {
        public Dictionary<string, FormatterOptionInfo> Options { get; set; }
    }

    public class FormatterOptionInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class FormatterTypeGenerator
    {
        public FormatterTypeGenerator()
        {
            FormatterTypes = new Dictionary<string, FormatterTypeInfo>();

            UsingNamespaces = new HashSet<string>
            {
                "Serenity",
                "Serenity.ComponentModel",
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.ComponentModel"
            };

            RootNamespaces = new HashSet<string>
            {
                "Serenity"
            };
        }

        public Func<string, string> GetTypeName { get; set; }
        public HashSet<string> UsingNamespaces { get; private set; }
        public Dictionary<string, FormatterTypeInfo> FormatterTypes { get; private set; }
        public HashSet<string> RootNamespaces { get; private set; }

        private string DoGetTypeName(string typeName)
        {
            if (GetTypeName != null)
                return GetTypeName(typeName);

            var parts = typeName.Split(new char[] { '.' });

            return parts[parts.Length - 1] + "Attribute";
        }

        private string DoGetNamespace(string typeName)
        {
            var parts = typeName.Split(new char[] { '.' });
            var result = String.Join(".", parts.Take(parts.Length - 1).ToArray());
            if (result == "Serenity")
                result = "Serenity.ComponentModel";
            return result;
        }

        internal static string GetOptionTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return "object";

            switch (typeName) {
                case "number": return "Double";
                case "string": return "String";
                case "Date": return "DateTime";
                case "boolean": return "Boolean";
            }

            var nullablePrefix = "System.Nullable`1";
            bool nullable = typeName.StartsWith(nullablePrefix);
            if (nullable)
                typeName = typeName.Substring(nullablePrefix.Length + 1, 
                    typeName.Length - nullablePrefix.Length - 2);

            var systemType = Type.GetType(typeName);
            if (systemType == null)
                return "object";

            if (typeName.StartsWith("System."))
                return typeName.Substring(7);

            return typeName;
        }

        public string GenerateCode()
        {
            var codeByType = new Dictionary<string, string>();
            var sb = new StringBuilder();
            var cw = new Serenity.Reflection.CodeWriter(sb, 4);

            foreach (var key in FormatterTypes.Keys)
            {
                var ns = DoGetNamespace(key);

                var formatterInfo = FormatterTypes[key];

                sb.Clear();
                cw.Indented("public partial class ");
                var type = DoGetTypeName(key);
                sb.Append(type);
                sb.AppendLine(" : CustomFormatterAttribute");
                cw.InBrace(delegate
                {
                    cw.Indented("public const string Key = \"");
                    sb.Append(key);
                    sb.AppendLine("\";");
                    sb.AppendLine();

                    cw.Indented("public ");
                    sb.Append(type);
                    sb.AppendLine("()");
                    cw.IndentedLine("    : base(Key)");
                    cw.IndentedLine("{");
                    cw.IndentedLine("}"); 
                    
                    var opt = formatterInfo.Options.Keys.ToList();
                    opt.Sort();

                    foreach (var item in opt)
                    {
                        var option = formatterInfo.Options[item];
                        var typeName = GetOptionTypeName(option.Type);

                        sb.AppendLine();
                        cw.Indented("public ");
                        sb.Append(typeName);
                        sb.Append(" ");
                        sb.AppendLine(item);
                        cw.InBrace(() =>
                        {
                            string propName = item.Substring(0, 1).ToLowerInvariant() + item.Substring(1);
                            if (item == "ID")
                                propName = "id";
                            cw.Indented("get { return GetOption<");
                            sb.Append(typeName);
                            sb.Append(">(\"");
                            sb.Append(propName);
                            sb.AppendLine("\"); }");
                            cw.Indented("set { SetOption(\"");
                            sb.Append(propName);
                            sb.AppendLine("\", value); }");
                        });
                    }
                });

                codeByType[key] = sb.ToString();
                sb.Clear();
            }

            sb.Clear();
            sb.AppendLine();

            foreach (var ns in UsingNamespaces)
            {
                cw.Indented("using ");
                sb.Append(ns);
                sb.AppendLine(";");
            }

            var ordered = codeByType.Keys.OrderBy(x => DoGetNamespace(x)).ThenBy(x => x);
            var byNameSpace = ordered.ToLookup(x => DoGetNamespace(x));

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                sb.AppendLine();
                cw.Indented("namespace ");
                sb.AppendLine(ns.Key);
                cw.InBrace(delegate
                {
                    int i = 0;
                    foreach (var type in ns)
                    {
                        if (i++ > 0)
                            sb.AppendLine();

                        cw.IndentedMultiLine(codeByType[type].TrimEnd());
                    }
                });
            }

            return sb.ToString();
        }
    }
}