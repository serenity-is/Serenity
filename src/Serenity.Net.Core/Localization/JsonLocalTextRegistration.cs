using Newtonsoft.Json.Linq;

namespace Serenity.Localization;

/// <summary>
/// Contains helper methods for registration of local texts in hierarchical/dictionary formatted JSON files.
/// </summary>
public static class JsonLocalTextRegistration
{
    /// <summary>
    /// Adds translation from a hierarchical local text dictionary parsed from JSON file.
    /// </summary>
    /// <param name="nested">Object parsed from local text JSON string</param>
    /// <param name="prefix">Prefix to prepend before local text keys</param>
    /// <param name="languageID">Language ID</param>
    /// <param name="registry">Registry</param>
    public static void AddFromNestedDictionary(IDictionary<string, object> nested, string prefix, string languageID, ILocalTextRegistry registry)
    {
        if (nested == null)
            throw new ArgumentNullException(nameof(nested));

        if (registry == null)
            throw new ArgumentNullException(nameof(registry));

        var target = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        ProcessNestedDictionary(nested, prefix, target);

        foreach (var pair in target)
            registry.Add(languageID, pair.Key, pair.Value);
    }

    /// <summary>
    /// Converts translation from a hierarchical local text dictionary to a simple dictionary.
    /// </summary>
    /// <param name="nested">Object parsed from local text JSON string</param>
    /// <param name="prefix">Prefix to prepend before local text keys</param>
    /// <param name="target">Target dictionary that will contain keys and translations</param>
    public static void ProcessNestedDictionary<TValue>(IDictionary<string, TValue> nested, string prefix, Dictionary<string, string> target)
    {
        if (nested == null)
            throw new ArgumentNullException("nested");

        foreach (var k in nested)
        {
            var actual = prefix + k.Key;
            var o = k.Value;
            if (o is IDictionary<string, object> dictionary)
                ProcessNestedDictionary(dictionary, actual + ".", target);
            else if (o is JObject obj)
                ProcessNestedDictionary(obj, actual + ".", target);
            else if (o != null)
                target[actual] = o.ToString();
        }
    }

    /// <summary>
    /// Adds translations from JSON files at specified path. File names in this directory should be in format 
    /// {anyprefix}.{languageID}.json where {languageID} is a language code like 'en', 'en-GB' etc.
    /// </summary>
    /// <param name="registry">Registry</param>
    /// <param name="path">Path containing JSON files</param>
    /// <param name="fileSystem">File system</param>
    public static void AddJsonTexts(this ILocalTextRegistry registry, string path, IFileSystem? fileSystem = null)
    {
        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        if (path == null)
            throw new ArgumentNullException(nameof(path));

        fileSystem ??= new PhysicalFileSystem();

        if (!fileSystem.DirectoryExists(path))
            return;

        var files = fileSystem.GetFiles(path, "*.json", recursive: true);
        Array.Sort(files);

        foreach (var file in files)
        {
            var texts = JsonConvert.DeserializeObject<Dictionary<string, object>>(fileSystem.ReadAllText(file).TrimToNull() ?? "{}") ?? new();
            var langID = fileSystem.GetFileNameWithoutExtension(fileSystem.GetFileName(file));

            var idx = langID.LastIndexOf(".");
            if (idx >= 0)
                langID = langID[(idx + 1)..];

            if (langID.ToLowerInvariant() == "invariant")
                langID = "";

            AddFromNestedDictionary(texts, "", langID, registry);
        }
    }
}