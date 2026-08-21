namespace Serenity;

/// <summary>
/// Text localizer for the current context language and pending approval state.
/// </summary>
public interface ITextLocalizer
{
    /// <summary>
    /// Gets the translation for a key based on the context language and pending approval state,
    /// or <c>null</c> if not available.
    /// </summary>
    /// <param name="key">The local text key.</param>
    /// <returns>The translated text, or <c>null</c> if no translation is found in the context language.</returns>
    string? TryGet(string key);
}