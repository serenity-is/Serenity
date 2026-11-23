namespace Serenity.CodeGeneration;

/// <summary>
/// The conventions of the local text keys of the entities, e.g. the suffix of the key of the placeholder of a field,
/// and the identifiers that are derived from the segments of a key, which the sergen transforms and the source
/// generators share, so that the texts they register and the constants they generate (see TextKeysWriter) can not
/// drift apart.
/// </summary>
public static class LocalTextKeyRules
{
    /// <summary>
    /// The suffix of the key of the singular name of an entity, e.g. "Db.Northwind.Order.EntitySingular", which
    /// comes from its <c>[InstanceName]</c> attribute.
    /// </summary>
    public const string EntitySingular = "EntitySingular";

    /// <summary>
    /// The suffix of the key of the plural name of an entity, e.g. "Db.Northwind.Order.EntityPlural", which comes
    /// from its <c>[DisplayName]</c> attribute.
    /// </summary>
    public const string EntityPlural = "EntityPlural";

    /// <summary>
    /// The suffix of the key of the placeholder of a field, e.g. "Db.Northwind.Customer.Fax_Placeholder", which is
    /// the default key of its <c>[Placeholder]</c> attribute.
    /// </summary>
    public const string PlaceholderSuffix = "_Placeholder";

    /// <summary>
    /// The suffix of the key of the hint of a field, which is the default key of its <c>[Hint]</c> attribute.
    /// </summary>
    public const string HintSuffix = "_Hint";

    /// <summary>
    /// The suffix of the key of the placeholder of the quick filter of a field, which is the default key of its
    /// <c>[QuickFilter]</c> attribute.
    /// </summary>
    public const string QuickFilterPlaceholderSuffix = "_QuickFilter_Placeholder";

    /// <summary>
    /// The prefix of the keys of the categories of the fields of an entity, e.g. "Db.Northwind.Customer.Categories.General".
    /// </summary>
    public const string CategoryPrefix = "Categories.";

    /// <summary>
    /// The prefix of the keys of the tabs of the fields of an entity, e.g. "Db.Northwind.Customer.Tabs.Details".
    /// </summary>
    public const string TabPrefix = "Tabs.";

    /// <summary>
    /// The prefix of the keys of the navigation texts of an application, whose key is the path of a page, e.g.
    /// "Navigation.Basic Samples/Editors/Filtered Lookup in Detail", see <see cref="SplitKey"/>.
    /// </summary>
    public const string NavigationKeyPrefix = "Navigation.";

    // the separators of the segments of a key, as the overload of string.Split that takes one of them is not there
    // for the frameworks that the generators are built for
    private static readonly char[] DotSeparator = ['.'];
    /// <summary>
    /// The separator of a slash, which is two slashes in the path of a page, see <see cref="SplitKey"/>.
    /// </summary>
    public const string EscapedSlash = "//";

