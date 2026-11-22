namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private string? GetEnumKeyFor(TypeDefinition enumType)
    {
        var enumKeyAttr = TypingsUtils.FindAttr(enumType.GetAttributes(), "Serenity.ComponentModel", "EnumKeyAttribute");
        if (enumKeyAttr != null)
        {
            var args = enumKeyAttr.ConstructorArguments();
            if (args.Count >= 1 &&
                args[0].Type?.FullNameOf() == "System.String")
                return args[0].Value as string;

            if (args.Count == 0)
            {
                return AutoEnumKeyFor(enumType);
            }
        }

        return enumType.FullNameOf();
    }

    private string AutoEnumKeyFor(TypeDefinition type)
    {
        string? module = GetModuleKeyForType(type);
        if (module is null)
        {
            module = type.NamespaceOf() ?? "";

            if (module.EndsWith(".Entities", StringComparison.Ordinal))
                module = module[0..^9];
            else if (module.EndsWith(".Enums", StringComparison.Ordinal))
                module = module[0..^6];

            var idx = module.IndexOf('.');
            if (idx >= 0)
                module = module[(idx + 1)..];
        }

        var name = type.Name;
        return string.IsNullOrEmpty(module) ? name : module + "." + name;
    }    

    /// <summary>
    /// Returns the text and the local text key of an enum member, which its text attributes give, along with whether
    /// the key is empty, i.e. the text is shown as it is, and nothing is registered for it. A member whose key is the one
    /// its enum implies, i.e. "Enums.&lt;enumKey&gt;.&lt;Member&gt;", needs neither to travel to the client.
    /// </summary>
    private static (string? Text, string? TextKey) GetEnumTextFor(FieldDefinition field)
    {
        var textAttr = TypingsUtils.FindAttr(field.GetAttributes(), "Serenity.ComponentModel", "DisplayTextAttribute") ??
            TypingsUtils.FindAttr(field.GetAttributes(), "Serenity.ComponentModel", "DescriptionTextAttribute") ??
            TypingsUtils.FindAttr(field.GetAttributes(), "System.ComponentModel", "DescriptionAttribute");
        if (textAttr is null)
            return (null, null);

        var text = textAttr.ConstructorArguments()?.Count > 0 ? textAttr.ConstructorArguments()[0].Value as string : null;
        string? textKey = null;
        var named = textAttr.NamedArguments();
        if (named != null && named.FirstOrDefault(x => string.Equals(x.Name(), "TextKey")) is { } keyArg)
            textKey = keyArg.ArgumentValue() as string;

        return (text, textKey);
    }

    private static string? GetEnumIconFor(FieldDefinition field)
    {
        var iconAttr = field.HasCustomAttributes() ?
            TypingsUtils.FindAttr(field.GetAttributes(), "Serenity.ComponentModel", "IconClassAttribute") : null;
        if (iconAttr != null &&
            iconAttr.ConstructorArguments().Count >= 1 &&
            iconAttr.ConstructorArguments()[0].Type?.FullNameOf() == "System.String")
            return iconAttr.ConstructorArguments()[0].Value as string;

        return null;
    }

    /// <summary>
    /// Writes the registerEnum call for an enum or a string enumeration. The keys of the icons are the object
    /// literal keys, e.g. a quoted numeric value for an enum or a quoted string for a string enumeration. A [Flags]
    /// enum is registered with enumFlags, so its values are formatted as flags, and an enum editor with the multiple
    /// option gives the members selected as one number.
    /// </summary>
    private void WriteEnumRegistration(string identifier, string fullName, string? enumKey,
        IReadOnlyList<(string Key, string Value)> icons, bool flags = false,
        IReadOnlyList<(string Key, string Value)>? textKeys = null, IReadOnlyList<(string Key, string Value)>? texts = null)
    {
        var registerEnum = ImportFromCorelib("registerEnum");
        cw.Indented($"{registerEnum}(");

        sb.Append(identifier);
        sb.Append(", '");
        sb.Append(fullName);
        sb.Append('\'');

        var hasTextKeys = textKeys?.Count > 0;
        var hasTexts = texts?.Count > 0;

        // the options are only written when a member has an icon, a text key of its own, a literal text, or the enum is
        // a set of flags, so the others keep the simpler calls
        if (icons.Count == 0 && !flags && !hasTextKeys && !hasTexts)
        {
            if (enumKey != fullName)
            {
                sb.Append(", '");
                sb.Append(enumKey);
                sb.AppendLine("');");
            }
            else
                sb.AppendLine(");");

            return;
        }

        sb.AppendLine(", {");
        cw.Block(delegate
        {
            // the options that are written, in their order, each followed by a comma when another follows
            var parts = new List<Action>();

            if (enumKey != fullName)
            {
                parts.Add(delegate
                {
                    cw.Indented("enumKey: '");
                    sb.Append(enumKey);
                    sb.Append('\'');
                });
            }

            if (flags)
                parts.Add(() => cw.Indented("enumFlags: true"));

            if (hasTextKeys)
                parts.Add(() => WriteEnumMap("enumTextKeys", textKeys!));

            if (hasTexts)
                parts.Add(() => WriteEnumMap("enumTexts", texts!));

            if (icons.Count > 0)
                parts.Add(() => WriteEnumMap("enumIcons", icons));

            for (var i = 0; i < parts.Count; i++)
            {
                if (i > 0)
                    sb.AppendLine(",");

                parts[i]();
            }

            sb.AppendLine();
        });
        cw.IndentedLine("});");
    }

    /// <summary>
    /// Writes a map of enum values to strings, e.g. the local text keys of the members of an enum.
    /// </summary>
    private void WriteEnumMap(string name, IReadOnlyList<(string Key, string Value)> entries)
    {
        cw.IndentedLine(name + ": {");
        cw.Block(delegate
        {
            for (var i = 0; i < entries.Count; i++)
            {
                if (i > 0)
                    sb.AppendLine(",");

                cw.Indented(entries[i].Key);
                sb.Append(": ");
                sb.Append(entries[i].Value.ToSingleQuoted());
            }

            sb.AppendLine();
        });
        cw.Indented("}");
    }

    private void GenerateEnum(TypeDefinition enumType)
    {
        var codeNamespace = ScriptNamespaceFor(enumType);
        var enumKey = GetEnumKeyFor(enumType);

        var fields = enumType.FieldsOf().Where(x => x.IsStatic &&
            !x.IsSpecialName() && x.Constant() != null &&
            (!x.HasCustomAttributes() ||
                (TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "IgnoreUIFieldAttribute") == null &&
                 TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute") == null)))
            .Select(x => (Field: x, Value: Convert.ToInt64(x.Constant()!, CultureInfo.InvariantCulture), Icon: GetEnumIconFor(x)))
            .OrderBy(x => x.Value)
            .ToList();

        cw.Indented("export enum ");
        var identifier = MakeFriendlyName(enumType, codeNamespace);
        var fullName = (string.IsNullOrEmpty(codeNamespace) ? "" : codeNamespace + ".") + identifier;
        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);

        cw.InBrace(delegate
        {
            var inserted = 0;
            foreach (var field in fields)
            {
                if (inserted > 0)
                    sb.AppendLine(",");

                cw.Indented(field.Field.Name);
                sb.Append(" = ");
                sb.Append(field.Value);
                inserted++;
            }

            sb.AppendLine();
        });

        // C# allows aliases with the same value, in which case the icon of the first one is used. The keys are
        // quoted as a negative number is not valid as an object key
        var icons = fields.Where(x => !string.IsNullOrEmpty(x.Icon))
            .GroupBy(x => x.Value)
            .Select(group => (Key: $"'{group.Key}'", Value: group.First().Icon!))
            .ToList();

        var flags = TypingsUtils.FindAttr(enumType.GetAttributes(), "System", "FlagsAttribute") != null;

        // a member whose text key is the one its enum implies, i.e. "Enums.<enumKey>.<Member>", needs nothing, as the
        // client can work it out; only one with a key of its own, or with an empty key, is told
        var textKeys = new List<(string Key, string Value)>();
        var texts = new List<(string Key, string Value)>();
        foreach (var field in fields.GroupBy(x => x.Value).Select(group => group.First()))
        {
            var impliedKey = "Enums." + enumKey + "." + field.Field.Name;
            var (text, textKey) = GetEnumTextFor(field.Field);
            var valueKey = $"'{field.Value}'";

            // an empty key is no key: the member is shown as it is, and its text travels with the enum
            if (textKey is { Length: 0 })
            {
                textKeys.Add((valueKey, ""));
                if (text is { Length: > 0 })
                    texts.Add((valueKey, text));
            }
            else if (textKey is { Length: > 0 } && textKey != impliedKey)
                textKeys.Add((valueKey, textKey));
        }

        WriteEnumRegistration(identifier, fullName, enumKey, icons, flags, textKeys, texts);
    }
}
