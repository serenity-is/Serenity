namespace Serenity.Extensions;

/// <summary>
/// The request model for translating one or more texts.
/// </summary>
public class TranslateTextRequest : ServiceRequest
{
    /// <summary>
    /// The source language ID.
    /// </summary>
    public string SourceLanguageID { get; set; }

    /// <summary>
    /// The list of texts to translate.
    /// </summary>
    public List<TranslateTextInput> Inputs { get; set; }
}

/// <summary>
/// A single text to be translated.
/// </summary>
public class TranslateTextInput
{
    /// <summary>
    /// The key of the text to translate.
    /// </summary>
    public string TextKey { get; set; }

    /// <summary>
    /// The target language ID.
    /// </summary>
    public string TargetLanguageID { get; set; }

    /// <summary>
    /// The source text to translate.
    /// </summary>
    public string SourceText { get; set; }
}