    /// <summary>
    /// Splits a key into its segments, dropping the empty ones, e.g. the ones of a key that starts with a slash.
    /// </summary>
    /// <remarks>
    /// A dot separates the segments of a key, except for the keys of the navigation texts of an application, which have
    /// the path of a page after "Navigation.": the slashes of that path are their separators, and a dot that the path
    /// has, e.g. in the name of a file, is a part of its segment rather than a separator, and is removed like the other
    /// characters that an identifier can not have, see <see cref="ToIdentifier"/>. Two slashes in the path stand for a
    /// slash that is a part of the segment, e.g. a page that is in a folder that a slash is in the name of, so they are
    /// not a separator, and the segment keeps the slash that they stand for. A slash elsewhere in a key is no
    /// separator, as it is a part of a segment there.
    /// </remarks>
    /// <param name="key">The key, e.g. "Navigation.Basic Samples/Editors/Filtered Lookup in Detail".</param>
    /// <returns>The segments of the key, e.g. "Navigation", "Basic Samples", "Editors", "Filtered Lookup in Detail".</returns>
    public static string[] SplitKey(string? key)
    {
        if (string.IsNullOrEmpty(key))
            return [];

        if (key!.StartsWith(NavigationKeyPrefix, StringComparison.Ordinal))
        {
            return
            [
                NavigationKeyPrefix[..^1],
                .. SplitPath(key[NavigationKeyPrefix.Length..])
            ];
        }

        return key.Split(DotSeparator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Splits the path of a page into the segments of its key, where a single slash separates them, and two slashes
    /// stand for a slash that is a part of the segment that they are in.
    /// </summary>
    private static string[] SplitPath(string path)
    {
        var segments = new List<string>();
        var sb = new StringBuilder(path.Length);
        for (var i = 0; i < path.Length; i++)
        {
            var c = path[i];
            if (c != '/')
            {
                sb.Append(c);
                continue;
            }

            if (i + 1 < path.Length && path[i + 1] == '/')
            {
                sb.Append('/');
                i++;
                continue;
            }

            if (sb.Length > 0)
            {
                segments.Add(sb.ToString());
                sb.Clear();
            }
        }

        if (sb.Length > 0)
            segments.Add(sb.ToString());

        return [.. segments];
    }

    /// <summary>
    /// Turns a segment of a key into the identifier of the constant that carries it. A segment that is an identifier
    /// already is kept as it is, e.g. "Fax_Placeholder", as the keys of the texts are case sensitive and their
    /// segments are usually what the constant is best read as; one that is not has the characters that an identifier
    /// can not have removed as the separators of its words, which are capitalized, e.g. "Filtered Lookup in Detail" to
    /// "FilteredLookupInDetail", a digit at its start prefixed with an underscore, and a keyword escaped, e.g. "class"
    /// to "@class".
    /// </summary>
    /// <param name="segment">The segment of the key, e.g. "Fax_Placeholder".</param>
    /// <param name="changed">True if the identifier is not the segment itself.</param>
    /// <returns>The identifier, or null if the segment has no characters that an identifier can have.</returns>
    public static string? ToIdentifier(string? segment, out bool changed)
    {
        changed = false;
        if (string.IsNullOrEmpty(segment))
            return null;

        var text = segment!;
        if (CSharpSyntaxRules.IsValidIdentifier(text, ignoreKeywords: true) &&
            !CSharpSyntaxRules.IsKeyword(text))
            return text;

        // the characters that an identifier can not have separate its words, e.g. "Filtered Lookup in Detail" to
        // "FilteredLookupInDetail", as a name that is made of words reads better than one that has them run together
        var sb = new StringBuilder(text.Length);
        var wordStart = true;
        foreach (var c in text)
        {
            if (!IsIdentifierPart(c))
            {
                wordStart = true;
                continue;
            }

            sb.Append(wordStart ? char.ToUpperInvariant(c) : c);
            wordStart = false;
        }

        if (sb.Length == 0)
            return null;

        if (char.IsDigit(sb[0]))
            sb.Insert(0, '_');

        changed = true;
        var identifier = sb.ToString();
        return CSharpSyntaxRules.IsKeyword(identifier) ? "@" + identifier : identifier;
    }

    /// <summary>
    /// Capitalizes the first letter of an identifier, e.g. "fax" to "Fax". The generators prefer the Pascalized
    /// identifier while it is free, as the keys of the texts are case sensitive and "Fax" and "fax" are both legal,
    /// but a name that starts with a capital letter reads better as a constant.
    /// </summary>
    /// <param name="identifier">The identifier, e.g. "fax".</param>
    /// <returns>The Pascalized identifier, e.g. "Fax".</returns>
    public static string Pascalize(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        if (identifier.Length == 0 || !char.IsLower(identifier[0]))
            return identifier;

        return char.ToUpperInvariant(identifier[0]) + identifier[1..];
    }

    private static bool IsIdentifierPart(char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_')
            return true;

        return CharUnicodeInfo.GetUnicodeCategory(c) switch
        {
            UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or
            UnicodeCategory.ConnectorPunctuation or UnicodeCategory.Format => true,
            _ => false
        };
    }
}
