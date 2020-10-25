using Serenity.Abstractions;
using System.Text.RegularExpressions;

namespace Serenity.Localization
{
    /// <summary>
    /// Adds key fallback to any ILocalTextSource implementation
    /// </summary>
    public class FallbackLocalTextSource : ILocalTextSource
    {
        private readonly ILocalTextSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackLocalTextSource"/> class.
        /// </summary>
        /// <param name="source">The local text source.</param>
        public FallbackLocalTextSource(ILocalTextSource source)
        {
            Check.NotNull(source, nameof(source));

            this.source = source;
        }

        /// <summary>
        /// Returns localized representation which corresponds to the local text key or the fallback if none 
        /// found in the registry.
        /// </summary>
        /// <param name="key">Local text key (e.g. Enums.Month.June)</param>
        /// <param name="languageID">Language identifier</param>
        /// <param name="pending">If pending approval text should be used</param>
        /// <returns></returns>
        public string TryGet(string languageID, string key, bool pending)
        {
            string text = source.TryGet(languageID, key, pending);

            if (!string.IsNullOrEmpty(text) || string.IsNullOrEmpty(key))
                return text;

            // Get text without key's suffixes
            string[] suffixes = { ".EntitySingular", ".EntityPlural" };
            foreach (var suffix in suffixes)
                if (key.EndsWith(suffix))
                {
                    key = key.Substring(0, key.Length - suffix.Length);

                    text = source.TryGet(languageID, key, pending);
                    if (!string.IsNullOrEmpty(text))
                        return text;

                    break;
                }

            // Fallback to a sub-key (e.g. if Enums.Month.June not found, then try get June instead)
            key = TryGetKeyFallback(key);
            if (!string.IsNullOrEmpty(key))
            {
                text = source.TryGet(languageID, key, pending);
                if (!string.IsNullOrEmpty(text))
                    return text;

                return BreakUpString(key);
            }

            return null;
        }

        /// <summary>
        /// Get a fallback of the local text key
        /// </summary>
        /// <param name="key">Local text key</param>
        /// <returns>Local text key fallback</returns>
        public static string TryGetKeyFallback(string key)
        {
            // Get last part of the key after the last dot
            var lastDot = key.LastIndexOf('.');
            if (lastDot > 0 && lastDot < key.Length - 1)
            {
                key = key.Substring(lastDot + 1);

                // Remove Id
                if (key.Length > 2 && key.EndsWith("Id"))
                    key = key.Substring(0, key.Length - 2);

                return key;
            }

            // Get last part of the key after the last forward slash
            var lastSlash = key.LastIndexOf('/');
            if (lastSlash > 0 && lastSlash < key.Length - 1)
                return key.Substring(lastSlash + 1);

            return null;
        }

        /// <summary>
        /// Break up string without spaces (e.g. LastDirectoryUpdate) 
        /// into a normal string (e.g. 'Last Directory Update')
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string BreakUpString(string value)
        {
            return Regex.Replace(value, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1", RegexOptions.Compiled).Trim();
        }
    }
}