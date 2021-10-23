using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.CodeGeneration
{
    public partial class ClientTypesGenerator : ImportGeneratorBase
    {
        internal static string GetOptionTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return "object";

            switch (typeName)
            {
                case "number": return "double";
                case "string": return "string";
                case "Date": return "DateTime";
                case "boolean": return "bool";
            }

            var nullablePrefix = "System.Nullable`1";
            bool nullable = typeName.StartsWith(nullablePrefix, StringComparison.Ordinal);
            if (nullable)
                typeName = typeName.Substring(nullablePrefix.Length + 1,
                    typeName.Length - nullablePrefix.Length - 2);

            var systemType = Type.GetType(typeName);
            if (systemType == null)
                return "object";

            if (typeName.StartsWith("System.", StringComparison.Ordinal))
                return SystemTypes.ToCSKeyword(typeName[7..]);

            return typeName;
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

                var typeName = GetOptionTypeName(option.Type);

                sb.AppendLine();
                cw.Indented("public ");
                sb.Append(typeName);
                sb.Append(' ');

                string jsName = option.Name;
                string optionName = option.Name;
                if (option is ExternalProperty prop)
                    jsName = GetPropertyScriptName(prop, preserveMemberCase);
                else if (type.Origin == ExternalTypeOrigin.TS)
                {
                    if (option is ExternalMethod emo && emo.Arguments.Count == 1)
                    {
                        if (jsName.StartsWith("set_", StringComparison.Ordinal))
                        {
                            jsName = jsName[4..];
                            optionName = optionName[4..];
                        }

                        typeName = GetOptionTypeName(emo.Arguments[0].Type);
                    }
                }

                if (char.IsLower(optionName[0]))
                {
                    if (optionName == "id")
                        optionName = "ID";
                    else
                        optionName = char.ToUpperInvariant(optionName[0]) +
                            optionName[1..];
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

        private static void AddOptionMembers(SortedDictionary<string, ExternalMember> dict,
            ExternalType type, bool isOptions)
        {
            List<ExternalMember> members = new();

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

                    if (member is not ExternalProperty prop)
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

                if (member.Type.StartsWith("System.Func`", StringComparison.Ordinal) ||
                    member.Type.StartsWith("System.Action`", StringComparison.Ordinal) ||
                    member.Type == "System.Delegate" ||
                    member.Type.Contains("System.TypeOption", StringComparison.Ordinal) ||
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
                    if (optionsType != null)
                        AddOptionMembers(result, optionsType, isOptions: true);
                }
            }

            int loop = 0;
            do
            {
                if (type.Namespace.StartsWith("System", StringComparison.Ordinal))
                    break;

                AddOptionMembers(result, type, isOptions: false);
            }
            while ((type = GetBaseType(type)) != null && loop++ < 100);

            return result;
        }
    }
}
