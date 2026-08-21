namespace Serenity.Extensions;

/// <summary>
/// The request model for listing translations.
/// </summary>
public class TranslationListRequest : ListRequest
{
    /// <summary>
    /// The source language ID.
    /// </summary>
    public string SourceLanguageID { get; set; }

    /// <summary>
    /// The target language ID.
    /// </summary>
    public string TargetLanguageID { get; set; }
}