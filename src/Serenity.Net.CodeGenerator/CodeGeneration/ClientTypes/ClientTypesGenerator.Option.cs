namespace Serenity.CodeGeneration;

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
            return CodeWriter.ToCSKeyword(typeName[7..]);

        return typeName;
    }

    private void GenerateOptionMembers(ExternalType type, HashSet<string> skip)
    {
        bool preserveMemberCase = type.Attributes != null && type.Attributes.Any(x =>
            x.Type == "System.Runtime.CompilerServices.PreserveMemberCaseAttribute");

        var options = GetOptionMembers(type);

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
            else 
            {
                if (option is ExternalMethod emo && emo.Arguments?.Count == 1)
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
        List<ExternalMember> members = [];

        if (type.Fields != null)
            members.AddRange(type.Fields);

        if (type.Methods != null)
            members.AddRange(type.Methods.Where(x => x.Arguments?.Count == 1));

        foreach (var member in members)
        {
            if (dict.ContainsKey(member.Name))
                continue;

            if (member.Type?.StartsWith("System.Func`", StringComparison.Ordinal) == true ||
                member.Type?.StartsWith("System.Action`", StringComparison.Ordinal) == true ||
                member.Type == "System.Delegate" ||
                member.Type?.Contains("System.TypeOption", StringComparison.Ordinal) == true ||
                member.Type == "Function")
                continue;

            if (!isOptions &&
                (member.Attributes == null ||
                 !member.Attributes.Any(x =>
                    x.Type == "System.ComponentModel.DisplayNameAttribute" ||
                    x.Type == "Serenity.OptionAttribute" ||
                    x.Type == "Serenity.Decorators.option" ||
                    x.Type == "Decorators.option" ||
                    x.Type == "option" ||
                    x.Type == "@serenity-is/corelib:Decorators.option" ||
                    x.Type == "Serenity.Decorators.displayName")))
                continue;

            dict[member.Name] = member;
        }
    }

    private static readonly HashSet<string> PossibleNodeArguments = [
        "jQueryApi.jQueryObject",
        "JQuery",
        "HTMLElement",
        "JQuery | HTMLElement",
        "WidgetNode",
        "jQueryLike",
        "jQueryInstance"
    ];


    private ExternalType GetOptionsTypeFor(ExternalType type)
    {
        if (type is null)
            return null;
        var constructors = type.Methods?.Where(x => x.IsConstructor == true) ?? [];
        var argument = constructors.Where(x => x.Arguments != null)
            .SelectMany(x => x.Arguments.Where(x => !PossibleNodeArguments.Contains(x.Type)))
            .FirstOrDefault();

        if (argument != null)
        {
            var optionsType = GetScriptTypeFrom(type, argument.Type);
            if (!string.IsNullOrEmpty(argument.GenericArguments) &&
                (argument.Type == "@serenity-is/corelib:WidgetProps" ||
                 argument.Type == "Serenity.WidgetProps" ||
                 argument.Type == "@serenity-is/corelib:EditorProps" ||
                 argument.Type == "Serenity.EditorProps" ||
                 (optionsType == null && 
                  (argument.Type == "WidgetProps" || 
                   argument.Type == "EditorProps"))))
            {
                var genericParam = type.GenericParameters?.FirstOrDefault(x => x.Name == argument.GenericArguments);
                if (genericParam != null)
                {
                    if (!string.IsNullOrEmpty(genericParam.Default))
                        optionsType = GetScriptTypeFrom(type, genericParam.Default);
                    if (optionsType != null)
                        return optionsType;
                    return !string.IsNullOrEmpty(genericParam.Extends) ?
                        GetScriptTypeFrom(type, genericParam.Extends) : null;
                }
                else
                {
                    optionsType = GetScriptTypeFrom(optionsType ?? type, argument.GenericArguments) ??
                        GetScriptTypeFrom(type, argument.GenericArguments) ??
                        optionsType;
                }
            }

            return optionsType;
        }

        if (!constructors.Any() && (type = GetBaseType(type)) != null)
            return GetOptionsTypeFor(type);

        return null;
    }

    private SortedDictionary<string, ExternalMember> GetOptionMembers(ExternalType type)
    {
        var result = new SortedDictionary<string, ExternalMember>();
        var optionsType = GetOptionsTypeFor(type);
        if (optionsType != null)
            AddOptionMembers(result, optionsType, isOptions: true);
        
        int loop = 0;
        do
        {
            if (type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true)
                break;

            AddOptionMembers(result, type, isOptions: false);
        }
        while ((type = GetBaseType(type)) != null && loop++ < 100);

        return result;
    }
}
