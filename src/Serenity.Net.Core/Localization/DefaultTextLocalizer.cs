namespace Serenity.Localization;

/// <summary>
/// Implementation of text localizer for current ui culture name and text registry
/// </summary>
public class DefaultTextLocalizer : ITextLocalizer
{
    private readonly ILocalTextRegistry registry;

    /// <summary>
    /// Creates a new default text localizer instance
    /// </summary>
    /// <param name="registry"></param>
    public DefaultTextLocalizer(ILocalTextRegistry registry)
    {
        this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    /// <summary>
    /// Gets translation for a key based on the context language
    /// </summary>
    /// <param name="key">Local text key</param>
    /// <returns>Translated text or null if no translation found in the context language</returns>
    public string? TryGet(string key)
    {
        return registry.TryGet(CultureInfo.CurrentUICulture.Name, key, false);
    }
}