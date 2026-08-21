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
    public string Key { get; set; }

    /// <summary>
    /// The text in the source language.
    /// </summary>
    public string SourceText { get; set; }

    /// <summary>
    /// The text in the target language.
    /// </summary>
    public string TargetText { get; set; }

    /// <summary>
    /// The user-provided custom translation in the target language.
    /// </summary>
    public string CustomText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item has a translation.
    /// </summary>
    public bool HasTranslation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item was translated by a user.
    /// </summary>
    public bool UserTranslated { get; set; }
}