namespace Serenity.Localization;

/// <summary>
/// An <see cref="ITextLocalizer"/> implementation that resolves translations from an
/// <see cref="ILocalTextRegistry"/> using the current UI culture.
/// </summary>
/// <remarks>
/// Creates a new default text localizer instance.
/// </remarks>
/// <param name="registry">The local text registry to resolve translations from.</param>
public class DefaultTextLocalizer(ILocalTextRegistry registry) : ITextLocalizer
{
    private readonly ILocalTextRegistry registry = registry ?? throw new ArgumentNullException(nameof(registry));

    /// <summary>
    /// Gets the translation for a key based on the current UI culture language.
    /// </summary>
    /// <param name="key">The local text key.</param>
    /// <returns>The translated text, or <c>null</c> if no translation is found in the context language.</returns>
    public string? TryGet(string key)
    {
        return registry.TryGet(CultureInfo.CurrentUICulture.Name, key, false);
    }
}