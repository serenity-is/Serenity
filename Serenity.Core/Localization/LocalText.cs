
namespace Serenity
{
    using Serenity.Localization;
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
        [Obsolete("Use ILocalTextContext through DI")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
#if NET
            return Key;
#else
            return Get(Key);
#endif
        }

#if !NET
        /// <summary>
        /// Implicit conversion to String that returns localized representation which corresponds to the local 
        /// text key or the key itself if none found in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextContext through DI")]
#endif
        public static implicit operator string(LocalText localText)
        {
            return localText == null ? null : Get(localText.Key);
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the key itself if none 
        /// found in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextContext through DI")]
#endif
        public static string Get(string key)
        {
            return Dependency.TryResolve<ILocalTextContext>()?.TryGet(key) ?? key;
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or NULL if none found 
        /// in local text registry.
        /// </summary>
#if !NET45
        [Obsolete("Use ILocalTextContext through DI")]
#endif
        public static string TryGet(string key)
        {
            return Dependency.TryResolve<ILocalTextContext>()?.TryGet(key);
        }
#endif
    }
}