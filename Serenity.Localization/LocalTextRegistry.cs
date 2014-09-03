
namespace Serenity.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using ItemKey = System.Tuple<long, string>;

    public class LocalTextRegistry : ILocalTextProvider
    {
        private readonly ConcurrentDictionary<long, long> languageParents = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<ItemKey, string> approvedTexts = new ConcurrentDictionary<ItemKey, string>();
        private readonly ConcurrentDictionary<ItemKey, string> pendingTexts = new ConcurrentDictionary<ItemKey, string>();

        public long ContextLanguageID
        {
            get
            {
                var context = Dependency.TryResolve<ILocalTextContext>();
                return context != null ? context.LanguageID : LocalText.DefaultLanguageID;
            }
        }

        public bool ContextIsApprovalMode
        {
            get
            {
                var context = Dependency.TryResolve<ILocalTextContext>();
                return context != null && context.IsApprovalMode;
            }
        }

        public void SetParentLanguageID(int languageID, int parentLanguageID)
        {
            languageParents[languageID] = parentLanguageID;
        }

        public string TryGet(string key)
        {
            var context = Dependency.TryResolve<ILocalTextContext>();

            return TryGet(context != null ? context.LanguageID : LocalText.DefaultLanguageID, 
                key, context != null && context.IsApprovalMode);
        }

        public void Add(LocalTextEntry e, bool pendingApproval)
        {
            var dict = pendingApproval ? pendingTexts : approvedTexts;
            dict[new ItemKey(e.LanguageID, e.Key)] = e.Text;
        }

        public void AddList(IEnumerable<LocalTextEntry> list, bool pendingApproval)
        {
            var dict = pendingApproval ? pendingTexts : approvedTexts;

            foreach (var e in list)
                dict[new ItemKey(e.LanguageID, e.Key)] = e.Text;
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
        public string TryGet(Int64 languageID, string textKey, bool isApprovalMode)
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
                    if (languageID == LocalText.DefaultLanguageID)
                        return null;
                }
                else if (approvedTexts.TryGetValue(k, out s))
                {
                    // approved is available, return it
                    return s;
                }
                else if (languageID == LocalText.DefaultLanguageID)
                {
                    // if language is default language, return text key
                    return null;
                }

                var circularCheck1 = 0;
                while (true)
                {
                    if (!languageParents.TryGetValue(languageID, out languageID))
                        languageID = LocalText.DefaultLanguageID;

                    // search in parent or default language
                    k = new ItemKey(languageID, textKey);

                    if (pendingTexts.TryGetValue(k, out s))
                    {
                        // text available in pending parent language
                        if (s != null)
                            return s;

                        // if marked for deletion in default language, return key itself
                        if (languageID == LocalText.DefaultLanguageID)
                            return null;
                    }
                    else if (approvedTexts.TryGetValue(k, out s))
                    {
                        // text available in approved default language
                        return s;
                    }
                    else if (languageID == LocalText.DefaultLanguageID)
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
                if (languageID != LocalText.DefaultLanguageID)
                {
                    int circularCheck2 = 0;
                    while (true)
                    {
                        if (!languageParents.TryGetValue(languageID, out languageID))
                            languageID = LocalText.DefaultLanguageID;

                        // search in parent or default language
                        k = new ItemKey(languageID, textKey);

                        // search again
                        if (approvedTexts.TryGetValue(k, out s))
                            return s;

                        if (languageID == LocalText.DefaultLanguageID ||
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
        public Dictionary<string, string> GetAllAvailableTextsInLanguage(long languageID, bool pending)
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
                    currentID == LocalText.DefaultLanguageID)
                    break;

                if (!languageParents.TryGetValue(currentID, out currentID))
                    currentID = LocalText.DefaultLanguageID;
            }

            return texts;
        }
    }
}