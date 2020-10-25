
namespace Serenity.Localization
{
    using Serenity.Abstractions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using ItemKey = System.Tuple<string, string>;

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
    public class LocalTextRegistry : ILocalTextRegistry, IRemoveAll
    {
        private readonly ConcurrentDictionary<ItemKey, string> approvedTexts = 
            new ConcurrentDictionary<ItemKey, string>(ItemKeyComparer.Default);

        private readonly ConcurrentDictionary<ItemKey, string> pendingTexts = 
            new ConcurrentDictionary<ItemKey, string>(ItemKeyComparer.Default);

        private readonly ConcurrentDictionary<string, string> languageFallbacks =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds a local text entry to the registry
        /// </summary>
        /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
        /// <param name="key">Local text key</param>
        /// <param name="text">Translated text</param>
        public void Add(string languageID, string key, string text)
        {
            Check.NotNull(languageID, "languageID");
            Check.NotNull(key, "key");
            approvedTexts[new ItemKey(languageID, key)] = text;
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
            Check.NotNull(languageID, "languageID");
            Check.NotNull(key, "key");
            pendingTexts[new ItemKey(languageID, key)] = text;
        }

        /// <summary>
        /// Converts the local text key to its representation in requested language. Looks up text
        /// in requested language, its Fallbacks and invariant language in order. If not found in any,
        /// null is returned. See SetLanguageFallback for information about language fallbacks.
        /// </summary>
        /// <param name="languageID">Language ID.</param>
        /// <param name="textKey">Local text key (can be null).</param>
        /// <param name="pending">If pending approval texts to be used, true.</param>
        public string TryGet(string languageID, string textKey, bool pending)
        {
            Check.NotNull(languageID, "languageID");

            // create a key to lookup by language and text key pair
            var k = new ItemKey(languageID, textKey);

            string s;

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
                else if (approvedTexts.TryGetValue(k, out s))
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
                    k = new ItemKey(languageID, textKey);

                    if (pendingTexts.TryGetValue(k, out s))
                    {
                        // text available in pending language fallback
                        if (s != null)
                            return s;

                        // if marked for deletion in default language, return key itself
                        if (languageID == LocalText.InvariantLanguageID)
                            return null;
                    }
                    else if (approvedTexts.TryGetValue(k, out s))
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
                        k = new ItemKey(languageID, textKey);

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

        /// <summary>
        /// Sets the language fallback of the specified language.
        /// When a text is not found in one language, LocalTextRegistry checks its language fallback for
        /// a translation. Some implicit language fallback definitions exist even if none set. For example, "en" is 
        /// language fallback ID of "en-US" and "en-UK", "tr" is language fallback ID of "tr-TR". Also, 
        /// invariant language ID ("") is an implicit fallback of all languages.
        /// </summary>
        /// <param name="languageID">Language identifier. (e.g. en-US)</param>
        /// <param name="languageFallbackID">language fallback identifier. (e.g. en)</param>
        public void SetLanguageFallback(string languageID, string languageFallbackID)
        {
            Check.NotNull(languageID, "languageID");
            Check.NotNull(languageFallbackID, "languageFallbackID");

            languageFallbacks[languageID] = languageFallbackID;
        }

        private string TryGetLanguageFallback(string languageID)
        {
            string fallback;
            if (languageFallbacks.TryGetValue(languageID, out fallback))
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
            Check.NotNull(languageID, "languageID");

            var texts = new Dictionary<string, string>();
            string text;

            var currentID = languageID;
            int tries = 0;

            while (true)
            {
                if (pending)
                {
                    foreach (var item in pendingTexts)
                    {
                        var key = item.Key.Item2;
                        if (item.Key.Item1 == currentID && !texts.ContainsKey(key))
                        {
                            text = TryGet(languageID, key, true);
                            if (text != null)
                                texts[key] = text;
                        }
                    }
                }

                foreach (var item in approvedTexts)
                {
                    var key = item.Key.Item2;
                    if (item.Key.Item1 == currentID && !texts.ContainsKey(key))
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

        /// <summary>
        /// Gets all text keys that is currently registered in any language
        /// </summary>
        public HashSet<string> GetAllTextKeys(bool pending)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var k in (pending ? pendingTexts : approvedTexts).Keys)
                result.Add(k.Item2);
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

        private class ItemKeyComparer : IEqualityComparer<Tuple<string, string>>
        {
            public static readonly ItemKeyComparer Default = new ItemKeyComparer();

            public bool Equals(ItemKey lhs, ItemKey rhs)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(lhs.Item1, rhs.Item1)
                    && StringComparer.OrdinalIgnoreCase.Equals(lhs.Item2, rhs.Item2);
            }

            public int GetHashCode(ItemKey tuple)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(tuple.Item1)
                     ^ tuple.Item2.GetHashCode();
            }
        }
    }
}