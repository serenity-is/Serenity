
namespace Serenity
{
    using Localization;
    using Serenity.Abstractions;
    using System;
    using System.Globalization;

    /// <summary>
    /// Defines a localizable text resource. Contains a local text key and has implicit conversions to and 
    /// from String.
    /// </summary>
    public class LocalText
    {
        /// <summary>
        /// Invariant language ID is an empty string
        /// </summary>
        public const string InvariantLanguageID = "";

        /// <summary>
        /// An empty local text instance like String.Empty
        /// </summary>
        public static readonly LocalText Empty;

        private string key;

        static LocalText()
        {
            Empty = new LocalText("");
        }

        /// <summary>
        /// Creates a new LocalText instance that contains the specified local text key
        /// </summary>
        /// <param name="key">Local text key</param>
        public LocalText(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Gets the local text key
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>
        public override string ToString()
        {
            return Get(key);
        }

        /// <summary>
        /// Implicit conversion to String that returns localized representation which corresponds to the local 
        /// text key or the key itself if none found in local text registry.
        /// </summary>
        public static implicit operator string(LocalText localText)
        {
            return localText == null ? null : Get(localText.key);
        }

        /// <summary>
        /// Implicit conversion from String that creates a new instance of LocalText with the specified key.
        /// </summary>
        /// <param name="key">Local text key</param>
        public static implicit operator LocalText(string key)
        {
            return String.IsNullOrEmpty(key) ? Empty : new LocalText(key);
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>
        public static string Get(string key)
        {
            string text = TryGet(key);

            if (!string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key))
                return text;
                        
            // Remove suffixes
            string[] suffixes = { ".EntitySingular", ".EntityPlural"};
            foreach (var suffix in suffixes)
                if (key.EndsWith(suffix))
                    key = key.Substring(0, key.Length - suffix.Length);

            // Get last part after a dot
            var lastDot = key.LastIndexOf('.');
            if (lastDot > 0 && lastDot < key.Length - 1)
            {
                key = key.Substring(lastDot + 1);

                // Remove Id
                if (key.Length > 2 && key.EndsWith("Id"))
                    key = key.Substring(0, key.Length - 2);

                text = TryGet(key);

                if (!string.IsNullOrEmpty(text))
                    return text;
            }

            return key.BreakUpString();
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or NULL if none found 
        /// in local text registry.
        /// </summary>
        public static string TryGet(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var provider = Dependency.TryResolve<ILocalTextRegistry>();
            return provider == null ? null : provider.TryGet(CultureInfo.CurrentUICulture.Name, key);
        }
    }
}