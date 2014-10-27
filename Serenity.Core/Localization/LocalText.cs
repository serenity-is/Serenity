
namespace Serenity
{
    using Localization;
    using Serenity.Abstractions;
    using System;

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
        public static LocalText Empty;

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
            return TryGet(key) ?? key;
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or NULL if none found 
        /// in local text registry.
        /// </summary>
        public static string TryGet(string key)
        {
            var provider = Dependency.TryResolve<ILocalTextRegistry>();
            return provider == null ? null : provider.TryGet(key);
        }
    }
}