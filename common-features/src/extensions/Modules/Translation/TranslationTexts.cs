namespace Serenity.Extensions;

/// <summary>
/// Local text keys for the translation module.
/// </summary>
[NestedLocalTexts(Prefix = "Site.Translation.")]
public static class TranslationTexts
{
    /// <summary>
    /// All texts in the view are already translated.
    /// </summary>
    public static readonly LocalText AllTextsAlreadyTranslated = "All texts in the view are already translated!";
    /// <summary>
    /// Assembly.
    /// </summary>
    public static readonly LocalText Assembly = "Assembly";
    /// <summary>
    /// Copied texts to clipboard in JSON format.
    /// </summary>
    public static readonly LocalText CopySuccessMessage = "Copied texts to clipboard in JSON format.";
    /// <summary>
    /// Failed to copy to clipboard.
    /// </summary>
    public static readonly LocalText CopyFailMessage = "Failed to Copy to clipboard! It might be due to not having HTTPS or necessary permissions.";
    /// <summary>
    /// Copy source texts to clipboard in JSON format.
    /// </summary>
    public static readonly LocalText CopySourceTranslations = "Copy source texts to clipboard in JSON format";
    /// <summary>
    /// Copy target texts to clipboard in JSON format.
    /// </summary>
    public static readonly LocalText CopyTargetTranslations = "Copy target texts to clipboard in JSON format";
    /// <summary>
    /// User translation in target language.
    /// </summary>
    public static readonly LocalText CustomText = "User Translation in Target Language";
    /// <summary>
    /// Translations.
    /// </summary>
    public static readonly LocalText EntityPlural = "Translations";
    /// <summary>
    /// Has translation.
    /// </summary>
    public static readonly LocalText HasTranslation = "Has Translation";
    /// <summary>
    /// Local text key.
    /// </summary>
    public static readonly LocalText Key = "Local Text Key";
    /// <summary>
    /// Overwrite user translation with clicked text.
    /// </summary>
    public static readonly LocalText OverrideConfirmation = "Overwrite user translation with clicked text?";
    /// <summary>
    /// Save changes.
    /// </summary>
    public static readonly LocalText SaveChangesButton = "Save Changes";
    /// <summary>
    /// User translations in target language are saved.
    /// </summary>
    public static readonly LocalText SaveSuccessMessage = "User translations in target language are saved to {0}";
    /// <summary>
    /// Source language.
    /// </summary>
    public static readonly LocalText SourceLanguage = "Source Language";
    /// <summary>
    /// Source and target languages should be different.
    /// </summary>
    public static readonly LocalText SourceTargetLanguageSame = "Source and target languages should be different!";
    /// <summary>
    /// Text in source language.
    /// </summary>
    public static readonly LocalText SourceText = "Text in Source Language";
    /// <summary>
    /// Target language.
    /// </summary>
    public static readonly LocalText TargetLanguage = "Target Language";
    /// <summary>
    /// Please select target language.
    /// </summary>
    public static readonly LocalText TargetLanguageRequired = "Please select target language!";
    /// <summary>
    /// Translation in target language.
    /// </summary>
    public static readonly LocalText TargetText = "Translation in Target Language";
    /// <summary>
    /// AI translate.
    /// </summary>
    public static readonly LocalText TranslateAllText = "AI Translate";
    /// <summary>
    /// AI translate.
    /// </summary>
    public static readonly LocalText TranslateText = "AI Translate";
    /// <summary>
    /// AI translate confirmation.
    /// </summary>
    public static readonly LocalText TranslateTextConfirmation = "AI translate {0} of the texts which has no translation in the target language?";
    /// <summary>
    /// Auto translation is disabled.
    /// </summary>
    public static readonly LocalText TranslateTextDisabled = "Auto translation is disabled!";
    /// <summary>
    /// User translated.
    /// </summary>
    public static readonly LocalText UserTranslated = "User Translated";
}