using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Serenity.Reflection
{
    public class ScriptFormGenerator
    {
        public ScriptFormGenerator(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.Assembly = assembly;

            WidgetTypes = new HashSet<string>();

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
                "Serenity"
            };
        }

        public Assembly Assembly { get; private set; }
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

            return type.Namespace;
        }

        public string GenerateCode()
        {
            var codeByType = new Dictionary<Type, string>();
            var sb = new StringBuilder();
            var cw = new Serenity.Reflection.CodeWriter(sb, 4);

            foreach (var type in this.Assembly.GetTypes())
            {
                if (type.GetCustomAttribute(typeof(Serenity.FormScriptAttribute)) == null)
                    continue;

                var ns = DoGetNamespace(type);

                sb.Clear();
                cw.Indented("public partial class ");
                sb.Append(DoGetTypeName(type));
                sb.AppendLine(" : PrefixedContext");
                cw.InBrace(delegate
                {
                    cw.Indented("public ");
                    sb.Append(DoGetTypeName(type));
                    sb.AppendLine("(string idPrefix) : base(idPrefix) {}");
                    sb.AppendLine();

                    foreach (var item in Serenity.Web.PropertyEditor.PropertyEditorHelper.GetPropertyItemsFor(type))
                    {
                        var editorType = item.EditorType;
                        string widgetTypeName = null;
                        foreach (var rootNamespace in RootNamespaces)
                        {
                            string wn = rootNamespace + "." + editorType;
                            if (WidgetTypes.Contains(wn))
                            {
                                widgetTypeName = wn;
                                UsingNamespaces.Add(rootNamespace);
                                break;
                            }

                            wn += "Editor";
                            if (WidgetTypes.Contains(wn))
                            {
                                widgetTypeName = wn;
                                UsingNamespaces.Add(rootNamespace);
                                break;
                            }
                        }

                        if (widgetTypeName == null)
                            continue;

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
                        sb.Append(" { get { return ById<");
                        sb.Append(widgetTypeName);
                        sb.Append(">(\"");
                        sb.Append(item.Name);
                        sb.AppendLine("\"); } }");
                    }
                });

                codeByType[type] = sb.ToString();
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

            var ordered = codeByType.Keys.OrderBy(x => DoGetNamespace(x)).ThenBy(x => x.Name);
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