
namespace Serenity.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using ItemKey = System.Tuple<string, string>;

    public class LocalTextRegistry : ILocalTextRegistry
    {
        private readonly ConcurrentDictionary<string, string> languageParents = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<ItemKey, string> approvedTexts = new ConcurrentDictionary<ItemKey, string>();
        private readonly ConcurrentDictionary<ItemKey, string> pendingTexts = new ConcurrentDictionary<ItemKey, string>();

        public bool ContextIsApprovalMode
        {
            get
            {
                var context = Dependency.TryResolve<ILocalTextContext>();
                return context != null && context.IsApprovalMode;
            }
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
                return languageID.Substring(idx);

            return LocalText.InvariantLanguageID;
        }

        public void SetParentLanguageID(string languageID, string parentLanguageID)
        {
            languageParents[languageID] = parentLanguageID;
        }

        public string TryGet(string key)
        {
            var context = Dependency.TryResolve<ILocalTextContext>();

            return TryGet(CultureInfo.CurrentUICulture.Name, 
                key, context != null && context.IsApprovalMode);
        }

        public void Add(string languageID, string key, string text)
        {
            approvedTexts[new ItemKey(languageID, key)] = text;
        }

        /// <summary>
        ///   Converts <paramref name="textKey">the local text key</paramref>, to its representation in
        ///   <paramref name="languageID">requested language</paramref>.</summary>
        /// <param name="languageID"/>
        ///   Language ID.
        /// <param name="textKey">
        ///   Local text key (can be null).</param>
        /// <param name="isApprovalMode">
        ///   If pending approval texts to be used, true.</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, null is returned</returns>
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
        ///   Gets all available text keys (that has a translation in language or any of its
        ///   parent languages) and their local texts.</summary>
        /// <param name="languageID">
        ///   Language ID (required).</param>
        /// <param name="pending">
        ///   True if pending texts should be returned (e.g. in preview/edit mode).</param>
        /// <returns>
        ///   A dictionary of all text in the language.</returns>
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
    }
}