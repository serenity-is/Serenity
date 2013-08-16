using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity
{
    /// <summary>
    ///   Class that provides localized texts in applications.</summary>
    /// <remarks>
    ///   <p>This is a simple class that actually has only one field named "text_key". This key allows access 
    ///   to the actual localized texts.</p>
    ///   <p>Basically, there should be a database table that holds ([key+language] -> localtext) pairs. This table should simply
    ///   have "text_key, language_id, local_text" fields.</p>
    ///   <p>A sample for records in the table:</p>
    ///   <p>"cms.button.save", 1 (Turkish), "Kaydet"</p>
    ///   <p>"cms.button.save", 2 (English), "Save"</p>
    ///   <p>This class holds a static thread-safe cache of all these texts and allows access to them.</p></remarks>
    public class LocalText
    {
        /// <summary>
        ///   The default language ID is ISO 1055 (turkish)</summary>
        private static Int64 _defaultLanguageID = 1055;

        /// <summary>
        ///   Holds parent language ID for each language.</summary>
        private static Dictionary<Int64, Int64> _languageParents = new Dictionary<Int64, Int64>();

        public static Func<Int64> GetContextLanguageId { get; set; }
        public static Func<Boolean> GetContextPending { get; set; }

        /// <summary>
        ///   Default language ID.</summary>
        /// <remarks>
        ///   <p>In all applications, default language ID is "1055 - Turkish".
        ///   If needed, this default should be changed in site initialization through this property.</p>
        ///   <p>If a text is not found in active language, it will be looked up in parents of active
        ///   language in order, till the default language in the end (default language is the ultimate
        ///   parent language).</p></remarks>
        public static Int64 DefaultLanguageID
        {
            get { return _defaultLanguageID; }
            set { _defaultLanguageID = value; }
        }

        /// <summary>
        ///   Sets parent language IDs for a list of languages.</summary>
        /// <param name="parents">
        ///   Dictionary<string, object>() of parents, language ID as key and parent language ID as value (required).</param>
        /// <remarks>
        ///   This function adds new parents or updates current ones. Doesn't remove existing one
        ///   unless explicitly set to Null.Int64 in new dictionary.</remarks>
        public static void SetParentLanguages(IDictionary<Int64, Int64?> parents)
        {
            Dictionary<Int64, Int64> newLanguageParents = new Dictionary<Int64, Int64>();
            foreach (var pair in _languageParents)
                newLanguageParents.Add(pair.Key, pair.Value);
            foreach (var n in parents)
            {
                if (n.Value == null)
                {
                    if (newLanguageParents.ContainsKey(n.Key))
                        newLanguageParents.Remove(n.Key);
                }
                else
                    newLanguageParents[n.Key] = n.Value.Value;
            }
            _languageParents = newLanguageParents;
        }

        /// <summary>
        ///   This dictionary holds published (approved) localized texts. It has
        ///   "[textkey, language_id] ==> local_text" pairs.</summary> 
        private static Dictionary<LanguageIDTextKey, string> _publishedTexts;

        /// <summary>
        ///   This dictionary holds pending (non-approved) localized texts. It has
        ///   "[textkey, language_id] ==> local_text" pairs. Texts that are pending
        ///   deletion has a null local_text value in this dictionary.</summary> 
        private static Dictionary<LanguageIDTextKey, string> _pendingTexts;

        /// <summary>
        ///   An empty local text similar to <c>String.Empty</c>.</summary>
        public static LocalText Empty;

        /// <summary>
        ///   Text key of this local text instance (e.g. "cms.contact_form.address")</summary>
        private string _textKey;

        /// <summary>
        ///   Static constructor. Creates the empty local text object and local text dictionaries.</summary>
        static LocalText()
        {
            Empty = new LocalText("");
            _publishedTexts = new Dictionary<LanguageIDTextKey, string>();
            _pendingTexts = new Dictionary<LanguageIDTextKey, string>();
        }

        /// <summary>
        ///   Creates a new local text object containing the given key.</summary>
        /// <param name="textKey">
        ///   Text key</param>
        public LocalText(string textKey)
        {
            _textKey = textKey;
        }

        /// <summary>
        ///   Gets/sets text key that object will contain.</summary>
        public string TextKey
        {
            get { return _textKey; }
            set { _textKey = value ?? String.Empty; }
        }

        /// <summary>
        ///   Converts the <see cref="TextKey">text key</see>, 
        ///   local text object contains to its representation in <see cref="ContextLanguageID">active language</see>.</summary>
        /// <returns>
        ///   Local text, looked up from current language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public override string ToString()
        {
            return Get(_textKey);
        }

        /// <summary>
        ///   Converts the <see cref="TextKey">text key</see>, 
        ///   local text object contains to its representation in requested language.</summary>
        /// <param name="languageID">
        ///   Requested language</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public string ToString(Int64 languageID)
        {
            return Get(languageID, _textKey);
        }

        /// <summary>
        ///   Converts the <see cref="TextKey">text key</see>, local text object contains to its 
        ///   representation in <see cref="ContextLanguageID">active language</see> and
        ///   formats the resulting string with given arguments using String.Format function.</summary>
        /// <returns>
        ///   Formatted local text, looked up from current language, its parents, and default language in order. If not found
        ///   in any, textkey itself is formatted and returned</returns>
        public string Format(params object[] args)
        {
            return String.Format(ToString(), args);
        }

        /// <summary>
        ///   Implicit operator overload that lets LocalText to be converted to string.</summary>
        /// <param name="localText">
        ///   Local text.</param>
        /// <returns>
        ///   Representation of local text in active language.</returns>
        public static implicit operator string(LocalText localText)
        {
            if (localText == null)
                return null;
            else
                return Get(localText._textKey);
        }

        /// <summary>
        ///   Implicit operator overload that lets string to be converted to a LocalText.</summary>
        /// <param name="textKey">
        ///   Local text key.</param>
        /// <returns>
        ///   A local text object containing given key.</returns>
        public static implicit operator LocalText(string textKey)
        {
            if (textKey == null || textKey.Length == 0)
                return Empty;
            else
                return new LocalText(textKey);
        }

        /// <summary>
        ///   Gets/sets active language ID for the context.</summary>
        public static Int64 ContextLanguageID
        {
            get
            {
                return GetContextLanguageId != null ? 
                    GetContextLanguageId() : DefaultLanguageID; 
            }
        }

        /// <summary>
        ///   Gets/sets active context pending flag. It determines if local text conversion uses only the published
        ///   dictionary or both pending and published. When we are in editor interface or looking at a page
        ///   in editing mode context is pending and pending approval texts should be used, but when a user 
        ///   loads a front site page in published mode, the context is not pending and only approved
        ///   local texts should be used.</summary>
        public static bool ContextPending
        {
            get 
            {
                if (GetContextPending != null)
                    return GetContextPending();
                else
                    return false;
            }
        }

        /// <summary>
        ///   Adds all items in a list of <see cref="LocalText.Entry"/> elements
        ///   to local text dictionaries.</summary>
        /// <remarks>
        ///   If there are already translations in local text dictionaries with
        ///   same keys, their values are overriden, no exception occurs.</remarks>
        /// <param name="list">
        ///   List of local texts to be added to local text dictionaries.</param>
        /// <param name="pending">
        ///   True if this is a pending approval entry.</param>
        public static void Add(IList<Entry> list, bool pending)
        {
            if (list.Count > 0)
            {
                var newDictionary = new Dictionary<LanguageIDTextKey, string>(pending ? _pendingTexts : _publishedTexts);

                LanguageIDTextKey k = new LanguageIDTextKey(DefaultLanguageID, "");
                foreach (Entry e in list)
                {
                    k._languageID = e.LanguageID;
                    k._textKey = e.TextKey;
                    newDictionary[k] = e.LocalText;
                }

                if (pending)
                    _pendingTexts = newDictionary;
                else
                    _publishedTexts = newDictionary;
            }
        }

        /// <summary>
        ///   Clears all local texts.</summary>
        public static void Clear()
        {
            _publishedTexts = new Dictionary<LanguageIDTextKey, string>();
            _pendingTexts = new Dictionary<LanguageIDTextKey, string>();
        }

        /// <summary>
        ///   Converts <paramref name="textKey">local text key</paramref>, to its representation
        ///   in <see cref="ContextLanguageID">active language</see>.</summary>
        /// <param name="textKey">
        ///   Text key.</param>
        /// <returns>
        ///   Local text, looked up from current language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public static string Get(string textKey)
        {
            return Get(ContextLanguageID, textKey, ContextPending);
        }

        /// <summary>
        ///   Converts <paramref name="textKey">local text key</paramref>, to its representation
        ///   in <see cref="ContextLanguageID">active language</see>.</summary>
        /// <param name="textKey">
        ///   Text key.</param>
        /// <param name="pending">
        ///   True to use pending local texts</param>
        /// <returns>
        ///   Local text, looked up from current language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public static string Get(string textKey, bool pending)
        {
            return Get(ContextLanguageID, textKey);
        }


        /// <summary>
        ///   Converts <paramref name="textKey">local text key</paramref>, to its representation
        ///   in <see cref="ContextLanguageID">requested language</see>.</summary>
        /// <param name="textKey">
        ///   Text key.</param>
        /// <param name="languageID">
        ///   Language ID</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public static string Get(Int64 languageID, string textKey)
        {
            return Get(languageID, textKey, ContextPending);
        }

        /// <summary>
        ///   Converts <paramref name="textKey">the local text key</paramref>, to its representation in
        ///   <paramref name="languageID">requested language</paramref>.</summary>
        /// <param name="languageID"/>
        ///   Language ID.
        /// <param name="textKey">
        ///   Local text key (can be null).</param>
        /// <param name="pending">
        ///   If pending approval texts to be used, true.</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, null is returned</returns>
        public static string TryGet(string textKey)
        {
            return TryGet(ContextLanguageID, textKey, ContextPending);
        }

        /// <summary>
        ///   Converts <paramref name="textKey">the local text key</paramref>, to its representation in
        ///   <paramref name="languageID">requested language</paramref>.</summary>
        /// <param name="languageID"/>
        ///   Language ID.
        /// <param name="textKey">
        ///   Local text key (can be null).</param>
        /// <param name="pending">
        ///   If pending approval texts to be used, true.</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, null is returned</returns>
        public static string TryGet(Int64 languageID, string textKey, bool pending)
        {
            // create a key to lookup by language and text key pair
            LanguageIDTextKey k = new LanguageIDTextKey(languageID, textKey);

            string s;

            if (pending)
            {
                // first search in pending texts
                if (_pendingTexts.TryGetValue(k, out s))
                {
                    // pending text is available, return it
                    if (s != null)
                        return s;

                    // pending text is null, it means marked for deletion, 
                    // if this is default language, return key itself
                    if (languageID == DefaultLanguageID)
                        return null;
                }
                else if (_publishedTexts.TryGetValue(k, out s))
                {
                    // published is available, return it
                    return s;
                }
                else if (languageID == DefaultLanguageID)
                {
                    // if language is default language, return text key
                    return null;
                }

                int circularCheck1 = 0;
                while (true)
                {
                    if (!_languageParents.TryGetValue(languageID, out languageID))
                        languageID = DefaultLanguageID;

                    // search in parent or default language
                    k._languageID = languageID;
                    if (_pendingTexts.TryGetValue(k, out s))
                    {
                        // text available in pending parent language
                        if (s != null)
                            return s;

                        // if marked for deletion in default language, return key itself
                        if (languageID == DefaultLanguageID)
                            return null;
                    }
                    else if (_publishedTexts.TryGetValue(k, out s))
                    {
                        // text available in published default language
                        return s;
                    }
                    else if (languageID == DefaultLanguageID)
                    {
                        // not in pending nor published, or circular reference, return key itself
                        return null;
                    }
                    // check for possible circular parents
                    if (circularCheck1++ >= 10)
                        return null;
                    // try again for parent language...
                }
            }
            else if (!_publishedTexts.TryGetValue(k, out s))
            {
                // couldn't find, if requested language is not DefaultLanguageID, search in it too
                if (languageID != DefaultLanguageID)
                {
                    int circularCheck2 = 0;
                    while (true)
                    {
                        if (!_languageParents.TryGetValue(languageID, out languageID))
                            languageID = DefaultLanguageID;

                        // search in parent or default language
                        k._languageID = languageID;

                        // search again
                        if (_publishedTexts.TryGetValue(k, out s))
                            return s;

                        if (languageID == DefaultLanguageID ||
                            circularCheck2++ >= 10)
                            return null;
                    }
                }

                // couldn't find in requested language or its parents, return key itself
                return null;
            }
            else
            {
                // found in requested language, return it
                return s;
            }
        }

        /// <summary>
        ///   Converts <paramref name="textKey">the local text key</paramref>, to its representation in
        ///   <paramref name="languageID">requested language</paramref>.</summary>
        /// <param name="languageID"/>
        ///   Language ID.
        /// <param name="textKey">
        ///   Local text key (can be null).</param>
        /// <param name="pending">
        ///   If pending approval texts to be used, true.</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order. If not found
        ///   in any, textkey itself is returned</returns>
        public static string Get(Int64 languageID, string textKey, bool pending)
        {
            return TryGet(languageID, textKey, pending) ?? textKey;
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
        public static Dictionary<string, string> GetAllAvailableTextsInLanguage(Int64 languageID, bool pending)
        {
            var texts = new Dictionary<string, string>();
            string text;

            var currentID = languageID;
            int tries = 0;

            while (true)
            {
                if (pending)
                    foreach (var item in _pendingTexts.Keys)
                        if (item.LanguageID == currentID && !texts.ContainsKey(item.TextKey))
                        {
                            text = TryGet(languageID, item.TextKey, true);
                            if (text != null)
                                texts[item.TextKey] = text;
                        }

                foreach (var item in _publishedTexts.Keys)
                    if (item.LanguageID == currentID && !texts.ContainsKey(item.TextKey))
                    {
                        text = TryGet(languageID, item.TextKey, true);
                        if (text != null)
                            texts[item.TextKey] = text;
                    }

                tries++;

                if (tries > 10 ||
                    currentID == LocalText.DefaultLanguageID)
                    break;

                if (!_languageParents.TryGetValue(currentID, out currentID))
                    currentID = LocalText.DefaultLanguageID;
            }
            return texts;
        }

        /// <summary>
        ///   Converts <paramref name="textKey">the local text key</paramref>, to its representation in
        ///   <paramref name="languageID">requested language</paramref> and formats it with
        ///   specified arguments.</summary>
        /// <param name="textKey">
        ///   Local text key (can be null).</param>
        /// <param name="args">
        ///   Arguments</param>
        /// <returns>
        ///   Local text, looked up from requested language, its parents, and default language in order and
        ///   formatted by arguments. If not found in any, textkey itself is returned</returns>
        public static string GetFormat(string textKey, params object[] args)
        {
            return String.Format(Get(textKey, ContextPending), args);
        }

        /// <summary>
        ///   The key structure of local text dictionaries.</summary>
        private struct LanguageIDTextKey : IComparable, IComparable<LanguageIDTextKey>, IEquatable<LanguageIDTextKey>
        {
            /// <summary>
            ///   Language ID</summary>
            internal Int64 _languageID;
            /// <summary>
            ///   Key</summary>
            internal string _textKey;

            /// <summary>
            ///   Creates a new LanguageIDTextKey object.</summary>
            /// <param name="languageID">
            ///   Language ID</param>
            /// <param name="textKey">
            ///   Text key</param>
            public LanguageIDTextKey(Int64 languageID, string textKey)
            {
                _languageID = languageID;
                _textKey = textKey ?? String.Empty;
            }

            /// <summary>Language ID</summary>
            public Int64 LanguageID
            {
                get { return _languageID; }
            }

            /// <summary>Text key</summary>
            public string TextKey
            {
                get { return _textKey; }
            }

            /// <summary>
            ///   Gets hash code</summary>
            /// <returns>
            ///   Hash code</returns>
            public override int GetHashCode()
            {
                return _languageID.GetHashCode() ^ _textKey.GetHashCode();
            }

            /// <summary>
            ///   Compares this value to another.</summary>
            /// <param name="value">
            ///   Other value.</param>
            /// <returns>
            ///   -1 if this value is less, 0 if equal, 1 if greater</returns>
            public int CompareTo(LanguageIDTextKey value)
            {
                var result = _languageID.CompareTo(value._languageID);
                return (result == 0) ? _textKey.CompareTo(value._textKey) : result;
            }

            /// <summary>
            ///   Compares this value to another.</summary>
            /// <param name="value">
            ///   Other value.</param>
            /// <returns>
            ///   -1 if this value is less, 0 if equal, 1 if greater</returns>           
            public int CompareTo(object value)
            {
                if (value == null)
                    return 1;

                if (!(value is LanguageIDTextKey))
                    throw new InvalidCastException();

                return CompareTo((LanguageIDTextKey)value);
            }

            /// <summary>
            ///   Compares this value with another.</summary>
            /// <param name="value">
            ///   Other value.</param>
            /// <returns>
            ///   True if this is equal to other value</returns>
            public bool Equals(LanguageIDTextKey value)
            {
                return (this._languageID == value._languageID && this._textKey == value._textKey);
            }

            /// <summary>
            ///   Compares this value with another.</summary>
            /// <param name="value">
            ///   Other value.</param>
            /// <returns>
            ///   True if this is equal to other value</returns>
            public override bool Equals(object value)
            {
                return (value is LanguageIDTextKey && Equals((LanguageIDTextKey)value));
            }
        }

        /// <summary>
        ///   A helper class that will contain list of local text groups</summary>
        public class Package
        {
            private string _prefix;
            private List<Entry> _texts;
            private Int64 _languageId;
            private bool _isPending;

            public Package(Int64 languageId, bool isPending)
            {
                _languageId = languageId;
                _isPending = isPending;
                _prefix = "";
                _texts = new List<Entry>();
            }

            public Package()
            {
                _languageId = DefaultLanguageID;
                _prefix = "";
                _texts = new List<Entry>();
            }

            public Package Group(string prefix)
            {
                this._prefix = prefix + ".";
                return this;
            }

            public Package Add(string key, string text)
            {
                _texts.Add(new Entry(_languageId, _prefix + key, text));
                return this;
            }

            public Package AddPackage(Package package)
            {
                this._texts.AddRange(package._texts);
                return this;
            }

            public void Register()
            {
                LocalText.Add(_texts, _isPending);
            }

            public Package InitializeTextClass(Type type)
            {
                InitializeTextClass(type, "");
                return this;
            }

            private void InitializeTextClass(Type type, string prefix)
            {
                foreach (var member in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    var fi = member as FieldInfo;
                    if (fi != null &&
                        fi.FieldType == typeof(LocalText))
                    {
                        var value = fi.GetValue(null) as LocalText;
                        if (value != null)
                        {
                            this.Add(prefix + fi.Name, value.TextKey);
                            fi.SetValue(null, new LocalText(prefix + fi.Name));
                        }
                    }
                }

                foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    var name = nested.Name;
                    if (name.EndsWith("_"))
                        name = name.Substring(0, name.Length - 1);

                    InitializeTextClass(nested, prefix + name + ".");
                }
            }
        }

        /// <summary>
        ///   A helper class that will be used in batch local text operations</summary>
        public class Entry
        {
            /// <summary>
            ///   Language ID</summary>
            public readonly Int64 LanguageID;
            /// <summary>
            ///   Text key</summary>
            public readonly string TextKey;
            /// <summary>
            ///   Local text</summary>
            public readonly string LocalText;

            /// <summary>
            ///   Creates a new local text entry.</summary>
            /// <param name="languageID">
            ///   Language ID</param>
            /// <param name="textKey">
            ///   Text key</param>
            /// <param name="localText">
            ///   Local text</param>
            public Entry(Int64 languageID, string textKey, string localText)
            {
                LanguageID = languageID;
                TextKey = textKey;
                LocalText = localText;
            }
        }

        /// <summary>
        ///   A list of local text entries</summary>
        public class EntryList : List<Entry>
        {
            /// <summary>
            ///   Creates a new local text EntryList.</summary>
            public EntryList()
            {
            }
        }
    }
}