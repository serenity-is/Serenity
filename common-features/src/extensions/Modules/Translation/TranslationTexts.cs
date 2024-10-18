namespace Serenity.Extensions;

[NestedLocalTexts(Prefix = "Site.Translation.")]
public static class TranslationTexts
{
    public static readonly LocalText AllTextsAlreadyTranslated = "All texts in the view are already translated!";
    public static readonly LocalText Assembly = "Assembly";
    public static readonly LocalText CopySuccessMessage = "Copied texts to clipboard in JSON format.";
    public static readonly LocalText CopyFailMessage = "Failed to Copy to clipboard! It might be due to not having HTTPS or necessary permissions.";
    public static readonly LocalText CopySourceTranslations = "Copy source texts to clipboard in JSON format";
    public static readonly LocalText CopyTargetTranslations = "Copy target texts to clipboard in JSON format";
    public static readonly LocalText CustomText = "User Translation in Target Language";
    public static readonly LocalText EntityPlural = "Translations";
    public static readonly LocalText HasTranslation = "Has Translation";
    public static readonly LocalText Key = "Local Text Key";
    public static readonly LocalText OverrideConfirmation = "Overwrite user translation with clicked text?";
    public static readonly LocalText SaveChangesButton = "Save Changes";
    public static readonly LocalText SaveSuccessMessage = "User translations in target language are saved to {0}";
    public static readonly LocalText SourceLanguage = "Source Language";
    public static readonly LocalText SourceTargetLanguageSame = "Source and target languages should be different!";
    public static readonly LocalText SourceText = "Text in Source Language";
    public static readonly LocalText TargetLanguage = "Target Language";
    public static readonly LocalText TargetLanguageRequired = "Please select target language!";
    public static readonly LocalText TargetText = "Translation in Target Language";
    public static readonly LocalText TranslateAllText = "AI Translate";
    public static readonly LocalText TranslateText = "AI Translate";
    public static readonly LocalText TranslateTextConfirmation = "AI translate {0} of the texts which has no translation in the target language?";
    public static readonly LocalText TranslateTextDisabled = "Auto translation is disabled!";
    public static readonly LocalText UserTranslated = "User Translated";
}