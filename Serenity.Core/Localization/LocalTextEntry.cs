namespace Serenity.Localization
{
    using System;

    public class LocalTextEntry
    {
        /// <summary>
        ///   Language ID</summary>
        public readonly Int64 LanguageID;
        /// <summary>
        ///   Text key</summary>
        public readonly string Key;
        /// <summary>
        ///   Local text</summary>
        public readonly string Text;

        /// <summary>
        ///   Creates a new local text entry.</summary>
        /// <param name="languageID">
        ///   Language ID</param>
        /// <param name="key">
        ///   Text key</param>
        /// <param name="text">
        ///   Local text</param>
        public LocalTextEntry(Int64 languageID, string key, string text)
        {
            LanguageID = languageID;
            Key = key;
            Text = text;
        }
    }
}