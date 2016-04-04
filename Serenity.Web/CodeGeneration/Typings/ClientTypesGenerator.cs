using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serenity.CodeGeneration
{
    public class ClientTypesGenerator : ImportGeneratorBase
    {
        protected override void GenerateAll()
        {
            foreach (var type in ssTypes.Values)
                GenerateType(type);

            foreach (var p in tsTypes)
                if (!ssTypes.ContainsKey(p.Key))
                    GenerateType(p.Value);
        }

        private string GetNamespace(string ns)
        {
            if (ns == "Serenity")
                return "Serenity.ComponentModel";

            return ns;
        }

        private void GenerateOptionMembers(ExternalType type,
            HashSet<string> skip, bool isWidget)
        {
            bool preserveMemberCase = type.Attributes.Any(x =>
                x.Type == "System.Runtime.CompilerServices.PreserveMemberCaseAttribute");

            var options = GetOptionMembers(type, isWidget);

            foreach (var option in options.Values)
            {
                if (skip != null &&
                    skip.Contains(option.Name))
                    continue;

                var typeName = FormatterTypeGenerator.GetOptionTypeName(option.Type);

                sb.AppendLine();
                cw.Indented("public ");
                sb.Append(typeName);
                sb.Append(" ");

                string jsName = option.Name;
                string optionName = option.Name;
                var prop = option as ExternalProperty;
                if (prop != null)
                    jsName = GetPropertyScriptName(prop, preserveMemberCase);
                else if (type.Origin == ExternalTypeOrigin.TS)
                {
                    var emo = option as ExternalMethod;
                    if (emo != null && emo.Arguments.Count == 1)
                    {
                        if (jsName.StartsWith("set_"))
                        {
                            jsName = jsName.Substring(4);
                            optionName = optionName.Substring(4);
                        }

                        typeName = FormatterTypeGenerator.GetOptionTypeName(emo.Arguments[0].Type);
                    }

                    if (Char.IsLower(jsName[0]))
                    {
                        if (optionName == "id")
                            optionName = "ID";
                        else
                            optionName = Char.ToUpperInvariant(optionName[0]) +
                                optionName.Substring(1);
                    }
                }

                sb.AppendLine(optionName);

                cw.InBrace(() =>
                {
                    cw.Indented("get { return GetOption<");
                    sb.Append(typeName);
                    sb.Append(">(\"");
                    sb.Append(jsName);
                    sb.AppendLine("\"); }");
                    cw.Indented("set { SetOption(\"");
                    sb.Append(jsName);
                    sb.AppendLine("\", value); }");
                });
            }
        }

        static HashSet<string> lookupEditorBaseOptions;

        static ClientTypesGenerator()
        {
            lookupEditorBaseOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            lookupEditorBaseOptions.AddRange(typeof(LookupEditorBaseAttribute)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => x.Name));
        }

        private void GenerateEditor(ExternalType type, string name)
        {
            cw.Indented("public partial class ");
            sb.Append(name);

            bool isLookupEditor = HasBaseType(type, "Serenity.LookupEditorBase`1");

            sb.AppendLine(isLookupEditor ?
                " : LookupEditorBaseAttribute" : " : CustomEditorAttribute");

            cw.InBrace(delegate
            {
                cw.Indented("public const string Key = \"");
                sb.Append(type.FullName);
                sb.AppendLine("\";");
                sb.AppendLine();

                cw.Indented("public ");
                sb.Append(name);
                sb.AppendLine("()");
                cw.IndentedLine("    : base(Key)");
                cw.IndentedLine("{");
                cw.IndentedLine("}");

                GenerateOptionMembers(type, 
                    skip: isLookupEditor ? lookupEditorBaseOptions : null,
                    isWidget: true);
            });
        }

        private void GenerateFormatter(ExternalType type, string name)
        {
            cw.Indented("public partial class ");
            sb.Append(name);
            sb.AppendLine(" : CustomFormatterAttribute");

            cw.InBrace(delegate
            {
                cw.Indented("public const string Key = \"");
                sb.Append(type.FullName);
                sb.AppendLine("\";");
                sb.AppendLine();

                cw.Indented("public ");
                sb.Append(name);
                sb.AppendLine("()");
                cw.IndentedLine("    : base(Key)");
                cw.IndentedLine("{");
                cw.IndentedLine("}");

                GenerateOptionMembers(type, skip: null, isWidget: false);
            });
        }

        private void AddOptionMembers(SortedDictionary<string, ExternalMember> dict,
            ExternalType type, bool isOptions)
        {
            List<ExternalMember> members = new List<ExternalMember>();

            members.AddRange(type.Properties);

            if (type.Origin == ExternalTypeOrigin.TS)
            {
                members.AddRange(type.Fields);
                members.AddRange(type.Methods.Where(x => x.Arguments.Count == 1));
            }

            foreach (var member in members)
            {
                if (dict.ContainsKey(member.Name))
                    continue;

                if (type.Origin == ExternalTypeOrigin.SS)
                {
                    if (member.Attributes.Any(x => x.Type == "System.ComponentModel.HiddenAttribute"))
                        continue;

                    var prop = member as ExternalProperty;
                    if (prop == null)
                        continue;

                    if (string.IsNullOrEmpty(prop.GetMethod) &&
                        string.IsNullOrEmpty(prop.SetMethod))
                        return;

                    var getMethod = type.Methods.FirstOrDefault(x => x.Name == prop.GetMethod);
                    var setMethod = type.Methods.FirstOrDefault(x => x.Name == prop.SetMethod);
                    if (getMethod == null || setMethod == null || getMethod.IsProtected || setMethod.IsProtected)
                        continue;
                }
                else if (type.Origin == ExternalTypeOrigin.TS)
                {
                }

                if (member.Type.StartsWith("System.Func`") ||
                    member.Type.StartsWith("System.Action`") ||
                    member.Type == "System.Delegate" ||
                    member.Type.Contains("System.TypeOption") ||
                    member.Type == "Function")
                    continue;

                if (!isOptions &&
                    !member.Attributes.Any(x =>
                        x.Type == "System.ComponentModel.DisplayNameAttribute" ||
                        x.Type == "Serenity.OptionAttribute" ||
                        x.Type == "Serenity.Decorators.option" ||
                        x.Type == "Serenity.Decorators.displayName"))
                    continue;

                dict[member.Name] = member;
            }
        }

        private SortedDictionary<string, ExternalMember> GetOptionMembers(ExternalType type,
            bool isWidget)
        {
            var result = new SortedDictionary<string, ExternalMember>();

            var constructor = type.Methods.FirstOrDefault(x => x.IsConstructor && 
                x.Arguments.Count == (isWidget ? 2 : 1));

            if (constructor != null)
            {
                if (!isWidget ||
                    (constructor.Arguments[0].Type == "jQueryApi.jQueryObject" ||
                     constructor.Arguments[0].Type == "JQuery"))
                {
                    var optionsType = GetScriptType(constructor.Arguments[isWidget ? 1 : 0].Type);
                    AddOptionMembers(result, optionsType, isOptions: true);
                }
            }

            int loop = 0;
            do
            {
                if (type.Namespace.StartsWith("System"))
                    break;

                AddOptionMembers(result, type, isOptions: false);
            }
            while ((type = GetBaseType(type)) != null && loop++ < 100);

            return result;
        }

        private void GenerateType(ExternalType type)
        {
            bool isEditorType = IsEditorType(type);
            bool isFormatterType = IsFormatterType(type);

            if (!isEditorType && !isFormatterType)
                return;

            AppendUsings(new string[] {
                "Serenity",
                "Serenity.ComponentModel",
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.ComponentModel"
            });

            sb.AppendLine();

            var ns = GetNamespace(type.Namespace);
            string name = type.Name + "Attribute";

            cw.Indented("namespace ");
            sb.AppendLine(ns);

            cw.InBrace(delegate
            {
                if (isEditorType)
                    GenerateEditor(type, name);
                else if (isFormatterType)
                    GenerateFormatter(type, name);
            });

            AddFile(RemoveRootNamespace(ns, name) + ".cs");
        }

        private bool IsEditorType(ExternalType type)
        {
            if (type.IsAbstract)
                return false;

            if (type.GenericParameters.Count > 0)
                return false;

            if (!HasBaseType(type, "Serenity.Widget"))
                return false;

            if (type.AssemblyName != null &&
                type.AssemblyName.StartsWith("Serenity."))
                return false;

            return GetAttribute(type, "Serenity.EditorAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.ElementAttribute", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.editor", inherited: true) != null ||
                GetAttribute(type, "Serenity.Decorators.element", inherited: true) != null;
        }

        private bool IsFormatterType(ExternalType type)
        {
            if (type.IsAbstract)
                return false;

            if (type.GenericParameters.Count > 0)
                return false;

            if (type.AssemblyName != null &&
                type.AssemblyName.StartsWith("Serenity."))
                return false;

            return type.Interfaces.Any(x =>
                x == "Serenity.ISlickFormatter" ||
                x == "Slick.Formatter");
        }
    }
}
