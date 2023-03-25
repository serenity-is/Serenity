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

        // create a key to lookup by language and text key pair
        var k = new LanguageIdKeyPair(languageID, textKey);

        string? s;

        if (pending)
        {
            // first search in pending texts
            if (pendingTexts.TryGetValue(k, out s))
            {
                // pending text is available, return it
                if (s != null)
                    return s;

                // pending text is null, it means marked for deletion, 
                // if this is default language, return key itself
                if (languageID == LocalText.InvariantLanguageID)
                    return null;
            }
            else if (approvedTexts.TryGetValue(k, out s) &&
                s != null)
            {
                // approved is available, return it
                return s;
            }
            else if (languageID == LocalText.InvariantLanguageID)
            {
                // if language is default language, return text key
                return null;
            }

            var circularCheck1 = 0;
            while (true)
            {
                languageID = TryGetLanguageFallback(languageID) ?? LocalText.InvariantLanguageID;

                // search in fallback or default language
                k = new LanguageIdKeyPair(languageID, textKey);

                if (pendingTexts.TryGetValue(k, out s))
                {
                    // text available in pending language fallback
                    if (s != null)
                        return s;

                    // if marked for deletion in default language, return key itself
                    if (languageID == LocalText.InvariantLanguageID)
                        return null;
                }
                else if (approvedTexts.TryGetValue(k, out s) &&
                    s != null)
                {
                    // text available in approved default language
                    return s;
                }
                else if (languageID == LocalText.InvariantLanguageID)
                {
                    // not in pending nor approved, or circular reference, return key itself
                    return null;
                }

                // check for possible circular Fallbacks
                if (circularCheck1++ >= 10)
                    return null;

                // try again for language fallback...
            }
        }

        if (!approvedTexts.TryGetValue(k, out s))
        {
            // couldn't find, if requested language is not DefaultLanguageID, search in it too
            if (languageID != LocalText.InvariantLanguageID)
            {
                int circularCheck2 = 0;
                while (true)
                {
                    languageID = TryGetLanguageFallback(languageID) ?? LocalText.InvariantLanguageID;

                    // search in fallback or default language
                    k = new LanguageIdKeyPair(languageID, textKey);

                    // search again
                    if (approvedTexts.TryGetValue(k, out s))
                        return s;

                    if (languageID == LocalText.InvariantLanguageID ||
                        circularCheck2++ >= 10)
                        return null;
                }
            }

            // couldn't find in requested language or its Fallbacks, return key itself
            return null;
        }

        // found in requested language, return it
        return s;
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
        if (languageFallbacks.TryGetValue(languageID, out string fallback))
            return fallback;

        if (languageID == LocalText.InvariantLanguageID)
            return null;

        var idx = languageID.LastIndexOf('-');
        if (idx >= 1)
            return languageID.SafeSubstring(0, idx);

        return LocalText.InvariantLanguageID;
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
        string? text;

        var currentID = languageID;
        int tries = 0;

        while (true)
        {
            if (pending)
            {
                foreach (var item in pendingTexts)
                {
                    var key = item.Key.Key;
                    if (item.Key.LanguageId == currentID && !texts.ContainsKey(key))
                    {
                        text = TryGet(languageID, key, true);
                        if (text != null)
                            texts[key] = text;
                    }
                }
            }

            foreach (var item in approvedTexts)
            {
                var key = item.Key.Key;
                if (item.Key.LanguageId == currentID && !texts.ContainsKey(key))
                {
                    text = TryGet(languageID, key, true);
                    if (text != null)
                        texts[key] = text;
                }
            }

            tries++;

            if (tries > 10 ||
                currentID == LocalText.InvariantLanguageID)
                break;

            currentID = TryGetLanguageFallback(currentID) ?? LocalText.InvariantLanguageID;
        }

        return texts;
    }

    /// <inheritdoc/>
    public IDictionary<LanguageIdKeyPair, string?> GetAllTexts(bool pending)
    {
        return pending ? approvedTexts : pendingTexts;
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