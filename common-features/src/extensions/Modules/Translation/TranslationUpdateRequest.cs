namespace Serenity.Extensions;

public class TranslationUpdateRequest : ServiceRequest
{
    public string TargetLanguageID { get; set; }
    public Dictionary<string, string> Translations { get; set; }
}