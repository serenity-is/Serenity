using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Serenity.ComponentModel;

namespace Serenity.CodeGeneration
{
    public class ScriptFormGenerator
    {
        public ScriptFormGenerator(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
                throw new ArgumentNullException("assembly");

            this.Assemblies = assemblies;

            WidgetTypes = new HashSet<string>();

            UsingNamespaces = new HashSet<string>
            {
                "Serenity",
                "Serenity.ComponentModel",
                "System",
                "System.Runtime.CompilerServices"
            };

            RootNamespaces = new HashSet<string>
            {
                "Serenity"
            };
        }

        public Assembly[] Assemblies { get; private set; }
        public Func<Type, string> GetTypeName { get; set; }
        public Func<Type, string> GetNamespace { get; set; }
        public HashSet<string> UsingNamespaces { get; private set; }
        public HashSet<string> WidgetTypes { get; private set; }
        public HashSet<string> RootNamespaces { get; private set; }

        private string DoGetTypeName(Type type)
        {
            if (GetTypeName != null)
                return GetTypeName(type);

            return type.Name;
        }

        private string DoGetNamespace(Type type)
        {
            if (GetNamespace != null)
                return GetNamespace(type);

            return type.Namespace ?? "";
        }

        public SortedDictionary<string, string> GenerateCode()
        {
            var codeByType = new Dictionary<Type, string>();
            var sb = new StringBuilder();
            var cw = new Serenity.Reflection.CodeWriter(sb, 4);

            foreach (var assembly in Assemblies)
            foreach (var type in assembly.GetTypes())
            {
                var formScriptAttribute = type.GetCustomAttribute<FormScriptAttribute>();
                if (formScriptAttribute == null)
                    continue;

                var ns = DoGetNamespace(type);

                sb.Clear();
                cw.IndentedLine("[Imported]");
                cw.Indented("public partial class ");
                sb.Append(DoGetTypeName(type));
                sb.AppendLine(" : PrefixedContext");
                cw.InBrace(delegate
                {
                    cw.Indented("[InlineConstant] public const string FormKey = \"");
                    sb.Append(formScriptAttribute.Key);
                    sb.AppendLine("\";");
                    sb.AppendLine();

                    cw.IndentedLine("[InlineCode(\"new Serenity.PrefixedContext({idPrefix})\")]");
                    cw.Indented("public ");
                    sb.Append(DoGetTypeName(type));
                    sb.AppendLine("(string idPrefix) : base(idPrefix) {}");
                    sb.AppendLine();

                    foreach (var item in Serenity.PropertyGrid.PropertyItemHelper.GetPropertyItemsFor(type))
                    {
                        var editorType = item.EditorType ?? "String";
                        string widgetTypeName = null;
                        foreach (var rootNamespace in RootNamespaces)
                        {
                            string wn = rootNamespace + "." + editorType;
                            if (WidgetTypes.Contains(wn))
                            {
                                widgetTypeName = wn;
                                break;
                            }

                            wn += "Editor";
                            if (WidgetTypes.Contains(wn))
                            {
                                widgetTypeName = wn;
                                break;
                            }
                        }

                        if (widgetTypeName == null)
                        {
                            var wn = editorType;
                            if (!WidgetTypes.Contains(editorType))
                                wn = editorType + "Editor";

                            if (WidgetTypes.Contains(wn))
                                widgetTypeName = wn;
                        }

                        if (widgetTypeName == null)
                            continue;

                        var fullName = widgetTypeName;

                        if (widgetTypeName.StartsWith(ns + "."))
                            widgetTypeName = widgetTypeName.Substring(ns.Length + 1);
                        else
                        {
                            foreach (var rn in RootNamespaces)
                            {
                                if (widgetTypeName.StartsWith(rn + "."))
                                    widgetTypeName = widgetTypeName.Substring(rn.Length + 1);
                            }
                        }

                        cw.Indented("public ");
                        sb.Append(widgetTypeName);
                        sb.Append(" ");
                        sb.Append(item.Name);
                        sb.Append(" { ");
                        sb.Append("[InlineCode(\"{this}.w('");
                        sb.Append(item.Name);
                        sb.Append("', ");
                        sb.Append(fullName);
                        sb.AppendLine(")\")] get; private set; }");
                    }
                });

                codeByType[type] = sb.ToString();
                sb.Clear();
            }

            var ordered = codeByType.Keys.OrderBy(x => DoGetNamespace(x)).ThenBy(x => x.Name);
            var byNameSpace = ordered.ToLookup(x => DoGetNamespace(x));

            var result = new SortedDictionary<string, string>();

            foreach (var ns in byNameSpace.ToArray().OrderBy(x => x.Key))
            {
                foreach (var type in ns)
                {
                    sb.Clear();
                    sb.AppendLine();
                    cw.Indented("namespace ");
                    sb.AppendLine(ns.Key);
                    cw.InBrace(delegate
                    {
                        foreach (var usingNamespace in UsingNamespaces.ToArray().OrderBy(x => x))
                        {
                            cw.Indented("using ");
                            sb.Append(usingNamespace);
                            sb.AppendLine(";");
                        }

                        sb.AppendLine();

                        int i = 0;

                        {
                            if (i++ > 0)
                                sb.AppendLine();

                            cw.IndentedMultiLine(codeByType[type].TrimEnd());
                        }
                    });

                    var filename = ns.Key + "." + DoGetTypeName(type) + ".cs";

                    foreach (var rn in RootNamespaces)
                    {
                        if (filename.StartsWith(rn + "."))
                            filename = filename.Substring(rn.Length + 1);
                    }

                    result.Add(filename, sb.ToString());
                }
            }

            return result;
        }
    }
}