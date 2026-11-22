namespace Serenity.Localization;

/// <summary>
/// Contains the local text key, invariant text, and fallback behavior resolved from an attribute.
/// </summary>
public sealed record ResolveLocalTextResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResolveLocalTextResult"/> record.
    /// </summary>
    /// <param name="key">The resolved local text key, or <c>null</c> if none is available.</param>
    /// <param name="text">The invariant text, if specified.</param>
    /// <param name="isKey">Whether the resolved key should be used as fallback text.</param>
    /// <param name="literal">Whether the text is shown as it is, as its attribute has an empty text key, which is no key,
    /// e.g. a text that is a name. The key that the type would imply for the text is not used either, and nothing is
    /// registered for it.</param>
    public ResolveLocalTextResult(string? key, string? text, bool isKey, bool literal = false)
    {
        Key = string.IsNullOrEmpty(key) ? null : key;
        Text = text;
        IsKey = isKey;
        Literal = literal;
    }

    /// <summary>The resolved local text key, or <c>null</c> if none is available.</summary>
    public string? Key { get; }

    /// <summary>The invariant text, if specified.</summary>
    public string? Text { get; }

    /// <summary>Whether the resolved key should be used as fallback text.</summary>
    public bool IsKey { get; }

    /// <summary>
    /// Whether the text is shown as it is, as the attribute that gives it has an empty text key, which is no key. Nothing
    /// is registered for such a text, and the key that its type would imply for it is not used.
    /// </summary>
    public bool Literal { get; }
}