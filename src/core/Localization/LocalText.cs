using Serenity.Localization;

namespace Serenity;

/// <summary>
/// Defines a localizable text resource. Contains a local text key and has implicit conversions to and
/// from <see cref="string"/>.
/// </summary>
/// <remarks>
/// Creates a new <see cref="LocalText"/> instance that contains the specified local text key.
/// </remarks>
/// <param name="key">The local text key.</param>
public class LocalText(string? key) : ILocalText
{
    /// <summary>
    /// The invariant language ID, which is an empty string.
    /// </summary>
    public const string InvariantLanguageID = "";

    /// <summary>
    /// An empty local text instance, similar to <see cref="string.Empty"/>.
    /// </summary>
    public static readonly LocalText Empty;

    static LocalText()
    {
        Empty = new LocalText("");
    }

    /// <summary>
    /// Gets the local text key.
    /// </summary>
    public string Key { get; private set; } = key ?? string.Empty;

    /// <summary>
    /// Implicit conversion from <see cref="string"/> that creates a new instance of <see cref="LocalText"/>
    /// with the specified key.
    /// </summary>
    /// <param name="key">The local text key.</param>
    public static implicit operator LocalText(string? key)
    {
        return string.IsNullOrEmpty(key) ? Empty : new LocalText(key);
    }

    /// <summary>
    /// Returns the local text key. Use the overload with <see cref="ITextLocalizer"/> to get a translation.
    /// </summary>
    [Obsolete("Use ILocalTextContext through DI")]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override string ToString()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
    {
        return Key;
    }

    /// <summary>
    /// Returns the translation for the current context.
    /// </summary>
    /// <param name="localizer">The text localizer used to resolve the translation.</param>
    /// <returns>The translated text, or the key itself if no translation is found.</returns>
    public string ToString(ITextLocalizer? localizer)
    {
        return localizer?.TryGet(Key) ?? Key;
    }

    private string? originalKey;

    string? ILocalText.OriginalKey => originalKey;

    void ILocalText.ReplaceKey(string newKey)
    {
        if (newKey is null)
            throw new ArgumentNullException(nameof(newKey));

        if (originalKey != null)
            throw new InvalidOperationException("Local text already has an original key!");

        originalKey = Key;
        Key = newKey;
    }
}

    