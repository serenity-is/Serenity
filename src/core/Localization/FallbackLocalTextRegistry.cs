namespace Serenity.Localization;

/// <summary>
/// Adds key fallback behavior to any <see cref="ILocalTextRegistry"/> implementation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FallbackLocalTextSource"/> class.
/// </remarks>
/// <param name="source">The local text source.</param>
public class FallbackLocalTextSource(ILocalTextRegistry source) : ILocalTextRegistry
{
    private readonly ILocalTextRegistry source = source ?? throw new ArgumentNullException(nameof(source));

    /// <summary>
    /// Returns the localized representation that corresponds to the local text key, or a fallback
    /// if none is found in the registry.
    /// </summary>
    /// <param name="key">The local text key (e.g. Enums.Month.June).</param>
    /// <param name="languageID">The language identifier.</param>
    /// <param name="pending">If pending approval text should be used.</param>
    /// <returns>The localized text, a fallback, or <c>null</c> if none is found.</returns>
    public string? TryGet(string languageID, string key, bool pending)
    {
        string? text = source.TryGet(languageID, key, pending);

        if (!string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key))
            return text;

        // Get text without key's suffixes
        string[] suffixes = [".EntitySingular", ".EntityPlural"];
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
    /// Gets a fallback of the local text key.
    /// </summary>
    /// <param name="key">The local text key.</param>
    /// <returns>The local text key fallback, or <c>null</c> if none can be derived.</returns>
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
    /// Breaks up a string without spaces (e.g. LastDirectoryUpdate) into a normal string
    /// (e.g. 'Last Directory Update').
    /// </summary>
    /// <param name="value">The string to break up.</param>
    /// <returns>The string with spaces inserted before each capital letter.</returns>
    public static string BreakUpString(string value)
    {
        return Regex.Replace(value, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1", RegexOptions.Compiled).Trim();
    }

    /// <summary>
    /// Adds a local text entry to the registry.
    /// </summary>
    /// <param name="languageID">The language ID (e.g. en-US, tr-TR).</param>
    /// <param name="key">The local text key.</param>
    /// <param name="text">The translated text.</param>
    public void Add(string languageID, string key, string? text)
    {
        source.Add(languageID, key, text);
    }
}