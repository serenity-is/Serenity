namespace Serenity.Extensions;

/// <summary>
/// Represents a single translation entry in the translation grid.
/// </summary>
[ScriptInclude]
public class TranslationItem
{
    /// <summary>
    /// The local text key.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The text in the source language.
    /// </summary>
    public string? SourceText { get; set; }

    /// <summary>
    /// The text in the target language.
    /// </summary>
    public string? TargetText { get; set; }

    /// <summary>
    /// The user-provided custom translation in the target language.
    /// </summary>
    public string? CustomText { get; set; }

    /// <summary>
    /// The translation the application has in the target language under the user translation, which the target text
    /// goes back to when the user translation is removed. It is only set for an item with a user translation: an empty
    /// string when the application has no translation of it, and null when it is not known.
    /// </summary>
    public string? BuiltInText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item has a translation.
    /// </summary>
    public bool HasTranslation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item was translated by a user.
    /// </summary>
    public bool UserTranslated { get; set; }
}