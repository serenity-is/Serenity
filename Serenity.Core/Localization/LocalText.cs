
namespace Serenity
{
    using System;
    using Localization;

    public class LocalText
    {
        public const string InvariantLanguageID = "";
        public static LocalText Empty;

        private string key;

        static LocalText()
        {
            Empty = new LocalText("");
        }

        public LocalText(string key)
        {
            this.key = key;
        }

        public string Key
        {
            get { return key; }
            set { key = value ?? String.Empty; }
        }

        public override string ToString()
        {
            return Get(key);
        }

        public static implicit operator string(LocalText localText)
        {
            return localText == null ? null : Get(localText.key);
        }

        public static implicit operator LocalText(string key)
        {
            return String.IsNullOrEmpty(key) ? Empty : new LocalText(key);
        }

        public static string Get(string key)
        {
            return TryGet(key) ?? key;
        }

        public static string TryGet(string key)
        {
            var provider = Dependency.TryResolve<ILocalTextRegistry>();
            return provider == null ? null : provider.TryGet(key);
        }
    }
}