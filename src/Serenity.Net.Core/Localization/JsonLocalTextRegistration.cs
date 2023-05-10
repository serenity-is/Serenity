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
            var langID = ParseLanguageIdFromPath(file);
            if (langID is null)
                continue;

            var texts = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                fileSystem.ReadAllText(file).TrimToNull() ?? "{}") ?? new();

            AddFromNestedDictionary(texts, "", langID, registry);
        }
    }

    /// <summary>
    /// Adds json texts from embedded resources
    /// </summary>
    /// <param name="registry">The text registry</param>
    /// <param name="typeSource">Type source</param>
    /// <returns>The text registry</returns>
    /// <exception cref="ArgumentNullException">registry, provider or sub path is null</exception>
    public static ILocalTextRegistry AddJsonResourceTexts(this ILocalTextRegistry registry, ITypeSource typeSource)
    {
        if (registry is null)
            throw new ArgumentNullException(nameof(registry));

        if (typeSource is null)
            throw new ArgumentNullException(nameof(typeSource));

        if (typeSource is not IGetAssemblies getAssemblies)
            return registry;

        return AddJsonResourceTexts(registry, getAssemblies.GetAssemblies().ToArray());
    }

    /// <summary>
    /// Adds json texts from embedded resources
    /// </summary>
    /// <param name="registry">The text registry</param>
    /// <param name="assemblies">List of assemblies</param>
    /// <returns>The text registry</returns>
    /// <exception cref="ArgumentNullException">registry, provider or sub path is null</exception>
    public static ILocalTextRegistry AddJsonResourceTexts(this ILocalTextRegistry registry, IEnumerable<Assembly> assemblies)
    {
        if (assemblies is null)
            throw new ArgumentNullException(nameof(assemblies));

        foreach (var assembly in assemblies)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
            {
                if (!resourceName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!resourceName.Contains(".texts."))
                    continue;

                var langID = ParseLanguageIdFromPath(resourceName);
                if (langID is null)
                    continue;

                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var sr = new System.IO.StreamReader(stream);
                string? json = sr.ReadToEnd().TrimToNull();
                if (json is null)
                    continue;
                var texts = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (texts is not null)
                    AddFromNestedDictionary(texts, "", langID, registry);
            }
        }

        return registry;
    }

    private static readonly Regex IsoLanguageIdRegex =
        new("^[a-z][a-z](-[A-Z][A-Z])?$");

    /// <summary>
    /// Parses language ID from the file path
    /// </summary>
    /// <param name="path">Path</param>
    public static string? ParseLanguageIdFromPath(string path)
    {
        var langID = System.IO.Path.GetFileNameWithoutExtension(
            System.IO.Path.GetFileName(path));

        var idx = langID.LastIndexOf(".");
        if (idx >= 0)
            langID = langID[(idx + 1)..];

        if (langID == "invariant")
            return "";
        else if (!IsoLanguageIdRegex.IsMatch(langID))
            return null;

        return langID;
    }
}