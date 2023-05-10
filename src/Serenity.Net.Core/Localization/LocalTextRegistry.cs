namespace Serenity.Localization;

/// <summary>
/// Default ILocalTextRegistry implementation.
/// </summary>
/// <seealso cref="ILocalTextRegistry" />
/// <seealso cref="IRemoveAll" />
/// <remarks>
/// This implementation also supports a "pending approval" mode. If your site needs some moderator
/// approval before translations are published, you may put your site to this mode when
/// some moderator is using the site by registering an ILocalTextContext provider. Thus,
/// moderators can see unapproved texts while they are logged in to the site.
/// </remarks>
public class LocalTextRegistry : ILocalTextRegistry, IRemoveAll, IGetAllTexts, ILanguageFallbacks
{
    private readonly ConcurrentDictionary<LanguageIdKeyPair, string?> approvedTexts = new();

    private readonly ConcurrentDictionary<LanguageIdKeyPair, string?> pendingTexts = new();

    private readonly ConcurrentDictionary<string, string> languageFallbacks = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Adds a local text entry to the registry
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    public void Add(string languageID, string key, string? text)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (key == null)
            throw new ArgumentNullException(nameof(languageID));

        if (text is null)
        {
            // probably null entry in texts.xy.json, save it to remember the key only
            var pair = new LanguageIdKeyPair(LocalText.InvariantLanguageID, key);
            approvedTexts.GetOrAdd(pair, (string?)null);
        }
        else
            approvedTexts[new LanguageIdKeyPair(languageID, key)] = text;
    }

    /// <summary>
    /// Adds a pending approval local text entry to the registry. These texts can only be seen
    /// while moderators are browsing the site. You can determine which users are moderators by
    /// implementing ILocalTextContext interface, and registering it through the service locator.
    /// </summary>
    /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
    /// <param name="key">Local text key</param>
    /// <param name="text">Translated text</param>
    public void AddPending(string languageID, string key, string text)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (key == null)
            throw new ArgumentNullException(nameof(key));

        pendingTexts[new LanguageIdKeyPair(languageID, key)] = text;
    }

    /// <summary>
    /// Converts the local text key to its representation in requested language. Looks up text
    /// in requested language, its Fallbacks and invariant language in order. If not found in any,
    /// null is returned. See SetLanguageFallback for information about language fallbacks.
    /// </summary>
    /// <param name="languageID">Language ID.</param>
    /// <param name="textKey">Local text key</param>
    /// <param name="pending">If pending approval texts to be used, true.</param>
    public string? TryGet(string languageID, string textKey, bool pending)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        if (textKey == null)
            throw new ArgumentNullException(nameof(textKey));
        
        var circularCheck = 0;
        LanguageIdKeyPair k;
        do
        {
            k = new LanguageIdKeyPair(languageID, textKey);

            if (pending && pendingTexts.TryGetValue(k, out string? s))
            {
                if (s != null)
                    return s;
            }
            else if (approvedTexts.TryGetValue(k, out s))
                return s;
            
            if (languageID == LocalText.InvariantLanguageID)
                return null;

            languageID = TryGetLanguageFallback(languageID) ?? LocalText.InvariantLanguageID;
        }
        while (circularCheck++ < 10);

        return null;
    }

    /// <inheritdoc/>
    public IDictionary<string, string> GetLanguageFallbacks()
    {
        return languageFallbacks;
    }

    /// <inheritdoc/>
    public void SetLanguageFallback(string languageID, string fallbackID)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        languageFallbacks[languageID] = fallbackID ?? throw new ArgumentNullException(nameof(fallbackID));
    }

    private string? TryGetLanguageFallback(string languageID)
    {
        if (string.IsNullOrEmpty(languageID))
            return null;

        return languageFallbacks.GetOrAdd(languageID, static id =>
        {
            if (id.Length == 5 &&
                id[2] == '-' &&
                id[0] >= 'a' && id[0] <= 'z' &&
                id[1] >= 'a' && id[1] <= 'z' &&
                id[3] >= 'A' && id[3] <= 'Z' &&
                id[4] >= 'A' && id[4] <= 'Z')
            {
                return id[..2];
            }

            return LocalText.InvariantLanguageID;
        });
    }

    /// <summary>
    ///   Gets all available text keys (that has a translation in language or any of its
    ///   language fallbacks) and their local texts.</summary>
    /// <param name="languageID">
    ///   Language ID (required).</param>
    /// <param name="pending">
    ///   True if pending texts should be returned (e.g. in preview/edit mode).</param>
    /// <returns>
    ///   A dictionary of all texts in the language.</returns>
    public Dictionary<string, string> GetAllAvailableTextsInLanguage(string languageID, bool pending)
    {
        if (languageID == null)
            throw new ArgumentNullException(nameof(languageID));

        var texts = new Dictionary<string, string>();

        var currentID = languageID;
        int circularCheck = 0;

        void scanItems(IEnumerable<KeyValuePair<LanguageIdKeyPair, string?>> items)
        {
            foreach (var item in items)
            {
                if (item.Key.LanguageId != currentID)
                    continue;

                var key = item.Key.Key;

                if (texts.ContainsKey(key))
                    continue;

                if (TryGet(languageID, key, pending) is string text)
                    texts[key] = text;
            }
        }

        do
        {
            if (pending)
                scanItems(pendingTexts);

            scanItems(approvedTexts);

            if (currentID == LocalText.InvariantLanguageID)
                break;

            currentID = TryGetLanguageFallback(currentID) ?? LocalText.InvariantLanguageID;
        }
        while (circularCheck++ < 10);

        return texts;
    }

    /// <inheritdoc/>
    public IDictionary<LanguageIdKeyPair, string?> GetAllTexts(bool pending)
    {
        return pending ? pendingTexts : approvedTexts;
    }

    /// <summary>
    /// Gets all text keys that is currently registered in any language
    /// </summary>
    public HashSet<string> GetAllTextKeys(bool pending)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var k in (pending ? pendingTexts : approvedTexts).Keys)
            result.Add(k.Key);
        return result;
    }

    /// <summary>
    /// Removes all cached texts
    /// </summary>
    public void RemoveAll()
    {
        approvedTexts.Clear();
        pendingTexts.Clear();
        languageFallbacks.Clear();
    }
}