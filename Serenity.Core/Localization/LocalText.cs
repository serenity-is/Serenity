
namespace Serenity
{
    using Serenity.Abstractions;
    using Serenity.Localization;
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
            Key = key;
        }

        /// <summary>
        /// Gets the local text key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>   
        [Obsolete("Use ILocalTextSource through DI")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            return Get(Key);
        }

        /// <summary>
        /// Implicit conversion to String that returns localized representation which corresponds to the local 
        /// text key or the key itself if none found in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextSource through DI")]
#endif        
        public static implicit operator string(LocalText localText)
        {
            return localText == null ? null : Get(localText.Key);
        }

        /// <summary>
        /// Implicit conversion from String that creates a new instance of LocalText with the specified key.
        /// </summary>
        /// <param name="key">Local text key</param>
        public static implicit operator LocalText(string key)
        {
            return string.IsNullOrEmpty(key) ? Empty : new LocalText(key);
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextSource through DI")]
#endif
        public static string Get(string key)
        {
            return TryGet(key) ?? key;
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or NULL if none found 
        /// in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextSource through DI")]
#endif
        public static string TryGet(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var provider = Dependency.TryResolve<ILocalTextSource>();
            return provider?.TryGet(CultureInfo.CurrentUICulture.Name, key, 
                Dependency.TryResolve<ILocalTextContext>()?.IsApprovalMode == true);
        }
    }
}