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
    public ResolveLocalTextResult(string? key, string? text, bool isKey)
    {
        Key = string.IsNullOrEmpty(key) ? null : key;
        Text = text;
        IsKey = isKey;
    }

    /// <summary>The resolved local text key, or <c>null</c> if none is available.</summary>
    public string? Key { get; }

    /// <summary>The invariant text, if specified.</summary>
    public string? Text { get; }

    /// <summary>Whether the resolved key should be used as fallback text.</summary>
    public bool IsKey { get; }
}