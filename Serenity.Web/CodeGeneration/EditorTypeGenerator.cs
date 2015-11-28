using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serenity.Reflection;
using Serenity.ComponentModel;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public class EditorTypeInfo
    {
        public Dictionary<string, EditorOptionInfo> Options { get; set; }
    }

    public class EditorOptionInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class EditorTypeGenerator
    {
        public EditorTypeGenerator()
        {
            EditorTypes = new Dictionary<string, EditorTypeInfo>();

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
        public Dictionary<string, EditorTypeInfo> EditorTypes { get; private set; }
        public HashSet<string> RootNamespaces { get; private set; }

        private static readonly string[] lookupEditorBaseOptions =
            typeof(LookupEditorBaseAttribute)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => x.Name)
                .ToArray();

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

        public string GenerateCode()
        {
            var codeByType = new Dictionary<string, string>();
            var sb = new StringBuilder();
            var cw = new Serenity.Reflection.CodeWriter(sb, 4);

            foreach (var key in EditorTypes.Keys)
            {
                var ns = DoGetNamespace(key);

                var editorInfo = EditorTypes[key];

                sb.Clear();
                cw.Indented("public partial class ");
                var type = DoGetTypeName(key);
                sb.Append(type);

                // yes it's ugly, but backward compatible
                if (editorInfo.Options.Count >= lookupEditorBaseOptions.Length &&
                    lookupEditorBaseOptions.All(x => editorInfo.Options.ContainsKey(x)))
                {
                    sb.AppendLine(" : LookupEditorBaseAttribute");
                    foreach (var x in lookupEditorBaseOptions)
                        editorInfo.Options.Remove(x);
                }
                else
                    sb.AppendLine(" : CustomEditorAttribute");

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
                    var opt = editorInfo.Options.Keys.ToList();
                    opt.Sort();

                    foreach (var item in opt)
                    {
                        var option = editorInfo.Options[item];
                        var typeName = option.Type;
                        var nullablePrefix = "System.Nullable`1";
                        bool nullable = option.Type.StartsWith(nullablePrefix);
                        if (nullable)
                            typeName = typeName.Substring(nullablePrefix.Length + 1, typeName.Length - nullablePrefix.Length - 2);

                        var systemType = Type.GetType(typeName);
                        if (systemType == null)
                            typeName = "object";
                        else if (typeName.StartsWith("System."))
                            typeName = typeName.Substring(7);

                        sb.AppendLine();
                        cw.Indented("public ");
                        sb.Append(typeName);
                        //if (nullable) //Attribute name argument nullable problem
                        //    sb.Append("?");
                        sb.Append(" ");
                        sb.AppendLine(item);
                        cw.InBrace(() =>
                        {
                            string propName = item.Substring(0, 1).ToLowerInvariant() + item.Substring(1);
                            if (item == "ID")
                                propName = "id";
                            cw.Indented("get { return GetOption<");
                            sb.Append(typeName);
                            //if (nullable) //Attribute name argument nullable problem
                            //    sb.Append("?");
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