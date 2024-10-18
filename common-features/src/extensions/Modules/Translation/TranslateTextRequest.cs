namespace Serenity.Extensions;

public class TranslateTextRequest : ServiceRequest
{
    public string SourceLanguageID { get; set; }
    public List<TranslateTextInput> Inputs { get; set; }
}

public class TranslateTextInput
{
    public string TextKey { get; set; }
    public string TargetLanguageID { get; set; }
    public string SourceText { get; set; }
}
