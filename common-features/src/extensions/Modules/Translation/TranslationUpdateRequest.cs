namespace Serenity.Extensions;

/// <summary>
/// The request model for updating user translations.
/// </summary>
public class TranslationUpdateRequest : ServiceRequest
{
    /// <summary>
    /// The target language ID.
    /// </summary>
    public string TargetLanguageID { get; set; }

    /// <summary>
    /// The dictionary of text keys and their translated values.
    /// </summary>
    public Dictionary<string, string> Translations { get; set; }
}