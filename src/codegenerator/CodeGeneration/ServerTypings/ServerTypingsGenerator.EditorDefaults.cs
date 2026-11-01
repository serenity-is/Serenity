#if ISSOURCEGENERATOR
using Microsoft.CodeAnalysis;
#endif

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private const string EditorDefaultsSuffix = "Defaults";

    /// <summary>
    /// Editor attribute families that support templates, like the server side does at runtime.
    /// </summary>
    protected enum EditorTemplateFamily
    {
        ServiceLookup,
        Lookup
    }

    private static readonly string[] ServiceLookupEditorBaseNames =
    [
        "Serenity.ServiceLookupEditorBase`1",
        "Serenity.ServiceLookupEditorBase",
        "@serenity-is/corelib:ServiceLookupEditorBase",
        "ServiceLookupEditorBase"
    ];

    private static readonly string[] LookupEditorBaseNames =
    [
        "Serenity.LookupEditorBase`1",
        "Serenity.LookupEditorBase",
        "@serenity-is/corelib:LookupEditorBase",
        "LookupEditorBase"
    ];

    /// <summary>
    /// Gets the editor template of a type, which is a service lookup or lookup editor attribute applied to the
    /// class of an editor attribute of the same family, typically to the partial class of an attribute generated
    /// for a client side editor. The server side applies its options as defaults for that editor attribute, and
    /// the client side editor can use the generated defaults when it is created directly.
    /// </summary>
    protected CustomAttribute? GetEditorTemplateAttribute(TypeDefinition type, out EditorTemplateFamily family)
    {
        family = default;

        if (GetEditorAttributeFamily(type) is not EditorTemplateFamily targetFamily)
            return null;

        foreach (var attr in type.GetAttributes())
        {
            if (GetFamilyOfAttributeType(attr.AttributeType()) == targetFamily)
            {
                family = targetFamily;
                return attr;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the family of an editor attribute type from its base classes, or for an attribute generated for a
    /// client side editor, whose base class is not visible to a source generator, from its client side editor.
    /// Other types, including editor attributes of other families, have none.
    /// </summary>
    private EditorTemplateFamily? GetEditorAttributeFamily(TypeDefinition type)
    {
        if (GetFamilyOfAttributeType(type) is EditorTemplateFamily family)
            return family;

        // a class with a visible base class other than object is not an attribute generated for a client side editor
        if (type.EnumerateBaseClasses().Any(x => x.FullNameOf() != "System.Object") ||
            !type.Name.EndsWith("Attribute", StringComparison.Ordinal))
            return null;

        var editorKey = type.NamespaceOf() is { Length: > 0 } ns ?
            ns + "." + type.Name[..^"Attribute".Length] : type.Name[..^"Attribute".Length];

        foreach (var editorType in modularEditorTypeByKey[editorKey])
        {
            if (HasBaseType(editorType, ServiceLookupEditorBaseNames))
                return EditorTemplateFamily.ServiceLookup;

            if (HasBaseType(editorType, LookupEditorBaseNames))
                return EditorTemplateFamily.Lookup;
        }

        return null;
    }

    private static EditorTemplateFamily? GetFamilyOfAttributeType(TypeReference? attributeType)
    {
        if (TypingsUtils.IsOrSubClassOf(attributeType, "Serenity.ComponentModel", "ServiceLookupEditorBaseAttribute"))
            return EditorTemplateFamily.ServiceLookup;

        if (TypingsUtils.IsOrSubClassOf(attributeType, "Serenity.ComponentModel", "LookupEditorBaseAttribute"))
            return EditorTemplateFamily.Lookup;

        return null;
    }

    /// <summary>
    /// Gets the identifier for the editor defaults of an editor attribute type, e.g. SqlObjectEditorDefaults
    /// for SqlObjectEditorAttribute.
    /// </summary>
    protected static string GetEditorDefaultsIdentifier(TypeDefinition type)
    {
        var name = type.Name;
        if (name.EndsWith("Attribute", StringComparison.Ordinal))
            name = name[..^"Attribute".Length];

        return name + EditorDefaultsSuffix;
    }

    private void GenerateEditorDefaults(TypeDefinition type, CustomAttribute template, EditorTemplateFamily family,
        string identifier)
    {
        var codeNamespace = ScriptNamespaceFor(type);
        var options = family == EditorTemplateFamily.Lookup ?
            GetLookupEditorTemplateOptions(template) : GetEditorTemplateOptions(template);

        cw.Indented("export const ");
        sb.Append(identifier);
        sb.Append(": ");
        sb.Append(ImportFromCorelib(family == EditorTemplateFamily.Lookup ?
            "LookupEditorOptions" : "ServiceLookupEditorOptions"));
        sb.Append(" =");
        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);

        cw.InBrace(delegate
        {
            var inserted = 0;
            foreach (var (key, value) in options)
            {
                if (inserted++ > 0)
                    sb.AppendLine(",");

                cw.Indented(key);
                sb.Append(": ");
                AppendEditorOptionValue(value);
            }

            if (inserted > 0)
                sb.AppendLine();
        }, endLine: false);

        sb.AppendLine(";");
    }

    /// <summary>
    /// Determines the client side options of a service lookup editor template like the server side does at
    /// runtime: explicit options first, then the service and capabilities from the endpoint action, and the
    /// id, text and icon fields from the item type. Options that can't be determined statically are omitted.
    /// </summary>
    private List<(string key, object? value)> GetEditorTemplateOptions(CustomAttribute template)
    {
        var options = new List<(string key, object? value)>();
        bool has(string key) => options.Any(x => x.key == key);

        TypeReference? itemType = null;
        TypeReference? endpointType = null;
        string? actionName = null;

        var ctorArgs = template.ConstructorArguments()?.Select(x => NormalizeAttributeValue(ArgumentValueOf(x))).ToArray() ?? [];
        if (ctorArgs.Length == 1 && ctorArgs[0] is TypeReference ctorItemType)
            itemType = ctorItemType;
        else if (ctorArgs.Length == 3 && ctorArgs.All(x => x is string))
        {
            options.Add(("service", ctorArgs[0]));
            options.Add(("idField", ctorArgs[1]));
            options.Add(("textField", ctorArgs[2]));
        }

        foreach (var property in template.GetProperties())
        {
            var name = property.Name();
            var value = NormalizeAttributeValue(property.ArgumentValue());

            switch (name)
            {
                case "ItemType":
                    itemType = value as TypeReference;
                    break;

                case "EndpointType":
                    endpointType = value as TypeReference;
                    break;

                case "ActionName":
                    actionName = value as string;
                    break;

                case "Capabilities":
                    options.RemoveAll(x => x.key == "capabilities");
                    options.Add(("capabilities", ToCapabilitiesValue(NamedArgumentTypeOf(property), value)));
                    break;

                default:
                    if (value is TypeReference || string.IsNullOrEmpty(name))
                        break;

                    var key = char.ToLowerInvariant(name[0]) + name[1..];
                    options.RemoveAll(x => x.key == key);
                    options.Add((key, value));
                    break;
            }
        }

        if (endpointType?.Resolve() is TypeDefinition endpoint)
        {
            var action = FindEndpointAction(endpoint, actionName);
            if (action is not null)
            {
                if (!has("service"))
                    options.Add(("service", GetServiceUrlFromRoute(endpoint) + "/" + action.Name));

                if (!has("capabilities") && GetListRequestCapabilities(action) is { } capabilities)
                    options.Add(("capabilities", capabilities));
            }
        }

        if (itemType?.Resolve() is TypeDefinition item)
        {
            var properties = item.PropertiesOf()
                .Where(TypingsUtils.IsPublicInstanceProperty)
                .ToArray();

            if (!has("idField") && FindPropertyByAttributeOrName(properties, "IdPropertyAttribute",
                "Id", "Key", "Code", "Value") is string idField)
                options.Add(("idField", idField));

            if (!has("textField") && FindPropertyByAttributeOrName(properties, "NamePropertyAttribute",
                "Name", "DisplayName", "Text") is string textField)
                options.Add(("textField", textField));

            if (properties.Length == 1 && properties[0].PropertyType().FullNameOf() == "System.String")
            {
                if (!has("idField"))
                    options.Add(("idField", properties[0].Name));
                if (!has("textField"))
                    options.Add(("textField", properties[0].Name));
            }

            if (!has("iconField") && FindPropertyByName(properties, "IconClass", "Icon") is string iconField)
                options.Add(("iconField", iconField));
        }

        return options;
    }

    /// <summary>
    /// Determines the client side options of a lookup editor template like the server side does at runtime:
    /// the lookup key from the constructor, either directly or from the [LookupScript] of the lookup type,
    /// then the other explicit options.
    /// </summary>
    private List<(string key, object? value)> GetLookupEditorTemplateOptions(CustomAttribute template)
    {
        var options = new List<(string key, object? value)>();

        var ctorArgs = template.ConstructorArguments()?.Select(x => NormalizeAttributeValue(ArgumentValueOf(x))).ToArray() ?? [];
        if (ctorArgs.Length == 1)
        {
            if (ctorArgs[0] is string lookupKey)
                options.Add(("lookupKey", lookupKey));
            else if (ctorArgs[0] is TypeReference lookupType &&
                GetLookupScriptKey(lookupType.Resolve()) is string typeLookupKey)
                options.Add(("lookupKey", typeLookupKey));
        }

        foreach (var property in template.GetProperties())
        {
            var name = property.Name();
            var value = NormalizeAttributeValue(property.ArgumentValue());
            if (value is TypeReference || string.IsNullOrEmpty(name))
                continue;

            var key = char.ToLowerInvariant(name[0]) + name[1..];
            options.RemoveAll(x => x.key == key);
            options.Add((key, value));
        }

        return options;
    }

    /// <summary>
    /// Gets the lookup key from the [LookupScript] attribute declared on a type, like the server side does
    /// for a lookup editor attribute with a lookup type.
    /// </summary>
    private string? GetLookupScriptKey(TypeDefinition? type)
    {
        if (type is null ||
            TypingsUtils.FindAttr(type.GetAttributes(), "Serenity.ComponentModel", "LookupScriptAttribute") is not { } attr)
            return null;

        var args = attr.ConstructorArguments()?.Select(x => NormalizeAttributeValue(ArgumentValueOf(x))).ToArray() ?? [];
        return args.Length switch
        {
            0 => AutoLookupKeyFor(type),
            1 when args[0] is string key => key,
            1 when args[0] is TypeReference otherType => GetLookupScriptKey(otherType.Resolve()),
            _ => null
        };
    }

    private static MethodDefinition? FindEndpointAction(TypeDefinition endpoint, string? actionName)
    {
        MethodDefinition? find(string name) => endpoint.MethodsOf().FirstOrDefault(x => x.IsPublic() &&
            !x.IsStatic && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));

        // like the server side, ListLookup is preferred when no action name is specified
        return actionName is not null ? find(actionName) : find("ListLookup") ?? find("List");
    }

    private static object? GetListRequestCapabilities(MethodDefinition action)
    {
        var attr = TypingsUtils.FindAttr(action.GetAttributes(), "Serenity.Services", "ListRequestCapabilitiesAttribute");
        if (attr is null)
            return null;

        var args = attr.ConstructorArguments();
        if (args is null || args.Count != 1 || NormalizeAttributeValue(ArgumentValueOf(args[0])) is not int capabilities)
            return null;

        foreach (var property in attr.GetProperties())
        {
            if (property.Name() == "Exclude" && NormalizeAttributeValue(property.ArgumentValue()) is int exclude)
                capabilities &= ~exclude;
        }

        return ToCapabilitiesValue(ArgumentTypeOf(args[0]), capabilities);
    }

    /// <summary>
    /// A flags enum value, written as a combination of its named members, e.g.
    /// ListRequestCapabilities.FilteringOptions | ListRequestCapabilities.Sort.
    /// </summary>
    private sealed class EnumFlagsValue(string enumName, IReadOnlyList<string> members, long remainder)
    {
        public string EnumName { get; } = enumName;
        public IReadOnlyList<string> Members { get; } = members;
        public long Remainder { get; } = remainder;
    }

    /// <summary>
    /// Converts a ListRequestCapabilities value to the combination of its named members, preferring the
    /// members covering the most flags, like SourceAdapter, in their declaration order. The members are read
    /// from the enum itself. Other values are returned as they are.
    /// </summary>
    private static object? ToCapabilitiesValue(TypeReference? enumType, object? value)
    {
        if (value is not int and not long ||
            enumType?.FullNameOf() != "Serenity.Services.ListRequestCapabilities" ||
            enumType.Resolve() is not TypeDefinition enumDefinition)
            return value;

        var number = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        var members = enumDefinition.FieldsOf()
            .Where(x => x.HasConstant())
            .Select((x, index) => (x.Name, Value: Convert.ToInt64(x.Constant(), CultureInfo.InvariantCulture), Index: index))
            .ToArray();

        if (number == 0)
        {
            var none = members.FirstOrDefault(x => x.Value == 0).Name;
            return none is null ? value : new EnumFlagsValue(enumType.Name, [none], 0);
        }

        static int bitCount(long value)
        {
            var count = 0;
            for (; value != 0; value &= value - 1)
                count++;
            return count;
        }

        var remaining = number;
        var chosen = new List<(string Name, long Value, int Index)>();
        foreach (var member in members.Where(x => x.Value != 0)
            .OrderByDescending(x => bitCount(x.Value)).ThenBy(x => x.Index))
        {
            // only members without flags outside the remaining value, so that they don't overlap
            if ((member.Value & remaining) == member.Value)
            {
                chosen.Add(member);
                remaining &= ~member.Value;
            }
        }

        return new EnumFlagsValue(enumType.Name, [.. chosen.OrderBy(x => x.Index).Select(x => x.Name)], remaining);
    }

    private static string? FindPropertyByAttributeOrName(PropertyDefinition[] properties, string attributeName,
        params string[] names)
    {
        var attributed = properties.Where(x => TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.Data", attributeName) != null)
            .Take(2).ToArray();

        return attributed.Length switch
        {
            1 => attributed[0].Name,
            > 1 => null,
            _ => FindPropertyByName(properties, names)
        };
    }

    private static string? FindPropertyByName(PropertyDefinition[] properties, params string[] names)
    {
        foreach (var name in names)
        {
            var matches = properties.Where(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
                .Take(2).ToArray();
            if (matches.Length == 1)
                return matches[0].Name;
        }

        return null;
    }

#if ISSOURCEGENERATOR
    private static object? ArgumentValueOf(TypedConstant argument) => argument.Value();
    private static TypeReference? ArgumentTypeOf(TypedConstant argument) => argument.Type;
    private static TypeReference? NamedArgumentTypeOf(KeyValuePair<string, TypedConstant> argument) => argument.Value.Type;
#else
    private static object? ArgumentValueOf(Mono.Cecil.CustomAttributeArgument argument) => argument.Value;
    private static TypeReference? ArgumentTypeOf(Mono.Cecil.CustomAttributeArgument argument) => argument.Type;
    private static TypeReference? NamedArgumentTypeOf(Mono.Cecil.CustomAttributeNamedArgument argument) => argument.Argument.Type;
#endif

    private static object? NormalizeAttributeValue(object? value)
    {
#if ISSOURCEGENERATOR
        if (value is System.Collections.Immutable.ImmutableArray<TypedConstant> array)
            return array.Select(x => NormalizeAttributeValue(x.Value())).ToArray();
#else
        if (value is Mono.Cecil.CustomAttributeArgument[] array)
            return array.Select(x => NormalizeAttributeValue(x.Value)).ToArray();

        if (value is Mono.Cecil.CustomAttributeArgument argument)
            return NormalizeAttributeValue(argument.Value);
#endif
        return value;
    }

    private void AppendEditorOptionValue(object? value)
    {
        switch (value)
        {
            case null:
                sb.Append("null");
                break;

            case string s:
                sb.Append(s.ToDoubleQuoted());
                break;

            case bool b:
                sb.Append(b ? "true" : "false");
                break;

            case object[] array:
                sb.Append('[');
                for (var i = 0; i < array.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    AppendEditorOptionValue(array[i]);
                }
                sb.Append(']');
                break;

            case EnumFlagsValue flags:
                var enumName = ImportFromCorelib(flags.EnumName);
                var parts = flags.Members.Select(x => enumName + "." + x).ToList();
                if (flags.Remainder != 0)
                    parts.Add(flags.Remainder.ToString(System.Globalization.CultureInfo.InvariantCulture));
                sb.Append(string.Join(" | ", parts));
                break;

            case IFormattable formattable:
                sb.Append(formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
                break;

            default:
                sb.Append(value.ToString().ToDoubleQuoted());
                break;
        }
    }
}
