namespace Serenity.Extensions;

/// <summary>
/// The response model for a text translation request.
/// </summary>
public class TranslateTextResponse : ServiceResponse
{
    /// <summary>
    /// The list of translated texts.
    /// </summary>
    public List<TranslateTextOutput> Translations { get; set; }
}

/// <summary>
/// A single translated text.
/// </summary>
public class TranslateTextOutput
{
    /// <summary>
    /// The key of the translated text.
    /// </summary>
    public string TextKey { get; set; }

    /// <summary>
    /// The target language ID.
    /// </summary>
    public string TargetLanguageID { get; set; }

    /// <summary>
    /// The translated text.
    /// </summary>
    public string TranslatedText { get; set; }
}
