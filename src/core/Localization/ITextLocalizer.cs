namespace Serenity;

/// <summary>
/// Text localizer for current context language / pending state.
/// </summary>
public interface ITextLocalizer
{
    /// <summary>
    /// Gets translation for a key based on the context language / pending approval state, or null if not available
    /// </summary>
    /// <param name="key">Local text key</param>
    /// <returns>Translated text or null if no translation found in the context language</returns>
    string? TryGet(string key);
}