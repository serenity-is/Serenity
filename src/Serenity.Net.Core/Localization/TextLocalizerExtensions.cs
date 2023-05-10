namespace Serenity;

/// <summary>
/// Extensions for local text context
/// </summary>
public static class TextLocalizerExtensions
{
    /// <summary>
    /// Gets translation for a key
    /// </summary>
    /// <param name="localTexts">The local texts</param>
    /// <param name="key">Key</param>
    /// <returns>Translated text or key itself if no translation found</returns>
    public static string Get(this ITextLocalizer localTexts, string key)
    {
        if (localTexts == null)
            throw new ArgumentNullException(nameof(localTexts));

        return localTexts.TryGet(key) ?? key;
    }
}