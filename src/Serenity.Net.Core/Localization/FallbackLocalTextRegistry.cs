namespace Serenity.Localization;

/// <summary>
/// Adds key fallback to any ILocalTextSource implementation
/// </summary>
public class FallbackLocalTextSource : ILocalTextRegistry
{
    private readonly ILocalTextRegistry source;

    /// <summary>
    /// Initializes a new instance of the <see cref="FallbackLocalTextSource"/> class.
    /// </summary>
    /// <param name="source">The local text source.</param>
    public FallbackLocalTextSource(ILocalTextRegistry source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Returns localized representation which corresponds to the local text key or the fallback if none 
    /// found in the registry.
    /// </summary>
    /// <param name="key">Local text key (e.g. Enums.Month.June)</param>
    /// <param name="languageID">Language identifier</param>
    /// <param name="pending">If pending approval text should be used</param>
    /// <returns></returns>
    public string? TryGet(string languageID, string key, bool pending)
    {
        string? text = source.TryGet(languageID, key, pending);

        if (!string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key))
            return text;

        // Get text without key's suffixes
        string[] suffixes = { ".EntitySingular", ".EntityPlural" };
        foreach (var suffix in suffixes)
            if (key.EndsWith(suffix))
            {
                key = key[..^suffix.Length];

                text = source.TryGet(languageID, key, pending);
                if (!string.IsNullOrEmpty(text))
                    return text;

                break;
            }

        // Fallback to a sub-key (e.g. if Enums.Month.June not found, then try get June instead)
        var fbKey = TryGetKeyFallback(key);
        if (!string.IsNullOrEmpty(fbKey))
        {
            text = source.TryGet(languageID, fbKey, pending);
            if (!string.IsNullOrEmpty(text))
                return text;

            return BreakUpString(fbKey);
        }

        return null;
    }

    /// <summary>
    /// Get a fallback of the local text key
    /// </summary>
    /// <param name="key">Local text key</param>
    /// <returns>Local text key fallback</returns>
    public static string? TryGetKeyFallback(string key)
    {
        // Get last part of the key after the last dot
        var lastDot = key.LastIndexOf('.');
        if (lastDot > 0 && lastDot < key.Length - 1)
        {
            key = key[(lastDot + 1)..];

            // Remove Id
            if (key.Length > 2 && key.EndsWith("Id"))
                key = key[0..^2];

            return key;
        }

        // Get last part of the key after the last forward slash
        var lastSlash = key.LastIndexOf('/');
        if (lastSlash > 0 && lastSlash < key.Length - 1)
            return key[(lastSlash + 1)..];

        return null;
    }

    /// <summary>
    /// Break up string without spaces (e.g. LastDirectoryUpdate) 
    /// into a normal string (e.g. 'Last Directory Update')
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string BreakUpString(string value)
    {
        return Regex.Replace(value, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1", RegexOptions.Compiled).Trim();
    }

    /// <summary>
    /// Adds a local text entry to the registry
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    public void Add(string languageID, string key, string? text)
    {
        source.Add(languageID, key, text);
    }
}