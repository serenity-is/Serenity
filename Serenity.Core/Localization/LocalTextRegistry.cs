
namespace Serenity.Localization
{
    using Serenity.Abstractions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using ItemKey = System.Tuple<string, string>;

    /// <summary>
    /// Default ILocalTextRegistry implementation.
    /// </summary>
    /// <remarks>
    /// This implementation also supports a "pending approval" mode. If your site needs some moderator
    /// approval before translations are published, you may put your site to this mode when
    /// some moderator is using the site by registering an ILocalTextContext provider. Thus,
    /// moderators can see unapproved texts while they are logged in to the site.
    /// </remarks>
    public class LocalTextRegistry : ILocalTextRegistry
    {
        private readonly ConcurrentDictionary<ItemKey, string> approvedTexts = 
            new ConcurrentDictionary<ItemKey, string>(ItemKeyComparer.Default);

        private readonly ConcurrentDictionary<ItemKey, string> pendingTexts = 
            new ConcurrentDictionary<ItemKey, string>(ItemKeyComparer.Default);

        private readonly ConcurrentDictionary<string, string> languageParents =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds a local text entry to the registry
        /// </summary>
        /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
        /// <param name="key">Local text key</param>
        /// <param name="text">Translated text</param>
        public void Add(string languageID, string key, string text)
        {
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
            pendingTexts[new ItemKey(languageID, key)] = text;
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>
        public string TryGet(string key)
        {
            var context = Dependency.TryResolve<ILocalTextContext>();

            return TryGet(CultureInfo.CurrentUICulture.Name,
                key, context != null && context.IsApprovalMode);
        }

        /// <summary>
        /// Converts the local text key to its representation in requested language. Looks up text
        /// in requested language, its parents and invariant language in order. If not found in any,
        /// null is returned. See SetParentLanguageID for information about parent languages.
        /// </summary>
        /// <param name="languageID"/>Language ID.</param>
        /// <param name="textKey">Local text key (can be null).</param>
        /// <param name="isApprovalMode">If pending approval texts to be used, true.</param>
        public string TryGet(string languageID, string textKey, bool isApprovalMode)
        {
            // create a key to lookup by language and text key pair
            var k = new ItemKey(languageID, textKey);

            string s;

            if (isApprovalMode)
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
                    languageID = TryGetParentLanguageID(languageID) ?? LocalText.InvariantLanguageID;

                    // search in parent or default language
                    k = new ItemKey(languageID, textKey);

                    if (pendingTexts.TryGetValue(k, out s))
                    {
                        // text available in pending parent language
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
                    
                    // check for possible circular parents
                    if (circularCheck1++ >= 10)
                        return null;

                    // try again for parent language...
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
                        languageID = TryGetParentLanguageID(languageID) ?? LocalText.InvariantLanguageID;

                        // search in parent or default language
                        k = new ItemKey(languageID, textKey);

                        // search again
                        if (approvedTexts.TryGetValue(k, out s))
                            return s;

                        if (languageID == LocalText.InvariantLanguageID ||
                            circularCheck2++ >= 10)
                            return null;
                    }
                }

                // couldn't find in requested language or its parents, return key itself
                return null;
            }

            // found in requested language, return it
            return s;
        }

        /// <summary>
        /// Sets the parent language of the specified language.
        /// When a text is not found in one language, LocalTextRegistry checks its parent language for
        /// a translation. Some implicit parent language definitions exist even if none set. For example, "en" is 
        /// parent language ID of "en-US" and "en-UK", "tr" is parent language ID of "tr-TR". Also, 
        /// invariant language ID ("") is an implicit parent of all languages.
        /// </summary>
        /// <param name="languageID">Language identifier. (e.g. en-US)</param>
        /// <param name="parentLanguageID">Parent language identifier. (e.g. en)</param>
        public void SetParentLanguageID(string languageID, string parentLanguageID)
        {
            languageParents[languageID] = parentLanguageID;
        }

        private string TryGetParentLanguageID(string languageID)
        {
            string parent;
            if (languageParents.TryGetValue(languageID, out parent))
                return parent;

            if (languageID == LocalText.InvariantLanguageID)
                return null;

            var idx = languageID.LastIndexOf('-');
            if (idx >= 1)
                return languageID.SafeSubstring(0, idx);

            return LocalText.InvariantLanguageID;
        }


        /// <summary>
        ///   Gets all available text keys (that has a translation in language or any of its
        ///   parent languages) and their local texts.</summary>
        /// <param name="languageID">
        ///   Language ID (required).</param>
        /// <param name="pending">
        ///   True if pending texts should be returned (e.g. in preview/edit mode).</param>
        /// <returns>
        ///   A dictionary of all texts in the language.</returns>
        public Dictionary<string, string> GetAllAvailableTextsInLanguage(string languageID, bool pending)
        {
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

                currentID = TryGetParentLanguageID(currentID) ?? LocalText.InvariantLanguageID;
            }

            return texts;
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