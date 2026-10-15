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

    private static readonly Regex LocalTextKeyLike = new(@"^([A-Z][A-za-z0-9_]*\.)+[A-Z][A-za-z0-9_]*$", RegexOptions.Compiled);

    private static readonly Regex UIKeyCandidates = new(@"^(Columns|Controls|Db|Dialogs|Enums|Forms|Permission|Site|Validation)\.", RegexOptions.Compiled);


    /// <summary>
    /// Determines whether the specified text is a candidate for a local text key, optionally restricting to UI key prefixes.
    /// </summary>
    /// <param name="text">The text to evaluate as a potential local text key.</param>
    /// <param name="uiKeyOnly">If set to <c>true</c>, only considers keys with UI-related prefixes.</param>
    /// <returns><c>true</c> if the text is a candidate for a local text key; otherwise, <c>false</c>.</returns>
    public static bool IsLocalTextKeyCandidate(string? text, bool uiKeyOnly = true)
    {
        return !string.IsNullOrEmpty(text) &&
            LocalTextKeyLike.IsMatch(text) &&
            (!uiKeyOnly || UIKeyCandidates.IsMatch(text));
    }

    /// <summary>
    /// Resolves the local text key and invariant text for an attribute value.
    /// </summary>
    /// <param name="attribute">The attribute that defines the text.</param>
    /// <param name="text">The attribute's invariant text.</param>
    /// <param name="defaultKey">The fallback key, or <c>null</c> or empty when no fallback key is available.</param>
    /// <returns>The resolved local text key, invariant text, and fallback behavior.</returns>
    public static ResolveLocalTextResult ResolveFromAttribute(
        Attribute attribute, string? text, string? defaultKey)
    {
        if (attribute == null)
            throw new ArgumentNullException(nameof(attribute));

        if (string.IsNullOrEmpty(defaultKey))
            defaultKey = null;

        if (attribute is IHasTextKey hasTextKey)
        {
            var textKey = hasTextKey.TextKey;
            if (!string.IsNullOrEmpty(textKey))
                return new(textKey, text, true);

            return new(defaultKey, text, false);
        }

        // Older attributes without explicit key support may use a key-looking string as the key.
        if (IsLocalTextKeyCandidate(text, uiKeyOnly: true))
            return new(text!, null, true);

        return new(defaultKey, text, false);
    }

}

    