namespace Serenity;

/// <summary>
/// Extensions for <see cref="ITextLocalizer"/>.
/// </summary>
public static class TextLocalizerExtensions
{
    /// <summary>
    /// Gets the translation for a key.
    /// </summary>
    /// <param name="localTexts">The text localizer.</param>
    /// <param name="key">The key.</param>
    /// <returns>The translated text, or the key itself if no translation is found.</returns>
    public static string Get(this ITextLocalizer localTexts, string key)
    {
        if (localTexts == null)
            throw new ArgumentNullException(nameof(localTexts));

        return localTexts.TryGet(key) ?? key;
    }
}