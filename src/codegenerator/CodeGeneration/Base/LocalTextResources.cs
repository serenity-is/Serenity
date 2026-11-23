using Newtonsoft.Json.Linq;

namespace Serenity.CodeGeneration;

/// <summary>
/// Reads the text resources of a project (texts/resources/*.texts.*.json), which have the invariant text of the keys
/// of the texts that the assembly registers, or their translations, so that the generator of the text keys can tell the
/// text of a key next to it (see <see cref="TextKeysWriter"/>) and find the keys that only the texts have. A project
/// has one set of files for each of its text packages, e.g. "site.texts.__.json" for the texts of its own application,
/// and "dataexplorer.texts.__.json" for the ones of a package that it uses, and the file with the language
/// <see cref="InvariantLanguage"/> is the template that the others are made from.
/// </summary>
public static class LocalTextResources
{
    /// <summary>
    /// The language of the file that has the invariant text of the keys, as the "*.texts.__.json" of a project is the
    /// template that the translations are made from.
    /// </summary>
    public const string InvariantLanguage = "__";

    /// <summary>
    /// Reads the keys of a text resource file and their text, which is null for a key that the file leaves for a
    /// translation to fill in. A key is the path of its property, which the files of a project write as it is, but
    /// a nested one is read the same way, and the comments of the template, which start with two underscores, are
    /// not keys.
    /// </summary>
    /// <param name="json">The contents of the file.</param>
    /// <returns>The keys of the file, with their text, in the order of the file.</returns>
    public static IReadOnlyDictionary<string, string?> Read(string? json)
    {
        var texts = new Dictionary<string, string?>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(json))
            return texts;

        var root = JToken.Parse(json!);
        Read(root, null, texts);
        return texts;
    }

    /// <summary>
    /// Merges the keys of the text resources of a project, which has a set of them for each of its text packages, e.g.
    /// "site.texts.__.json" and "dataexplorer.texts.__.json". A key that more than one of them has takes the first text
    /// that one of them gives it.
    /// </summary>
    /// <param name="files">The keys of the files, with their text, in the order that they are to be merged.</param>
    /// <returns>The keys of all of the files, with their text.</returns>
    public static IReadOnlyDictionary<string, string?> Merge(IEnumerable<IReadOnlyDictionary<string, string?>> files)
    {
        ArgumentNullException.ThrowIfNull(files);

        var texts = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var file in files)
        {
            foreach (var pair in file)
            {
                if (texts.TryGetValue(pair.Key, out var text) && !string.IsNullOrEmpty(text))
                    continue;

                texts[pair.Key] = pair.Value;
            }
        }

        return texts;
    }

    private static void Read(JToken? token, string? key, Dictionary<string, string?> texts)
    {
        switch (token)
        {
            case JObject obj:
                foreach (var property in obj.Properties())
                {
                    if (property.Name.StartsWith("__", StringComparison.Ordinal))
                        continue;

                    Read(property.Value, key is null ? property.Name : key + "." + property.Name, texts);
                }
                break;

            case JValue value when key != null:
                texts[key!] = value.Type switch
                {
                    JTokenType.String => value.Value<string>(),
                    JTokenType.Null => null,
                    _ => value.ToString()
                };
                break;
        }
    }
}
