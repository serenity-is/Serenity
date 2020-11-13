namespace Serenity
{
    using Serenity.Localization;
    using System;

    /// <summary>
    /// Extensions for local text context
    /// </summary>
    public static class LocalTextContextExtensions
    {
        /// <summary>
        /// Gets translation for a key
        /// </summary>
        /// <param name="context">The local text context</param>
        /// <param name="key">Key</param>
        /// <returns>Translated text or key itself if no translation found</returns>
        public static string Get(this ILocalTextContext context, string key)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Source.TryGet(context.UICulture.Name, key, context.IsApprovalMode) ?? key;
        }

        /// <summary>
        /// Gets translation for a key
        /// </summary>
        /// <param name="context">The local text context</param>
        /// <param name="key">Key</param>
        /// <returns>Translated text or null if no translation found</returns>
        public static string TryGet(this ILocalTextContext context, string key)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrEmpty(key))
                return null;

            return context?.Source.TryGet(context.UICulture.Name, key, context.IsApprovalMode);
        }
    }
}