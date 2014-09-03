
namespace Serenity.Localization
{
    using System;
    using System.Collections.Generic;

    public class LocalTextPackage
    {
        private readonly List<LocalTextEntry> texts;
        private readonly Int64 languageID;

        public LocalTextPackage(Int64 languageID = default(Int64))
        {
            this.languageID = languageID;
            this.texts = new List<LocalTextEntry>();
        }

        public LocalTextPackage Add(string key, string text)
        {
            texts.Add(new LocalTextEntry(languageID, key, text));
            return this;
        }

        public LocalTextPackage AddPackage(LocalTextPackage package)
        {
            this.texts.AddRange(package.texts);
            return this;
        }

        public IEnumerable<LocalTextEntry> Entries
        {
            get { return texts; }
        }
    }
}