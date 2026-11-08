namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Checks whether a type reference is a <see cref="StringEnum"/> or derives from it.
    /// </summary>
    internal static bool IsStringEnumType(TypeReference? type)
    {
        return type != null && TypingsUtils.IsSubclassOf(type, "Serenity", "StringEnum");
    }

    /// <summary>
    /// Generates the TypeScript for a string enumeration, which is an object with its members as constant values,
    /// a union type of those values with the same name, and a registerEnum call that carries the icons of the members.
    /// </summary>
    private void GenerateStringEnum(TypeDefinition stringEnumType)
    {
        var codeNamespace = ScriptNamespaceFor(stringEnumType);
        var enumKey = GetEnumKeyFor(stringEnumType);

        var members = stringEnumType.FieldsOf()
            .Where(x => x.IsStatic && !x.IsSpecialName() &&
                x.Constant() is string &&
                x.FieldType()?.FullNameOf() == "System.String" &&
                (!x.HasCustomAttributes() ||
                    (TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "IgnoreUIFieldAttribute") == null &&
                     TypingsUtils.FindAttr(x.GetAttributes(), "Serenity.ComponentModel", "TransformIgnoreAttribute") == null)))
            .Select(x => (Field: x, Value: (string)x.Constant()!, Icon: GetEnumIconFor(x)))
            .ToList();

        // like GenerateEnum, the identifier is appended to the code by MakeFriendlyName, after its prefix
        cw.Indented("export const ");
        var identifier = MakeFriendlyName(stringEnumType, codeNamespace);
        var fullName = (string.IsNullOrEmpty(codeNamespace) ? "" : codeNamespace + ".") + identifier;
        RegisterGeneratedType(codeNamespace, identifier, typeOnly: false);

        sb.AppendLine(" = {");
        cw.Block(delegate
        {
            for (var i = 0; i < members.Count; i++)
            {
                if (i > 0)
                    sb.AppendLine(",");

                cw.Indented(members[i].Field.Name);
                sb.Append(": ");
                sb.Append(members[i].Value.ToSingleQuoted());
            }

            sb.AppendLine();
        });
        cw.IndentedLine("} as const;");

        cw.Indented("export type ");
        sb.Append(identifier);
        sb.Append(" = typeof ");
        sb.Append(identifier);
        sb.AppendLine("[keyof typeof " + identifier + "];");

        // a member value can repeat, unlike the values of an enum, in which case the first icon is used
        var icons = members.Where(x => !string.IsNullOrEmpty(x.Icon))
            .GroupBy(x => x.Value, StringComparer.Ordinal)
            .Select(group => (Key: group.Key.ToSingleQuoted(), Icon: group.First().Icon!))
            .ToList();

        WriteEnumRegistration(identifier, fullName, enumKey, icons);
    }
}
