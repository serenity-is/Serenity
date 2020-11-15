
namespace Serenity
{
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
        /// An empty local text instance like string.Empty
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
        /// Returns the local text key, use overload with ILocalText to get translation.
        /// </summary>   
        [Obsolete("Use ILocalTextContext through DI")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            return Key;
        }

        /// <summary>
        /// Returns the translation for current context
        /// </summary>   
        public string ToString(ITextLocalizer localizer)
        {
            return localizer?.TryGet(Key) ?? Key;
        }
    }
}