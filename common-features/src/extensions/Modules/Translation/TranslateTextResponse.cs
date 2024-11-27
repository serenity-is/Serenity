namespace Serenity.Extensions;

public class TranslateTextResponse : ServiceResponse
{
    public List<TranslateTextOutput> Translations { get; set; }
}

public class TranslateTextOutput
{
    public string TextKey { get; set; }
    public string TargetLanguageID { get; set; }
    public string TranslatedText { get; set; }
}
